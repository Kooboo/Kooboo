(function() {
    Kooboo.loadJS([
        '/_Admin/Scripts/vue/components/kbModal/index.js',
    ]);
    Kooboo.vue.component.kbTableModal = Vue.component('kb-table-modal', {
        props: {
            config: Object,
            show: Boolean
        },
        data: function() {
            return {
                detailData:{
                    docs:[],
                    cols:[],
                    rowActions:[],
                    actions:[],
                    selectable:false,
                    modelName:''
                },
                showModal: false,
                relations: [],
                modalConfig: {
                    title: '',
                    size: '',
                    showCloseBtn: true,
                    closeBtnText: Kooboo.text.common.OK,
                    onHiden: function() {
                        
                    }
                }
            }
        },
        methods: {
            onClose: function() {
                this.showModal = false;
                this.$emit('close')
            }
        },
        watch: {
            config:function(config){
                var detailConfigs= this.config.detailConfigs;
                if(detailConfigs && detailConfigs.length>0){
                    var detailConfig=_.find(detailConfigs,function(detailConfig){
                        if(detailConfig.name=="tableData")
                            return true;
                        return false;
                    });
                    var titleField=_.find(detailConfigs,function(detailConfig){
                        if(detailConfig.name=="title")
                            return true;
                        return false;
                    });
                    if(titleField &&titleField.value){
                        this.modalConfig.title=titleField.value;
                    }
                    

                    var apiField=_.find(detailConfigs,function(detailConfig){
                        if(detailConfig.name=="api")
                            return true;
                        return false;
                    });
                    this.detailData= detailConfig.value;
                    if(apiField && apiField.value){
                        var api=apiField.value;
                        var para=this.config.data;
                        var self=this;
                        Kooboo.Table.execApi(api,para,function(data){
                            Kooboo.Table.setDocs(data,self.detailData);
                        })
                        
                    }
                    
                }
            },
            show: function(show) {
                if(show){
                    this.showModal = true;
                }

            }
        },
        components: {
            'kb-modal': Kooboo.vue.component.kbModal,
            'kb-table':Kooboo.vue.component.kbTable
        },
        template: Kooboo.getTemplate('/_Admin/Scripts/vue/components/kbTableModal/index.html')
    })
})()