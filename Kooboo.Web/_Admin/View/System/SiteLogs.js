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
            name: Kooboo.text.common.siteLogs
          }
        ],
        pager: {},
        selected: [],
        showCheckoutDialog: false,
        domains: [],
        checkoutRootDomain: "",
        tableData: [],
        checkoutForm: {
          siteName: "",
          subDomain: "",
          rootDomain: ""
        },
        checkoutRules: {
          siteName: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              pattern: /^[A-Za-z][\w\-]*$/,
              message: Kooboo.text.validation.siteNameInvalid
            },
            {
              message: Kooboo.text.validation.taken,
              remote: {
                url: Kooboo.Site.isUniqueName(),
                type: "GET",
                data: function() {
                  return {
                    name: self.checkoutForm.siteName
                  };
                }
              }
            }
          ],
          subDomain: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              pattern: /^[A-Za-z][\w\-]*$/,
              message: Kooboo.text.validation.siteNameInvalid
            },
            {
              message: Kooboo.text.validation.taken,
              remote: {
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                type: "GET",
                data: function() {
                  return {
                    SubDomain: self.checkoutForm.subDomain,
                    RootDomain: function() {
                      if (!self.checkoutForm.rootDomain) {
                        self.checkoutForm.rootDomain =
                          self.domains[0].domainName;
                      }
                      return self.checkoutForm.rootDomain;
                    }
                  };
                }
              }
            },
            {
              min: 1,
              max: 63,
              message:
                Kooboo.text.validation.minLength +
                1 +
                ", " +
                Kooboo.text.validation.maxLength +
                63
            }
          ]
        }
      };
    },
    mounted: function() {
      self.getList();
      Kooboo.Domain.getAvailable().then(function(res) {
        if (res.success) {
          self.domains = res.model;
          self.checkoutForm.rootDomain =
            self.domains[0] && self.domains[0].domainName;
        }
      });
    },
    methods: {
      getList: function(page) {
        Kooboo.SiteLog.getList({
          pageNr: page
        }).then(function(res) {
          if (res.success) {
            // location.hash = "page=" + data.pageNr;
            var data = res.model;
            self.pager = data;
            var logList = [];
            data.list.forEach(function(log) {
              var date = new Date(log.lastModified);
              var model = {
                id: log.id,
                name: {
                  text: log.itemName,
                  url: Kooboo.Route.Get(
                    Kooboo.Route.SiteLog.LogVersions,
                    Object.assign(
                      {
                        Id: log.id,
                        KeyHash: log.keyHash,
                        StoreNameHash: log.storeNameHash
                      },
                      log.hasOwnProperty("tableNameHash")
                        ? {
                            tableNameHash: log.tableNameHash
                          }
                        : {}
                    )
                  )
                },
                type: {
                  text: log.storeName
                    ? Kooboo.text.component.table[
                        log.storeName.toLowerCase()
                      ] || log.storeName
                    : "",
                  class: "label-sm label-primary"
                },
                action: {
                  text:
                    Kooboo.text.action[log.actionType.toLowerCase()] ||
                    log.actionType,
                  class:
                    "label-sm " +
                    (log.actionType.toLowerCase() == "add"
                      ? "label-success"
                      : log.actionType.toLowerCase() == "update"
                      ? "blue"
                      : "label-danger")
                },
                user: log.userName,
                date: date.toDefaultLangString(),
                viewVersion: {
                  iconClass: "fa-clock-o",
                  title: Kooboo.text.common.viewAllVersions,
                  url: Kooboo.Route.Get(
                    Kooboo.Route.SiteLog.LogVersions,
                    Object.assign(
                      {
                        KeyHash: log.keyHash,
                        StoreNameHash: log.storeNameHash
                      },
                      log.hasOwnProperty("tableNameHash")
                        ? {
                            tableNameHash: log.tableNameHash
                          }
                        : {}
                    )
                  ),
                  newWindow: true
                }
              };
              logList.push(model);
            });
            self.tableData = logList;
          }
        });
      },
      blame: function() {
        Kooboo.SiteLog.Blame(JSON.stringify(self.selectedIds)).then(function(
          res
        ) {
          if (res.success) {
            location.reload();
          }
        });
      },
      onBlameChanges: function() {
        if (self.selected.length > 1) {
          if (confirm(Kooboo.text.confirm.siteLogs.blame)) {
            self.blame();
          }
        } else {
          self.blame();
        }
      },
      onRestoreToPoint: function() {
        if (confirm(Kooboo.text.confirm.siteLogs.restore)) {
          Kooboo.SiteLog.Restore({
            id: self.selected[0].id
          }).then(function(res) {
            if (res.success) {
              location.reload();
            }
          });
        }
      },
      showCheckoutDialogClick: function() {
        self.showCheckoutDialog = true;
      },
      exportIncrementPackage: function() {
        Kooboo.SiteLog.ExportBatch({
          id: self.selected[0].id
        }).then(function(res) {
          if (res.success) {
            window.open(
              Kooboo.Route.Get(Kooboo.SiteLog.DownloadPageUrl(), {
                id: res.model
              })
            );
          }
        });
      },
      exportItem: function() {
        Kooboo.SiteLog.ExportItem({
          id: self.selected[0].id
        }).then(function(res) {
          if (res.success) {
            window.open(
              Kooboo.Route.Get(Kooboo.SiteLog.DownloadPageUrl(), {
                id: res.model
              })
            );
          }
        });
      },
      exportItems: function() {
        Kooboo.SiteLog.ExportItems({
          ids: self.selectedIds
        }).then(function(res) {
          if (res.success) {
            window.open(
              Kooboo.Route.Get(Kooboo.SiteLog.DownloadPageUrl(), {
                id: res.model
              })
            );
          }
        });
      },
      checkoutDialogSubmit: function() {
        var isValid = self.$refs.checkoutForm.validate();
        if (isValid) {
          Kooboo.SiteLog.CheckOut({
            id: self.selected[0].id,
            SiteName: self.checkoutForm.siteName,
            SubDomain: self.checkoutForm.subDomain,
            RootDomain: self.checkoutForm.rootDomain
          }).then(function(res) {
            if (res.success) {
              self.checkoutDialogReset();
              window.info.show(Kooboo.text.info.checkout.success, true);
            } else {
              window.info.show(Kooboo.text.info.checkout.fail, false);
            }
          });
        }
      },
      checkoutDialogReset: function() {
        self.showCheckoutDialog = false;
        self.checkoutForm.siteName = "";
        self.checkoutForm.subDomain = "";
        self.$refs.checkoutForm.clearValid();
      }
    },
    computed: {
      selectedIds: function() {
        return self.selected.map(function(s) {
          return s.id;
        });
      }
    },
    watch: {
      "checkoutForm.siteName": function(siteName) {
        self.checkoutForm.subDomain = _.kebabCase(siteName);
      }
    },
    beforeDestory: function() {
      self = null;
    }
  });
});
