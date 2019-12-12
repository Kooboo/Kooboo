$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      var self = this;
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
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
            { required: Kooboo.text.validation.required },
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
            },
            {
              remote: {
                url: Kooboo.Site.isUniqueName(),
                data: function() {
                  return {
                    SiteName: self.remoteSiteModel.remoteSiteName
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ],
          preDomain: [
            { required: Kooboo.text.validation.required },
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
            },
            {
              remote: {
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                data: function() {
                  return {
                    SubDomain: self.remoteSiteModel.remoteSiteName,
                    RootDomain: self.remoteSiteModel.suffixDomain
                  };
                }
              },
              message: Kooboo.text.validation.taken
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
        avaliableRemoteDomain: [],
        validateModel: []
      };
    },
    created: function() {
      self = this;
      this.getTableData();
    },
    watch: {
      editableServers: {
        handler: function(value) {
          self.validateModel = [];
          value.forEach(function(item) {
            item.validateModel = {
              name: { msg: "", valid: true },
              serverUrl: { msg: "", valid: true }
            };
          });
        },
        deep: true
      },
      currentServer: function() {
        this.isNextStep = false;
      }
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
        self.selectedPushType = pushType;
      },
      cancelEditEditableServers: function(event, row) {
        if (row.isNew) {
          delete row.isNew;
          self.editableServers.pop();
        }
        row.editable = false;
        this.$forceUpdate();
        self.ableAddNewServer = true;
      },
      saveEditableServers: function(event, row) {
        var patternString =
          "^((https|http)?://)?" +
          "(([0-9]{1,3}.){3}[0-9]{1,3}" + // IP形式的URL- 199.194.52.184
          "|" + // 允许IP和DOMAIN（域名）
          "([0-9a-z_!~*'()-]+.)*" + // 域名- www.
          "([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]." + // 二级域名
          "[a-z]{2,6})" + // first level domain- .com or .museum
          "(:[0-9]{1,4})?"; // 端口- :80

        var pattern = new RegExp(patternString);
        var keyOptions = {
          name: [{ required: true, message: Kooboo.text.validation.required }],
          serverUrl: [
            { required: true, message: Kooboo.text.validation.required },
            {
              pattern: pattern,
              message: Kooboo.text.validation.urlInvalid
            }
          ]
        };
        var keyValues = {
          name: row.name,
          serverUrl: row.serverUrl
        };
        var validate = Kooboo.validate(keyValues, keyOptions);
        row.validateModel = validate.result;
        this.$forceUpdate();
        if (!validate.hasError) {
          Kooboo.UserPublish.updateServer({
            id: row.id,
            name: row.name,
            serverUrl: row.serverUrl
          }).then(function(res) {
            if (res.success) {
              Kooboo.EventBus.publish("server/list/refresh/needed");
              row.id = res.model;
              row.editable = false;
              row.isNew = false;
              self.$forceUpdate();
              self.ableAddNewServer = true;
              window.info.done(Kooboo.text.info.update.success);
            } else {
              window.info.fail(Kooboo.text.info.update.fail);
            }
          });
        }
      },
      addNewAbleServersHandle: function() {
        var newServer = {
          id: Kooboo.Guid.Empty,
          name: "",
          serverUrl: "",
          reserved: false,
          editable: true,
          isNew: true
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
            self.editableServers = self.editableServers.filter(function(item) {
              return !_.isEqual(item, row);
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
              window.info.done(Kooboo.text.info.delete.success);
              self.getTableData();
            } else {
              window.info.fail(Kooboo.text.info.delete.failed);
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
        self.remoteSiteModel = {
          remoteSiteName: "",
          preDomain: "",
          suffixDomain: ""
        };
        self.$refs.createSiteForm.clearValid();
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
        self.remoteSiteModel = {
          remoteSiteName: "",
          preDomain: "",
          suffixDomain: ""
        };
        self.$refs.createSiteForm.clearValid();
        this.isShowModal = false;
        this.ableAddSite = false;
      },
      createRemoteSite: function() {
        var validateStatus = this.$refs.createSiteForm.validate();
        //add new Site to avaliableSites
        if (validateStatus) {
          var newSite = {
            id: Kooboo.Guid.Empty,
            name: self.remoteSiteModel.remoteSiteName
          };
          self.avaliableSites.push(newSite);
          self.selectedSite = newSite;
          self.ableAddSite = true;
          self.showCreateSiteModal = false;
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
        this.ableAddNewServer = true;
        this.isShowManageServerModal = true;
        this._editableServers = _.cloneDeep(this.editableServers);
      },
      hideManageServerModal: function() {
        this.editableServers = this.editableServers.filter(function(item) {
          if (!item.isNew) return true;
        });
        this.ableAddNewServer = false;
        this.isShowManageServerModal = false;
        if (!_.isEqual(this.editableServers, this._editableServers)) {
          this.getModalData();
        }
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
              item.validateModel = {
                name: { hasError: false, message: "" },
                serverUrl: { hasError: false, message: "" }
              };
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
