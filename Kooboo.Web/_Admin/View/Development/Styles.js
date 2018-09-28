$(function() {
    var styleViewModel = function() {
        var self = this;

        this.styleCreateUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.Style.Create);
        });

        this.types = ko.observableArray([{
            displayName: Kooboo.text.site.script.external,
            value: "External"
        }, {
            displayName: Kooboo.text.site.script.embedded,
            value: "Embedded"
        }, {
            displayName: Kooboo.text.site.style.inline,
            value: "Inline"
        }, {
            displayName: Kooboo.text.site.script.group,
            value: "Group"
        }]);

        this.curType = ko.observable('External');

        this.fetchDataByType = function(type) {
            if (type !== self.curType() || type == "Inline") {
                self.getDataByType[type]();
                self.curType(type);
            }
        };

        this.groupNameList = ko.observableArray();

        this.getDataByType = {
            External: function() {

                Kooboo.Style.getExternalList().then(function(res) {
                    if (res.success) {
                        var styleList = [];
                        self.ExternalStyles(res.model);
                        _.forEach(res.model, function(style) {
                            var date = new Date(style.lastModified);

                            var model = {
                                id: style.id,
                                name: {
                                    text: style.name,
                                    url: Kooboo.Route.Get(Kooboo.Route.Style
                                        .DetailPage, {
                                            Id: style.id
                                        })
                                },
                                preview: {
                                    text: style.routeName,
                                    url: style.fullUrl,
                                    newWindow: true
                                },
                                date: date.toDefaultLangString(),
                                relationsComm: "kb/relation/modal/show",
                                relationsTypes: Object.keys(style.references),
                                relations: style.references,
                                versions: {
                                    iconClass: "fa-clock-o",
                                    title: Kooboo.text.common.viewAllVersions,
                                    url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
                                        KeyHash: style.keyHash,
                                        storeNameHash: style.storeNameHash
                                    }),
                                    newWindow: true
                                }
                            }

                            styleList.push(model);
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
                            docs: styleList,
                            tableActions: [{
                                fieldName: "versions",
                                type: "link-icon"
                            }],
                            kbType: Kooboo.Style.name
                        }

                        self.tableData(cpnt);

                    }
                });
            },
            Embedded: function() {

                Kooboo.Style.getEmbeddedList().then(function(res) {
                    if (res.success) {
                        var styleList = [];
                        _.forEach(res.model, function(style) {
                            var date = new Date(style.lastModified),
                                model = {
                                    id: style.id,
                                    name: {
                                        text: style.name,
                                        url: Kooboo.Route.Get(Kooboo.Route.Style
                                            .DetailPage, {
                                                Id: style.id
                                            })
                                    },
                                    relationsComm: "kb/relation/modal/show",
                                    relationsTypes: Object.keys(style.references),
                                    relations: style.references,
                                    date: date.toDefaultLangString()
                                };

                            styleList.push(model);
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
                            docs: styleList,
                            kbType: Kooboo.Style.name
                        }

                        self.tableData(cpnt);
                    }
                });
            },
            Inline: function() {

                Kooboo.CSSRule.getInlineList().then(function(res) {
                    if (res.success) {
                        var styleList = [];
                        _.forEach(res.model, function(style) {
                            var date = new Date(style.lastModified);

                            var model = {
                                id: style.id,
                                name: {
                                    text: style.name,
                                    url: "ko/table/style/inline/modal"
                                },
                                ownerType: {
                                    text: Kooboo.text.component.table[style.ownerType.toLowerCase()],
                                    class: "label-sm " /*+ Kooboo.getLabelClass(style.ownerType)*/ ,
                                    bgColor: Kooboo.getLabelColor(style.ownerType)
                                },
                                ownerName: style.ownerName,
                                date: date.toDefaultLangString()
                            };

                            styleList.push(model);
                        });

                        var columns = [{
                            displayName: Kooboo.text.site.style.name,
                            fieldName: "name",
                            type: "communication-link"
                        }, {
                            displayName: Kooboo.text.site.style.ownerType,
                            fieldName: "ownerType",
                            type: "label"
                        }, {
                            displayName: Kooboo.text.site.style.siteObject,
                            fieldName: "ownerName",
                            type: "text"
                        }, {
                            displayName: Kooboo.text.common.lastModified,
                            fieldName: "date",
                            type: "text"
                        }];

                        var cpnt = {
                            columns: columns,
                            docs: styleList,
                            kbType: Kooboo.CSSRule.name
                        }

                        self.tableData(cpnt);
                    }
                });
            },
            Group: function() {
                Kooboo.ResourceGroup.Style().then(function(res) {

                    if (res.success) {
                        var styleList = [],
                            nameList = [];
                        _.forEach(res.model, function(style) {
                            var date = new Date(style.lastModified),
                                model = {
                                    id: style.id,
                                    name: {
                                        text: style.name,
                                        url: "ko/table/style/group/modal"
                                    },
                                    childrenCount: {
                                        text: style.childrenCount > 0 ? style.childrenCount : "NO_VALUE",
                                        url: "ko/table/style/group/modal",
                                        class: "blue"
                                    },
                                    relationsComm: "kb/relation/modal/show",
                                    relationsTypes: Object.keys(style.references),
                                    relations: style.references,
                                    date: date.toDefaultLangString(),
                                    preview: {
                                        text: Kooboo.text.common.preview,
                                        url: style.previewUrl,
                                        newWindow: true
                                    }
                                };

                            styleList.push(model);
                            nameList.push(style.name);
                        });
                        self.groupNameList(nameList);

                        var columns = [{
                            displayName: Kooboo.text.common.name,
                            fieldName: "name",
                            type: "communication-link"
                        }, {
                            displayName: Kooboo.text.site.style.children,
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
                            docs: styleList,
                            tableActions: actions,
                            kbType: Kooboo.ResourceGroup.name
                        }

                        self.tableData(cpnt);
                    }
                })
            }
        }

        this.uploadStyle = function(data, files) {

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

                    Kooboo.Upload.Style(formdata, counter).then(function(res) {
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
                if (!Kooboo.isFileNameExist(files, self.ExternalStyles())) {
                    upload();
                } else {
                    if (confirm(Kooboo.text.confirm.overrideFile)) {
                        upload();
                    }
                }
            }
        }

        this.ExternalStyles = ko.observableArray();

        this.getDataByType[this.curType()]();

        // Group modal
        this.showError = ko.observable(false);

        this.createGroup = function() {
            self.groupName("");
            self.groupModal(true);
            self.isGroupNew(true);
            self.availableOptions(self.ExternalStyles());
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
                        return "style"
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
                        typeName: Kooboo.Style.name,
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
                    typeName: Kooboo.Style.name,
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
            var item = _.findLast(self.ExternalStyles(), function(style) {
                return style.routeId == self.selectedOption();
            });

            self.groupChildren.push(item);

            self.selectedOption("");
        };

        this.groupChildren = ko.observableArray();
        this.groupChildren.subscribe(function(newChildren) {
            var _avail = [];
            _.forEach(self.ExternalStyles(), function(style) {
                var isExist = _.findLast(newChildren, function(selected) {
                    return style.routeId === (selected["routeId"] || selected[
                        "objectId"]);
                });

                if (!isExist) {
                    _avail.push(style);
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

        this.inlineModal = ko.observable(false);

        this.hideInlineModal = function() {
            self.styleRules([]);
            self.inlineModal(false);
            setTimeout(function() {
                self.getStyleRules(null);
            }, 800);
        }

        this.saveInlineModal = function() {
            setTimeout(function() {
                if (self.isInlineRuleChange()) {
                    var rule = self.getStyleRules()[0];

                    var ruleText = "";
                    _.forEach(rule.declarations, function(dec) {
                        ruleText += dec.name + ": " + dec.value + (dec.important ? " !important;" : ";");
                    })

                    Kooboo.CSSRule.updateInline({
                        id: rule.id,
                        ruleText: ruleText
                    }).then(function(res) {

                        if (res.success) {
                            self.hideInlineModal();
                            self.fetchDataByType("Inline");
                            self.getStyleRules(null);
                        }
                    })
                } else {
                    self.hideInlineModal();
                    $(".page-loading").hide();
                }
            }, 1000);
            $(".page-loading").show();
        }

        this.isInlineRuleChange = function() {

            if (self.getStyleRules()) {
                var rule = _.cloneDeep(self.getStyleRules()[0]),
                    orig = self.styleRules()[0];

                if ((rule.id !== orig.id) ||
                    (rule.selector !== orig.selector) ||
                    (rule.declarations.length !== rule.declarations.length)) {
                    return true;
                }

                var _decChanged = false;

                _.forEach(orig.declarations, function(dec) {
                    var find = _.findLast(rule.declarations, function(d) {
                        return d.id == dec.id;
                    })

                    if (find) {
                        _decChanged = _decChanged ||
                            ((dec.name !== find.name) || (dec.value !== find.value) || (dec.important !== find.important));

                        _.remove(rule.declarations, function(d) {
                            return d.id == dec.id;
                        });
                    } else {
                        _decChanged = true;
                    }
                })

                return rule.declarations.length ? true : _decChanged;

            } else {
                return false;
            }
        }

        this.styleRules = ko.observable();

        this.getStyleRules = ko.observable();

        this.mediaDialogData = ko.observable();

        Kooboo.EventBus.subscribe("kb/table/delete/finish", function() {
            Kooboo.Style.getExternalList().then(function(res) {
                if (res.success) {
                    self.ExternalStyles(res.model);
                }
            });
        })
    }
    styleViewModel.prototype = new Kooboo.tableModel(Kooboo.Style.name);

    var vm = new styleViewModel();

    Kooboo.EventBus.subscribe("ko/table/style/inline/modal", function(id) {

        Kooboo.CSSRule.getInline({
            id: id
        }).then(function(res) {

            if (res.success) {
                var decList = [];
                _.forEach(res.model.declarations, function(dec) {
                    decList.push({
                        id: newID(),
                        name: dec.propertyName,
                        value: dec.value,
                        important: dec.important
                    })
                })

                vm.styleRules([{
                    id: res.model.id,
                    selector: res.model.selector,
                    declarations: decList
                }]);

                vm.inlineModal(true);
            }
        })
    });

    Kooboo.EventBus.subscribe("ko/table/style/group/modal", function(id) {
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

    Kooboo.EventBus.subscribe("ko/style/list/pickimage/show", function(ctx) {

        Kooboo.Media.getList().then(function(res) {

            if (res.success) {
                res.model["show"] = true;
                res.model["context"] = ctx;
                res.model["onAdd"] = function(selected) {
                    var _oldValue = ctx.valueString(),
                        regex = _oldValue.match(/url\((\S+)\)/);

                    if (regex && regex.length) {
                        var newValue = _oldValue.split(regex[1]).join("'" + selected.url + "'");
                        ctx.valueString(newValue);
                    } else {
                        ctx.valueString(_oldValue + " url('" + selected.url + "')");
                    }
                }
                vm.mediaDialogData(res.model);
            }
        });
    })

    ko.applyBindings(vm, document.getElementById("main"));

    var newID = function() {
        return Math.ceil(Math.random() * Math.pow(2, 53));
    }
});