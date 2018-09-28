$(function() {

    var URLModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.curPage = ko.observable(1);

        this.routeTypes = ko.observableArray([{
            displayName: Kooboo.text.site.URL.internal,
            value: "internal"
        }, {
            displayName: Kooboo.text.site.URL.external,
            value: "external"
        }]);

        this.curType = ko.observable();

        this.handleData = function(data, type) {
            self.pager(data);
            var list = [];
            data.list.forEach(function(r) {
                var date = new Date(r.lastModified);

                var model = {
                    id: r.id,
                    name: {
                        value: r.name,
                        updateUrl: Kooboo.Route.Get(Kooboo.Url.updateUrl(), {
                            type: type,
                            id: r.id
                        })
                    },
                    type: type,
                    date: date.toDefaultLangString(),
                    hasObject: {
                        text: Kooboo.text.common[r.hasObject ? "yes" : "no"],
                        class: "label-sm " + (r.hasObject ? "label-success" : "label-default"),
                        value: r.hasObject
                    },
                    resourceType: {
                        text: Kooboo.text.common[r.resourceType] ? Kooboo.text.common[r.resourceType] : r.resourceType,
                        class: "label-sm blue"
                    },
                    relationsComm: "kb/url/relation/modal",
                    relationsTypes: Object.keys(r.relations),
                    relations: r.relations,
                    preview: {
                        text: Kooboo.text.common.preview,
                        url: r.previewUrl,
                        newWindow: true
                    }
                }

                list.push(model);
            })
            var columns = [{
                displayName: Kooboo.text.common.name,
                fieldName: "name",
                type: "editable"
            }, {
                displayName: Kooboo.text.site.URL.resourceType,
                fieldName: "resourceType",
                type: "label"
            }]

            if (self.curType() == "internal") {
                columns.push({
                    displayName: Kooboo.text.site.URL.hasObject,
                    fieldName: "hasObject",
                    type: "label"
                });
            }

            columns = columns.concat([{
                displayName: Kooboo.text.common.usedBy,
                fieldName: "relations",
                type: "communication-refer"
            }, {
                displayName: Kooboo.text.common.lastModified,
                fieldName: "date",
                type: "text"
            }])

            self.tableData({
                docs: list,
                columns: columns,
                tableActions: [{
                    fieldName: "preview",
                    type: "link-btn"
                }],
                kbType: Kooboo.Url.name,
                onDelete: function(selected) {
                    if (confirm(Kooboo.text.confirm.deleteItems)) {
                        var ids = _.map(selected, function(sel) {
                            return sel.id;
                        })

                        Kooboo.Url.Deletes({
                            type: self.curType(),
                            ids: JSON.stringify(ids)
                        }).then(function(res) {

                            if (res.success) {
                                var tableData = _.cloneDeep(self.tableData());
                                _.forEach(ids, function(id) {
                                    var find = _.find(tableData.docs, function(doc) {
                                        return doc.id == id;
                                    })

                                    if (find) {

                                        var idx = _.findIndex(tableData.docs, function(doc) {
                                            return doc.id == id;
                                        })

                                        if (find.type == "internal") {
                                            find.relations = {};
                                            find.relationsTypes = [];

                                            tableData.docs.splice(idx, 1);

                                            if (find.hasObject.value) {
                                                tableData.docs.splice(idx, 0, find);
                                            }
                                        } else {
                                            tableData.docs.splice(idx, 1);
                                        }
                                    }
                                });
                                self.tableData(tableData);
                                window.info.show(Kooboo.text.info.delete.success, true);
                            } else {
                                window.info.show(Kooboo.text.info.delete.fail, false);
                            }
                        });
                    }
                }
            });

        }

        this.changeType = function(type) {

            if (self.curType() !== type) {
                Kooboo.Url["get" + _.capitalize(type) + "List"]().then(function(res) {

                    if (res.success) {
                        self.curType(type);
                        self.handleData(res.model, type);
                    }
                })
            }
        }

        Kooboo.EventBus.subscribe("kb/table/rendered", function () {
            $('[data-inline-edit="true"]').editable({
                mode: "inline",
                type: "text",
                validate: function(value) {
                    if ($.trim(value) == '') {
                        return Kooboo.text.validation.required;
                    } else if (!/^[^\s|\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\||\\|\[|\]|\{|\}|\;|\:|\"|\'|\,|\<|\>|\?]*\w$/.test(value)) {
                        return Kooboo.text.validation.urlInvalid;
                    }
                },
                send: 'always',
                success: function(res, newName) {
                    if (!res.success) {
                        Kooboo.handleFailMessages(res.messages);
                    } else {
                        DataCache.removeRelatedData("url");
                        var tableData = _.cloneDeep(self.tableData());
                        var find = _.find(tableData.docs, function(doc) {
                            return doc.id == res.model.oldId;
                        })

                        if (find) {
                            var idx = _.findIndex(tableData.docs, function(doc) {
                                return doc.id == res.model.oldId;
                            })

                            find.id = res.model.newId;
                            find.name.value = newName;
                            find.name.updateUrl = Kooboo.Route.Get(Kooboo.Url.updateUrl(), {
                                type: find.type,
                                id: res.model.newId
                            })
                            find.preview.url = res.model.previewUrl;

                            tableData.docs.splice(idx, 1);
                            tableData.docs.splice(idx, 0, find);
                            self.tableData(tableData);
                        }
                    }
                }
            }).on("shown", function(e, editor) {
                editor.input.$input.click(function(e) {
                    e.stopPropagation();
                })
            });
        })

        Kooboo.EventBus.subscribe("kb/url/relation/modal", function(r) {
            Kooboo.EventBus.publish("kb/relation/modal/show", {
                type: self.curType() == "internal" ? "route" : "externalResource",
                id: r.id,
                by: r.by
            })
        })

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {

            Kooboo.Url["get" + _.capitalize(self.curType()) + "List"]({
                pageNr: page
            }).then(function(res) {
                if (res.success) {
                    self.handleData(res.model, self.curType());
                    self.curPage(page);
                }
            })
        })
    }

    URLModel.prototype = new Kooboo.tableModel();
    var vm = new URLModel();

    ko.applyBindings(vm, document.getElementById("main"));
    vm.changeType("internal");
})