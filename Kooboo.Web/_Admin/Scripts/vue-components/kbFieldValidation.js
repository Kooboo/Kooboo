(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbFieldValidation.html");

    ko.components.register("kb-field-validation", {
        viewModel: function(params) {
            var self = this;

            this.field = params.field();
            this.curValidations = this.field.validations;

            this.field.on('controlType/change', function(type) {
                var options = [];
                self.field._validations([]);

                switch (type.toLowerCase()) {
                    case "textbox":
                    case "textarea":
                        options.push(new ValidationRule.required);
                        options.push(new ValidationRule.stringLength);
                        options.push(new ValidationRule.minLength);
                        options.push(new ValidationRule.maxLength);
                        options.push(new ValidationRule.regex);
                        break;
                    case "richeditor":
                    case "mediafile":
                    case "datetime":
                    case "radiobox":
                        options.push(new ValidationRule.required);
                        break;
                    case "checkbox":
                        options.push(new ValidationRule.required);
                        options.push(new ValidationRule.minChecked);
                        options.push(new ValidationRule.maxChecked);
                        break;
                    case "number":
                        options.push(new ValidationRule.required);
                        options.push(new ValidationRule.range);
                        options.push(new ValidationRule.regex);
                        options.push(new ValidationRule.min);
                        options.push(new ValidationRule.max);
                        break;
                }

                var _temp = options.map(function(o) {
                    return {
                        displayName: o.name(),
                        value: o.type()
                    }
                });

                var newValidations = [];
                if (self.curValidations().length) {
                    self.curValidations().forEach(function(v) {

                        var idx = _.findIndex(_temp, function(t) {
                            return t.value == v.type
                        });

                        if (idx > -1) {
                            _temp.splice(idx, 1);
                            newValidations.push(v);

                            switch (v.type) {
                                case "required":
                                    self.field._validations.push(new ValidationRule.required(v.msg));
                                    break;
                                case "stringLength":
                                    self.field._validations.push(new ValidationRule.stringLength(v.min, v.max, v.msg));
                                    break;
                                case "range":
                                    self.field._validations.push(new ValidationRule.range(v.min, v.max, v.msg));
                                    break;
                                case "regex":
                                    self.field._validations.push(new ValidationRule.regex(v.pattern, v.msg));
                                    break;
                                case "min":
                                case "max":
                                case "minLength":
                                case "maxLength":
                                case "minChecked":
                                case "maxChecked":
                                    self.field._validations.push(new ValidationRule[v.type](v.value, v.msg));
                                    break;
                            }
                        }
                    })
                }

                self.availableOptions(_temp);
                self.curValidations(newValidations);
            })

            this.availableOptions = ko.observableArray();

            this.rule = ko.observable();

            this.addRule = function(m, e) {
                if (this.availableOptions().length) {
                    this.field._validations.push(new ValidationRule[this.rule()]);
                    var options = this.availableOptions(),
                        idx = _.findIndex(options, function(opt) {
                            return opt.value == self.rule();
                        })

                    options.splice(idx, 1);
                    this.availableOptions(options);
                } else {
                    e.preventDefault();
                }
            }

            this.removeRule = function(m, e) {

                var showingError = m.showError();

                self.field._validations().forEach(function(v) {
                    v.showError(false);
                })

                self.field._validations.remove(m);
                self.availableOptions.push({
                    displayName: m.name(),
                    value: m.type()
                })

                if (showingError) {
                    self.field._validations().forEach(function(v) {
                        v.showError(true);
                    })
                }
            }

            this.inputNumberOnly = function(m, e) {
                if (e.keyCode >= 48 && e.keyCode <= 57 /*number*/ ) {
                    return true;
                } else if (e.keyCode >= 96 && e.keyCode <= 105 /*number*/ ) {
                    return true;
                } else if (e.keyCode == 190 /*.*/ ||
                    e.keyCode == 69 /*e*/ ||
                    e.keyCode == 8 /*BACKSPACE*/ ||
                    e.keyCode == 189 /*-*/ ) {
                    return true;
                } else {
                    return false;
                }
            }
        },
        template: template
    })

    var ValidationRule = {
        required: function(msg) {
            this.showError = ko.observable(false);
            this.name = ko.observable(Kooboo.text.validationRule.required);
            this.type = ko.observable("required");
            this.errorMessage = ko.observable(msg);
            this.isValid = function() { return true; }
            this.getValue = function() {
                return {
                    type: this.type(),
                    msg: this.errorMessage()
                }
            }
        },
        stringLength: function(min, max, msg) {
            var self = this;
            this.showError = ko.observable(false);
            this.name = ko.observable(Kooboo.text.validationRule.stringLength);
            this.type = ko.observable("stringLength");
            this.min = ko.validateField(min || 0, {
                required: "",
                min: { value: 0 }
            });
            this.min.subscribe(function() {
                if (self.showError()) {
                    self.showError(!self.isValid());
                }
            })
            this.max = ko.validateField(max || 0, {
                required: "",
                min: { value: 0 }
            });
            this.max.subscribe(function() {
                if (self.showError()) {
                    self.showError(!self.isValid());
                }
            })
            this.errorMessage = ko.observable(msg);
            this.result = ko.validateField({
                required: Kooboo.text.error.rangeError
            });
            this.isValid = function() {
                if (!this.min.isValid() || !this.max.isValid()) {
                    this.result("ERROR BEFORE");
                    return false;
                }

                var min = Number(this.min()),
                    max = Number(this.max()),
                    flag = true;

                if (min > max) {
                    this.result("");
                    flag = false;
                } else {
                    this.result("DONE");
                    flag = true
                }

                return flag;
            }
            this.getValue = function() {
                return {
                    type: this.type(),
                    min: this.min(),
                    max: this.max(),
                    msg: this.errorMessage()
                }

            }
        },
        range: function(min, max, msg) {
            var self = this;
            this.showError = ko.observable(false);
            this.name = ko.observable(Kooboo.text.validationRule.range);
            this.type = ko.observable("range");
            this.min = ko.validateField(min || 0, {
                required: ""
            });
            this.min.subscribe(function() {
                if (self.showError()) {
                    self.showError(!self.isValid());
                }
            })
            this.max = ko.validateField(max || 0, {
                required: ""
            });
            this.max.subscribe(function() {
                if (self.showError()) {
                    self.showError(!self.isValid());
                }
            })
            this.errorMessage = ko.observable(msg);
            this.result = ko.validateField({
                required: Kooboo.text.error.rangeError
            });
            this.isValid = function() {
                if (!this.min.isValid() || !this.max.isValid()) {
                    this.result("ERROR BEFORE");
                    return false;
                }

                var min = Number(this.min()),
                    max = Number(this.max()),
                    flag = true;

                if (min > max) {
                    this.result("");
                    flag = false;
                } else {
                    this.result("DONE");
                }

                return flag;
            }
            this.getValue = function() {
                return {
                    type: this.type(),
                    min: this.min(),
                    max: this.max(),
                    msg: this.errorMessage()
                }

            }
        },
        regex: function(pattern, msg) {
            this.showError = ko.observable(false);
            this.name = ko.observable(Kooboo.text.validationRule.regex);
            this.type = ko.observable("regex");
            this.pattern = ko.validateField(pattern || "", {
                required: ""
            })
            this.errorMessage = ko.observable(msg);
            this.isValid = function() {
                return this.pattern.isValid();
            }
            this.getValue = function() {
                return {
                    type: this.type(),
                    pattern: this.pattern(),
                    msg: this.errorMessage()
                }

            }
        },
        min: function(value, msg) {
            this.showError = ko.observable(false);
            this.name = ko.observable(Kooboo.text.validationRule.min);
            this.type = ko.observable("min");
            this.value = ko.validateField(value || 0, {
                required: ""
            });
            this.errorMessage = ko.observable(msg);
            this.isValid = function() {
                return this.value.isValid();
            }
            this.getValue = function() {
                return {
                    type: this.type(),
                    value: this.value(),
                    msg: this.errorMessage()
                }

            }
        },
        max: function(value, msg) {
            this.showError = ko.observable(false);
            this.name = ko.observable(Kooboo.text.validationRule.max);
            this.type = ko.observable("max");
            this.value = ko.validateField(value || 0, {
                required: ""
            });
            this.errorMessage = ko.observable(msg);
            this.isValid = function() {
                return this.value.isValid();
            }
            this.getValue = function() {
                return {
                    type: this.type(),
                    value: this.value(),
                    msg: this.errorMessage()
                }

            }
        },
        minLength: function(value, msg) {
            this.showError = ko.observable(false);
            this.name = ko.observable(Kooboo.text.validationRule.minLength);
            this.type = ko.observable("minLength");
            this.value = ko.validateField(value || 0, {
                required: ""
            });
            this.errorMessage = ko.observable(msg);
            this.isValid = function() {
                return this.value.isValid();
            }
            this.getValue = function() {
                return {
                    type: this.type(),
                    value: this.value(),
                    msg: this.errorMessage()
                }

            }
        },
        maxLength: function(value, msg) {
            this.showError = ko.observable(false);
            this.name = ko.observable(Kooboo.text.validationRule.maxLength);
            this.type = ko.observable("maxLength");
            this.value = ko.validateField(value || 0, {
                required: "",
                min: { value: 0 }
            });
            this.errorMessage = ko.observable(msg);
            this.isValid = function() {
                return this.value.isValid();
            }
            this.getValue = function() {
                return {
                    type: this.type(),
                    value: this.value(),
                    msg: this.errorMessage()
                }

            }
        },
        minChecked: function(value, msg) {
            this.showError = ko.observable(false);
            this.name = ko.observable(Kooboo.text.validationRule.minChecked);
            this.type = ko.observable("minChecked");
            this.value = ko.validateField(value || 0, {
                required: "",
                min: { value: 0 }
            });
            this.errorMessage = ko.observable(msg);
            this.isValid = function() {
                return this.value.isValid();
            }
            this.getValue = function() {
                return {
                    type: this.type(),
                    value: this.value(),
                    msg: this.errorMessage()
                }

            }
        },
        maxChecked: function(value, msg) {
            this.name = ko.observable(Kooboo.text.validationRule.maxChecked);
            this.showError = ko.observable(false);
            this.type = ko.observable("maxChecked");
            this.value = ko.validateField(value || 0, {
                required: "",
                min: { value: 0 }
            });
            this.errorMessage = ko.observable(msg);
            this.isValid = function() {
                return this.value.isValid();
            }
            this.getValue = function() {
                return {
                    type: this.type(),
                    value: this.value(),
                    msg: this.errorMessage()
                }
            }
        }
    }
})()