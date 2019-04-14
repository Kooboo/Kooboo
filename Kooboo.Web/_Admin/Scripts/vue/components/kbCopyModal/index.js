(function() {
    Kooboo.loadJS([
        '/_Admin/Scripts/vue/components/kbModal/index.js'
    ]);
    Kooboo.vue.component.kbCopyModel = Vue.component('kb-copy-modal', {
        props: {
            config: Object,
            selectedDocs:Array,
            show: Boolean
        },
        data: function() {
            var self=this;
            return {
                showModal: false,
                modalConfig: {
                    //title: 'Copy layout',
                    title: 'Copy',
                    //size: 'sm',
                    copyName: '',
                    btns: [{
                        text: Kooboo.text.common.start,
                        class: 'green',
                        onClick: self.onClick
                    }],
                    onShow: self.onShow,
                    onClose: function() {
                        this.copyName = ''
                    },
                    showCloseBtn: true,
                    closeBtnText: Kooboo.text.common.cancel,
                    labelName: Kooboo.text.common.name,
                }
            }
        },
        methods: {
            onClick:function(){
                var self=this;
                var copyData = self.config.docs.find(function(doc) {
                    return doc.id == self.selectedDocs[0];
                })
                
                if (copyData) {
                    var title=copyData.name.text;
                    this.modalConfig.title += (': ' + title);
                    this.modalConfig.copyName = title + '_Copy';
                   
                    Kooboo.Table.copy(self.config,{
                        id: copyData.id,
                        name: this.modalConfig.copyName
                    },function(success){
                        if(success){
                            self.showModal=false;
                            self.$emit("close");
                            info.done(Kooboo.text.info.copy.success);
                        }else{
                            info.fail(Kooboo.text.info.copy.fail);
                        }
                    })
                    
                }
            },
            onShow:function(){
                var self=this;
                var copyData = self.config.docs.find(function(doc) {
                    return doc.id == self.selectedDocs[0];
                })

                if (copyData) {
                    this.modalConfig.title += (': ' + copyData.name.text);
                    this.modalConfig.copyName = copyData.name.text + '_Copy'
                }
            },
            onClose: function() {
                this.showModal = false;
                this.$emit("close");
            }
        },
        watch:{
            show:function(show){
                if(show){
                    this.showModal=true;
                }
            },

        },
        components: {
            'kb-modal': Kooboo.vue.component.kbModal
        },
        template: Kooboo.getTemplate('/_Admin/Scripts/vue/components/kbCopyModal/index.html')
    })
})()