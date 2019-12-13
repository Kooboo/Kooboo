(function() {
  Vue.component("sidebar", {
    template: Kooboo.getTemplate("/_Admin/Scripts/components/sidebar.html"),
    props: {
      siteType: String
    },
    data: function() {
      return {
        hasSiteName: Kooboo.getQueryString("siteId") || false,
        siteName: "",
        level: 1,
        sidebar: []
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.EventBus.subscribe("kb/sidebar/refresh", function() {
        self
          .getSideBar()()
          .then(function(res) {
            if (res.success) {
              self.level = 1;
              self.dataMapping(res.model);
              self.sidebar = res.model;
            }
          });
      });

      self
        .getSideBar()()
        .then(function(res) {
          if (res.success) {
            self.dataMapping(res.model);
            self.sidebar = res.model;
          }
        });

      if (self.hasSiteName) {
        Kooboo.Site.getName().then(function(res) {
          if (res.success) {
            self.siteName = res.model;
          } else {
            setTimeout(function() {
              location.href = Kooboo.Route.Get(Kooboo.Route.Site.ListPage);
            }, 600);
          }
        });
      }
    },
    computed: {
      innerSideType: function() {
        return this.siteType || "sitemenu";
      },
      dashboardUrl: function() {
        return Kooboo.Route.Get(Kooboo.Route.Site.DetailPage);
      }
    },
    methods: {
      getSideBar: function() {
        if (this.innerSideType === "sitemenu") {
          return Kooboo.Sidebar.getSidebar;
        } else if (this.innerSideType === "extension") {
          this.hasSiteName = false;
          return Kooboo.Sidebar.getExtensionSidebar;
        } else if (this.innerSideType === "domain") {
          this.hasSiteName = false;
          return Kooboo.Sidebar.getDomainSidebar;
        } else if (this.innerSideType === "market") {
          this.hasSiteName = false;
          return Kooboo.Sidebar.getMarketSidebar;
        }
      },
      isUrlMapping: function(url, locationUrl) {
        return url ? url.toLowerCase() === locationUrl.toLowerCase() : false;
      },
      isCurrentMenuActive: function(menu, locationUrl) {
        var self = this;
        var active = false;
        if (menu.url) {
          active = active || self.isUrlMapping(menu.url, locationUrl);
        }

        if (menu.items.length) {
          menu.items.forEach(function(subMenu) {
            active = active || self.isCurrentMenuActive(subMenu, locationUrl);
          });
          return active;
        } else {
          return self.isUrlMapping(menu.url, locationUrl);
        }
      },
      dataMapping: function(menu) {
        var self = this;
        var locationUrl = location.pathname + location.search;
        menu.forEach(function(itemLevelOne) {
          itemLevelOne.level = self.level;
          if (self.isUrlMapping(itemLevelOne.url, locationUrl)) {
            itemLevelOne.current = true;
          } else {
            itemLevelOne.current = false;
          }
          if (
            itemLevelOne.items instanceof Array &&
            itemLevelOne.items.length > 0
          ) {
            itemLevelOne.active = self.isCurrentMenuActive(
              itemLevelOne,
              locationUrl
            );

            if (itemLevelOne.name == "Contents" && itemLevelOne.current) {
              itemLevelOne.active = true;
            }

            if (itemLevelOne.active === true) return;

            if (
              itemLevelOne.items.some(function(itemLevelTwo) {
                return (
                  itemLevelTwo.items instanceof Array &&
                  itemLevelTwo.items.length > 0
                );
              })
            ) {
              var active = false;
              itemLevelOne.items.forEach(function(itemLevelTwo) {
                itemLevelTwo.items.forEach(function(itemLevelThree) {
                  if (self.isUrlMapping(itemLevelThree.url, locationUrl)) {
                    active = true;
                    itemLevelTwo.active = true;
                  }
                });
              });
              itemLevelOne.active = active;
            }
          } else {
            itemLevelOne.active = false;
          }
        });
        self.level += 1;
        if (menu instanceof Array && menu.length > 0) {
          menu.forEach(function(item) {
            self.dataMapping(item.items);
          });
        }
      },
      SPAClick: function() {
        if (history) {
          var path =
            this.dashboardUrl.toLowerCase().indexOf("?siteid=") > -1
              ? this.dashboardUrl.toLowerCase().split("?siteid=")[0]
              : this.dashboardUrl;
          if (
            location.pathname.toLowerCase() !== path ||
            !Kooboo.isSameURLParams(
              Kooboo.getURLParams(this.dashboardUrl),
              Kooboo.getURLParams(location.search)
            )
          ) {
            Kooboo.SPA.getView(this.dashboardUrl);
          }
        } else {
          return true;
        }
      }
    }
  });
})();
