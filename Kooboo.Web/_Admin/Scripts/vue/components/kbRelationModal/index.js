(function() {
    Kooboo.loadJS([
        '/_Admin/Scripts/vue/components/kbModal/index.js'
    ]);

    Kooboo.vue.component.kbRelationModal = Vue.component('kb-relation-modal', {
        props: {
            config: Object,
            show: Boolean
        },
        data: function() {
            return {
                showModal: false,
                relations: [],
                modalConfig: {
                    title: Kooboo.text.common.Relation,
                    size: '',
                    showCloseBtn: true,
                    closeBtnText: Kooboo.text.common.OK,
                    onHiden: function() {
                        Kooboo.vue.component.kbRelationModal.relations = [];
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
            show: function(show) {
                if (show) {
                    var self = this;
                    Kooboo.Relation.showBy(this.config).then(function(res) {
                        if (res.success) {
                            self.relations = res.model && res.model.map(function(rel) {
                                var editUrl = '';
                                switch (self.config.by) {
                                    case "layout":
                                        editUrl = Kooboo.Route.Get(Kooboo.Route.Layout.DetailPage, {
                                            Id: rel.objectId
                                        });
                                        break;
                                    case "view":
                                        editUrl = Kooboo.Route.Get(Kooboo.Route.View.DetailPage, {
                                            Id: rel.objectId
                                        });
                                        break;
                                    case "page":
                                        editUrl = Kooboo.Route.Get(Kooboo.Route.Page.EditRedirector, {
                                            Id: rel.objectId
                                        });
                                        break;
                                    case "textcontent":
                                        editUrl = Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
                                            Id: rel.objectId
                                        });
                                        break;
                                    case "style":
                                    case "cssdeclaration":
                                        editUrl = Kooboo.Route.Get(Kooboo.Route.Style.DetailPage, {
                                            Id: rel.objectId
                                        });
                                        break;
                                    case "menu":
                                        editUrl = Kooboo.Route.Get(Kooboo.Route.Menu.DetailPage, {
                                            Id: rel.objectId
                                        });
                                        break;
                                    case "htmlblock":
                                        editUrl = Kooboo.Route.Get(Kooboo.Route.HtmlBlock.DetailPage, {
                                            Id: rel.objectId
                                        })
                                        break;
                                    case "form":
                                        editUrl = Kooboo.Route.Get(Kooboo.Route.Form.Redirector, {
                                            Id: rel.objectId
                                        })
                                        break;
                                    case "datamethodsetting":
                                        editUrl = Kooboo.Route.Get(Kooboo.Route.DataSource.DataMethodSetting, {
                                            Id: rel.objectId
                                        })
                                        break;
                                    default:
                                        editUrl = "";
                                        /*  window.info.show("Unhandle relation type: " + self.by(), false);
                                        console.warn("unhandle relation type:" + self.by());*/
                                        break;
                                }
                                return {
                                    url: rel.url,
                                    name: rel.name,
                                    remark: rel.remark,
                                    editUrl: editUrl
                                }
                            });
                            self.showModal = true;
                        }
                    })
                }
            }
        },
        components: {
            'kb-modal': Kooboo.vue.component.kbModal
        },
        template: Kooboo.getTemplate('/_Admin/Scripts/vue/components/kbRelationModal/index.html')
    })
})()