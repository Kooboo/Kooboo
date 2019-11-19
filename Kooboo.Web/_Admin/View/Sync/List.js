$(function() {
  var self;

  new Vue({
    el: "#app",
    data: function() {
      return {
        breads: [
          {
            name: Kooboo.text.component.breadCrumb.sites
          },
          {
            name: Kooboo.text.component.breadCrumb.dashboard
          },
          {
            name: Kooboo.text.common.KConfig,
            url: Kooboo.Route.Get(Kooboo.Route.Publish.ListPage)
          },
          { name: Kooboo.text.site.sync.listName }
        ],
        settingId: Kooboo.getQueryString("id") || location.hash.split("#")[1],
        tabIndex: 0,
        isPulling: false,
        isPushing: false,
        pushItems: [],
        selectedItems: [],
        dynamicItems: [],
        pager: undefined,
        CONTIUNE_PULLING: true
      };
    },
    created: function() {
      self = this;
      self.getPushItems();
    },
    watch: {
      tabIndex: function(value) {
        switch (value) {
          case 0:
            self.getPushItems();
            break;
          case 1:
            self.getPullLog();
            break;
          case 2:
            self.getPushLogs();
            break;
        }
      }
    },
    methods: {
      pushToRemote: function() {
        if (self.selectedItems.length > 0) {
          self.isPushing = true;
          var ids = [];
          ids = self.selectedItems.map(function(item) {
            return item.id;
          });

          Kooboo.Publish.push({
            id: self.settingId,
            logids: JSON.stringify(ids)
          }).then(function(res) {
            if (res.success) {
              window.info.show(Kooboo.text.info.push.success, true);
              self.selectedItems = [];
              localStorage.clear();
              self.getPushLogs(function() {
                self.tabIndex = 2;
              });
            } else {
              window.info.show(Kooboo.text.info.push.fail, false);
            }
            self.isPushing = false;
          });
        } else {
          alert(Kooboo.text.alert.selectBeforePushing);
        }
      },
      getPushItems: function() {
        Kooboo.Publish.pushItems({
          id: self.settingId
        }).then(function(res) {
          if (res.success) {
            self.pushItems = res.model;
          }
        });
      },
      getPushLogs: function(callback) {
        Kooboo.Publish.pushLog({
          SyncSettingId: self.settingId
        }).then(function(res) {
          if (res.success) {
            self.dynamicItems = res.model.list;
            self.pager = res.model;
          }
          if (callback) {
            callback();
          }
        });
      },
      pollingPull: function(senderVersion) {
        if (self.CONTIUNE_PULLING) {
          Kooboo.Publish.pull({
            id: Kooboo.getQueryString("id"),
            senderVersion: senderVersion
          }).then(function(res) {
            if (!res.model.isFinish) {
              self.pollingPull(res.model.senderVersion);
            } else {
              self.isPulling = false;
              window.onbeforeunload = function() {};
              window.info.show(Kooboo.text.info.pull.success, true);
              self.tabIndex = 1;
              Kooboo.SPA.beforeUnload = undefined;
            }
          });
        }
      },
      onPullToLocal: function() {
        window.info.show(Kooboo.text.info.startPulling, true);
        self.pollingPull();
        self.isPulling = true;

        Kooboo.SPA.beforeUnload = function() {
          var flag = false;

          if (self.isPulling()) {
            flag = confirm(Kooboo.text.confirm.siteSynchronizing);
          }

          if (flag) {
            self.CONTIUNE_PULLING = false;
            return "refresh";
          }
        };
      },
      getPullLog: function(callback) {
        Kooboo.Publish.pullLog({
          SyncSettingId: self.settingId
        }).then(function(res) {
          if (res.success) {
            self.dynamicItems = res.model.list;
            self.pager = res.model;
            if (callback) {
              callback();
            }
          }
        });
      },
      getChangeClass: function(type) {
        var _class = "";
        switch (type.toLowerCase()) {
          case "add":
            _class = "green";
            break;
          case "update":
            _class = "blue";
            break;
          case "delete":
            _class = "red";
            break;
        }
        return _class;
      },
      changePage: function(index) {
        var name = undefined;
        if (self.tabIndex === 1) {
          name = "pullLog";
        } else if (self.tabIndex === 2) {
          name = "pushLog";
        }
        if (name) {
          Kooboo.Publish[name]({
            SyncSettingId: self.settingId,
            pageNr: index
          }).then(function(res) {
            if (res.success) {
              self.dynamicItems = res.model.list;
              self.pager = res.model;
            }
          });
        }
      }
    }
  });
});
