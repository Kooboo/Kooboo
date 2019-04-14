(function() {
    Kooboo.vue.component.kbBreadcrumbItem = Vue.component('kb-breadcrumb-item', {
        props: {
            item: [String, Object],
            active: Boolean
        },
        data: function() {
            return {
                obj: {}
            }
        },
        mounted: function() {
            var obj = {};
            if (typeof this.item == 'string') {
                switch (this.item) {
                    case 'SITES':
                        obj = {
                            name: Kooboo.text.component.breadCrumb.sites,
                            url: Kooboo.Route.Site.ListPage
                        }
                        break;
                    case 'DASHBOARD':
                        obj = {
                            name: Kooboo.text.component.breadCrumb.dashboard,
                            url: Kooboo.Route.Get(Kooboo.Route.Site.DetailPage)
                        }
                        break;
                    case 'LAYOUTS':
                        obj = {
                            name: Kooboo.text.common.Layouts,
                            url: Kooboo.Route.Layout.ListPage
                        }
                        break;
                    case 'VIEWS':
                        obj = {
                            name: Kooboo.text.common.Views,
                            url: Kooboo.Route.View.ListPage
                        };
                        break;
                    case 'MENUS':
                        obj = {
                            name: Kooboo.text.common.Menus,
                            url: Kooboo.Route.Menu.ListPage
                        };
                        break;
                    case 'SCIRPTS':
                        obj = {
                            name: Kooboo.text.common.Scripts,
                            url: Kooboo.Route.Script.ListPage
                        };
                        break;
                    case 'STYLES':
                        obj = {
                            name: Kooboo.text.common.Styles,
                            url: Kooboo.Route.Style.ListPage
                        };
                        break;
                    case 'FORMS':
                        obj = {
                            name: Kooboo.text.common.Forms,
                            url: Kooboo.Route.Form.ListPage
                        };
                        break;
                }
            } else {
                obj = this.item;
            }

            this.obj = obj;
        },
        watch: {
            item: function(item) {
                this.obj = item;
            }
        },
        template: Kooboo.getTemplate('/_Admin/Scripts/vue/components/kbBreadcrumb/item/index.html')
    })
})()