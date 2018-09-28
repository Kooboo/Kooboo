(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/viewEditor/components/condition.html"),
        ComparerStore = Kooboo.viewEditor.store.ComparerStore,
        DataContext = Kooboo.viewEditor.DataContext,
        Filter = Kooboo.viewEditor.util.fieldFilter;

    var bindingType = "condition",
        repeatKey = "k-repeat",
        repeatAttr = "[" + repeatKey + "]",
        repeatSelfKey = repeatKey + "-self";

    ko.components.register("kb-view-condition", {
        viewModel: function(params) {

            var self = this;

            this.elem = null;

            this.isShow = ko.observable(false);

            this.onSave = params.onSave;

            this.isRepeatConditionEnabled = ko.observable(false);

            this.conditionType = ko.observable("normal");

            this.normalConditionField = ko.validateField({
                required: Kooboo.text.validation.required
            });

            this.showError = ko.observable(false);

            this.fields = ko.observableArray([]);

            this.operators = ko.observableArray([]);

            this.conditionOperator = ko.validateField({
                required: Kooboo.text.validation.required
            });

            this.conditionValue = ko.validateField({
                required: Kooboo.text.validation.required
            });

            this.repeatConditionField = ko.observable();

            this.repeatConditionNumberParam = ko.validateField({
                required: Kooboo.text.validation.required
            });

            this.reset = function() {
                self.isShow(false);
                self.showError(false);
                self.normalConditionField("");
            }

            this.save = function() {
                if (self.valid()) {
                    var context = {
                        bindingType: bindingType,
                        elem: self.elem,
                        text: self.getConditionExpression()
                    };
                    self.onSave(context);
                    self.reset();
                } else {
                    self.showError(true);
                }
            }

            this.getConditionExpression = function() {
                if (self.conditionType() === 'normal') {
                    return self.normalConditionField() + ' ' + self.conditionOperator() + ' ' + self.conditionValue();
                }
                if (self.repeatConditionField() === 'repeat/') {
                    return 'repeat/' + self.repeatConditionNumberParam();
                }
                return self.repeatConditionField();
            };

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
                        var list = _.uniqBy(field.list, function(li) { return li.name });
                        field.list = list;
                    })
                    self.fields(_.uniqBy(_fields, function(o) { return o.name }));
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

            this.parseConditionExpression = function(exp) {
                var result = {
                    type: null,
                    normalField: null,
                    repeatField: null,
                    operator: null,
                    value: null,
                    number: null
                };
                var index = exp.indexOf(' ');
                if (index < 0 && exp !== "") {
                    result = {
                        type: 'repeat',
                        repeatField: exp
                    };
                    if (["odd", "even", "first", "last"].indexOf(exp.split("repeat/")[1]) == -1) {
                        result.repeatField = 'repeat/';
                        result.number = exp.substr('repeat/'.length);
                    }
                    return result;
                } else {
                    var operatorEndIndex = exp.indexOf(' ', index + 1) - 1;
                    result = {
                        type: 'normal',
                        normalField: exp.substr(0, index),
                        operator: exp.substring(index + 1, operatorEndIndex + 1),
                        value: exp.substr(operatorEndIndex + 2)
                    };
                    return result;
                }
            };

            this.valid = function() {
                if (self.conditionType() === 'normal') {
                    return self.normalConditionField.valid() &&
                        self.conditionOperator.valid() &&
                        self.conditionValue.valid();
                } else if (self.conditionType() === 'repeat') {
                    if (self.repeatConditionField() === 'repeat/') {
                        return self.repeatConditionNumberParam.valid();
                    } else {
                        return true
                    }
                }
            };

            Kooboo.EventBus.subscribe("binding/edit", function(data) {
                if (data.bindingType == bindingType) {
                    var condition = self.parseConditionExpression(data.text);
                    self.elem = data.elem;
                    self.refreshFields();
                    self.isShow(true);
                    self.isRepeatConditionEnabled(self.getRepeatElements().length > 0);
                    if (condition.type !== 'repeat' || self.isRepeatConditionEnabled()) {
                        self.conditionType(condition.type);
                        self.conditionValue(condition.value);
                        self.repeatConditionField(condition.repeatField);
                        self.repeatConditionNumberParam(condition.number);
                        self.normalConditionField(condition.normalField);
                        self.conditionOperator(condition.operator);
                    }
                }
            })

            this.normalConditionField.subscribe(function(fieldName) {
                if (!fieldName || self.conditionType() !== 'normal') {
                    self.operators([]);
                    return;
                }
                var field = null;
                var fields = self.fields();
                for (var i = 0, group; group = fields[i]; i++) {
                    for (var j = 0, f; f = group.list[j]; j++) {
                        if (fieldName === f.name) {
                            field = f;
                            break;
                        }
                    }
                }
                if (!field) {
                    self.operators([]);
                    return;
                }

                var temp = field.type.split('.');
                temp[0] = temp[0].toLowerCase();
                var type = temp.join('.');

                self.operators(ComparerStore.getByType(type));
            });
        },
        template: template
    })
})()