$(function() {
  var versionViewModel = function() {
    var self = this;

    this.selected = ko.observableArray();

    this.onUndo = function() {
      Kooboo.SiteLog.Revert({
        id: self.selected()[0].id()
      }).then(function(res) {
        if (res.success) {
          location.reload();
        }
      });
    };

    this.compareLink = ko.pureComputed(function() {
      var items = _.sortBy(self.selected(), [
        function(ver) {
          return ver.id();
        }
      ]);
      switch (items.length) {
        case 1:
        case 2:
          return Kooboo.Route.Get(Kooboo.Route.SiteLog.VersionsCompare, {
            id1: items[0].id(),
            id2: items.length == 1 ? -1 : items[1].id(),
            KeyHash: Kooboo.getQueryString("KeyHash"),
            StoreNameHash: Kooboo.getQueryString("StoreNameHash"),
            tableNameHash: Kooboo.getQueryString("tableNameHash")
          });
        default:
          return "javascript:;";
      }
    });

    Kooboo.EventBus.subscribe("ko/table/docs/selected", function(selected) {
      self.selected(selected);
    });
  };
  versionViewModel.prototype = new Kooboo.tableModel(Kooboo.SiteLog.name);

  var vm = new versionViewModel();

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

      var columns = [
        {
          displayName: Kooboo.text.site.siteLog.version,
          fieldName: "version",
          type: "text"
        },
        {
          displayName: Kooboo.text.common.user,
          fieldName: "user",
          type: "label"
        },
        {
          displayName: Kooboo.text.common.date,
          fieldName: "date",
          type: "text"
        }
      ];

      var cpnt = {
        docs: verList,
        columns: columns,
        kbType: Kooboo.SiteLog.name
      };

      vm.tableData(cpnt);
    } else {
      window.info.show(Kooboo.text.info.versionLogParameterMissing);
      setTimeout(function() {
        location.href = Kooboo.Route.Get(Kooboo.Route.SiteLog.ListPage);
      }, 4000);
    }
  });

  ko.applyBindings(vm, document.getElementById("main"));
});
