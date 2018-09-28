(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/contentType/fieldEditor.html"),
        controlTypeConfig = Kooboo.fieldEditor.controlTypeConfig;

    ko.components.register("field-editor", {
        viewModel: function(params) {
            var self = this;
            var storedField = {};
            var changableControlType = ["TextBox", "TextArea", "Tinymce", "Number"];
            this.showDialog = params.showDialog;
            this.choosedField = params.choosedField;
            this.selectionOptions = ko.observableArray([]);
            this.isNew = params.isNewField;
            this.controlTypeConfig = controlTypeConfig;
            this.changableControlTypeConfig = controlTypeConfig.filter(function(o) {
                return changableControlType.indexOf(o.value) > -1;
            })
            this.onSave = params.onSave;
            this.previewImage = ko.observable();
            this.previewType = ko.observable();
            this.showError = ko.observable(false);
            this.isValid = function() {
                if (self.isOptionRequired()) {
                    self._optionRequired(self.selectionOptions().length ? "required" : "");
                    return self.choosedField().name.isValid() && self.isAbleToAddOption() && self._optionRequired.isValid();
                } else {
                    return self.choosedField().name.isValid();
                }
            }

            this.isOptionRequired = function() {
                return ["selection", "checkbox", "radiobox"].indexOf(self.choosedField().controlType().toLowerCase()) > -1
            }

            this.isValidationValid = function() {
                self.hideValidatioError();
                var allValid = true;

                _.forEach(self.choosedField().validations(), function(validation) {
                    switch (validation.name()) {
                        case "regex":
                            if (!validation.pattern.isValid()) {
                                validation.showError(true);
                                allValid = false;
                            } else {
                                validation.showError(false);
                            }
                            break;
                        case "range":
                        case "stringLength":
                            if (validation.isValid()) {
                                validation.showError(false);
                            } else {
                                validation.showError(true);
                                allValid = false;
                            }
                            break;
                        case "min":
                            if (!validation.minValue.isValid()) {
                                validation.showError(true);
                                allValid = false;
                            } else {
                                validation.showError(false);
                            }
                            break;
                        case "max":
                            if (!validation.maxValue.isValid()) {
                                validation.showError(true);
                                allValid = false;
                            } else {
                                validation.showError(false);
                            }
                            break;
                    }
                })
                return allValid;
            }
            this.hideValidatioError = function() {
                _.forEach(self.choosedField().validations(), function(validation) {
                    validation.showError && validation.showError(false);
                })
            }

            this.fullValidationRules = [{
                displayName: Kooboo.text.validationRule.required,
                value: "required"
            }, {
                displayName: Kooboo.text.validationRule.stringLength,
                value: "stringLength"
            }, {
                displayName: Kooboo.text.validationRule.regex,
                value: "regex"
            }, {
                displayName: Kooboo.text.validationRule.dataType,
                value: "dataType"
            }, {
                displayName: Kooboo.text.validationRule.range,
                value: "range"
            }, {
                displayName: Kooboo.text.validationRule.min,
                value: "min"
            }, {
                displayName: Kooboo.text.validationRule.max,
                value: "max"
            }];

            this.onlyRequireValidationRule = [{
                displayName: Kooboo.text.validationRule.required,
                value: "required"
            }];

            this.numberValidationRules = [{
                displayName: Kooboo.text.validationRule.required,
                value: "required"
            }, {
                displayName: Kooboo.text.validationRule.stringLength,
                value: "stringLength"
            }, {
                displayName: Kooboo.text.validationRule.regex,
                value: "regex"
            }, {
                displayName: Kooboo.text.validationRule.range,
                value: "range"
            }, {
                displayName: Kooboo.text.validationRule.min,
                value: "min"
            }, {
                displayName: Kooboo.text.validationRule.max,
                value: "max"
            }];

            this.checkboxValidationRules = [{
                displayName: Kooboo.text.validationRule.required,
                value: "required"
            }, {
                displayName: Kooboo.text.validationRule.minChecked,
                value: "minChecked"
            }, {
                displayName: Kooboo.text.validationRule.maxChecked,
                value: "maxChecked"
            }]

            this.validateType = ko.observable();
            this.validationRules = ko.observableArray(_.cloneDeep(self.fullValidationRules));
            this.controlTypeChangable = ko.observable(true);

            this.choosedField.subscribe(function(data) {
                if (data) {
                    data.dataType = ko.pureComputed(function() {
                        if (!!data.controlType()) {
                            return _.find(self.controlTypeConfig, function(o) { return o.value === data.controlType() })["dataType"];
                        }
                    });

                    if (data.controlType()) {
                        if (data.controlType() == "Boolean") {
                            data.controlType("Switch");
                        }
                        var ct = _.find(self.controlTypeConfig, function(o) { return o.value === data.controlType() });
                        if (ct) {
                            self.previewImage(ct.preview);
                            self.previewType(ct.value);
                        }
                    }

                    if (data.controlType) {
                        data.controlType.subscribe(function(controlType) {
                            if (!!controlType) {
                                var ct = _.find(self.controlTypeConfig, function(o) { return o.value === controlType });
                                self.previewImage(ct.preview)
                                self.previewType(ct.value);
                                if (changableControlType.indexOf(controlType) < 0) {
                                    self.controlTypeChangable(false);
                                } else {
                                    self.controlTypeChangable(true);
                                }

                                setValidationsRuleByControlType(controlType);

                                var _list = [],
                                    newValidations = _.map(self.validationRules(), function(o) { return o.value });
                                _.forEach(getOriginalValidations(), function(v) {
                                    if (newValidations.indexOf(v.validateType()) > -1) {
                                        _list.push(v);
                                    }
                                })

                                data.validations(getValidationsList(_list));
                                checkDuplicateValidation();

                                self.selectionOptions().forEach(function(opt) {
                                    self.removeOption(opt);
                                })
                                self._optionRequired("");
                                self.showError(false);
                            }
                        })
                    }

                    self.selectionOptions(data.selectionOptions().map(function(opt) {
                        return new optionModel(opt);
                    }));

                    if (!self.isNew()) {
                        if (changableControlType.indexOf(data.controlType()) < 0) {
                            self.controlTypeChangable(false);
                        } else {
                            self.controlTypeChangable(true);
                        }
                    } else {
                        data.controlType("TextBox");
                    }

                    storedField = storeChoosedField(data);

                    setValidationsRuleByControlType(data.controlType());

                    data.validations = ko.observableArray(getValidationsList(getOriginalValidations()));

                    checkDuplicateValidation();

                    function setValidationsRuleByControlType(type) {
                        switch (type) {
                            case "TextBox":
                            case "TextArea":
                                self.validationRules(_.cloneDeep(self.fullValidationRules));
                                break;
                            case "Boolean":
                            case "Selection":
                                self.validationRules([]);
                                break;
                            case "CheckBox":
                                self.validationRules(_.cloneDeep(self.checkboxValidationRules));
                                break;
                            case "RadioBox":
                                self.validationRules(_.cloneDeep(self.onlyRequireValidationRule));
                                break;
                            case "Tinymce":
                            case "MediaFile":
                            case "DateTime":
                                self.validationRules(_.cloneDeep(self.onlyRequireValidationRule));
                                break;
                            case "Number":
                                self.validationRules(_.cloneDeep(self.numberValidationRules));
                                break;
                        }
                    }

                    function getValidationsList(validations) {
                        var list = [];
                        _.forEach(validations, function(v) {
                            switch (v.validateType()) {
                                case "required":
                                    list.push(new requireValidation(v.errorMessage()));
                                    break;
                                case "stringLength":
                                    list.push(new stringLengthValidation(v.min(), v.max(), v.errorMessage()));
                                    break;
                                case "range":
                                    list.push(new rangeValidation(v.min(), v.max(), v.errorMessage()));
                                    break;
                                case "dataType":
                                    list.push(new dataTypeValidation(v.dataType(), v.errorMessage()));
                                    break;
                                case "regex":
                                    list.push(new regexValidation(v.pattern(), v.errorMessage()));
                                    break;
                                case "min":
                                    list.push(new minValidation(v.minValue(), v.errorMessage()));
                                    break;
                                case "max":
                                    list.push(new maxValidation(v.maxValue(), v.errorMessage()));
                                    break;
                                case "minChecked":
                                    list.push(new minCheckedValidation(v.minChecked(), v.errorMessage()));
                                    break;
                                case "maxChecked":
                                    list.push(new maxCheckedValidation(v.maxChecked(), v.errorMessage()));
                                    break;
                            }
                        });
                        return list;
                    }

                    function getOriginalValidations() {
                        var validations = null;
                        if (typeof data.validations() === "string") {
                            validations = ko.mapping.fromJS(JSON.parse(data.validations()))();
                        } else {
                            validations = ko.mapping.fromJS(data.validations())();
                        }
                        return validations;
                    }

                    function checkDuplicateValidation() {
                        data.validations().forEach(function(item) {
                            var rule = _.find(self.validationRules(), function(rule) {
                                return rule.value == item.validateType()
                            })
                            if (rule) {
                                self.validationRules.remove(rule);
                            }
                        })
                    }
                }
            })

            this._optionRequired = ko.validateField("", {
                required: ''
            })
            this.addOption = function() {
                if (self.isAbleToAddOption()) {
                    self.selectionOptions.push(new optionModel());
                    self._optionRequired("required");
                }
            }

            this.isAbleToAddOption = ko.pureComputed(function() {
                var allValid = true;
                self.selectionOptions().forEach(function(s) {
                    !s.isValid() && (allValid = false);
                })
                return allValid;
            })

            this.removeOption = function(m) {
                m.showError(false);
                self.selectionOptions.remove(m);

                self._optionRequired(self.selectionOptions().length ? "requried" : "");
                self.hideOptionsError();
                self.showOptionsError();
            }

            this.showOptionsError = function() {
                this.selectionOptions().forEach(function(o) {
                    !o.isValid() && o.showError(true);
                })
            }
            this.hideOptionsError = function() {
                this.selectionOptions().forEach(function(o) {
                    o.showError(false);
                })
            }

            function storeChoosedField(data) {
                var ob = {};
                ob = ko.mapping.toJS(data);
                return ob;
            }

            function requireValidation(msg) {
                this.name = ko.observable("required");
                this.validateType = ko.observable("required");
                this.errorMessage = ko.observable(Kooboo.text.validation.required);

                if (arguments.length) {
                    this.errorMessage(msg);
                }
            }

            function stringLengthValidation(min, max, msg) {
                var self = this;
                this.name = ko.observable("stringLength");
                this.validateType = ko.observable("stringLength");
                this.errorMessage = ko.observable(Kooboo.text.validation.inputError);
                this.showError = ko.observable(false);
                this.min = ko.validateField({
                    required: Kooboo.text.validation.required,
                    min: {
                        value: 0
                    }
                });
                this.min.subscribe(function() {
                    if (self.showError()) {
                        self.showError(!self.isValid());
                    }
                })
                this.max = ko.validateField({
                    required: Kooboo.text.validation.required
                });
                this.max.subscribe(function() {
                    if (self.showError()) {
                        self.showError(!self.isValid());
                    }
                })
                this.result = ko.validateField("result", {
                    required: Kooboo.text.error.rangeError
                })

                this.isValid = function() {
                    if (!this.min.isValid()) return false;
                    if (!this.max.isValid()) return false;

                    var min = Number(this.min()),
                        max = Number(this.max());
                    if (min > max) {
                        this.result("");
                        return false;
                    }
                    return true;
                }

                if (arguments.length) {
                    this.min(min);
                    this.max(max);
                    this.errorMessage(msg);
                }
            }

            function rangeValidation(min, max, msg) {
                var self = this;
                this.name = ko.observable("range");
                this.validateType = ko.observable("range");
                this.errorMessage = ko.observable(Kooboo.text.validation.inputError);
                this.showError = ko.observable(false);
                this.min = ko.validateField({
                    required: Kooboo.text.validation.required,
                });
                this.min.subscribe(function() {
                    if (self.showError()) {
                        self.showError(!self.isValid());
                    }
                })
                this.max = ko.validateField({
                    required: Kooboo.text.validation.required,
                });
                this.max.subscribe(function() {
                    if (self.showError()) {
                        self.showError(!self.isValid());
                    }
                })
                this.result = ko.validateField("result", {
                    required: Kooboo.text.error.rangeError
                })

                this.isValid = function() {
                    if (!this.min.isValid()) return false;
                    if (!this.max.isValid()) return false;

                    var min = Number(this.min()),
                        max = Number(this.max());
                    if (min > max) {
                        this.result("");
                        return false;
                    }
                    return true;
                }

                if (arguments.length) {
                    this.min(min);
                    this.max(max);
                    this.errorMessage(msg);
                }
            }

            function dataTypeValidation(type, msg) {
                this.name = ko.observable("dataType");
                this.validateType = ko.observable("dataType");
                this.errorMessage = ko.observable(Kooboo.text.error.dataType);
                this.dataType = ko.observable("");
                this.types = ko.observableArray([{
                    displayName: Kooboo.text.validation.dataType.Integer,
                    value: "Integer"
                }, {
                    displayName: Kooboo.text.validation.dataType.Decimal,
                    value: "Decimal"
                }, {
                    displayName: Kooboo.text.validation.dataType.Number,
                    value: "Number"
                }, {
                    displayName: Kooboo.text.validation.dataType.Guid,
                    value: "Guid"
                }, {
                    displayName: Kooboo.text.validation.dataType.String,
                    value: "String"
                }, {
                    displayName: Kooboo.text.validation.dataType.DateTime,
                    value: "DateTime"
                }, {
                    displayName: Kooboo.text.validation.dataType.Bool,
                    value: "Bool"
                }])

                if (arguments.length) {
                    this.dataType(type);
                    this.errorMessage(msg);
                }
            }

            function regexValidation(pattern, msg) {
                this.name = ko.observable("regex");
                this.validateType = ko.observable("regex");
                this.errorMessage = ko.observable(Kooboo.text.validation.inputError);
                this.pattern = ko.validateField({
                    required: Kooboo.text.validation.required
                });
                this.showError = ko.observable(false);

                if (arguments.length) {
                    this.pattern(pattern);
                    this.errorMessage(msg);
                }
            }

            function minValidation(min, msg) {
                this.name = ko.observable("min");
                this.validateType = ko.observable("min");
                this.errorMessage = ko.observable(Kooboo.text.validation.greaterThan + "...");
                this.showError = ko.observable(false);
                this.minValue = ko.validateField({
                    required: Kooboo.text.validation.required,
                    dataType: {
                        type: "Number",
                        message: Kooboo.text.error.dataType + ', ' + Kooboo.text.correct.dataType.number
                    }
                });

                if (arguments.length) {
                    this.minValue(min);
                    this.errorMessage(msg);
                }
            }

            function maxValidation(max, msg) {
                this.name = ko.observable("max");
                this.validateType = ko.observable("max");
                this.errorMessage = ko.observable(Kooboo.text.validation.lessThan + "...");
                this.showError = ko.observable(false);
                this.maxValue = ko.validateField({
                    required: Kooboo.text.validation.required,
                    dataType: {
                        type: "Number",
                        message: Kooboo.text.error.dataType + ', ' + Kooboo.text.correct.dataType.number
                    }
                });

                if (arguments.length) {
                    this.maxValue(max);
                    this.errorMessage(msg);
                }
            }

            function minCheckedValidation(min, msg) {
                this.name = ko.observable("minChecked");
                this.validateType = ko.observable("minChecked");
                this.errorMessage = ko.observable();
                this.showError = ko.observable(false);
                this.minChecked = ko.validateField({
                    required: "",
                    dateType: {
                        type: "Number",
                        message: Kooboo.text.error.dataType + ', ' + Kooboo.text.correct.dataType.number
                    }
                })

                if (arguments.length) {
                    this.minChecked(min);
                    this.errorMessage(msg);
                }
            }

            function maxCheckedValidation(max, msg) {
                this.name = ko.observable("maxChecked");
                this.validateType = ko.observable("maxChecked");
                this.errorMessage = ko.observable();
                this.showError = ko.observable(false);
                this.maxChecked = ko.validateField({
                    required: "",
                    dateType: {
                        type: "Number",
                        message: Kooboo.text.error.dataType + ', ' + Kooboo.text.correct.dataType.number
                    }
                })

                if (arguments.length) {
                    this.maxChecked(max);
                    this.errorMessage(msg);
                }
            }

            this.addValidation = function() {
                switch (self.validateType()) {
                    case "required":
                        this.choosedField().validations.push(new requireValidation);
                        break;
                    case "stringLength":
                        this.choosedField().validations.push(new stringLengthValidation);
                        break;
                    case "range":
                        this.choosedField().validations.push(new rangeValidation);
                        break;
                    case "dataType":
                        this.choosedField().validations.push(new dataTypeValidation);
                        break;
                    case "regex":
                        this.choosedField().validations.push(new regexValidation);
                        break;
                    case "min":
                        this.choosedField().validations.push(new minValidation);
                        break;
                    case "max":
                        this.choosedField().validations.push(new maxValidation);
                        break;
                    case "minChecked":
                        this.choosedField().validations.push(new minCheckedValidation);
                        break;
                    case "maxChecked":
                        this.choosedField().validations.push(new maxCheckedValidation);
                        break;
                }
                var rule = _.find(self.validationRules(), function(rule) {
                    return rule.value == self.validateType();
                })
                self.validationRules.remove(rule);
            }

            this.removeValidation = function() {}

            this.getSavedSelectionOptions = function() {
                return this.selectionOptions().map(function(opt) {
                    return { key: opt.key(), value: opt.value() }
                })
            }

            this.cancel = function() {
                self.restore();
                self.showError(false);
                self.hideValidatioError();
                self.hideOptionsError();
                self.reset();
            };
            this.save = function() {
                if (self.isValidationValid()) {
                    if (!self.isValid()) {
                        if (!$("#field-editor li").first().hasClass("active")) {
                            $("#field-editor .nav-tabs a").first().tab("show").on("shown.bs.tab", function() {
                                self.showError(true);
                                if (self.isOptionRequired()) {
                                    self.showOptionsError();
                                }
                            })
                        } else {
                            self.showError(true);
                            if (self.isOptionRequired()) {
                                self.showOptionsError();
                            }
                        }
                    } else {
                        self.choosedField().selectionOptions(self.getSavedSelectionOptions());
                        if (['CheckBox', 'MediaFile', 'Selection', 'Boolean', 'RadioBox', 'Number', 'DateTime'].indexOf(self.choosedField().controlType()) !== -1) {
                            self.choosedField().multipleLanguage(false);
                        }

                        if (self.choosedField().controlType().toLowerCase() == "checkbox") {
                            self.choosedField().multipleValue(true);
                        }

                        if (!self.choosedField().displayName()) {
                            self.choosedField().displayName(self.choosedField().name());
                        }

                        self.onSave(self.choosedField());
                        self.reset();
                    }
                } else {
                    $("a[href=#tab_1_3]").tab("show");
                }
            };
            Kooboo.EventBus.subscribe("ko/contentType/field/removeValidate", function(validateRule) {
                validateRule.hasError && validateRule.hasError(false);
                self.choosedField().validations.remove(validateRule);
                var rule = _.find(self.fullValidationRules, function(rule) {
                    return rule.value == validateRule.validateType();
                })
                self.validationRules.push(rule);
            });

            this.restore = function() {
                self.choosedField().name(storedField.name);
                self.choosedField().isSummaryField(storedField.isSummaryField);
                self.choosedField().controlType(storedField.controlType);
                self.choosedField().displayName(storedField.displayName);
                self.choosedField().multipleLanguage(storedField.multipleLanguage);
                self.choosedField().tooltip(storedField.tooltip);
                self.choosedField().editable(storedField.editable);
                if (typeof storedField.validations === "string") {
                    self.choosedField().validations = ko.mapping.fromJS(JSON.parse(storedField.validations));
                } else {
                    self.choosedField().validations = ko.mapping.fromJS(storedField.validations);
                }

                self.validationRules.removeAll();
                self.validationRules(_.cloneDeep(self.fullValidationRules));

                if (typeof storedField.selectionOptions === "string") {
                    self.choosedField().selectionOptions(JSON.parse(storedField.selectionOptions));
                } else {
                    self.choosedField().selectionOptions(storedField.selectionOptions);
                }

                self.choosedField().multipleValue(storedField.multipleValue);
                $("a[href=#tab_1_1]").tab("show");
            }

            this.reset = function() {
                $("a[href=#tab_1_1]").tab("show");
                self.showError(false);
                self.validationRules.removeAll();
                self.validationRules(_.cloneDeep(self.fullValidationRules));
                self.showDialog(false);
                self.choosedField(null);
            }
        },
        template: template
    })

    function optionModel(opt) {
        var self = this;

        this.key = ko.validateField((opt && opt.key) || "", {
            required: ''
        })

        this.value = ko.validateField((opt && opt.value) || "", {
            required: ''
        })

        this.isValid = function() {
            return self.key.isValid() && self.value.isValid();
        }

        this.showError = ko.observable(false);
    }
})()