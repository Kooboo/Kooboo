$(function() {
    var siteLogViewModel = function() {

        var self = this;

        this.pager = ko.observable();

        this.onBlameChanges = function() {

            function blame() {

                Kooboo.SiteLog.Blame(JSON.stringify(self.getSelectedIds())).then(function(res) {

                    if (res.success) {
                        location.reload();
                    }
                })
            }

            if (self.selected().length > 1) {

                if (confirm(Kooboo.text.confirm.siteLogs.blame)) {
                    blame();
                }
            } else {
                blame();
            }

        }

        this.onRestoreToPoint = function() {

            if (confirm(Kooboo.text.confirm.siteLogs.restore)) {

                Kooboo.SiteLog.Restore({
                    id: self.selected()[0].id()
                }).then(function(res) {

                    if (res.success) {
                        location.reload();
                    }
                })
            }
        }

        this.showCheckoutDialogClick = function() {
            self.showCheckoutDialog(true);
        }

        this.exportIncrementPackage = function() {
            Kooboo.SiteLog.ExportBatch({
                id: self.selected()[0].id()
            }).then(function(res) {
                if (res.success) {
                    window.open(Kooboo.Route.Get(Kooboo.SiteLog.DownloadPageUrl(), {
                        id: res.model
                    }))
                }
            })
        }

        this.exportItem = function() {
            Kooboo.SiteLog.ExportItem({
                id: self.selected()[0].id()
            }).then(function(res) {
                if (res.success) {
                    window.open(Kooboo.Route.Get(Kooboo.SiteLog.DownloadPageUrl(), {
                        id: res.model
                    }))
                }
            })
        }

        this.exportItems = function() {
            Kooboo.SiteLog.ExportItems({
                ids: self.selected().map(function(s) {
                    return s.id()
                })
            }).then(function(res) {
                if (res.success) {
                    window.open(Kooboo.Route.Get(Kooboo.SiteLog.DownloadPageUrl(), {
                        id: res.model
                    }))
                }
            })
        }

        this.showCheckoutDialog = ko.observable(false);

        this.isCheckoutSubmitValid = function() {
            return self.checkoutSiteName.isValid() && self.checkoutSubDomain.isValid();
        };

        this.checkoutDialogSubmit = function() {

            if (self.isCheckoutSubmitValid()) {
                self.showError(false);
                Kooboo.SiteLog.CheckOut({
                    id: self.selected()[0].id(),
                    SiteName: self.checkoutSiteName(),
                    SubDomain: self.checkoutSubDomain(),
                    RootDomain: self.checkoutRootDomain()
                }).then(function(res) {

                    if (res.success) {
                        self.checkoutDialogReset();
                        window.info.show(Kooboo.text.info.checkout.success, true);
                    } else {
                        window.info.show(Kooboo.text.info.checkout.fail, false);
                    }
                })
            } else {
                self.showError(true);
            }
        };

        this.showError = ko.observable(false);

        this.checkoutSiteName = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^[A-Za-z][\w\-]*$/,
                message: Kooboo.text.validation.siteNameInvalid
            },
            remote: {
                message: Kooboo.text.validation.taken,
                url: Kooboo.Site.isUniqueName(),
                type: "GET",
                data: {
                    name: function() {
                        return self.checkoutSiteName()
                    }
                }
            }
        });
        this.checkoutSiteName.subscribe(function(siteName) {
            self.checkoutSubDomain(_.kebabCase(siteName));
        })

        this.checkoutSubDomain = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^[A-Za-z][\w\-]*$/,
                message: Kooboo.text.validation.siteNameInvalid
            },
            remote: {
                message: Kooboo.text.validation.taken,
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                type: "GET",
                data: {
                    SubDomain: function() {
                        return self.checkoutSubDomain()
                    },
                    RootDomain: function() {
                        if (!self.checkoutRootDomain()) {
                            self.checkoutRootDomain(self.domains()[0].domainName);
                        }
                        return self.checkoutRootDomain()
                    }
                }
            },
            stringlength: {
                min: 1,
                max: 63,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 63
            }
        });

        this.domains = ko.observableArray();

        this.checkoutRootDomain = ko.observable();

        this.checkoutDialogReset = function() {
            self.showCheckoutDialog(false);
            self.checkoutSiteName("");
            self.checkoutSubDomain("");
            self.showError(false);
        };

        this.selected = ko.observableArray();

        this.getSelectedIds = function() {
            var _ids = [];
            self.selected().forEach(function(selected) {
                _ids.push(selected.id());
            });
            return _ids;
        }

        Kooboo.EventBus.subscribe("ko/table/docs/selected", function(selected) {
            self.selected(selected);
        });

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {

            Kooboo.SiteLog.getList({
                pageNr: page
            }).then(function(res) {
                handleData(res.model);
            })
        })
    }

    siteLogViewModel.prototype = new Kooboo.tableModel(Kooboo.SiteLog.name);

    var vm = new siteLogViewModel();

    Kooboo.SiteLog.getList().then(function(res) {
        if (res.success) {
            handleData(res.model);
        }
    });

    Kooboo.Domain.getAvailable().then(function(res) {

        if (res.success) {
            vm.domains(res.model);
        }
    })

    ko.applyBindings(vm, document.getElementById("main"));

    var handleData = function(data) {
        // location.hash = "page=" + data.pageNr;
        vm.pager(data);
        var logList = [];
        data.list.forEach(function(log) {
            var date = new Date(log.lastModified);
        
            var model = {
                id: log.id,
                name: {
                    text: log.itemName,
                    url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, Object.assign({
                        Id: log.id,
                        KeyHash: log.keyHash,
                        StoreNameHash: log.storeNameHash
                    }, log.hasOwnProperty('tableNameHash') ? {
                        tableNameHash: log.tableNameHash
                    } : {}))
                },
                type: {
                    text: log.storeName ? (Kooboo.text.component.table[log.storeName.toLowerCase()] || log.storeName) : "",
                    class: "label-sm label-primary"
                },
                action: {
                    text: Kooboo.text.action[log.actionType.toLowerCase()] || log.actionType,
                    class: "label-sm " + (log.actionType.toLowerCase() == 'add' ? "label-success" : (log.actionType.toLowerCase() == "update" ? "blue" : "label-danger"))
                },
                user: log.userName,
                date: date.toDefaultLangString(),
                viewVersion: {
                    iconClass: "fa-clock-o",
                    title: Kooboo.text.common.viewAllVersions,
                    url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, Object.assign({
                        KeyHash: log.keyHash,
                        StoreNameHash: log.storeNameHash,
                    }, log.hasOwnProperty('tableNameHash') ? {
                        tableNameHash: log.tableNameHash
                    } : {})),
                    newWindow: true
                }
            };

            logList.push(model);
        });

        var columns = [{
                displayName: Kooboo.text.site.siteLog.logItem,
                fieldName: "name",
                type: "link"
            }, {
                displayName: Kooboo.text.site.siteLog.type,
                fieldName: "type",
                type: "label"
            }, {
                displayName: Kooboo.text.site.siteLog.action,
                fieldName: "action",
                type: "label"
            }, {
                displayName: Kooboo.text.common.user,
                fieldName: "user",
                type: "text"
            }, {
                displayName: Kooboo.text.common.lastModified,
                fieldName: "date",
                type: "text"
            }],
            tableActions = [{
                fieldName: "viewVersion",
                type: "link-icon"
            }];

        var cpnt = {
            docs: logList,
            columns: columns,
            tableActions: tableActions,
            kbType: Kooboo.SiteLog.name
        };

        vm.tableData(cpnt);

    }
});