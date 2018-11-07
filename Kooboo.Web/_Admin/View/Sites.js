$(function() {
    var copyPath = new Clipboard("#copy");

    if ($.fn.bootstrapSwitch) {
        $.fn.bootstrapSwitch.defaults.onText = Kooboo.text.common.yes;
        $.fn.bootstrapSwitch.defaults.offText = Kooboo.text.common.no;
    }

    copyPath.on("success", function(e) {
        $(e.trigger).attr('title', Kooboo.text.tooltip.copied).tooltip('fixTitle').tooltip('show');
        setTimeout(function() {
            $(e.trigger).tooltip('destroy');
        }, 2000);
    });

    $("#create_site_tab a").click(function(e) {
        e.preventDefault();
        $(this).tab("show");
    })

    var siteModel = function(site) {
        var self = this;

        ko.mapping.fromJS(site, {}, self);

        self.selected = ko.observable(false);

        self.exportUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Site.ExportUrl(), {
                SiteId: self.siteId()
            });
        })

        self.siteUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.Site.DetailPage, {
                SiteId: self.siteId()
            });
        });
    }

    var sitesViewModel = function(sites) {
        var self = this;

        this.siteList = ko.observableArray();

        self.batchManagementMode = ko.observable(false);
        self.batchManagementMode.subscribe(function(active) {
            if (!active) {
                self.selectedSites.removeAll();
                _.forEach(self.siteList(), function(site) {
                    site.selected(false);
                })
            }
        })

        self.activeBatch = function() {
            self.batchManagementMode(!self.batchManagementMode());
        }

        self.selectedSites = ko.observableArray();

        self.deleteSites = function() {

            if (confirm(Kooboo.text.confirm.deleteItems)) {
                var _ids = [];
                _.forEach(self.selectedSites(), function(site) {
                    _ids.push(site.siteId());
                });

                Kooboo.Site.Deletes({
                    ids: _ids
                }).then(function(res) {

                    if (res.success) {
                        window.info.show(Kooboo.text.info.delete.success, true);
                        _.forEach(_ids, function(id) {
                            var site = _.findLast(self.siteList(), function(s) {
                                return s.siteId() == id;
                            })

                            site && self.siteList.remove(site);
                        });
                        self.batchManagementMode(false);
                        self.siteList().length && self.batchManagementMode(true);
                    } else {
                        window.info.show(Kooboo.text.info.delete.fail, false);
                    }
                })
            }
        }

        self.batchExport = function() {
            _.forEach(self.selectedSites(), function(site) {
                setTimeout(function() {
                    window.open(Kooboo.Route.Get(Kooboo.Site.ExportUrl(), {
                        SiteId: site.siteId()
                    }))
                }, 1000);
            })
            self.batchManagementMode(false);
        }

        self.selectAllSites = function() {
            _.forEach(self.siteList(), function(site) {
                site.selected(true);
                self.selectedSites.push(site);
            })
        }

        self.switchStatus = function(m) {
            Kooboo.Site.SwitchStatus({
                id: m.siteId()
            }).then(function(res) {

                if (res.success) {
                    m.online(!m.online());
                }
            });
        };

        self.syncSiteId = ko.observable();

        self.syncEnabled = ko.observable(false);

        self._syncEnabled = ko.observable(false);

        self.isSyncInfoChange = ko.pureComputed(function() {
            return (self.syncEnabled() !== self._syncEnabled()) || (self.folderPath() !== self._folderPath());
        });

        self.folderPath = ko.observable();

        self._folderPath = ko.observable();

        self.hasSyncSwitch = ko.observable();

        self.onSyncFileDisk = function(m) {

            Kooboo.Site.getDiskSync({
                SiteId: m.siteId()
            }).then(function(res) {

                if (res.success) {
                    self.folderPath(res.model.folder);
                    self._folderPath(self.folderPath());

                    self.syncEnabled(res.model.enableDiskSync);
                    self._syncEnabled(self.syncEnabled());

                    self.hasSyncSwitch(res.model.enableDiskSync);

                    $("#sync-switch").bootstrapSwitch("state", res.model.enableDiskSync).on("switchChange.bootstrapSwitch", function(e, data) {
                        self.syncEnabled(data);
                    });

                    self.showSyncFileDiskModal(true);

                    self.syncSiteId(m.siteId());
                }
            });
        };

        self.saveSync = function() {
            Kooboo.Site.updateDiskSync({
                SiteId: self.syncSiteId(),
                localPath: self.folderPath(),
                EnableDiskSync: self.syncEnabled()
            }).then(function(res) {

                if (res.success) {
                    self.resetSyncFileDiskModal();
                    window.info.show(Kooboo.text.info.update.success, true);
                } else {
                    window.info.show(Kooboo.text.info.update.fail, false);
                }
            });
        }

        self.showSyncFileDiskModal = ko.observable(false);

        self.resetSyncFileDiskModal = function() {

            $("#sync-switch").bootstrapSwitch("destroy", true);
            $("#sync-switch").bootstrapSwitch("state", false);
            self.showSyncFileDiskModal(false);
        }

        self.siteDetail = function(site) {

            if (self.batchManagementMode()) {
                site.selected(!site.selected());
                self.selectedSites[site.selected() ? "push" : "remove"](site);
            } else {
                if (!site.inProgress()) {
                    location.href = site.siteUrl();
                } else {
                    window.info.done(Kooboo.text.info.inProgress);
                }
            }
        }

        self.deleteSite = function(m) {
            if (confirm(Kooboo.text.confirm.deleteItem)) {
                Kooboo.Site.Delete({
                    Id: m.siteId()
                }).then(function(res) {

                    if (res.success) {
                        window.info.show(Kooboo.text.info.delete.success, true);
                        self.siteList.remove(m);
                    } else {
                        window.info.show(Kooboo.text.info.delete.fail, false);
                    }
                });
            }
        }

        // Share modal
        self.selectableSites = ko.observableArray();

        _.forEach(sites, function(site) {
            self.selectableSites.push({
                name: site.siteDisplayName,
                id: site.siteId
            })
        })

        self.selectedSiteId = ko.observable();

        self.shareSiteModal = ko.observable(false);

        self.showShareModal = function() {
            self.selectableSites.removeAll();
            self.siteList().forEach(function(site) {
                self.selectableSites.push({
                    name: site.siteDisplayName,
                    id: site.siteId
                })
            })
            self.shareSiteModal(true);
        }

        self.hideShareSiteModal = function() {
            self.shareSiteModal(false);
        }

        self.goSharing = function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.Site.Share, {
                SiteId: self.selectedSiteId()
            });
        }

        // create modal
        self.showCreateModal = function() {
            Kooboo.EventBus.publish("kb/sites/create/modal/show");
        }

        this.SPAClick = function(type, m, e) {
            e.preventDefault();
            Kooboo.SPA.getView(getUrl(type), {
                container: "[layout='default']"
            });

            function getUrl(type) {
                var url;
                switch (type) {
                    case 'transfer':
                        url = Kooboo.Route.Site.TransferPage;
                        break;
                    case 'template':
                        url = Kooboo.Route.Site.TemplatePage;
                        break;
                    case 'create':
                        url = Kooboo.Route.Site.CreatePage;
                        break;
                    case 'import':
                        url = Kooboo.Route.Site.ImportPage;
                        break;
                }
                return url;
            }
        }

        this.showExportModal = ko.observable(false);

        this.exportSite = function(m, e) {
            self.showExportModal(true);
            self.selectedSiteId(m.siteId());
        }

        this.shareSite = function(m, e) {
            location.href = Kooboo.Route.Get(Kooboo.Route.Site.Share, {
                SiteId: m.siteId()
            })
        }

        Kooboo.EventBus.subscribe("kb/sites/list/reload", function(sites) {
            var _list = [],
                inProgressSites = [];
            _.forEach(sites, function(site) {
                _list.push(new siteModel(site));
                if (site.inProgress) {
                    inProgressSites.push(site);
                }
            })
            self.siteList(_list);

            if (inProgressSites.length) {
                var intervalList = {};
                inProgressSites.forEach(function(site) {
                    var interval = setInterval(function() {
                        checkSiteStatus(site.siteId);
                    }, 2000);
                    intervalList[site.siteId] = interval;
                })

                function checkSiteStatus(id) {
                    Kooboo.Transfer.getTaskStatus({
                        siteId: id
                    }).then(function(res) {
                        if (res.success) {
                            var loadedSite = _.find(self.siteList(), function(site) {
                                return site.siteId() == id;
                            })

                            if (res.model && res.model.done) {
                                clearInterval(intervalList[id]);
                                if (loadedSite) {
                                    loadedSite.inProgress(false);
                                    loadedSite.pageCount(res.model.pages);
                                    loadedSite.imageCount(res.model.images);
                                }
                            } else {
                                if (loadedSite) {
                                    loadedSite.pageCount(res.model.pages);
                                    loadedSite.imageCount(res.model.images);
                                }
                            }
                        }
                    })
                }
            }

        })

        Kooboo.EventBus.publish("kb/sites/list/reload", sites);

    }

    Kooboo.Site.getList().then(function(res) {
        if (res.success) {
            var sites = new sitesViewModel(res.model);
            ko.applyBindings(sites, document.getElementById("main"));
        }
    });


    var createSiteViewModel = function() {
        var self = this;

        self.createSiteModal = ko.observable(false);

        self.hideCreateSiteModal = function() {
            self.showError(false);
            self.siteName("");
            self.subDomain("");

            self.allowRemote(false);
            self.server("http://www.kooboo.cn");
            self.customServer("");

            self.createSiteModal(false);
        }

        self.showError = ko.observable(false);

        self.siteName = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                message: Kooboo.text.validation.siteNameInvalid
            },
            remote: {
                url: Kooboo.Site.isUniqueName(),
                message: Kooboo.text.validation.taken,
                type: "get",
                data: {
                    SiteName: function() {
                        return self.siteName();
                    }
                }
            }
        });
        self.siteName.subscribe(function(val) {
            self.subDomain(val);
        });

        self.subDomain = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                message: Kooboo.text.validation.siteNameInvalid
            },
            remote: {
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                message: Kooboo.text.validation.taken,
                type: "get",
                data: {
                    SubDomain: function() {
                        return self.subDomain();
                    },
                    RootDomain: function() {
                        return self.rootDomain();
                    }
                }
            },
            stringlength: {
                min: 1,
                max: 63,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 63
            }
        });

        self.domains = ko.observableArray();

        self.rootDomain = ko.observable("");

        self.sitesPage = ko.pureComputed(function() {
            return Kooboo.Route.Site.ListPage;
        });

        self.isCreateValid = function() {
            return self.siteName.isValid() && self.subDomain.isValid()
        }

        self.onCreateSubmit = function() {

            if (self.isCreateValid()) {
                self.showError(false);

                Kooboo.Site.Create({
                    SiteName: self.siteName(),
                    SubDomain: self.subDomain(),
                    RootDomain: self.rootDomain()
                }).then(function(res) {

                    if (res.success) {

                        Kooboo.Site.getList().then(function(res) {
                            if (res.success) {
                                self.hideCreateSiteModal();
                                Kooboo.EventBus.publish("kb/sites/list/reload", res.model);
                            }
                        });
                    }
                });

            } else {
                self.showError(true);
            }
        }

        self.onCreateWithRemote = function() {
            var remoteSiteName = _.findLast(self.remoteSites(), function(site) {
                return site.id === self.remoteSiteId()
            }).name;

            if (self.isCreateValid()) {
                Kooboo.Site.Create({
                    SiteName: self.siteName(),
                    SubDomain: self.subDomain(),
                    RootDomain: self.rootDomain()
                }).then(function(res) {

                    if (res.success) {

                        Kooboo.Publish.post(JSON.stringify({
                            SiteId: res.model.id,
                            remoteServerUrl: self.remoteServerUrl(),
                            remoteWebSiteId: self.remoteSiteId(),
                            remoteSiteName: remoteSiteName
                        })).then(function() {

                            Kooboo.Site.getList().then(function(res) {
                                if (res.success) {
                                    self.hideCreateSiteModal();
                                    Kooboo.EventBus.publish("kb/sites/list/reload", res.model);
                                }
                            });
                        })
                    }
                });
            } else {
                self.showError(true);
            }
        }

        self.allowRemote = ko.observable(false);
        self.allowRemote.subscribe(function() {
            self.showError(false);
            self.createWithRemote(false);
            self.remoteSitesAvaliable(false);
        })

        Kooboo.EventBus.subscribe("kb/sites/create/modal/show", function() {

            if (!self.domains() || !self.domains().length) {
                Kooboo.Domain.getAvailable().then(function(res) {
                    if (res.success) {
                        var domains = [];
                        res.model.forEach(function(domain) {
                            domains.push({
                                displayName: "." + domain.domainName,
                                value: domain.domainName
                            })
                        })
                        self.domains(domains);
                        self.createSiteModal(true);
                        self.allowRemote(false);
                    }
                })
            } else {
                self.createSiteModal(true);
                self.allowRemote(false);
            }
        })

        // remote 

        this.createWithRemote = ko.observable(false);

        this.server = ko.observable("http://www.kooboo.cn");
        this.customServer = ko.validateField({
            required: Kooboo.text.validation.required
        });
        this.customServer.subscribe(function() {

            if (self.isLogined()) {
                self.isLogined(false);
                self.remoteSitesAvaliable(false);
                self.allowRemote(false);
                self.allowRemote(true);
            }
        })
        this.remoteSiteId = ko.observable("");
        this.remoteSiteName = ko.observable("");
        this.remoteServerUrl = ko.observable("");

        this.remoteSitesAvaliable = ko.observable(false);

        this.remoteSites = ko.observableArray();

        this.isLoginValid = function() {
            return (self.server() ? true : self.customServer.isValid());
        }

        this.isLogined = ko.observable(false);

        this.login = function() {

            if (self.isLoginValid()) {
                self.remoteServerUrl(this.server() || this.customServer())

                Kooboo.Publish.getRemoteSiteList({
                    remoteUrl: self.remoteServerUrl()
                }).then(function(res) {

                    if (res.success) {

                        self.isLogined(true);

                        if (res.model.length) {
                            self.remoteSites(res.model);
                            self.remoteSitesAvaliable(true);
                            self.createWithRemote(true);
                        } else {
                            window.info.show(Kooboo.text.info.noRemoteSiteAvaliable, false);
                            self.allowRemote(false);
                        }
                    }
                })
            } else {
                self.showError(true);
            }

        }

        this.post = function() {
            var data = ko.mapping.toJS(self);
            data.remoteWebSiteId = data.remoteSiteId;
            delete data.remoteSiteId;
            Kooboo.Publish.post(JSON.stringify(data)).then(function() {
                Kooboo.Publish.getList().then(function(data) {
                    var ob = {
                        columns: [{
                            displayName: Kooboo.text.site.sync.remoteSite,
                            fieldName: "remoteSiteName",
                            type: "link"
                        }, {
                            displayName: Kooboo.text.site.sync.server,
                            fieldName: "remoteServerUrl",
                            type: "label"
                        }, {
                            displayName: Kooboo.text.site.sync.difference,
                            fieldName: "difference",
                            type: "badge",
                            class: "blue"
                        }],
                        kbType: "Sync"
                    }
                    ob.docs = dataMapping(data.model)
                    self.tableData(ob);
                })
                self.hideSyncModal();
            })
        }
    };

    $("#component_container_header").removeAttr('style');
    var vm = new createSiteViewModel();
    ko.applyBindings(vm, document.getElementById("create"));
});