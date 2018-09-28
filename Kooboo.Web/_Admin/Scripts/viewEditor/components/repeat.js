(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/viewEditor/components/repeat.html"),
        DataStore = Kooboo.viewEditor.store.DataStore,
        ActionStore = Kooboo.viewEditor.store.ActionStore,
        DataContext = Kooboo.viewEditor.DataContext,
        Filter = Kooboo.viewEditor.util.fieldFilter;

    var bindingType = "repeat",
        repeatKey = "k-repeat",
        repeatAttr = "[" + repeatKey + "]",
        repeatSelfKey = repeatKey + "-self";

    ko.components.register("kb-view-repeat", {
        viewModel: function(params) {
            var self = this;

            this.onSave = params.onSave;

            this.elem = null;
            this.text = ko.validateField({
                required: Kooboo.text.validation.required
            });
            this.showError = ko.observable(false);

            this.isShow = ko.observable();

            this.fields = ko.observableArray();

            this.repeatSelf = ko.observable(false);

            this.fieldId = ko.observable();

            Kooboo.EventBus.subscribe("binding/edit", function(data) {

                if (data.bindingType == bindingType) {
                    self.elem = data.elem;
                    self.isShow(true);
                    self.refreshFields();
                    self.text(data.text);
                    self.repeatSelf(data.repeatSelf);
                }
            });

            Kooboo.EventBus.subscribe("DataStore/change", function() {
                self.refreshFields();
            })

            this.refreshFields = function() {
                if (self.elem) {
                    var fields = DataContext.get(self.elem).getDataSource(),
                        _fields = [];

                    var repeatElements = self.getRepeatElements(self.elem);

                    if (repeatElements.length) {
                        repeatElements.forEach(function(el) {

                            var itemName = el.key.split(" ")[0],
                                listName = el.key.split(" ")[1],
                                find = null;

                            if (self.elem == el.elem) {
                                find = _.findLast(fields, function(field) {
                                    if (listName.indexOf(".") > -1) {
                                        return field.name == listName.split(".")[0];
                                    } else {
                                        return field.name == listName;
                                    }
                                })

                                if (find) {
                                    _fields.push({
                                        name: find.name,
                                        list: Filter.getEnumerableList(find)
                                    })
                                }
                            } else {
                                find = _.find(fields, function(field) {
                                    return field.name == itemName;
                                })

                                if (find && find.name.match(/(\w*)_Item$/)) {
                                    _fields.push({
                                        name: find.name,
                                        list: Filter.getEnumerableList(find)
                                    })
                                }
                            }
                        })

                        var usedName = [];
                        _fields.forEach(function(field) {
                            if (field.name.indexOf("_Item") > -1) {
                                usedName.push(field.name.split("_Item")[0]);
                            } else {
                                usedName.push(field.name);
                            }
                        })

                        var filtered = _.filter(fields, function(field) {
                            if (field.name.indexOf("_Item") > -1) {
                                return usedName.indexOf(field.name.split("_Item")[0]) == -1;
                            } else {
                                return usedName.indexOf(field.name) == -1;
                            }
                        });

                        filtered.forEach(function(field) {
                            _fields.push({
                                name: field.name,
                                list: Filter.getEnumerableList(field)
                            });
                        })
                    } else {
                        fields.forEach(function(field) {
                            _fields.push({
                                name: field.name,
                                list: Filter.getEnumerableList(field)
                            });
                        });
                    }
                }

                self.fields(_.uniqBy(_fields, function(o) { return o.name }));
            };

            this.add = function() {
                Kooboo.EventBus.publish('action/edit', {
                    parentId: null,
                    context: {
                        actions: ko.observableArray(DataStore.getAll())
                    }
                });
            };

            this.valid = function() {
                return self.text.isValid();
            };

            this.save = function() {
                if (self.valid()) {
                    self.onSave({
                        bindingType: bindingType,
                        elem: self.elem,
                        text: self.text(),
                        repeatSelf: self.repeatSelf(),
                        dataSourceId: self.getDataSourceId(self.text()) || null
                    });
                    self.reset();
                } else {
                    self.showError(true);
                }
            };

            this.reset = function() {
                self.elem = null;
                self.text(null);
                self.showError(false);
                self.isShow(false);
            };

            this.getRepeatElements = function(elem) {
                var repeatElements = [],
                    _parent = elem || self.elem;

                while ($(_parent).closest(repeatAttr).length &&
                    !repeatElements.length) {
                    var __parent = $(_parent).closest(repeatAttr)[0];
                    repeatElements.push({
                        elem: __parent,
                        key: $(__parent).attr(repeatKey),
                        repeatSelf: !!$(__parent).attr(repeatSelfKey)
                    });
                    _parent = $(__parent).parent()[0];
                }

                return repeatElements;
            }

            this.getDataSourceId = function(text) {
                var id = null;
                self.fields().forEach(function(field) {
                    var find = _.find(field.list, function(li) { return li.name == text; })
                    find && (id = find.id);
                })
                return id;
            }
        },
        template: template
    });

})();