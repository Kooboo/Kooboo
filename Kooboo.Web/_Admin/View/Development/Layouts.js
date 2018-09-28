$(function() {
    var layoutViewModel = function() {
        var self = this;

        this.createUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.Layout.Create);
        });

        // copy page
        this.showError = ko.observable(false);

        this.copyLayout = ko.observable({
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
                url: Kooboo.Layout.isUniqueName(),
                type: "GET",
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
            self.copyName(self.copyLayout().name + "_Copy");
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
                Kooboo.Layout.Copy({
                    id: self.copyLayout().id,
                    name: self.copyName()
                }).then(function(res) {
                    if (res.success) {
                        var tableData = _.cloneDeep(self.tableData()),
                            layout = res.model,
                            date = new Date();

                        tableData.docs.push({
                            id: layout.id,
                            name: {
                                text: layout.name,
                                url: Kooboo.Route.Get(Kooboo.Route.Layout.DetailPage, {
                                    Id: layout.id
                                })
                            },
                            type: layout.extension,
                            date: date.toDefaultLangString(),
                            relationsComm: "kb/relation/modal/show",
                            relationsTypes: Object.keys(layout.relations),
                            relations: layout.relations,
                            versions: {
                                iconClass: "fa-clock-o",
                                title: Kooboo.text.common.viewAllVersions,
                                url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
                                    KeyHash: layout.keyHash,
                                    storeNameHash: layout.storeNameHash
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
                self.copyLayout({
                    name: docs[0].name.text(),
                    id: docs[0].id()
                });
            } else {
                self.showCopyBtn(false);
            }
        });

    };

    layoutViewModel.prototype = new Kooboo.tableModel(Kooboo.Layout.name);

    Kooboo.Layout.getList().then(function(res) {

        if (res.success) {
            var layoutList = [];
            var _list = _.sortBy(res.model, [function(r) {
                return r.lastModified;
            }]).reverse();

            _.forEach(_list, function(layout) {
                var date = new Date(layout.lastModified);

                var model = {
                    id: layout.id,
                    name: {
                        text: layout.name,
                        url: Kooboo.Route.Get(Kooboo.Route.Layout.DetailPage, {
                            Id: layout.id
                        })
                    },
                    type: layout.extension,
                    date: date.toDefaultLangString(),
                    relationsComm: "kb/relation/modal/show",
                    relationsTypes: Object.keys(layout.relations),
                    relations: layout.relations,
                    versions: {
                        iconClass: "fa-clock-o",
                        title: Kooboo.text.common.viewAllVersions,
                        url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
                            KeyHash: layout.keyHash,
                            storeNameHash: layout.storeNameHash
                        }),
                        newWindow: true
                    }
                }

                layoutList.push(model);
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
                displayName: Kooboo.text.common.lastModified,
                fieldName: "date",
                type: "text"
            }];

            var actions = [{
                fieldName: "versions",
                type: "link-icon"
            }];

            var cpnt = {
                columns: columns,
                docs: layoutList,
                tableActions: actions,
                kbType: Kooboo.Layout.name
            };

            var vm = new layoutViewModel();
            vm.tableData(cpnt);

            ko.applyBindings(vm, document.getElementById("main"));
        }
    });
});