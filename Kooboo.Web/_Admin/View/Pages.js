$(function() {
    var isLocalKooboo = $('#isLocalKooboo')[0];

    var pageViewModel = function() {
        var self = this;

        this.showError = ko.observable(false);

        this.pages = ko.observable();
        this.pages.subscribe(function(pages) {
            var emptyPage = _.findLast(pages, function(page) {
                return page.id == Kooboo.Guid.Empty;
            })

            if (!emptyPage) {
                var _list = _.cloneDeep(pages);
                _list.splice(0, 0, {
                    id: Kooboo.Guid.Empty,
                    path: Kooboo.text.site.page.systemDefault
                });

                self.RouterSelection(_list);
            }

            self.IndexRouterSelection(pages);
        })
        this.layouts = ko.observableArray();

        this.createNewPageUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.Page.EditPage);
        })

        this.createNewConntentPageUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.Page.EditRichText);
        })

        this.importPageUrl = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/,
                message: Kooboo.text.validation.urlInvalid
            }
        });
        this.importPageUrl.subscribe(function(val) {
            var path = /\w[^/]+:\/\/[\w\.]+:?\d*(.[^\?&]+)/.exec(val.trim());
            self.importBasePath(path ? (path[1] || "") : "");
        })

        this.importBasePath = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^[^\s|\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\||\[|\]|\;|\:|\"|\'|\,|\<|\>|\?]*$/,
                message: Kooboo.text.validation.urlInvalid
            },
            stringlength: {
                min: 1,
                max: 64,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
            },
        });

        this.importModal = ko.observable(false);

        this.importFrom = ko.observable("url");
        this.importFrom.subscribe(function(type) {
            if (type == "file") {
                self.importBasePath("");
                self.importPageUrl("");
                self.showError(false);
            }
        })

        this.showImportModal = function() {
            self.importModal(true);
            self.importPageUrl("");
            self.importBasePath("");
        };

        this.hideImportModal = function() {
            self.resetPageModal();
        };

        this.resetPageModal = function() {
            self.importPageUrl("");
            self.importBasePath("");
            self.showError(false);
            self.importModal(false);
        };

        this.isSinglePageFormValid = function() {
            return self.importPageUrl.isValid() &&
                self.importBasePath.isValid();
        };

        this.onImportSubmit = function() {

            if (self.isSinglePageFormValid()) {
                self.showError(false);
                Kooboo.Transfer.singlePage({
                    siteId: Kooboo.getQueryString("SiteId"),
                    pageUrl: self.importPageUrl(),
                    name: self.importBasePath()
                }).then(function(res) {

                    if (res.success) {
                        var _tableData = _.cloneDeep(self.tableData());
                        var newPage = generatePageTableData([res.model]);
                        _tableData.docs.push(newPage.docs[0]);
                        self.tableData(_tableData);
                        self.resetPageModal();
                    }
                })
            } else {
                self.showError(true);
            }
        };

        // upload 
        this.uploadFile = function(data, files) {
            if (files.length) {
                Kooboo.Page.ConvertFile(data).then(function(res) {

                    if (res.success) {
                        Kooboo.Page.getAll().then(function(res) {
                            if (res.success) {
                                self.resetPageModal();
                                window.info.show(Kooboo.text.info.upload.success, true);
                                self.tableData(generatePageTableData(res.model.pages));
                            }
                        })
                    }
                })
            }
        }

        // Router manager
        this.showRouterManager = ko.observable(false);

        this.IndexRouterSelection = ko.observableArray();
        this.RouterSelection = ko.observable();

        this.defaultPage = ko.observable();
        this.defaultPage.subscribe(function(id) {
            (self.notFoundPage() == id) && self.notFoundPage(Kooboo.Guid.Empty);
            (self.errorPage() == id) && self.errorPage(Kooboo.Guid.Empty);
        })
        this.notFoundPage = ko.observable();
        this.notFoundPage.subscribe(function(id) {
            (self.defaultPage() == id) && self.defaultPage(Kooboo.Guid.Empty);
            (self.errorPage() == id) && self.errorPage(Kooboo.Guid.Empty);
        });
        this.errorPage = ko.observable();
        this.errorPage.subscribe(function(id) {
            (self.defaultPage() == id) && self.defaultPage(Kooboo.Guid.Empty);
            (self.notFoundPage() == id) && self.notFoundPage(Kooboo.Guid.Empty);
        });

        this.hideRouterManager = function() {
            self.showRouterManager(false);
            self.defaultPage(Kooboo.Guid.Empty);
            self.notFoundPage(Kooboo.Guid.Empty);
            self.errorPage(Kooboo.Guid.Empty);
        }

        this.onShowRouterManager = function() {

            Kooboo.Page.getDefaultRoute().then(function(res) {
                if (res.success) {
                    self.showRouterManager(true);
                    self.defaultPage(res.model.startPage);
                    self.notFoundPage(res.model.notFound);
                    self.errorPage(res.model.error);
                }
            })
        }

        this.saveRouterManager = function() {
            debugger
            Kooboo.Page.defaultRouteUpdate({
                startPage: self.defaultPage(),
                notFound: self.notFoundPage(),
                error: self.errorPage()
            }).then(function(res) {

                if (res.success) {
                    window.info.show(Kooboo.text.info.update.success, true);

                    var tableData = _.cloneDeep(self.tableData()),
                        origIndexPage = _.findLast(tableData.docs, function(doc) {
                            return doc.icon.class;
                        })

                    if (origIndexPage) {

                        if (self.defaultPage() == Kooboo.Guid.Empty) {
                            var origIdx = _.findIndex(tableData.docs, function(doc) {
                                return doc.id == origIndexPage.id;
                            });

                            origIndexPage.icon = {
                                class: "",
                                title: ""
                            };
                            tableData.docs.splice(origIdx, 1);
                            tableData.docs.splice(origIdx, 0, origIndexPage);
                            self.tableData(tableData);
                        } else if (origIndexPage.id !== self.defaultPage()) {
                            var origIdx = _.findIndex(tableData.docs, function(doc) {
                                return doc.id == origIndexPage.id;
                            });

                            origIndexPage.icon = {
                                class: "",
                                title: ""
                            };
                            tableData.docs.splice(origIdx, 1);
                            tableData.docs.splice(origIdx, 0, origIndexPage);

                            var newIndexPage = _.findLast(tableData.docs, function(doc) {
                                    return doc.id == self.defaultPage();
                                }),
                                newIndexIdx = _.findIndex(tableData.docs, function(doc) {
                                    return doc.id == self.defaultPage();
                                });

                            newIndexPage.icon = {
                                class: "fa-home fa-lg",
                                title: Kooboo.text.site.page.indexPage
                            }

                            tableData.docs.splice(newIndexIdx, 1);
                            tableData.docs.splice(newIndexIdx, 0, newIndexPage);

                            self.tableData(tableData);
                        }
                    } else {

                        if (self.defaultPage() && (self.defaultPage() !== Kooboo.Guid.Empty)) {
                            var newIndexPage = _.findLast(tableData.docs, function(doc) {
                                    return doc.id == self.defaultPage();
                                }),
                                newIndexIdx = _.findIndex(tableData.docs, function(doc) {
                                    return doc.id == self.defaultPage()
                                });

                            newIndexPage.icon = {
                                class: "fa-home fa-lg",
                                title: Kooboo.text.site.page.indexPage
                            }

                            tableData.docs.splice(newIndexIdx, 1);
                            tableData.docs.splice(newIndexIdx, 0, newIndexPage);

                            self.tableData(tableData);
                        }
                    }

                    self.hideRouterManager();
                }
            })
        }

        // copy page
        this.copyPage = ko.observable({
            name: "",
            id: ""
        });

        this.showCopyBtn = ko.observable(false);

        this.copyName = ko.validateField({
            required: Kooboo.text.validation.required,
            stringlength: {
                min: 1,
                max: 64,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
            },
            remote: {
                url: Kooboo.Page.isUniqueName(),
                data: {
                    name: function() {
                        return self.copyName()
                    }
                },
                message: Kooboo.text.validation.taken
            }
        });

        this.copyRoute = ko.validateField({
            required: "",
            regex: {
                pattern: /^[^\s|\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\||\[|\]|\;|\:|\"|\'|\,|\<|\>|\?]*$/,
                message: Kooboo.text.validation.urlInvalid
            }
        });

        this.onShowCopyModal = function() {
            self.copyModal(true);
            var pageName = self.copyPage().name || "";
            if (pageName.indexOf('.') > -1) {
                var _temp = pageName.split('.');
                _temp[0] = _temp[0] + "_Copy";
                pageName = _temp.join('.');
            } else {
                pageName += "_Copy"
            }
            self.copyName(pageName);
            self.copyRoute("/" + pageName);
        }

        this.onHideCopyModal = function() {
            self.copyModal(false);
            self.copyName("");
            self.copyRoute("");
            self.showError(false);
        }

        this.copyModal = ko.observable(false);

        this.isCopyValid = function() {
            return self.copyName.isValid() && self.copyRoute.isValid();
        }

        this.onStartCopy = function() {
            if (self.isCopyValid()) {
                Kooboo.Page.Copy({
                    id: self.copyPage().id,
                    name: self.copyName(),
                    url: self.copyRoute()
                }).then(function(res) {

                    if (res.success) {
                        var tableData = _.cloneDeep(self.tableData()),
                            page = res.model,
                            date = new Date(page.lastModified);

                        var settingUrl = '';
                        switch (page.type.toLowerCase()) {
                            case 'normal':
                                settingUrl = Kooboo.Route.Get(Kooboo.Route.Page.EditPage, {
                                    id: page.id
                                });
                                break;
                            case 'layout':
                                settingUrl = Kooboo.Route.Get(Kooboo.Route.Page.EditLayout, {
                                    id: page.id,
                                    layoutId: page.layoutId
                                });
                                break;
                            case 'richtext':
                                settingUrl = Kooboo.Route.Get(Kooboo.Route.Page.EditRichText, {
                                    id: page.id
                                });
                                break;
                        }

                        tableData.docs.push({
                            id: page.id,
                            icon: {
                                class: page.startPage ? "fa-home fa-lg" : "",
                                title: page.startPage ? "Index page" : ""
                            },
                            name: page.name,
                            linked: {
                                text: page.linked,
                                class: "badge-primary"
                            },
                            pageView: {
                                text: page.pageView,
                                class: "badge-primary"
                            },
                            relationsComm: "kb/relation/modal/show",
                            relationsTypes: Object.keys(page.relations),
                            relations: page.relations,
                            online: {
                                text: page.online ? Kooboo.text.online.yes : Kooboo.text.online.no,
                                class: page.online ? "label-sm label-success" : "label-sm label-default"
                            },
                            lastModified: date.toDefaultLangString(),
                            preview: {
                                text: page.path,
                                url: page.previewUrl,
                                newWindow: true
                            },
                            qrcode: {
                                title: Kooboo.text.site.page.previewInMobile,
                                class: 'btn-default',
                                iconClass: 'fa-qrcode',
                                url: 'kb/page/show/qrcode'
                            },
                            setting: {
                                text: Kooboo.text.common.setting,
                                url: settingUrl
                            },
                            design: {
                                text: Kooboo.text.site.page.design,
                                url: page.inlineUrl,
                                newWindow: true,
                                class: "btn-primary"
                            }
                        })

                        self.tableData(tableData);
                        self.onHideCopyModal();
                        self.showCopyBtn(false);
                        window.info.show(Kooboo.text.info.copy.success, true);
                    } else {
                        window.info.show(Kooboo.text.info.copy.fail, false);
                    }
                })
            } else {
                self.showError(true);
            }
        }

        this.showQRCodeModal = ko.observable(false);
        this.hideQRCodeModal = function() {
            self.showQRCodeModal(false);
            $('#qr-code').empty();
        }

        Kooboo.EventBus.subscribe("ko/table/docs/selected", function(docs) {
            if (docs.length == 1 && typeof docs[0].name == "function") {
                self.showCopyBtn(true);
                self.copyPage({
                    name: docs[0].name(),
                    id: docs[0].id()
                });
            } else {
                self.showCopyBtn(false);
            }
        });

        Kooboo.EventBus.subscribe("kb/table/delete/finish", function() {
            Kooboo.Page.getAll().then(function(res) {
                if (res.success) {
                    self.pages(res.model.pages);
                }
            });
        })

        Kooboo.EventBus.subscribe("kb/page/show/qrcode", function(target) {
            var find = _.findLast(self.tableData().docs, function(page) {
                return page.id == target.id
            })
            $("#qr-code").empty().qrcode(find.preview.url);
            self.showQRCodeModal(true);
        })
    }
    pageViewModel.prototype = new Kooboo.tableModel(Kooboo.Page.name);

    var vm = new pageViewModel();

    function getLayoutList(layouts) {
        var _list = [];

        _.forEach(layouts, function(layout) {
            _list.push({
                name: layout.name,
                href: Kooboo.Route.Get(Kooboo.Route.Page.EditLayout, {
                    layoutId: layout.id
                })
            })
        });

        if (!_list.length) {
            _list.push({
                name: Kooboo.text.site.page.createNewLayout,
                href: Kooboo.Route.Get(Kooboo.Route.Layout.Create)
            })
        }

        return _list;
    }

    function generatePageTableData(pages) {
        var _pages = [];

        pages.forEach(function(page) {
            var date = new Date(page.lastModified);
            var model = {
                id: page.id,
                icon: {
                    class: page.startPage ? "fa-home fa-lg" : "",
                    title: page.startPage ? "Index page" : ""
                },
                name: page.name,
                linked: {
                    text: page.linked,
                    class: "badge-primary"
                },
                relationsComm: "kb/relation/modal/show",
                relationsTypes: Object.keys(page.relations),
                relations: page.relations,
                online: {
                    text: page.online ? Kooboo.text.online.yes : Kooboo.text.online.no,
                    class: page.online ? "label-sm label-success" : "label-sm label-default"
                },
                lastModified: date.toDefaultLangString(),
                preview: {
                    text: page.path,
                    url: page.previewUrl,
                    newWindow: true
                },
                qrcode: {
                    title: Kooboo.text.site.page.previewInMobile,
                    class: 'btn-default',
                    iconClass: 'fa-qrcode',
                    url: 'kb/page/show/qrcode'
                },
                setting: {
                    text: Kooboo.text.common.setting,
                    url: page.layoutId == Kooboo.Guid.Empty ? Kooboo.Route.Get(Kooboo.Route.Page[(page.type == "Normal") ? "EditPage" : "EditRichText"], {
                        Id: page.id
                    }) : Kooboo.Route.Get(Kooboo.Route.Page.EditLayout, {
                        Id: page.id,
                        layoutId: page.layoutId
                    })
                },
                design: {
                    text: Kooboo.text.site.page.design,
                    url: page.inlineUrl,
                    newWindow: true,
                    class: "btn-primary hidden-xs"
                }
            };
            _pages.push(model);
        })

        var tableActions = [{
            fieldName: "setting",
            type: "link-btn"
        }, {
            fieldName: "design",
            type: "link-btn"
        }];

        if (!isLocalKooboo) {
            tableActions.push({
                fieldName: 'qrcode',
                type: 'communication-icon-btn'
            })
        }

        var _data = {
            docs: _pages,
            columns: [{
                displayName: "",
                fieldName: "icon",
                type: "icon"
            }, {
                displayName: Kooboo.text.common.name,
                fieldName: "name",
                type: "text"
            }, {
                displayName: Kooboo.text.site.page.linked,
                fieldName: "linked",
                type: "badge"
            }, {
                displayName: Kooboo.text.site.page.online,
                fieldName: "online",
                type: "label"
            }, {
                displayName: Kooboo.text.site.page.references,
                fieldName: "relations",
                type: "communication-refer"
            }, {
                displayName: Kooboo.text.common.lastModified,
                fieldName: "lastModified",
                type: "text"
            }, {
                displayName: Kooboo.text.common.preview,
                fieldName: "preview",
                type: "link"
            }],
            tableActions: tableActions,
            kbType: Kooboo.Page.name
        };

        return _data;
    }

    var BASE_URL, SITE_ID = Kooboo.getQueryString("SiteId");

    Kooboo.Page.getAll().then(function(res) {
        if (res.success) {
            BASE_URL = res.model.baseUrl;
            vm.tableData(generatePageTableData(res.model.pages));
            vm.layouts(getLayoutList(res.model.layouts));
            vm.pages(res.model.pages);
            ko.applyBindings(vm, document.getElementById("main"));
        }
    });
});