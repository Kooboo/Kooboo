$(function () {
    var self;
    new Vue({
        el: "#app",
        data: function () {
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
                      name: Kooboo.text.common.Settings
                  }
                ],
                siteTypes: [],
                clusters: [],
                langs: [],
                startValidLang: false,
                editingLangs: [],
                customSettingArray: [],
                langModalShow: false,
                uploadModal: false,
                showExportModal: false,
                model: {
                    displayName: "",
                    siteType: "",
                    enableVisitorLog: false,
                    enableConstraintFixOnSave: false,
                    enableFrontEvents: false,
                    enableJsCssCompress: false,
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
                },
                loading: true,
                domains: [],
                remoteSiteList: [],
                langKeys: [],
                showSiteMirrorConfigModal: false,
                isSlaveSite: false,
                _clusters: [],
                showError: false,
                showCustomServerModal: false
            };
        },
        mounted: function () {
            Kooboo.Site.Get().then(function (response) {
                $.when(
                  Kooboo.Site.getCultures(),
                  Kooboo.Site.Langs(),
                  Kooboo.Site.getTypes()
                ).then(function (cultureLists, defaultCulture, siteTypes) {
                    self.loading = false;
                    var model = response.model; 
                    // console.log(model);
                    // automapping
                    _.keys(self.model).forEach(function (key) {
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
                    self.langs.forEach(function (lang) {
                        self.model.sitePath[lang.key] =
                          model.sitePath[lang.key] || lang.key;
                    });
                    self.model.defaultCulture = defaultCulture[0].model.default;
                });
            });
            // Kooboo.Domain.getList().then(function(res) {
            //   if (res.success) {
            //     res.model.forEach(function(r) {
            //       self.domains.push({
            //         name: "." + r.domainName,
            //         value: r.domainName
            //       });
            //     });
            //   }
            // });

            var copyPath = new Clipboard("#copy_sync_path");
            copyPath.on("success", function (e) {
                $(e.trigger)
                  .attr("title", Kooboo.text.tooltip.copied)
                  .tooltip("fixTitle")
                  .tooltip("show");
                setTimeout(function () {
                    $(e.trigger).tooltip("destroy");
                }, 2000);
            });
        },
        computed: {
            culturesKey: function () {
                var cs = Kooboo.objToArr(self.cultures);
                return _.map(cs, function (c) {
                    return c.key;
                });
            }
        },
        watch: {
            langs(val) {
                var hasLang = _.some(val, function (item) {
                    return item.key === self.model.defaultCulture;
                });
                if (val.length > 0 && !hasLang) {
                    self.model.defaultCulture = val[0].key;
                }
            }
        },
        methods: {
            addCustomSetting: function () {
                self.customSettingArray.push({
                    key: null,
                    value: null
                });
            },
            removeCustomSetting: function (m) {
                self.customSettingArray = _.without(self.customSettingArray, m);
            },
            onShowUploadModal: function () {
                self.uploadModal = true;
            },
            onHideUploadModal: function () {
                self.uploadModal = false;
            },
            uploadFile: function (data, files) {
                if (files.length) {
                    Kooboo.Upload.Package(data).then(function (res) {
                        if (res.success) {
                            window.info.show(Kooboo.text.info.upload.success, true);
                            self.onHideUploadModal();
                        } else {
                            window.info.show(Kooboo.text.info.upload.fail, false);
                        }
                    });
                }
            },
            onShowExportModal: function () {
                self.showExportModal = true;
            },
            save: function () {
                var newData = _.cloneDeep(self.model);

                var customSettings = {};
                _.forEach(self.customSettingArray, function (s) {
                    s.key && (customSettings[s.key] = s.value);
                });

                newData["customSettings"] = customSettings;
                newData["culture"] = Kooboo.arrToObj(self.langs);

                if (!newData.enableMultilingual) {
                    self.model.enableSitePath = false;
                    newData.enableSitePath = false;
                }
                // console.log(newData);
                Kooboo.Site.post(newData).then(function (res) {
                    if (res.success) {
                        window.info.show(Kooboo.text.info.save.success, true);
                        Kooboo.EventBus.publish("kb/sidebar/refresh");
                    } else {
                        window.info.show(Kooboo.text.info.save.fail, false);
                    }
                });
            },

            //#region langs modal
            onAddLangModalShow: function () {
                self.startValidLang = false;
                _.forEach(self.langs, function (lang) {
                    self.editingLangs.push({
                        key: lang.key,
                        value: lang.value,
                        error: {
                            key: "",
                            value: ""
                        }
                    });
                });
                self.langModalShow = true;
            },
            editLang: function (target, lang) {
                lang.value = self.cultures[lang.key] && self.cultures[lang.key];
                $(target)
                  .siblings("input:first")
                  .focus();
            },
            removeLang: function (lang) {
                self.langs = _.without(self.langs, lang);
            },
            removeEditingLang: function (lang) {
                this.clearValidLang(lang);
                self.editingLangs = _.without(self.editingLangs, lang);
            },
            onAddLangModalHide: function () {
                this.clearValidLang();
                self.editingLangs = [];
                self.langModalShow = false;
            },
            clearValidLang: function (lang) {
                var _langs;
                if (lang) {
                    _langs = [lang];
                } else {
                    _langs = self.editingLangs;
                }
                _.forEach(_langs, function (lang) {
                    lang.error = {};
                });
            },
            validateLangModal: function (lang) {
                if (!self.startValidLang) {
                    return;
                }
                var ableToSave = true;
                var _langs;
                if (lang) {
                    _langs = [lang];
                } else {
                    _langs = self.editingLangs;
                }
                _.forEach(_langs, function (lang) {
                    if (!lang.key) {
                        lang.error.key = Kooboo.text.validation.required;
                        ableToSave = false;
                    } else {
                        lang.error.key = "";
                    }
                    if (!lang.value) {
                        lang.error.value = Kooboo.text.validation.required;
                        ableToSave = false;
                    } else {
                        lang.error.value = "";
                    }
                });
                return ableToSave;
            },
            onAddLangModalSave: function () {
                self.startValidLang = true;
                if (this.validateLangModal()) {
                    var langs = [];
                    _.forEach(self.editingLangs, function (lang) {
                        langs.push(lang);
                    });
                    self.langs = langs;
                    self.onAddLangModalHide();
                }
            },
            addNewLang: function () {
                self.editingLangs.push({
                    key: "",
                    value: "",
                    error: {
                        key: "",
                        value: ""
                    }
                });
            }
            //#endregion


        }
    });
});
