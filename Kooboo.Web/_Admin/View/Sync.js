$(function() {
    var Sync = function() {
        var self = this;

        function dataMapping(data) {
            var arr = [];
            arr = data.map(function(o) {
                o.remoteSiteName = {
                    text: o.remoteSiteName,
                    url: "kb/sync/spa"
                }
                o.remoteServerUrl = {
                    text: o.remoteServerUrl,
                    class: "label-sm blue"
                }
                o.difference = {
                    text: o.difference,
                    class: "blue"
                }
                o.href = Kooboo.Route.Get(Kooboo.Route.Publish.DetailList, {
                    Id: o.id
                })

                return o
            })
            return arr
        }

        Kooboo.EventBus.subscribe("kb/sync/spa", function(id) {
            var doc = _.find(self.tableData().docs, function(doc) {
                return doc.id == id;
            })

            self.SPAClick(doc.href, id);
        })

        this.showServersModal = ko.observable(false);
        this.onHideServersModal = function() {
            var invalidServer = [];
            self.editableServers().forEach(function(server) {
                if (server.editMode()) {
                    self.cancelEdit(server);
                }

                if (server.id() == Kooboo.Guid.Empty && !server.isAbleToSave()) {
                    invalidServer.push(server);
                }
            })

            invalidServer.forEach(function(server) {
                server.showError(false);
                self.editableServers.remove(server);
            })

            self.showServersModal(false);

            if (self.serverListRefreshRequired()) {
                var serverList = self.editableServers().map(function(server) {
                    if (server.isAbleToSave()) {
                        return {
                            id: server.id(),
                            name: server.name(),
                            serverUrl: server.serverUrl(),
                            reserved: server.reserved(),
                        }
                    }
                })

                self.servers(serverList);
            }
        }
        this.serverListRefreshRequired = ko.observable(false);

        this.getTableData = function(data) {
            var ob = {
                columns: [{
                    displayName: Kooboo.text.site.sync.remoteSite,
                    fieldName: "remoteSiteName",
                    type: "communication-link"
                }, {
                    displayName: Kooboo.text.site.sync.server,
                    fieldName: "remoteServerUrl",
                    type: "label"
                }, {
                    displayName: Kooboo.text.site.sync.difference,
                    fieldName: "difference",
                    type: "badge"
                }],
                kbType: "Publish"
            }
            ob.docs = dataMapping(data);
            return ob;
        }

        this.syncModal = ko.observable(false);

        this.showSyncModal = function() {
            Kooboo.UserPublish.getList().then(function(res) {
                if (res.success) {
                    self.servers(res.model);
                    self.editableServers.removeAll();
                    res.model.forEach(function(ser) {
                        self.editableServers.push(new ServerModel(ser));
                    });
                    self.syncModal(true);
                }
            })
        }

        this.hideSyncModal = function() {
            self.syncModal(false);
            self.selectSiteStep(false);
            self.ableToAddSite(true);
        }

        this.servers = ko.observableArray();
        this.server = ko.observable();
        this.server.subscribe(function(server) {
            self.selectSiteStep(false);
        })

        this.configServer = function() {
            self.showServersModal(true);
        }

        this.editableServers = ko.observableArray();

        this.ableToAddNewServer = ko.observable(true);

        this.addNewServer = function() {
            self.editableServers.push(new ServerModel({
                id: Kooboo.Guid.Empty,
                name: "",
                serverUrl: "",
                reserved: false,
                editMode: true
            }))
            self.ableToAddNewServer(false);
        }

        this.cancelEdit = function(m, e) {
            if (!m.name() && !m.serverUrl()) {
                m.showError(false);
                self.editableServers.remove(m);
            } else {
                m.editMode(false);
                m.editableName(m.name());
                m.editableUrl(m.serverUrl());
            }
            self.ableToAddNewServer(true);
        }

        this.saveServer = function(m, e) {
            if (m.isAbleToSave()) {
                Kooboo.UserPublish.updateServer({
                    id: m.id(),
                    name: m.editableName(),
                    serverUrl: m.editableUrl()
                }).then(function(res) {
                    if (res.success) {
                        Kooboo.EventBus.publish("server/list/refresh/needed");
                        window.info.done(Kooboo.text.info.update.success);
                        m.name(m.editableName());
                        m.serverUrl(m.editableUrl());
                        m.editMode(false);
                        m.showError(false);
                        m.id(res.model);
                        self.ableToAddNewServer(true);
                    } else {
                        window.info.fail(Kooboo.text.info.update.fail);
                    }
                })
            } else {
                m.showError(true);
            }
        }

        this.removeServer = function(m, e) {
            Kooboo.UserPublish.deleteServer({
                id: m.id()
            }).then(function(res) {
                if (res.success) {
                    Kooboo.EventBus.publish("server/list/refresh/needed");
                    window.info.done(Kooboo.text.info.delete.success);
                    self.editableServers.remove(m);
                } else {
                    window.info.fail(Kooboo.text.info.delete.fail);
                }
            })
        }

        this.selectSiteStep = ko.observable(false);
        this.avaliableSites = ko.observableArray();
        this.choosedSiteId = ko.observable();

        this.pushTypes = ko.observableArray([{
            displayName: Kooboo.text.site.sync.pushType.all,
            value: 'all'
        }, {
            displayName: Kooboo.text.site.sync.pushType.update,
            value: 'update'
        }])

        this.pushType = ko.observable('all');

        this.next = function() {
            Kooboo.Publish.getRemoteSiteList({
                remoteUrl: this.server()
            }).then(function(res) {
                if (res.success) {
                    self.selectSiteStep(true);
                    self.avaliableSites(res.model);
                }
            })
        }

        this.ableToAddSite = ko.observable(true);
        this.showSiteModal = ko.observable(false);
        this.onShowSiteModal = function() {
            self.remoteSiteName("");
            self.remoteSiteDomain("");
            Kooboo.UserPublish.getRemoteDomains({
                serverUrl: self.server()
            }).then(function(res) {
                if (res.success) {
                    self.avaliableRemoteDomain(res.model.map(function(dm) {
                        return {
                            id: dm.id,
                            domainName: '.' + dm.domainName
                        }
                    }));
                    self.showSiteModal(true);
                }
            })
        }
        this.onHideSiteModal = function() {
            self.showSiteModal(false);
            self.showError(false);
        }
        this.isRemoteSiteValid = function() {
            return self.remoteSiteName.isValid() && self.remoteSiteDomain.isValid();
        }
        this.createRemoteSite = function() {
            if (self.isRemoteSiteValid()) {
                self.avaliableSites.push({
                    id: Kooboo.Guid.Empty,
                    name: self.remoteSiteName()
                })
                self.choosedSiteId(Kooboo.Guid.Empty);
                self.onHideSiteModal();
                self.ableToAddSite(false);
            } else {
                self.showError(true);
            }
        }

        this.showError = ko.observable(false);
        this.remoteSiteName = ko.validateField({
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
                        return self.remoteSiteName();
                    }
                }
            }
        })
        this.remoteSiteName.subscribe(function(name) {
            self.remoteSiteDomain(name);
            // self.remoteSiteDomain(_.words(name).join("-"));
        })
        this.remoteSiteDomain = ko.validateField({
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
                        return self.remoteSiteDomain();
                    },
                    RootDomain: function() {
                        return self.remoteDomain();
                    }
                }
            },
            stringlength: {
                min: 1,
                max: 63,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 63
            }
        })
        this.avaliableRemoteDomain = ko.observableArray();
        this.remoteDomain = ko.observable();

        this.getChoosedSiteNameById = function(id) {
            var find = _.find(self.avaliableSites(), function(site) {
                return site.id == id;
            })
            return find.name;
        }

        this.save = function() {
            var params = {
                remoteServerUrl: self.server(),
                remoteWebSiteId: self.choosedSiteId(),
                remoteSiteName: self.getChoosedSiteNameById(self.choosedSiteId()),
                pushType: self.pushType()
            };

            if (self.choosedSiteId() == Kooboo.Guid.Empty) {
                params.siteName = self.remoteSiteName();
                params.fullDomain = self.remoteSiteDomain() + self.remoteDomain()
            }

            Kooboo.Publish.post(params).then(function(res) {
                if (res.success) {
                    getPublishList();
                    self.hideSyncModal();
                }
            })
        }

        this.SPAClick = function(url, id) {
            location.hash = id;
            var path = (url.toLowerCase().indexOf("?siteid=") > -1) ? url.toLowerCase().split("?siteid=")[0] : url;
            if (location.pathname.toLowerCase() !== path ||
                !Kooboo.isSameURLParams(Kooboo.getURLParams(url), Kooboo.getURLParams(location.search))) {
                Kooboo.SPA.getView(url);
            }
        };

        getPublishList();

        function ServerModel(info) {
            var self = this;
            ko.mapping.fromJS(info, {}, self);

            self.showError = ko.observable(false);

            self.editMode = ko.observable(!!info.editMode);

            self.editableName = ko.validateField(info.name, {
                required: Kooboo.text.validation.required
            })

            var pattern = "^((https|http)?://)?" +
                "(([0-9]{1,3}\.){3}[0-9]{1,3}" + // IP形式的URL- 199.194.52.184
                "|" + // 允许IP和DOMAIN（域名）
                "([0-9a-z_!~*'()-]+\.)*" + // 域名- www.
                "([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." + // 二级域名
                "[a-z]{2,6})" + // first level domain- .com or .museum
                "(:[0-9]{1,4})?"; // 端口- :80

            self.editableUrl = ko.validateField(info.serverUrl, {
                required: Kooboo.text.validation.required,
                regex: {
                    pattern: new RegExp(pattern),
                    message: Kooboo.text.validation.urlInvalid
                }
            })

            self.editServer = function() {
                self.editMode(true);
            }

            self.isAbleToSave = function() {
                return self.editableName.isValid() && self.editableUrl.isValid();
            }
        }

        function getPublishList() {
            Kooboo.Publish.getList().then(function(data) {
                self.tableData(self.getTableData(data.model));
            })
        }

        Kooboo.EventBus.subscribe("server/list/refresh/needed", function() {
            self.serverListRefreshRequired(true);
        })
    }

    Sync.prototype = new Kooboo.tableModel();
    ko.applyBindings(new Sync, document.getElementById("main"));
})