(function() {
    Kooboo.loadJS([
        '/_Admin/Scripts/vue/components/kbTable/row/index.js',
        '/_Admin/Scripts/vue/components/kbCopyModal/index.js'
    ]);

    Kooboo.vue.component.kbTable = Vue.component('kb-table', {
        props: {
            data: Object,
            //deleteTrigger: Boolean
        },
        data: function() {
            return {
                selectedDocs: [],
                deleteTrigger:false,
                showCopyModal:false,
            }
        },
        watch: {
            selectedDocs: function(ids) {
                this.$emit('select', ids);
            },
            deleteTrigger: function(trigged) {
                var self = this;
                if (trigged) {
                    Kooboo.Table.deletes(self.data,self.selectedDocs,function(){
                        self.selectedDocs = [];
                        self.deleteTrigger=false;
                    });
                }
            },
            'data.docs': function() {
                this.selectedDocs = [];
            }
        },
        methods: {
            onToggleSelectDoc: function(id) {
                var idx = this.selectedDocs.indexOf(id);
                if (idx == -1) {
                    this.selectedDocs.push(id);
                } else {
                    this.selectedDocs.splice(idx, 1);
                }
            }
        },
        computed: {
            computedActions:function(){
                var self=this;
                var actions=this.data.actions;
                //clone
                var computeActions=JSON.parse(JSON.stringify(actions));
                computeActions.forEach(function(action){
                    if(action.url){
                        action.url=Kooboo.UrlHelper.Get(action.url);
                    }else{
                        //can be optimized
                        action.url="javascript:void(0)"
                    }
                    action.click=function(){};
                    //todo confirm,why render twice
                    if(action.name=="Copy"){
                        action.condition=self.selectedDocs.length==1;
                        action.click=function(){
                            self.showCopyModal=true;
                        }
                    }else if(action.name=="Delete"){
                        action.condition=self.selectedDocs.length>0;
                        action.click=function(){
                            self.deleteTrigger=true;
                        }
                    }else{
                        action.condition=true;
                    }
                    
                });
                return computeActions;
            },
            allSelected: {
                get: function() {
                    if (this.data.docs && this.data.docs.length) {
                        return this.selectedDocs.length == this.data.docs.length;
                    } else {
                        return false;
                    }
                },
                set: function(val) {
                    var self = this;
                    if (this.data.docs && this.data.docs.length) {
                        if (val) {
                            this.data.docs.forEach(function(doc) {
                                if (self.selectedDocs.indexOf(doc.id) == -1) {
                                    self.selectedDocs.push(doc.id);
                                }
                            })
                        } else {
                            self.selectedDocs = [];
                        }
                    } else {
                        self.selectedDocs = [];
                    }
                }
            }
        },
        components: {
            'kb-table': Kooboo.vue.component.kbTableRow
        },
        template: Kooboo.getTemplate('/_Admin/Scripts/vue/components/kbTable/index.model.html')
    })
})()