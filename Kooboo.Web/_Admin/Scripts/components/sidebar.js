$(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/sidebar.html");

    ko.components.register("sidebar", {
        viewModel: function(params) {
            var self = this;
            this.hasSiteName = Kooboo.getQueryString("siteId") || false;
            var sideType = params.sideType || "sitemenu";
            var getSideBar = null;
            if (sideType === "sitemenu") {
                getSideBar = Kooboo.Sidebar.getSidebar
            } else if (sideType === "extension") {
                getSideBar = Kooboo.Sidebar.getExtensionSidebar
                this.hasSiteName = false;
            } else if (sideType === "domain") {
                getSideBar = Kooboo.Sidebar.getDomainSidebar
                this.hasSiteName = false;
            }

            this.sidebar = ko.observableArray();
            this.siteName = ko.observable();
            var level = 1;

            function isUrlMapping(url, locationUrl) {
                return url ? (url.toLowerCase() === locationUrl.toLowerCase()) : false;
            }

            function isCurrentMenuActive(menu, locationUrl) {
                var active = false;
                if (menu.url) {
                    active = active || isUrlMapping(menu.url, locationUrl);
                }

                if (menu.items.length) {
                    menu.items.forEach(function(subMenu) {
                        active = active || isCurrentMenuActive(subMenu, locationUrl);
                    })
                    return active;
                } else {
                    return isUrlMapping(menu.url, locationUrl);
                }
            }

            function dataMapping(menu) {
                var locationUrl = location.pathname + location.search;
                menu.forEach(function(itemLevelOne) {
                    itemLevelOne.level = level;
                    if (isUrlMapping(itemLevelOne.url, locationUrl)) {
                        itemLevelOne.current = true;
                    } else {
                        itemLevelOne.current = false;
                    }
                    if (itemLevelOne.items instanceof Array && itemLevelOne.items.length > 0) {
                        itemLevelOne.active = isCurrentMenuActive(itemLevelOne, locationUrl);

                        if (itemLevelOne.name == "Contents" && itemLevelOne.current) {
                            itemLevelOne.active = true;
                        }

                        if (itemLevelOne.active === true) return;

                        if (itemLevelOne.items.some(function(itemLevelTwo) {
                                return itemLevelTwo.items instanceof Array && itemLevelTwo.items.length > 0
                            })) {
                            var active = false;
                            itemLevelOne.items.forEach(function(itemLevelTwo) {
                                itemLevelTwo.items.forEach(function(itemLevelThree) {
                                    if (isUrlMapping(itemLevelThree.url, locationUrl)) {
                                        active = true;
                                        itemLevelTwo.active = true
                                    }
                                })
                            });
                            itemLevelOne.active = active;
                        }
                    } else {
                        itemLevelOne.active = false;
                    }
                });
                level++;
                if (menu instanceof Array && menu.length > 0) {
                    menu.forEach(function(item) {
                        dataMapping(item.items);
                    });
                }
            }

            getSideBar().then(function(res) {
                if (res.success) {
                    dataMapping(res.model);
                    self.sidebar(res.model);
                }
            });

            if (self.hasSiteName) {
                Kooboo.Site.getName().then(function(res) {
                    if (res.success) {
                        self.siteName(res.model);
                    } else {
                        setTimeout(function() {
                            location.href = Kooboo.Route.Get(Kooboo.Route.Site.ListPage);
                        }, 600);
                    }
                });
            }

            this.dashboardUrl = ko.pureComputed(function() {
                return Kooboo.Route.Get(Kooboo.Route.Site.DetailPage);
            })

            this.SPAClick = _.debounce(function(m, e) {
                if (history) {
                    e.preventDefault();
                    var path = (m.dashboardUrl().toLowerCase().indexOf("?siteid=") > -1) ? m.dashboardUrl().toLowerCase().split("?siteid=")[0] : m.dashboardUrl();
                    if (location.pathname.toLowerCase() !== path ||
                        !Kooboo.isSameURLParams(Kooboo.getURLParams(m.dashboardUrl()), Kooboo.getURLParams(location.search))) {
                        Kooboo.SPA.getView(m.dashboardUrl());
                    }
                } else {
                    return true;
                }
            })

            Kooboo.EventBus.subscribe("kb/sidebar/refresh", function() {
                getSideBar().then(function(res) {
                    if (res.success) {
                        level = 1;
                        dataMapping(res.model);
                        self.sidebar(res.model);
                    }
                });
            })
        },
        template: template
    });

    function SidebarViewModel() {}

    if ($("#sidebarDiv").length > 0) {
        ko.applyBindings(new SidebarViewModel, $("#sidebarDiv")[0]);
    }
})