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
            name: Kooboo.text.common.sync
          }
        ],
        remoteSiteModel: {
          remoteSiteName: "",
          preDomain: "",
          suffixDomain: ""
        },
        remoteSiteRules: {
          remoteSiteName: [
            {
              pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
              message: Kooboo.text.validation.siteNameInvalid
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
          ],
          preDomain: [
            {
              pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
              message: Kooboo.text.validation.siteNameInvalid
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
        },
        pushTypes: [
          {
            displayName: Kooboo.text.site.sync.pushType.all,
            value: "all"
          },
          {
            displayName: Kooboo.text.site.sync.pushType.update,
            value: "update"
          }
        ],
        tableData: [],
        tableDataSelected: [],
        allServers: [],
        isShowModal: false,
        isShowManageServerModal: false,
        editableServers: [],
        currentServer: undefined,
        ableAddNewServer: true,
        isNextStep: false,
        avaliableSites: [],
        selectedSite: "",
        showCreateSiteModal: false,
        ableAddSite: false,
        selectedPushType: undefined,
        avaliableRemoteDomain: []
      };
    },
    created: function() {
      self = this;
      this.getTableData();
    },
    methods: {
      getTableData: function() {
        Kooboo.Publish.getList().then(function(data) {
          self.tableData = data.model;
        });
      },
      editEditableServers: function(event, row) {
        row.editable = true;
        this.$forceUpdate();
      },
      pushTypeRadioChange: function(event, pushType) {
        this.selectedPushType = pushType;
      },
      cancelEditEditableServers: function(event, row) {
        row.editable = false;
        //The change of row  or this.editableServers`s children  not arouse kb-table`s view update
        //unless The change of this.editableServers.length
        this.$forceUpdate();
        self.ableAddNewServer = true;
      },

      saveEditableServers: function(event, row) {
        Kooboo.UserPublish.updateServer({
          id: row.id,
          name: row.name,
          serverUrl: row.serverUrl
        }).then(function(res) {
          if (res.success) {
            Kooboo.EventBus.publish("server/list/refresh/needed");
            window.info.done(Kooboo.text.info.update.success);
            row.editable = false;
            self.$forceUpdate();
            self.ableAddNewServer = true;
          } else {
            window.info.fail(Kooboo.text.info.update.fail);
          }
        });
      },
      addNewAbleServersHandle: function() {
        var newServer = {
          id: Kooboo.Guid.Empty,
          name: "",
          serverUrl: "",
          reserved: false,
          editable: true
        };
        self.editableServers.push(newServer);
        self.ableAddNewServer = false;
      },
      nextStepHandle: function() {
        this.getAvaliableSites();
      },
      deleteEditableServers: function(event, row) {
        Kooboo.UserPublish.deleteServer({
          id: row.id
        }).then(function(res) {
          if (res.success) {
            window.info.done(Kooboo.text.info.delete.success);
            self.editableServers = _.remove(self.editableServers, function(
              item
            ) {
              return item.id === row.id;
            });
          } else {
            window.info.fail(Kooboo.text.info.delete.fail);
          }
        });
      },
      getConfirmMessage: function(doc) {
        if (doc.relations) {
          doc.relationsTypes = _.sortBy(Object.keys(doc.relations));
        }
        var find = _.find(doc, function(item) {
          return item.relations && Object.keys(item.relations).length;
        });

        if (!!find) {
          return Kooboo.text.confirm.deleteItemsWithRef;
        } else {
          return Kooboo.text.confirm.deleteItems;
        }
      },
      onDelete: function() {
        if (confirm(this.getConfirmMessage(this.tableDataSelected))) {
          var ids = this.tableDataSelected.map(function(m) {
            return m.id;
          });
          Kooboo.Publish.Deletes({
            ids: ids
          }).then(function(res) {
            if (res.success) {
              window.info.done(Kooboo.text.info.enable.success);
              self.getTableData();
            } else {
              window.info.fail(Kooboo.text.info.enable.failed);
            }
          });
        }
      },
      showModalHandle: function() {
        this.getModalData();
      },
      showCreateSiteModalHandle: function() {
        var url = this.currentServer.serverUrl;
        Kooboo.UserPublish.getRemoteDomains({
          serverUrl: url
        }).then(function(res) {
          if (res.success) {
            self.avaliableRemoteDomain = res.model.map(function(dm) {
              return {
                id: dm.id,
                domainName: "." + dm.domainName
              };
            });
            self.remoteSiteModel.suffixDomain =
              self.avaliableRemoteDomain[0].domainName;
            self.showCreateSiteModal = true;
          }
        });
      },
      remoteSiteNameChangeHandle: function(event) {
        self.remoteSiteModel.remoteSiteName = event.target.value;
        self.remoteSiteModel.preDomain = self.remoteSiteModel.remoteSiteName;
      },
      hideCreateSiteModalHandle: function() {
        self.showCreateSiteModal = false;
      },
      saveHandle: function() {
        var params = {
          remoteServerUrl: self.currentServer.serverUrl,
          remoteWebSiteId: self.selectedSite.id,
          remoteSiteName: self.selectedSite.name,
          pushType: self.selectedPushType.value
        };

        if (params.remoteWebSiteId === Kooboo.Guid.Empty) {
          params.siteName = self.remoteSiteModel.remoteSiteName;
          params.fullDomain =
            self.remoteSiteModel.preDomain + self.remoteSiteModel.suffixDomain;
        }
        Kooboo.Publish.post(params).then(function(res) {
          if (res.success) {
            self.getTableData();
            self.hidelModalHandle();
          }
        });
      },
      hidelModalHandle: function() {
        this.isShowModal = false;
      },
      createRemoteSite: function() {
        //需要验证
        console.log(this.$refs.createSiteForm);
        var validateStatus = this.$refs.createSiteForm.validate();

        //add new Site to avaliableSites
        if (validateStatus) {
          var newSite = {
            id: Kooboo.Guid.Empty,
            name: self.remoteSiteModel.remoteSiteName
          };
          self.avaliableSites.push(newSite);
          self.selectedSite = newSite;
          this.hideCreateSiteModalHandle();
        }
      },
      getAvaliableSites: function() {
        var url = this.currentServer.serverUrl;
        Kooboo.Publish.getRemoteSiteList({
          remoteUrl: url
        }).then(function(res) {
          if (res.success) {
            self.avaliableSites = res.model;
            self.selectedSite = self.avaliableSites[0];
            self.isNextStep = true;
            self.selectedPushType = self.pushTypes[0];
          }
        });
      },

      remoteSiteClickHandle: function(event, id) {
        event.stopPropagation();
        this.getDetailList(id);
      },
      manageServerHandle: function() {
        this.showManageServerModal();
      },
      showManageServerModal: function() {
        this.isShowManageServerModal = true;
      },
      hideManageServerModal: function() {
        this.isShowManageServerModal = false;
        this.getModalData();
      },
      getModalData: function() {
        Kooboo.UserPublish.getList().then(function(res) {
          if (res.success) {
            self.currentServer = res.model[0];

            res.model.forEach(function(m) {
              m.editable = false;
            });
            self.allServers = res.model;

            self.editableServers = self.allServers.filter(function(item) {
              return !item.reserved;
            });
            self.isShowModal = true;
          }
        });
      },
      getDetailList: function(id) {
        var data = _.find(self.tableData, function(item) {
          return item.id === id;
        });
        var href = Kooboo.Route.Get(Kooboo.Route.Publish.DetailList, {
          Id: data.id
        });
        this.SPAClick(href, id);
      },
      SPAClick: function(url, id) {
        location.hash = id;
        var path =
          url.toLowerCase().indexOf("?siteid=") > -1
            ? url.toLowerCase().split("?siteid=")[0]
            : url;
        if (
          location.pathname.toLowerCase() !== path ||
          !Kooboo.isSameURLParams(
            Kooboo.getURLParams(url),
            Kooboo.getURLParams(location.search)
          )
        ) {
          Kooboo.SPA.getView(url);
        }
      }
    }
  });
});
