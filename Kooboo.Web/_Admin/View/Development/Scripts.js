$(function() {

    var scriptViewModel = function() {
        var self = this;

        this.scriptCreateUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.Script.Create);
        });

        this.types = ko.observableArray([{
            displayName: Kooboo.text.site.script.external,
            value: "External"
        }, {
            displayName: Kooboo.text.site.script.embedded,
            value: "Embedded"
        }, {
            displayName: Kooboo.text.site.script.group,
            value: "Group"
        }]);

        this.curType = ko.observable('External');

        this.fetchDataByType = function(type) {
            if (type !== self.curType()) {
                self.getDataByType[type]();
                self.curType(type);
            }
        };

        this.groupNameList = ko.observableArray();

        this.getDataByType = {
            External: function() {

                Kooboo.Script.getExternalList().then(function(res) {
                    if (res.success) {
                        var scriptList = [];
                        self.ExternalScripts(res.model);
                        _.forEach(res.model, function(script) {
                            var date = new Date(script.lastModified);

                            var model = {
                                id: script.id,
                                name: {
                                    text: script.name,
                                    url: Kooboo.Route.Get(Kooboo.Route.Script
                                        .DetailPage, {
                                            Id: script.id
                                        })
                                },
                                preview: {
                                    text: script.routeName,
                                    url: script.fullUrl,
                                    newWindow: true
                                },
                                date: date.toDefaultLangString(),
                                relationsComm: "kb/relation/modal/show",
                                relationsTypes: Object.keys(script.references),
                                relations: script.references,
                                versions: {
                                    iconClass: "fa-clock-o",
                                    title: Kooboo.text.common.viewAllVersions,
                                    url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
                                        KeyHash: script.keyHash,
                                        storeNameHash: script.storeNameHash
                                    }),
                                    newWindow: true
                                }
                            }

                            scriptList.push(model);
                        });

                        var columns = [{
                            displayName: Kooboo.text.common.name,
                            fieldName: "name",
                            type: "link"
                        }, {
                            displayName: 'progressor',
                            fieldName: 'progressor',
                            type: 'progressor',
                            showClass: 'hidden'
                        }, {
                            displayName: Kooboo.text.common.preview,
                            fieldName: "preview",
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

                        var cpnt = {
                            columns: columns,
                            docs: scriptList,
                            tableActions: [{
                                fieldName: "versions",
                                type: "link-icon"
                            }],
                            kbType: Kooboo.Script.name
                        }

                        self.tableData(cpnt);
                    }
                });
            },
            Embedded: function() {

                Kooboo.Script.getEmbeddedList().then(function(res) {
                    if (res.success) {
                        var scriptList = [];
                        _.forEach(res.model, function(script) {
                            var date = new Date(script.lastModified),
                                model = {
                                    id: script.id,
                                    name: {
                                        text: script.name,
                                        url: Kooboo.Route.Get(Kooboo.Route.Script
                                            .DetailPage, {
                                                Id: script.id
                                            })
                                    },
                                    relationsComm: "kb/relation/modal/show",
                                    relationsTypes: Object.keys(script.references),
                                    relations: script.references,
                                    date: date.toDefaultLangString()
                                };

                            scriptList.push(model);
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

                        var cpnt = {
                            columns: columns,
                            docs: scriptList,
                            kbType: Kooboo.Script.name
                        }

                        self.tableData(cpnt);
                    }
                });
            },
            Group: function() {
                Kooboo.ResourceGroup.Script().then(function(res) {

                    if (res.success) {
                        var scriptList = [],
                            nameList = [];
                        _.forEach(res.model, function(script) {
                            var date = new Date(script.lastModified),
                                model = {
                                    id: script.id,
                                    name: {
                                        text: script.name,
                                        url: "ko/table/script/group/modal"
                                    },
                                    childrenCount: {
                                        text: script.childrenCount > 0 ? script.childrenCount : "NO_VALUE",
                                        url: "ko/table/script/group/modal",
                                        class: "blue"
                                    },
                                    relationsComm: "kb/relation/modal/show",
                                    relationsTypes: Object.keys(script.references),
                                    relations: script.references,
                                    date: date.toDefaultLangString(),
                                    preview: {
                                        text: Kooboo.text.common.preview,
                                        url: script.previewUrl,
                                        newWindow: true
                                    }
                                };

                            scriptList.push(model);
                            nameList.push(script.name);
                        });
                        self.groupNameList(nameList);

                        var columns = [{
                            displayName: Kooboo.text.common.name,
                            fieldName: "name",
                            type: "communication-link"
                        }, {
                            displayName: Kooboo.text.site.script.children,
                            fieldName: "childrenCount",
                            type: "communication-badge"
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
                            fieldName: "preview",
                            type: "link-btn"
                        }];

                        var cpnt = {
                            columns: columns,
                            docs: scriptList,
                            tableActions: actions,
                            kbType: Kooboo.ResourceGroup.name
                        }

                        self.tableData(cpnt);
                    }
                })
            }
        }

        this.uploadScript = function(data, files) {
            function upload() {
                self.fetchDataByType("External");
                var filesCount = files.length,
                    finishedCount = 0;

                var tableData = self.tableData();

                var docs = tableData.docs;

                files.forEach(function(file, idx) {
                    var counter = ko.observable(0);
                    docs.push({
                        unselectable: true,
                        name: {
                            text: file.name,
                            url: 'javascript:;'
                        },
                        progressor: {
                            value: counter
                        }
                    })

                    var formdata = new FormData();
                    formdata.append("file_" + idx, file);

                    Kooboo.Upload.Script(formdata, counter).then(function(res) {
                        if (res.success) {
                            ++finishedCount;
                            if (filesCount == finishedCount) {
                                self.getDataByType["External"]();
                                window.info.show(Kooboo.text.info.upload.success, true);
                            }
                        }
                    })

                })

                tableData.docs = docs;
                self.tableData(tableData);
            }

            if (files.length) {
                if (!Kooboo.isFileNameExist(files, self.ExternalScripts())) {
                    upload();
                } else {
                    if (confirm(Kooboo.text.confirm.overrideFile)) {
                        upload();
                    }
                }
            }

        }

        this.ExternalScripts = ko.observableArray();

        this.getDataByType[this.curType()]();

        // Group modal
        this.showError = ko.observable(false);

        this.createGroup = function() {
            self.groupName("");
            self.groupModal(true);
            self.isGroupNew(true);
            self.availableOptions(self.ExternalScripts());
        };

        this.groupName = ko.validateField({
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
                url: Kooboo.ResourceGroup.isUniqueName(),
                type: "GET",
                data: {
                    name: function() {
                        return self.groupName()
                    },
                    type: function() {
                        return "script"
                    }
                },
                message: Kooboo.text.validation.taken
            }
        });

        this.groupId = ko.observable();

        this.groupModal = ko.observable(false);

        this.isGroupNew = ko.observable(true);

        this.isNewGroupValid = function() {
            return self.groupName.isValid();
        }

        this.handleEnter = function(m, e) {
            return e.keyCode !== 13;
        }

        this.onGroupModalSubmit = function() {
            function afterUpdate() {
                self.hideGroupModal();
                self.curType("Group");
                self.getDataByType[self.curType()]();
            }

            if (self.isGroupNew()) {

                if (self.isNewGroupValid()) {
                    self.showError(false);
                    Kooboo.ResourceGroup.Create(JSON.stringify({
                        Id: Kooboo.Guid.Empty,
                        name: self.groupName(),
                        typeName: Kooboo.Script.name,
                        children: self.groupChildren()
                    })).then(function(res) {

                        if (res.success) {
                            afterUpdate();
                        }
                    })
                } else {
                    self.showError(true);
                }
            } else {
                Kooboo.ResourceGroup.Update(JSON.stringify({
                    Id: self.groupId(),
                    name: self.groupName(),
                    typeName: Kooboo.Script.name,
                    children: self.groupChildren()
                })).then(function(res) {

                    if (res.success) {
                        afterUpdate();
                    }
                });
            }
        };

        this.hideGroupModal = function() {
            self.groupModal(false);
            self.groupName("");
            self.groupId("");
            self.groupChildren([]);
            self.showError(false);
        };

        this.availableOptions = ko.observableArray();

        this.selectedOption = ko.observable();

        this.optionsCache = [];
        this.addGroupItem = function() {
            var item = _.findLast(self.ExternalScripts(), function(script) {
                return script.routeId == self.selectedOption();
            });

            self.groupChildren.push(item);

            self.selectedOption("");
        };

        this.groupChildren = ko.observableArray();
        this.groupChildren.subscribe(function(newChildren) {
            var _avail = [];
            _.forEach(self.ExternalScripts(), function(script) {
                var isExist = _.findLast(newChildren, function(selected) {
                    return script.routeId === (selected["routeId"] || selected["objectId"]);
                });

                if (!isExist) {
                    _avail.push(script);
                }
            });

            self.availableOptions(_avail);
        });

        this.removeItem = function(m) {
            var item = _.findLast(self.groupChildren(), function(child) {

                if (child["routeId"]) {
                    return child.routeId == m["routeId"] || m["objectId"];
                } else {
                    return child.objectId == m["routeId"] || m["objectId"];
                }
            });

            self.groupChildren.remove(item);
        }

        Kooboo.EventBus.subscribe("kb/table/delete/finish", function() {
            Kooboo.Script.getExternalList().then(function(res) {
                if (res.success) {
                    self.ExternalScripts(res.model);
                }
            });
        })
    }
    scriptViewModel.prototype = new Kooboo.tableModel(Kooboo.Script.name);

    var vm = new scriptViewModel();

    Kooboo.EventBus.subscribe("ko/table/script/group/modal", function(id) {
        Kooboo.ResourceGroup.Get({
            id: id
        }).then(function(res) {

            if (res.success) {
                vm.isGroupNew(false);
                vm.groupModal(true);
                vm.groupId(id);
                vm.groupName(res.model.name);
                vm.groupChildren(res.model.children);
            }
        })
    });

    ko.applyBindings(vm, document.getElementById("main"));

});