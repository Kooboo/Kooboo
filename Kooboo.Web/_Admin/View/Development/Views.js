$(function() {
    var viewViewModel = function() {
        var self = this;

        this.createUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.View.Create);
        });

        this.isDataSourceModalShow = ko.observable(false);

        this.hideDataSourceModal = function() {
            self.dataSources([]);
            self.isDataSourceModalShow(false);
        }

        this.getDataSourceEditUrl = function(data) {
            return Kooboo.Route.Get(Kooboo.Route.DataSource.DataMethodSetting, {
                isNew: false,
                id: data.methodId
            })
        }

        this.dataSources = ko.observableArray();

        Kooboo.EventBus.subscribe("kb/view/data/source/modal", function(id) {
            Kooboo.View.ViewMethods({
                id: id
            }).then(function(res) {
                if (res.success) {
                    self.isDataSourceModalShow(true);
                    self.dataSources(res.model);
                }
            })
        })


        // copy page
        this.showError = ko.observable(false);

        this.copyView = ko.observable({
            name: "",
            id: ""
        });

        this.showCopyBtn = ko.observable(false);

        this.copyName = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                message: Kooboo.text.validation.objectNameRegex
            },
            stringlength: {
                min: 1,
                max: 64,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
            },
            remote: {
                url: Kooboo.View.isUniqueName(),
                data: {
                    name: function() {
                        return self.copyName()
                    }
                },
                message: Kooboo.text.validation.taken
            }

        })

        this.onShowCopyModal = function() {
            self.copyModal(true);
            self.copyName(self.copyView().name + "_Copy");
        }

        this.onHideCopyModal = function() {
            self.copyModal(false);
            self.copyName("");
            self.showError(false);
        }

        this.copyModal = ko.observable(false);

        this.isCopyValid = function() {
            return self.copyName.isValid();
        }

        this.onStartCopy = function() {
            if (self.isCopyValid()) {
                Kooboo.View.Copy({
                    id: self.copyView().id,
                    name: self.copyName()
                }).then(function(res) {

                    if (res.success) {
                        var tableData = _.cloneDeep(self.tableData()),
                            view = res.model,
                            date = new Date();

                        tableData.docs.push({
                            id: view.id,
                            name: {
                                text: view.name,
                                url: Kooboo.Route.Get(Kooboo.Route.View.DetailPage, {
                                    Id: view.id
                                })
                            },
                            date: date.toDefaultLangString(),
                            relationsComm: "kb/relation/modal/show",
                            relationsTypes: Object.keys(view.relations),
                            relations: view.relations,
                            dataSourceCount: {
                                text: view.dataSourceCount > 0 ? view.dataSourceCount : "NO_VALUE",
                                class: view.dataSourceCount > 0 ? "blue" : "default",
                                url: "kb/view/data/source/modal"
                            },
                            Preview: {
                                text: view.preview,
                                url: view.preview,
                                newWindow: true
                            },
                            versions: {
                                iconClass: "fa-clock-o",
                                title: Kooboo.text.common.viewAllVersions,
                                url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
                                    KeyHash: view.keyHash,
                                    storeNameHash: view.storeNameHash
                                }),
                                newWindow: true
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

        Kooboo.EventBus.subscribe("ko/table/docs/selected", function(docs) {
            if (docs.length == 1 && docs[0].name.hasOwnProperty("text") && typeof docs[0].name.text == 'function') {
                self.showCopyBtn(true);
                self.copyView({
                    name: docs[0].name.text(),
                    id: docs[0].id()
                });
            } else {
                self.showCopyBtn(false);
            }
        });

    }
    viewViewModel.prototype = new Kooboo.tableModel(Kooboo.View.name);

    Kooboo.View.getList().then(function(res) {

        if (res.success) {
            var viewList = [];

            var _list = _.sortBy(res.model, [function(r) {
                return r.lastModified;
            }]).reverse();

            _.forEach(_list, function(view) {
                var date = new Date(view.lastModified);

                var model = {
                    id: view.id,
                    name: {
                        text: view.name,
                        url: Kooboo.Route.Get(Kooboo.Route.View.DetailPage, {
                            Id: view.id
                        })
                    },
                    date: date.toDefaultLangString(),
                    relationsComm: "kb/relation/modal/show",
                    relationsTypes: Object.keys(view.relations),
                    relations: view.relations,
                    dataSourceCount: {
                        text: view.dataSourceCount > 0 ? view.dataSourceCount : "NO_VALUE",
                        class: view.dataSourceCount > 0 ? "blue" : "default",
                        url: "kb/view/data/source/modal"
                    },
                    Preview: {
                        text: view.preview,
                        url: view.preview,
                        newWindow: true
                    },
                    versions: {
                        iconClass: "fa-clock-o",
                        title: Kooboo.text.common.viewAllVersions,
                        url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
                            KeyHash: view.keyHash,
                            storeNameHash: view.storeNameHash
                        }),
                        newWindow: true
                    }
                }

                viewList.push(model);
            });

            var columns = [{
                displayName: Kooboo.text.common.name,
                fieldName: "name",
                type: "link"
            }, {
                displayName: Kooboo.text.common.usedBy,
                fieldName: "relations",
                type: "communication-refer"
            }, {
                displayName: Kooboo.text.site.view.dataSource,
                fieldName: "dataSourceCount",
                type: "communication-badge"
            }, {
                displayName: Kooboo.text.common.preview,
                fieldName: "Preview",
                type: "link"
            }, {
                displayName: Kooboo.text.common.lastModified,
                fieldName: "date",
                type: "text"
            }];

            var cpnt = {
                columns: columns,
                docs: viewList,
                tableActions: [{
                    fieldName: "versions",
                    type: "link-icon"
                }],
                kbType: Kooboo.View.name
            };

            var vm = new viewViewModel();
            vm.tableData(cpnt);

            ko.applyBindings(vm, document.getElementById("main"));
        }
    });
});