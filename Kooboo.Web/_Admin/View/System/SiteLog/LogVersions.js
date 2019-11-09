$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.siteLogs,
            url: Kooboo.Route.Get(Kooboo.Route.SiteLog.ListPage)
          },
          {
            name: Kooboo.text.common.logVersions
          }
        ],
        selected: [],
        tableData: []
      };
    },
    mounted: function() {
      self.getList();
    },
    methods: {
      getList: function() {
        Kooboo.SiteLog.Versions(
          Object.assign(
            {
              keyHash: Kooboo.getQueryString("KeyHash"),
              storeNameHash: Kooboo.getQueryString("StoreNameHash")
            },
            Kooboo.getQueryString("tableNameHash")
              ? {
                  tableNameHash: Kooboo.getQueryString("tableNameHash")
                }
              : {}
          )
        ).then(function(res) {
          if (res.success) {
            var verList = [];
            _.forEach(res.model, function(version, idx) {
              var date = new Date(version.lastModified);

              var model = {
                id: version.id,
                version:
                  version.id +
                  (idx == 0
                    ? " [" + Kooboo.text.site.siteLog.currentVersion + "]"
                    : ""),
                user: {
                  text: version.userName,
                  class: "label-sm label-info"
                },
                date: date.toDefaultLangString()
              };

              verList.push(model);
            });
            self.tableData = verList;
          } else {
            window.info.show(Kooboo.text.info.versionLogParameterMissing);
            setTimeout(function() {
              location.href = Kooboo.Route.Get(Kooboo.Route.SiteLog.ListPage);
            }, 4000);
          }
        });
      },
      onUndo: function() {
        Kooboo.SiteLog.Revert({
          id: self.selected[0].id
        }).then(function(res) {
          if (res.success) {
            location.reload();
          }
        });
      }
    },
    computed: {
      compareLink: function() {
        var items = _.sortBy(self.selected, [
          function(ver) {
            return ver.id;
          }
        ]);
        switch (items.length) {
          case 1:
          case 2:
            return Kooboo.Route.Get(Kooboo.Route.SiteLog.VersionsCompare, {
              id1: items[0].id,
              id2: items.length == 1 ? -1 : items[1].id,
              KeyHash: Kooboo.getQueryString("KeyHash"),
              StoreNameHash: Kooboo.getQueryString("StoreNameHash"),
              tableNameHash: Kooboo.getQueryString("tableNameHash")
            });
          default:
            return "javascript:;";
        }
      }
    },
    beforeDestory: function() {
      self = null;
    }
  });
});
