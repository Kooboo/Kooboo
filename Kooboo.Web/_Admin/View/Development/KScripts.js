$(function() {

    var scriptViewModel = function() {
        var self = this;

        this.kscriptCreateUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.KScript.Create);
        });

        this.types = ko.observableArray([{
            displayName: Kooboo.text.site.script.external,
            value: "External"
        }, {
            displayName: Kooboo.text.site.script.embedded,
            value: "Embedded"
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

                Kooboo.KScript.getExternalList().then(function(res) {
                    if (res.success) {
                        var scriptList = [];
                        self.ExternalScripts(res.model);
                        _.forEach(res.model, function(script) {
                            var date = new Date(script.lastModified);

                            var model = {
                                id: script.id,
                                name: {
                                    text: script.name,
                                    url: Kooboo.Route.Get(Kooboo.Route.KScript
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
                            kbType: Kooboo.KScript.name
                        }

                        self.tableData(cpnt);
                    }
                });
            },
            Embedded: function() {

                Kooboo.KScript.getEmbeddedList().then(function(res) {
                    if (res.success) {
                        var scriptList = [];
                        _.forEach(res.model, function(script) {
                            var date = new Date(script.lastModified),
                                model = {
                                    id: script.id,
                                    name: {
                                        text: script.name,
                                        url: Kooboo.Route.Get(Kooboo.Route.KScript
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
                            kbType: Kooboo.KScript.name
                        }

                        self.tableData(cpnt);
                    }
                });
            }
        }

        this.ExternalScripts = ko.observableArray();

        this.getDataByType[this.curType()]();

        Kooboo.EventBus.subscribe("kb/table/delete/finish", function() {
            Kooboo.Script.getExternalList().then(function(res) {
                if (res.success) {
                    self.ExternalScripts(res.model);
                }
            });
        })
    }
    scriptViewModel.prototype = new Kooboo.tableModel(Kooboo.KScript.name);

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