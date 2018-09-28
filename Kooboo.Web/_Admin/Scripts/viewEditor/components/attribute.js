(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/viewEditor/components/attribute.html"),
        DataStore = Kooboo.viewEditor.store.DataStore,
        BindingStore = Kooboo.viewEditor.store.BindingStore,
        DataContext = Kooboo.viewEditor.DataContext,
        Filter = Kooboo.viewEditor.util.fieldFilter;

    var bindingType = "attribute",
        repeatKey = "k-repeat",
        repeatAttr = "[" + repeatKey + "]",
        repeatSelfKey = repeatKey + "-self";

    ko.components.register("kb-view-attribute", {
        viewModel: function(params) {
            var self = this;
            this.onSave = params.onSave;

            this.origIds = [];

            Kooboo.EventBus.subscribe("binding/edit", function(data) {

                if (data.bindingType === bindingType) {
                    self.elem = data.elem;
                    self.isShow(true);
                    self.attributes(_.map(Kooboo.objToArr(data.attributes), function(attr) {
                        return new attributeViewModel(attr);
                    }));
                    Kooboo.EventBus.publish("tal-attribute:DataStoreChanged", { type: "edit" });
                    self.origIds = self.getDataSourceIds();
                }
            })

            Kooboo.EventBus.subscribe("DataStore/change", function() {
                Kooboo.EventBus.publish("tal-attribute:DataStoreChanged", { type: "change" });
            })

            function attributeViewModel(attribute) {
                var _self = this;

                attribute = attribute || {};

                var possibleKeys = [
                    "alt",
                    "class",
                    "href",
                    "id",
                    "src",
                    "style",
                    "title",
                    "value"
                ];

                this.key = ko.validateField(attribute.key, {
                    required: Kooboo.text.validation.required
                });

                this.key.subscribe(function(val) {});

                this.keys = ko.observableArray(possibleKeys);

                this.value = ko.validateField(attribute.value, {
                    required: Kooboo.text.validation.required
                });

                //self.dataSource
                this.fields = ko.observableArray();

                this.showError = ko.observable(false);

                Kooboo.EventBus.subscribe("tal-attribute:DataStoreChanged", function(data) {
                    _self.refreshFields(data.type);
                });

                this.refreshFields = function(type) {
                    if (self.elem) {
                        var fields = DataContext.get(self.elem).getDataSource(),
                            _fields = [];

                        _.forEach(fields, function(field) {
                            _fields.push({
                                name: field.name,
                                list: Filter.getNotEnumerableList(field)
                            })
                        });

                        var repeatElements = self.getRepeatElements();

                        if (repeatElements.length) {
                            _.forEach(repeatElements, function(el) {

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
                    }

                    switch (type) {
                        case "change":
                        case "edit":
                            _self.fields(_.uniqBy(_fields, function(o) { return o.name }));
                            _self.value(attribute.value);
                            break;
                        case "add":
                            if (_.isEmpty(attribute.value)) {
                                _self.fields(_.uniqBy(_fields, function(o) { return o.name }));
                            }
                            break;
                    }
                }

                this.valid = function() {
                    return this.key.valid() && this.value.valid();
                }
            }

            this.elem = null;

            this.attributes = ko.observableArray();

            this.addAttribute = function() {
                self.attributes.push(new attributeViewModel({ key: "", value: "" }));
                Kooboo.EventBus.publish("tal-attribute:DataStoreChanged", { type: "add" });
            }

            this.removeAttribute = function(m) {
                self.attributes.remove(m);
            }

            this.insertDataField = function(attr, dataField) {
                attr.value(attr.value() + "{" + dataField.name + "}");
            }

            this.getRawAttributes = function() {
                var rawAttribute = "";
                _.forEach(self.attributes(), function(attr) {
                    var value = attr.value().split('"').join("'");
                    rawAttribute += attr.key();
                    rawAttribute += " ";
                    rawAttribute += value;
                    rawAttribute += ";";
                });
                return rawAttribute;
            }

            this.getDataSourceIds = function() {
                var ids = [];
                _.forEach(self.attributes(), function(attr) {
                    var list = [];
                    attr.fields().forEach(function(field) {
                        list = _.concat(field.list, list);
                    });

                    list.forEach(function(li) {
                        if (attr.value().indexOf("{" + li.name + "}") > -1) {
                            ids.push(li.id);
                        }
                    })
                })
                return _.uniq(ids);
            }

            this.isShow = ko.observable(false);

            this.showError = ko.observable(false);

            this.valid = function() {
                var val = true;

                if (self.attributes() && self.attributes().length) {
                    _.forEach(self.attributes(), function(attr) {
                        if (!attr.valid()) {
                            val = false;
                        }
                    });
                } else {
                    val = false;
                }

                return val;
            }

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

            this.save = function() {
                if (self.valid()) {
                    var rawAttributes = ""
                    var context = {
                        bindingType: bindingType,
                        elem: self.elem,
                        text: self.getRawAttributes(),
                        ids: self.getDataSourceIds()
                    };

                    self.onSave(context);
                    BindingStore.removeAttributeBindings(self.getRemovedDataSourceId());
                    self.reset();
                }
            }

            this.reset = function() {
                self.elem = null;
                self.isShow(false);
                self.attributes(null);
                self.origIds = [];
            }

            this.getRemovedDataSourceId = function() {
                var newIds = self.getDataSourceIds(),
                    oldIds = self.origIds;

                var ids = [];

                oldIds.forEach(function(id) {
                    newIds.indexOf(id) == -1 && ids.push(id);
                })

                return ids;
            }
        },
        template: template
    });
})();