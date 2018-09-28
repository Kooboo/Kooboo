$(function() {

    if ($.fn.bootstrapSwitch) {
        $.fn.bootstrapSwitch.defaults.onText = Kooboo.text.common.yes;
        $.fn.bootstrapSwitch.defaults.offText = Kooboo.text.common.no;
    }

    var copyPath = new Clipboard("#copy_sync_path");

    copyPath.on("success", function(e) {
        $(e.trigger).attr('title', Kooboo.text.tooltip.copied).tooltip('fixTitle').tooltip('show');
        setTimeout(function() {
            $(e.trigger).tooltip('destroy');
        }, 2000);
    });

    var SettingsViewModel = function(data) {
        var self = this;
        ko.mapping.fromJS(data, {}, self);
        this.cultures = ko.observableArray([]);
        this.cultures.subscribe(function(cultures) {
            var cs = Kooboo.objToArr(cultures);
            self.culturesKey(_.map(cs, function(c) {
                return c.key;
            }))
        })
        this.culturesKey = ko.observableArray();
        this.defaultCulture = ko.observable("");
        self.customSettingArray = ko.observableArray(Kooboo.objToArr(data.customSettings));
        self.addCustomSetting = function(m, e) {
            e.preventDefault();
            self.customSettingArray.push({
                key: null,
                value: null
            });
        }
        self.removeCustomSetting = function(m, e) {
            e.preventDefault();
            self.customSettingArray.remove(m);
        }

        this.siteTypes = ko.observableArray();
        this.siteType = ko.observable(data.siteType);

        // Upload Modal 
        this.uploadModal = ko.observable(false);

        this.onShowUploadModal = function() {
            self.uploadModal(true);
        }

        this.onHideUploadModal = function() {
            self.uploadModal(false);
        }

        this.uploadFile = function(data, files) {
            if (files.length) {
                Kooboo.Upload.Package(data).then(function(res) {

                    if (res.success) {
                        window.info.show(Kooboo.text.info.upload.success, true);
                        self.onHideUploadModal();
                    } else {
                        window.info.show(Kooboo.text.info.upload.fail, false);
                    }
                })
            }
        }
        this.showExportModal = ko.observable(false);
        this.onShowExportModal = function() {
            self.showExportModal(true);
        }

        var sitePaths = data.sitePath;

        self.getSitePath = function(id) {
            return sitePaths[id] || id;
        }
        self.remoteSiteList = ko.observableArray();

        var keys = [
            "displayName",
            "siteType",
            "enableConstraintFixOnSave",
            "enableDiskSync",
            "enableFrontEvents",
            "forceSSL",
            "enableMultilingual",
            "enableSitePath",
            "enableVisitorLog",
            "sitePath",
            "autoDetectCulture",
            "diskSyncFolder"
        ];

        self.save = function() {
            var newData = {};
            _.forEach(keys, function(k) {
                newData[k] = ko.mapping.toJS(self[k]);
            });

            var sitePathValueElements = $("[name^='SitePath.']");
            _.forEach(sitePathValueElements, function(el) {
                var id = $(el).attr("name").split(".")[1];
                newData["sitePath"][id] = $(el).val()
            });

            var customSettings = {};
            _.forEach(self.customSettingArray(), function(s) {
                s.key && (customSettings[s.key] = s.value);
            });

            newData["customSettings"] = customSettings;
            newData["culture"] = Kooboo.arrToObj(ko.mapping.toJS(self.langs()));
            newData["defaultCulture"] = self.defaultCulture();

            if (!newData.enableMultilingual) {
                self.enableSitePath(false);
                newData.enableSitePath = false;
            }

            Kooboo.Site.post(newData).then(function(res) {
                if (res.success) {
                    window.info.show(Kooboo.text.info.save.success, true);
                    Kooboo.EventBus.publish("kb/sidebar/refresh");
                } else {
                    window.info.show(Kooboo.text.info.save.fail, false);
                }
            })

        }

        this.langModalShow = ko.observable(false);

        this.langs = ko.observableArray();

        this.langKeys = ko.observableArray();

        this.editingLangs = ko.observableArray();

        this.onAddLangModalShow = function() {
            _.forEach(self.langs(), function(lang) {
                self.editingLangs.push(new Language({
                    key: lang.key(),
                    value: lang.value(),
                    cultures: self.cultures()
                }))
            })
            self.langModalShow(true);
        }

        this.removeLang = function(lang) {
            self.langs.remove(lang);
        }

        this.removeEditingLang = function(lang) {
            lang.showError(false);
            self.editingLangs.remove(lang);
        }

        this.onAddLangModalHide = function() {
            _.forEach(self.editingLangs(), function(lang) {
                lang.showError(false);
            })
            self.editingLangs([]);
            self.langModalShow(false);
        }

        this.onAddLangModalSave = function() {
            var ableToSave = true;
            _.forEach(self.editingLangs(), function(lang) {
                if (!lang.isValid()) {
                    lang.showError(true);
                    ableToSave = false;
                }
            })

            if (ableToSave) {
                var langs = [];
                _.forEach(self.editingLangs(), function(lang) {
                    langs.push(lang);
                });
                self.langs(langs);
                self.onAddLangModalHide();
            }
        }

        this.addNewLang = function() {
            self.editingLangs.push(new Language({
                key: "",
                value: "",
                cultures: self.cultures()
            }))
        }

        this.domains = ko.observableArray();

        this.showSiteMirrorConfigModal = ko.observable(false);

        this.isSlaveSite = ko.observable(false);

        this.onShowSiteMirrorConfigModal = function() {
            self.showSiteMirrorConfigModal(true);

            Kooboo.Cluster.get().then(function(res) {
                if (res.success) {
                    if (res.model.isSlave) {
                        self.isSlaveSite(true);
                    } else {
                        self.isSlaveSite(false);
                        self.enableCluster(res.model.enableCluster);
                        $("#enable_cluster").bootstrapSwitch("state", self.enableCluster()).on("switchChange.bootstrapSwitch", function(e, data) {
                            self.enableCluster(data);
                        });
                        self.enableLocationRedirect(res.model.enableLocationRedirect);
                        $("#enable_location_redirect").bootstrapSwitch("state", self.enableLocationRedirect()).on("switchChange.bootstrapSwitch", function(e, data) {
                            self.enableLocationRedirect(data);
                        });

                        res.model.dataCenter.forEach(function(dc) {
                            var find = _.find(res.model.locationRedirect, function(lr) {
                                return lr.name == dc.name;
                            })

                            dc.rootDomain = find ? find.rootDomain : "";
                            dc.subDomain = find ? find.subDomain : "";

                            self._clusters.push(new clusterModel(dc));
                        })
                    }
                }
            })
        }

        this.onClearLocalCache = function() {
            localStorage.clear();
            window.info.done(Kooboo.text.info.delete.success);
        }

        this.hideSiteMirrorConfigModal = function() {
            self._clusters().forEach(function(cls) {
                cls.showError(false);
            })
            self._clusters([]);
            self.enableCluster(false);
            self.enableLocationRedirect(false);
            $("#enable_cluster").bootstrapSwitch("destroy", true);
            $("#enable_cluster").bootstrapSwitch("state", false);
            $("#enable_location_redirect").bootstrapSwitch("destroy", true);
            $("#enable_location_redirect").bootstrapSwitch("state", false);
            self.showSiteMirrorConfigModal(false);
        }

        this.saveSiteMirrorConfigModal = function() {
            var allClusterValid = true;

            if (self.enableLocationRedirect()) {
                self._clusters().forEach(function(cls) {
                    if (!cls.isValid()) {
                        allClusterValid = false;
                    }
                })
            }

            if (allClusterValid) {

                var saveData = {
                    dataCenter: [],
                    locationRedirect: []
                };

                saveData.enableCluster = self.enableCluster();
                saveData.enableLocationRedirect = self.enableLocationRedirect();

                self._clusters().forEach(function(cluster) {

                    saveData.dataCenter.push({
                        name: cluster.name(),
                        ip: cluster.ip(),
                        port: cluster.port(),
                        displayName: cluster.displayName(),
                        isSelected: cluster.isSelected(),
                        isCompleted: cluster.isCompleted(),
                        isRoot: cluster.isRoot()
                    })

                    if (cluster.isSelected() && self.enableLocationRedirect()) {
                        self.locationRedirect = self.enableLocationRedirect();
                        saveData.locationRedirect.push({
                            name: cluster.name(),
                            subDomain: cluster.subDomain(),
                            rootDomain: cluster.rootDomain()
                        });
                    }
                })

                Kooboo.Cluster.post(saveData).then(function(res) {
                    if (res.success) {
                        self.hideSiteMirrorConfigModal();
                        window.info.done(Kooboo.text.info.update.success);
                    } else {
                        window.info.fail(Kooboo.text.info.update.fail);
                    }
                })
            } else {
                self._clusters().forEach(function(cls) {
                    cls.showError(true);
                })
            }
        }

        self.clusters = ko.observableArray();

        this._clusters = ko.observableArray();

        this.showError = ko.observable(false);

        this.customServerName = ko.validateField({
            required: ''
        });

        this.customServerIP = ko.validateField({
            required: '',
            regex: {
                pattern: /^([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})+(:([0-9]{1,5}))?$/,
                message: Kooboo.text.validation.noValidIP
            }
        });

        this.customServerPort = ko.validateField(80, {
            required: ''
        })

        this.customServerDisplayName = ko.validateField({
            required: ''
        });

        this.isCustomServerValid = function() {
            return self.customServerName.isValid() &&
                self.customServerIP.isValid() &&
                self.customServerPort.isValid() &&
                self.customServerDisplayName.isValid();
        }

        this.showError = ko.observable(false);

        this.resetCustomerModal = function() {
            self.customServerName("");
            self.customServerIP("");
            self.customServerPort(80);
            self.customServerDisplayName("");
            self.showError(false);
        }

        this.addCustomServer = function() {
            self.showCustomServerModal(true);
        }

        this.removeCustomCluster = function(m) {
            self._clusters.remove(m);
        }

        this.showCustomServerModal = ko.observable(false);

        this.onHideCustomServerModal = function() {
            self.showCustomServerModal(false)
            self.resetCustomerModal();
        }

        this.onSaveCustomServer = function() {
            if (self.isCustomServerValid()) {
                Kooboo.Cluster.isValidateCustomServer({
                    ip: self.customServerIP(),
                    port: self.customServerPort(),
                    name: self.customServerName()
                }).then(function(res) {
                    if (res.success) {
                        self._clusters.push(new clusterModel({
                            displayName: self.customServerDisplayName(),
                            ip: self.customServerIP(),
                            port: self.customServerPort(),
                            isCompleted: false,
                            isRoot: false,
                            isSelected: false,
                            name: self.customServerName(),
                            isCustom: true,
                            rootDomain: "",
                            subDomain: ""
                        }))
                        self.onHideCustomServerModal();
                    }
                });
            } else {
                self.showError(true);
            }
        }

        $("#enable_visitor_log").bootstrapSwitch("state", self.enableVisitorLog()).on("switchChange.bootstrapSwitch", function(e, data) {
            self.enableVisitorLog(data);
        });
        $("#enable_constraint_fix_on_save").bootstrapSwitch("state", self.enableConstraintFixOnSave()).on("switchChange.bootstrapSwitch", function(e, data) {
            self.enableConstraintFixOnSave(data);
        });
        $("#enable_front_events").bootstrapSwitch("state", self.enableFrontEvents()).on("switchChange.bootstrapSwitch", function(e, data) {
            self.enableFrontEvents(data);
        });
        $("#enable_force_ssl").bootstrapSwitch("state", self.forceSSL()).on("switchChange.bootstrapSwitch", function(e, data) {
            self.forceSSL(data);
        });
        $("#enable_disk_sync").bootstrapSwitch("state", self.enableDiskSync()).on("switchChange.bootstrapSwitch", function(e, data) {
            self.enableDiskSync(data);
        });
        $("#enable_multilingual").bootstrapSwitch("state", self.enableMultilingual()).on("switchChange.bootstrapSwitch", function(e, data) {
            self.enableMultilingual(data);
            if (!data) {
                self.enableSitePath(false);
                $("#enable_site_path").bootstrapSwitch("destroy", true);
                $("#enable_site_path").bootstrapSwitch("state", false).on("switchChange.bootstrapSwitch", function(e, data) {
                    self.enableSitePath(data);
                });
            }
        });
        $("#enable_site_path").bootstrapSwitch("state", self.enableSitePath()).on("switchChange.bootstrapSwitch", function(e, data) {
            self.enableSitePath(data);
        });
        $("#enable_auto_detect_culture").bootstrapSwitch("state", self.autoDetectCulture()).on("switchChange.bootstrapSwitch", function(e, data) {
            self.autoDetectCulture(data);
        });

        this.enableLocationRedirect = ko.observable(false);

        Kooboo.Domain.getList().then(function(res) {
            if (res.success) {
                res.model.forEach(function(r) {
                    self.domains.push({
                        name: '.' + r.domainName,
                        value: r.domainName
                    })
                })
            }
        })
    };

    Kooboo.Site.Get().then(function(response) {
        $.when(Kooboo.Site.getCultures(), Kooboo.Site.Langs(), Kooboo.Site.getTypes()).then(function(cultureLists, defaultCulture, siteTypes) {
            var model = response.model,
                viewModel = new SettingsViewModel(model);

            viewModel.cultures(cultureLists[0].model);
            viewModel.siteTypes(Kooboo.objToArr(siteTypes[0].model));
            for (var cul in defaultCulture[0].model.cultures) {
                viewModel.langs.push(new Language({
                    key: cul,
                    value: defaultCulture[0].model.cultures[cul]
                }))
            }

            viewModel.defaultCulture(defaultCulture[0].model.default);

            ko.applyBindings(viewModel, document.getElementById("main"));
        })
    });

    function Language(lang) {
        var self = this;

        ko.mapping.fromJS(lang, {}, self);

        this.showError = ko.observable(false);

        this.key = ko.validateField(lang.key, {
            required: Kooboo.text.validation.required
        });

        this.key.subscribe(function(lang) {
            self.focus(true);
            self.value(self.cultures[lang] && self.cultures[lang]());
        })

        this.value = ko.validateField(lang.value, {
            required: Kooboo.text.validation.required
        });

        this.isValid = function() {
            return self.key.isValid() && self.value.isValid();
        }

        this.focus = ko.observable(!!lang.focus);
    }

    var clusterModel = function(info) {
        var self = this;

        ko.mapping.fromJS(info, {}, self);

        this.disabled = ko.observable(info.isSelected && !info.isComplete);

        this.changeStatus = function(m, e) {
            if (!self.disabled()) {
                self.isSelected(!self.isSelected());
            } else {
                e.preventDefault();
            }
        }

        this.isCustom = ko.observable(!!info.isCustom);

        this.showError = ko.observable(false);

        this.subDomain = ko.validateField(info.subDomain, {
            required: Kooboo.text.validation.required,
            remote: {
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                message: Kooboo.text.validation.taken,
                type: "get",
                data: {
                    subDomain: function() {
                        return self.subDomain()
                    },
                    rootDomain: function() {
                        return self.rootDomain()
                    }
                }
            }
        })

        this.rootDomain.subscribe(function() {
            self.subDomain.valueHasMutated();
        });

        this.isValid = function() {
            if (self.isSelected()) {
                return self.subDomain.isValid();
            } else {
                return true;
            }
        }
    }
});