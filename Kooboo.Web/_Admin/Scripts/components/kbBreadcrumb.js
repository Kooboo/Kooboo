(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbBreadcrumb.html");

    ko.components.register("kb-breadcrumb", {
        viewModel: function(modifiedCrumb) {

            var self = this;

            this.breads = ko.observableArray();

            var current = _.startCase(location.pathname.split("/").reverse()[0]),
                multiLang = Kooboo.getQueryString("lang");

            this.SPAClick = function(m, e) {
                if (m.url.toLowerCase() == "/_admin/sites") {
                    return true;
                } else {
                    e.preventDefault();
                    var path = (m.url.toLowerCase().indexOf("?siteid=") > -1) ? m.url.toLowerCase().split("?siteid=")[0] : m.url;

                    if (path == "#") {
                        return false;
                    }

                    if (location.pathname.toLowerCase() !== path ||
                        !isSameParams(Kooboo.getURLParams(m.url), Kooboo.getURLParams(location.search))) {
                        Kooboo.SPA.getView(m.url);
                    }
                }
            }

            var breadCrumb = [{
                text: Kooboo.text.component.breadCrumb.sites,
                url: Kooboo.Route.Get(Kooboo.Route.Site.ListPage)
            }];

            if (current == "Site") {
                breadCrumb.push({
                    text: Kooboo.text.component.breadCrumb.dashboard,
                    url: ""
                })
            } else {

                breadCrumb.push({
                    text: Kooboo.text.component.breadCrumb.dashboard,
                    url: Kooboo.Route.Get(Kooboo.Route.Site.DetailPage)
                })
                if (location.pathname.split("/").length == 5) {
                    var _name = location.pathname.split("/").reverse()[1];
                    breadCrumb.push({
                        text: _name,
                        url: Kooboo.Route.Get(Kooboo.Route[_name].ListPage)
                    })
                }

                multiLang && breadCrumb.push({
                    text: multiLang,
                    url: "#"
                })

                breadCrumb.push({
                    text: current,
                    url: ""
                });
            }

            !modifiedCrumb && self.breads(breadCrumb);

            if (modifiedCrumb) {
                var _bc = [];
                _.forEach(modifiedCrumb, function(crumb) {
                    switch (crumb.name) {
                        case 'SITES':
                            _bc.push({
                                text: Kooboo.text.component.breadCrumb.sites,
                                url: Kooboo.Route.Get(Kooboo.Route.Site.ListPage)
                            })
                            break;
                        case 'DASHBOARD':
                            _bc.push({
                                text: Kooboo.text.component.breadCrumb.dashboard,
                                url: Kooboo.Route.Get(Kooboo.Route.Site.DetailPage)
                            })
                            break;
                        case 'MARKET':
                            _bc.push({
                                text: 'Market',
                                url: '/_Admin/Market/Index'
                            })
                            break;
                        default:
                            _bc.push({
                                text: crumb.name,
                                url: crumb.url
                            })
                            break;
                    }
                });

                self.breads(_bc);
            }
        },
        template: template
    });
})();