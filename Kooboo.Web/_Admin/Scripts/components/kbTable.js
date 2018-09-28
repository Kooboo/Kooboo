(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbTable.html");
    ko.components.register("kb-table", {
        viewModel: function(params) {
            var self = this;

            this.kbType = ko.observable(params.kbType);

            // modified column
            /* 
             * format should be:
             * [{
             *    displayName: "Name",
             *    fieldName: "name",
             *    type: "text"
             * },{
             *    displayName: "Layout type",
             *    fieldName: "type",
             *    type: "label"
             * },
             *    ...]
             */

            self.columns = ko.observableArray(params.columns);
            self.columns.subscribe(function(newCol) {
                self.allSelected(false);
            })

            var _class = "table table-striped table-hover ";
            self.tableClass = ko.observable(_class + (params.class || ""))

            // display documents
            var list = [];
            _.forEach(params.docs, function(doc) {
                list.push(new docModel(doc));
            });
            self.docs = ko.observableArray(list);

            // status: if all the docs is selected
            self.allSelected = ko.pureComputed({
                read: function() {

                    var selectableDocs = _.filter(self.docs(), function(doc) {
                        return !doc.unselectable();
                    })

                    if (selectableDocs.length === 0) {
                        return false;
                    }

                    return self.selectedDocs().length === selectableDocs.length;
                },
                write: function(value) {

                    if (typeof value === "boolean") {
                        self.selectedDocs.removeAll();

                        var selectableDocs = _.filter(self.docs(), function(doc) {
                            return !doc.unselectable();
                        })

                        _.forEach(selectableDocs, function(doc) {
                            doc.selected(value);
                            value && self.selectedDocs.push(doc);
                        })
                    }
                },
                owner: this
            });

            self.enableAllSelected = ko.pureComputed(function() {
                var unselectableCount = _.filter(self.docs(), function(doc) {
                    return doc.unselectable();
                }).length;
                return self.docs().length && (self.docs().length !== unselectableCount)
            })

            // Array: a storage of all the selected docs.
            self.selectedDocs = ko.observableArray();
            self.selectedDocs.subscribe(function(selected) {
                Kooboo.EventBus.publish("ko/table/delete/show", selected.length > 0);
                // publish another event for customizing
                Kooboo.EventBus.publish("ko/table/docs/selected", selected);
            });

            // function: toggle selecte the current doc.
            self.onSelectCurrentDoc = function(doc) {

                if (!self.unselectable() && !doc.unselectable()) {
                    doc.selected(!doc.selected());

                    if (doc.selected()) {
                        self.selectedDocs.push(doc);
                    } else {
                        self.selectedDocs.remove(doc);
                    }

                    return true;
                }
            };

            self.tableActions = ko.observableArray(params.tableActions || []);

            self.unselectable = ko.observable(!!params.unselectable);

            self.deleteSelected = function() {
                if (self.kbType()) {
                    var confirmStr = self.isSelectedDocsContainsRef() ? Kooboo.text.confirm.deleteItemsWithRef : Kooboo.text.confirm.deleteItems;
                    if (confirm(confirmStr)) {
                        var ids = self.getSelectedId();
                        Kooboo[self.kbType()].Deletes({
                            ids: JSON.stringify(ids)
                        }).then(function(res) {

                            if (res.success) {
                                _.forEach(ids, function(id) {
                                    var doc = _.findLast(self.docs(), function(doc) {
                                        return doc.id() === id;
                                    });

                                    self.docs.remove(doc);

                                    var _idx = _.findIndex(params.docs, function(d) {
                                        return d.id == doc.id()
                                    });
                                    params.docs.splice(_idx, 1);
                                });

                                self.selectedDocs.removeAll();

                                Kooboo.EventBus.publish("kb/table/delete/finish", {
                                    ids: ids
                                });

                                window.info.show(Kooboo.text.info.delete.success, true);
                            }
                        });
                    }
                } else {
                    console.warn("kbType not defined.");
                }
            }

            self.getSelectedId = function() {
                return _.map(self.selectedDocs(), function(selected) {
                    return selected.id();
                });
            }

            self.isSelectedDocsContainsRef = function() {
                var find = _.find(self.selectedDocs(), function(doc) {
                    return doc.relations && Object.keys(doc.relations).length;
                })

                return !!find;
            }

            // handle outer operations
            self.linkTo = function(href, newWindow) {

                if (!newWindow) {
                    location.href = href
                } else {
                    window.open(href)
                }
            }

            self.communicate = function(command, args) {
                Kooboo.EventBus.publish(command(), args);
            }

            self.afterTableRendered = function() {
                Kooboo.EventBus.publish("kb/table/rendered");
            }

            Kooboo.EventBus.publish("ko/table/delete/show", (self.selectedDocs().length > 0));

            Kooboo.EventBus.unsubscribe("ko/table/on/delete");

            Kooboo.EventBus.subscribe("ko/table/on/delete", function(para) {
                if (params.onDelete) {
                    params.onDelete(ko.mapping.toJS(self.selectedDocs()));
                } else {
                    self.deleteSelected(para);
                }
            });
        },
        template: template
    });

    var docModel = function(doc) {

        var self = this;

        if (doc.relations) {
            var reorderedKeys = _.sortBy(Object.keys(doc.relations));
            doc.relationsTypes = reorderedKeys;
        }

        ko.mapping.fromJS(doc, {}, self);

        self.selected = ko.observable(false);

        self.onSelect = function(doc) {
            doc.selected(!doc.selected());
            return true;
        }

        if (!doc.hasOwnProperty("unselectable")) {
            self.unselectable = ko.observable(false);
        }

    }
})()