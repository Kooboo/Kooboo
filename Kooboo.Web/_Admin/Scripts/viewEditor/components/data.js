(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/viewEditor/components/data.html"),
        DataStore = Kooboo.viewEditor.store.DataStore,
        ComparerStore = Kooboo.viewEditor.store.ComparerStore,
        DataContext = Kooboo.viewEditor.DataContext,
        Filter = Kooboo.viewEditor.util.fieldFilter;

    var bindingType = "data",
        repeatKey = "k-repeat",
        repeatAttr = "[" + repeatKey + "]",
        repeatSelfKey = repeatKey + "-self";

    ko.components.register("kb-view-data", {
        viewModel: function(params) {
            var self = this;
            this.onSave = params.onSave;
            this.elem = null;
            this.text = ko.validateField({
                required: Kooboo.text.validation.required
            });
            this.showError = ko.observable(false);
            this.fields = ko.observableArray([]);
            this.isShow = ko.observable(false);

            Kooboo.EventBus.subscribe("binding/edit", function(data) {
                if (data.bindingType == bindingType) {
                    self.elem = data.elem;
                    self.isShow(true);
                    self.refreshFields();
                    self.text(data.text);
                }
            });
            Kooboo.EventBus.subscribe("DataStore/change", function() {
                self.refreshFields();
            })
            Kooboo.EventBus.subscribe("kb/view/editor/data/pager", function(info) {
                self.onSave({
                    bindingType: bindingType,
                    elem: info.elem,
                    text: info.pager,
                    dataSourceId: info.dataSourceId || null
                })
            })
            this.refreshFields = function() {
                if (self.elem) {
                    var fields = DataContext.get(self.elem).getDataSource(),
                        _fields = [];

                    fields.forEach(function(field) {
                        _fields.push({
                            name: field.name,
                            list: Filter.getNotEnumerableList(field, [])
                        })
                    });

                    var repeatElements = self.getRepeatElements();

                    if (repeatElements.length) {
                        repeatElements.forEach(function(el) {

                            var itemName = el.key.split(" ")[0];

                            var find = _.find(fields, function(field) {
                                return field.name == itemName;
                            });

                            if (find) {
                                _fields.push({
                                    name: find.name,
                                    list: Filter.getNotEnumerableList(find)
                                });
                            } else {
                                var match = itemName.match(/(\w*)_Item$/);
                                if (match && el.repeatSelf) {
                                    itemName = match[1];
                                    var _find = _.find(fields, function(field) {
                                        return field.name == itemName;
                                    })

                                    if (_find) {
                                        _fields.push({
                                            name: _find.name + "_Item",
                                            list: Filter.getNotEnumerableList(_find)
                                        })
                                    }
                                }
                            }
                        })
                    }

                    _fields.forEach(function(field) {

                        var nameGroup = _.groupBy(field.list, function(li) {
                            return li.name;
                        })

                        var temp = []

                        var names = Object.keys(nameGroup);
                        names.forEach(function(name) {
                            if (nameGroup[name].length > 1) {
                                var _temp = nameGroup[name][0];
                                var ids = [];
                                nameGroup[name].forEach(function(item) {
                                    ids.push(item.id);
                                })
                                delete _temp.id;
                                _temp.ids = ids;
                                temp.push(_temp);
                            } else {
                                temp = temp.concat(nameGroup[name])
                            }
                        })

                        var list = _.uniqBy(field.list, function(li) { return li.name });
                        field.list = list;
                    })
                    self.fields(_.uniqBy(_fields, function(o) { return o.name }));
                }
            };
            this.valid = function() {
                if (!self.text.valid()) {
                    return false;
                }
                return true;
            };
            this.add = function() {
                Kooboo.EventBus.publish('action/edit', {
                    parentId: null,
                    context: {
                        actions: ko.observableArray(DataStore.getAll())
                    }
                });
            };
            this.save = function() {
                if (self.valid()) {
                    var context = {
                        bindingType: bindingType,
                        elem: self.elem,
                        text: self.text(),
                        dataSourceId: self.getDataSourceId(self.text()) || null
                    };
                    self.onSave(context);
                    self.reset();
                } else {
                    self.showError(true);
                }
            };
            this.getRepeatElements = function(elem) {
                var repeatElements = [],
                    _parent = elem || self.elem;

                while ($(_parent).closest(repeatAttr).length) {
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
            this.reset = function() {
                self.elem = null;
                self.text(null);
                self.showError(false);
                self.isShow(false);
            };

            this.getDataSourceId = function(text) {
                var id = null;
                self.fields().forEach(function(field) {
                    var find = _.find(field.list, function(li) { return li.name == text; })
                    find && (id = find.id || find.ids);
                })
                return id;
            }
        },
        template: template
    });
})();