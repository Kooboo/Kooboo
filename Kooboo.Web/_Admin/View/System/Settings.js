$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.Settings
          }
        ],
        siteTypes: [],
        clusters: [],
        langs: [],
        domains: [],
        editingLangs: [],
        customSettingArray: [],
        langModalShow: false,
        uploadModal: false,
        showExportModal: false,

        remoteSiteList: [],
        langKeys: [],
        showSiteMirrorConfigModal: false,
        isSlaveSite: false,
        _clusters: [],
        showError: false,
        showCustomServerModal: false,

        model: {
          displayName: "",
          siteType: "",
          enableVisitorLog: false,
          enableConstraintFixOnSave: false,
          enableFrontEvents: false,
          enableDiskSync: false,
          enableMultilingual: false,
          enableSitePath: false,
          defaultCulture: "",
          sitePath: {},
          autoDetectCulture: false,
          forceSSL: false,
          customSettings: {},
          enableLocationRedirect: false,
          diskSyncFolder: ""
        }
      };
    },
    beforeCreate: function() {
      self = this;
    },
    mounted: function() {
      Kooboo.Site.Get().then(function(response) {
        $.when(
          Kooboo.Site.getCultures(),
          Kooboo.Site.Langs(),
          Kooboo.Site.getTypes()
        ).then(function(cultureLists, defaultCulture, siteTypes) {
          var model = response.model;
          // console.log(model);
          // automapping
          _.keys(self.model).forEach(function(key) {
            if (model[key] !== void 0 || model[key] !== null) {
              self.model[key] = model[key];
            }
          });
          self.customSettingArray = Kooboo.objToArr(model.customSettings);
          self.siteTypes = Kooboo.objToArr(siteTypes[0].model);
          self.cultures = cultureLists[0].model;
          for (var cul in defaultCulture[0].model.cultures) {
            self.langs.push({
              key: cul,
              value: defaultCulture[0].model.cultures[cul]
            });
          }
          self.langs.forEach(function(lang) {
            self.model.sitePath[lang.key] =
              model.sitePath[lang.key] || lang.key;
          });
          self.model.defaultCulture = defaultCulture[0].model.default;
        });
      });
      Kooboo.Domain.getList().then(function(res) {
        if (res.success) {
          res.model.forEach(function(r) {
            self.domains.push({
              name: "." + r.domainName,
              value: r.domainName
            });
          });
        }
      });

      var copyPath = new Clipboard("#copy_sync_path");
      copyPath.on("success", function(e) {
        $(e.trigger)
          .attr("title", Kooboo.text.tooltip.copied)
          .tooltip("fixTitle")
          .tooltip("show");
        setTimeout(function() {
          $(e.trigger).tooltip("destroy");
        }, 2000);
      });
    },
    computed: {
      culturesKey: function() {
        var cs = Kooboo.objToArr(self.cultures);
        return _.map(cs, function(c) {
          return c.key;
        });
      }
    },
    watch: {
      langs(val) {
        var hasLang = _.some(val, function(item) {
          return item.key === self.model.defaultCulture;
        });
        if (val.length > 0 && !hasLang) {
          self.model.defaultCulture = val[0].key;
        }
      }
    },
    methods: {
      addCustomSetting: function() {
        self.customSettingArray.push({
          key: null,
          value: null
        });
      },
      removeCustomSetting: function(m) {
        self.customSettingArray = _.without(self.customSettingArray, m);
      },
      onShowUploadModal: function() {
        self.uploadModal = true;
      },
      onHideUploadModal: function() {
        self.uploadModal = false;
      },
      uploadFile: function(data, files) {
        if (files.length) {
          Kooboo.Upload.Package(data).then(function(res) {
            if (res.success) {
              window.info.show(Kooboo.text.info.upload.success, true);
              self.onHideUploadModal();
            } else {
              window.info.show(Kooboo.text.info.upload.fail, false);
            }
          });
        }
      },
      onShowExportModal: function() {
        self.showExportModal = true;
      },
      save: function() {
        var newData = _.cloneDeep(self.model);

        var customSettings = {};
        _.forEach(self.customSettingArray, function(s) {
          s.key && (customSettings[s.key] = s.value);
        });

        newData["customSettings"] = customSettings;
        newData["culture"] = Kooboo.arrToObj(self.langs);

        if (!newData.enableMultilingual) {
          self.model.enableSitePath = false;
          newData.enableSitePath = false;
        }
        // console.log(newData);
        Kooboo.Site.post(newData).then(function(res) {
          if (res.success) {
            window.info.show(Kooboo.text.info.save.success, true);
            Kooboo.EventBus.publish("kb/sidebar/refresh");
          } else {
            window.info.show(Kooboo.text.info.save.fail, false);
          }
        });
      },
      onAddLangModalShow: function() {
        _.forEach(self.langs, function(lang) {
          self.editingLangs.push({
            key: lang.key,
            keyError: "",
            value: lang.value,
            valueError: "",
            cultures: self.cultures
          });
        });
        self.langModalShow = true;
      },
      changeLang: function(target, lang) {
        lang.value = self.cultures[lang.key] && self.cultures[lang.key];
        $(target)
          .siblings("input:first")
          .focus();
      },
      removeLang: function(lang) {
        self.langs = _.without(self.langs, lang);
      },
      removeEditingLang: function(lang) {
        lang.showError = false;
        self.editingLangs = _.without(self.editingLangs, lang);
      },
      onAddLangModalHide: function() {
        _.forEach(self.editingLangs, function(lang) {
          lang.keyError = "";
          lang.valueError = "";
        });
        self.editingLangs = [];
        self.langModalShow = false;
      },
      onAddLangModalSave: function() {
        var ableToSave = true;
        _.forEach(self.editingLangs, function(lang) {
          if (!lang.key) {
            lang.keyError = Kooboo.text.validation.required;
            ableToSave = false;
          }
          if (!lang.value) {
            lang.valueError = Kooboo.text.validation.required;
            ableToSave = false;
          }
        });

        if (ableToSave) {
          var langs = [];
          _.forEach(self.editingLangs, function(lang) {
            langs.push(lang);
          });
          self.langs = langs;
          self.onAddLangModalHide();
        }
      },
      addNewLang: function() {
        self.editingLangs.push({
          key: "",
          keyError:"",
          value: "",
          valueError: "",
          cultures: self.cultures
        });
      }
      ////  Site mirror config is  no need for now
      //, onShowSiteMirrorConfigModal: function() {
      //   self.showSiteMirrorConfigModal = true;

      //   Kooboo.Cluster.get().then(function(res) {
      //     if (res.success) {
      //       if (res.model.isSlave) {
      //         self.isSlaveSite(true);
      //       } else {
      //         self.isSlaveSite(false);
      //         self.enableCluster(res.model.enableCluster);
      //         $("#enable_cluster")
      //           .bootstrapSwitch("state", self.enableCluster())
      //           .on("switchChange.bootstrapSwitch", function(e, data) {
      //             self.enableCluster(data);
      //           });
      //         self.enableLocationRedirect(res.model.enableLocationRedirect);
      //         $("#enable_location_redirect")
      //           .bootstrapSwitch("state", self.enableLocationRedirect())
      //           .on("switchChange.bootstrapSwitch", function(e, data) {
      //             self.enableLocationRedirect(data);
      //           });

      //         res.model.dataCenter.forEach(function(dc) {
      //           var find = _.find(res.model.locationRedirect, function(lr) {
      //             return lr.name == dc.name;
      //           });

      //           dc.rootDomain = find ? find.rootDomain : "";
      //           dc.subDomain = find ? find.subDomain : "";

      //           self._clusters.push(new clusterModel(dc));
      //         });
      //       }
      //     }
      //   });
      // },
      // onClearLocalCache: function() {
      //   localStorage.clear();
      //   window.info.done(Kooboo.text.info.delete.success);
      // },
      // hideSiteMirrorConfigModal: function() {
      //   self._clusters().forEach(function(cls) {
      //     cls.showError(false);
      //   });
      //   self._clusters([]);
      //   self.enableCluster(false);
      //   self.enableLocationRedirect(false);
      //   $("#enable_cluster").bootstrapSwitch("destroy", true);
      //   $("#enable_cluster").bootstrapSwitch("state", false);
      //   $("#enable_location_redirect").bootstrapSwitch("destroy", true);
      //   $("#enable_location_redirect").bootstrapSwitch("state", false);
      //   self.showSiteMirrorConfigModal(false);
      // },
      // saveSiteMirrorConfigModal: function() {
      //   var allClusterValid = true;

      //   if (self.enableLocationRedirect()) {
      //     self._clusters().forEach(function(cls) {
      //       if (!cls.isValid()) {
      //         allClusterValid = false;
      //       }
      //     });
      //   }

      //   if (allClusterValid) {
      //     var saveData = {
      //       dataCenter: [],
      //       locationRedirect: []
      //     };

      //     saveData.enableCluster = self.enableCluster();
      //     saveData.enableLocationRedirect = self.enableLocationRedirect();

      //     self._clusters().forEach(function(cluster) {
      //       saveData.dataCenter.push({
      //         name: cluster.name(),
      //         ip: cluster.ip(),
      //         port: cluster.port(),
      //         displayName: cluster.displayName(),
      //         isSelected: cluster.isSelected(),
      //         isCompleted: cluster.isCompleted(),
      //         isRoot: cluster.isRoot()
      //       });

      //       if (cluster.isSelected() && self.enableLocationRedirect()) {
      //         self.locationRedirect = self.enableLocationRedirect();
      //         saveData.locationRedirect.push({
      //           name: cluster.name(),
      //           subDomain: cluster.subDomain(),
      //           rootDomain: cluster.rootDomain()
      //         });
      //       }
      //     });

      //     Kooboo.Cluster.post(saveData).then(function(res) {
      //       if (res.success) {
      //         self.hideSiteMirrorConfigModal();
      //         window.info.done(Kooboo.text.info.update.success);
      //       } else {
      //         window.info.fail(Kooboo.text.info.update.fail);
      //       }
      //     });
      //   } else {
      //     self._clusters().forEach(function(cls) {
      //       cls.showError(true);
      //     });
      //   }
      // },
      // resetCustomerModal: function() {
      //   self.customServerName("");
      //   self.customServerIP("");
      //   self.customServerPort(80);
      //   self.customServerDisplayName("");
      //   self.showError(false);
      // },
      // addCustomServer: function() {
      //   self.showCustomServerModal(true);
      // },
      // removeCustomCluster: function(m) {
      //   self._clusters.remove(m);
      // },
      // onHideCustomServerModal: function() {
      //   self.showCustomServerModal(false);
      //   self.resetCustomerModal();
      // },
      // onSaveCustomServer: function() {
      //   if (self.isCustomServerValid()) {
      //     Kooboo.Cluster.isValidateCustomServer({
      //       ip: self.customServerIP(),
      //       port: self.customServerPort(),
      //       name: self.customServerName()
      //     }).then(function(res) {
      //       if (res.success) {
      //         self._clusters.push(
      //           new clusterModel({
      //             displayName: self.customServerDisplayName(),
      //             ip: self.customServerIP(),
      //             port: self.customServerPort(),
      //             isCompleted: false,
      //             isRoot: false,
      //             isSelected: false,
      //             name: self.customServerName(),
      //             isCustom: true,
      //             rootDomain: "",
      //             subDomain: ""
      //           })
      //         );
      //         self.onHideCustomServerModal();
      //       }
      //     });
      //   } else {
      //     self.showError(true);
      //   }
      // }
    }
  });

  ////  Site mirror config is  no need for now
  // var SettingsViewModel = function(data) {
  //   var self = this;

  //   var sitePaths = data.sitePath;
  //   self.getSitePath = function(id) {
  //     return sitePaths[id] || id;
  //   };

  //   this.customServerName = ko.validateField({
  //     required: ""
  //   });

  //   this.customServerIP = ko.validateField({
  //     required: "",
  //     regex: {
  //       pattern: /^([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})+(:([0-9]{1,5}))?$/,
  //       message: Kooboo.text.validation.noValidIP
  //     }
  //   });

  //   this.customServerPort = ko.validateField(80, {
  //     required: ""
  //   });

  //   this.customServerDisplayName = ko.validateField({
  //     required: ""
  //   });

  //   this.isCustomServerValid = function() {
  //     return (
  //       self.customServerName.isValid() &&
  //       self.customServerIP.isValid() &&
  //       self.customServerPort.isValid() &&
  //       self.customServerDisplayName.isValid()
  //     );
  //   };
  //   this.enableLocationRedirect = ko.observable(false);

  //   Kooboo.Domain.getList().then(function(res) {
  //     if (res.success) {
  //       res.model.forEach(function(r) {
  //         self.domains.push({
  //           name: "." + r.domainName,
  //           value: r.domainName
  //         });
  //       });
  //     }
  //   });
  // };

  // var clusterModel = function(info) {
  //   var self = this;

  //   ko.mapping.fromJS(info, {}, self);

  //   this.disabled = ko.observable(info.isSelected && !info.isComplete);

  //   this.changeStatus = function(m, e) {
  //     if (!self.disabled()) {
  //       self.isSelected(!self.isSelected());
  //     } else {
  //       e.preventDefault();
  //     }
  //   };

  //   this.isCustom = ko.observable(!!info.isCustom);

  //   this.showError = ko.observable(false);

  //   this.subDomain = ko.validateField(info.subDomain, {
  //     required: Kooboo.text.validation.required,
  //     remote: {
  //       url: Kooboo.Site.CheckDomainBindingAvailable(),
  //       message: Kooboo.text.validation.taken,
  //       type: "get",
  //       data: {
  //         subDomain: function() {
  //           return self.subDomain();
  //         },
  //         rootDomain: function() {
  //           return self.rootDomain();
  //         }
  //       }
  //     }
  //   });

  //   this.rootDomain.subscribe(function() {
  //     self.subDomain.valueHasMutated();
  //   });

  //   this.isValid = function() {
  //     if (self.isSelected()) {
  //       return self.subDomain.isValid();
  //     } else {
  //       return true;
  //     }
  //   };
  // };
});
