(function() {
    Kooboo.loadJS([
        "/_Admin/Scripts/components/controlType/TextBox.js",
        "/_Admin/Scripts/components/controlType/Selection.js",
        "/_Admin/Scripts/components/controlType/CheckBox.js"
    ]);

    var template = Kooboo.getTemplate("/_Admin/Scripts/events/activityDialog.html");
    ko.components.register("activity-dialog", {
        viewModel: function(params) {
            var self = this;
            self.modalShow = params.modalShow;

            self.name = ko.observable();

            self.settings = ko.observableArray([]);
            self.container = ko.observableArray([]);

            self.cacheData = ko.observable();

            self.onSave = params.onSave;

            self.mode = ko.observable();

            params.activityData.subscribe(function(data) {
                self.name(data.name);
                self.mode(data.mode);
                self.cacheData(data);
                data.settings.forEach(function(setting) {
                    setting.displayName = setting.displayName || setting.name;
                    setting.fieldValue = setting.defaultValue;

                    if (data.values) {
                        setting.fieldValue = data.values[setting.name];
                    } else if (data.setting) {
                        setting.fieldValue = getValue(data.setting, setting.name);
                    }

                    if (setting.controlType == 'Selection') {
                        setting.selectionOptions = JSON.stringify(Kooboo.objToArr(setting.selectionValues).map(function(s) {
                            return {
                                key: s.value,
                                value: s.key
                            }
                        }))
                    }
                })

                function getValue(setting, name) {
                    var keys = Object.keys(setting).map(function(key) {
                            return key.toLowerCase();
                        }),
                        idx = keys.indexOf(name.toLowerCase());

                    return (idx > -1) ? setting[keys[idx]] : ''
                }

                self.settings(data.settings.map(function(setting) {
                    return new Field(setting);
                }));
            })

            self.submit = function() {
                var data = self.cacheData(),
                    values = {};

                self.settings().forEach(function(setting) {
                    values[setting.name()] = setting.fieldValue();

                    var orig = _.find(data.settings, function(s) {
                        return s.name == setting.name();
                    })

                    orig && (orig.defaultValue = setting.fieldValue());
                })

                data.values = values;

                self.onSave(data);
                self.close();

                // console.log(params);
                // console.log(self.container);
                // //transfer setting from [{name:xx, values:xx, controlType: "textBox", defaultValue: "", values:""}] to {key: value, key: value}
                // var o = {}
                // self.setting().forEach(function(setting) {
                //     o[setting.name()] = setting.values;
                // })
                // self.activity.setting = o;
                // if (params.activityData().mode === "edit") {
                //     var index = params.activityData().index;
                //     self.container.splice(index, 1, self.activity);
                // } else {
                //     self.container.push(self.activity);
                // }
                // self.close()
            }

            self.close = function() {
                self.modalShow(false)
            }

        },
        template: template
    })


    function Field(data) {

        this._id = ko.observable(Kooboo.getRandomId());

        this.showError = ko.observable(false);

        this.isShow = data.isShow;

        this.name = ko.observable(data.name);

        this.fieldName = ko.observable(data.displayName);

        this.fieldValue = ko.observable(data.fieldValue);

        this.lang = ko.observable(data.lang);

        this.controlType = ko.observable(data.controlType);

        this.disabled = ko.observable(data.disabled);

        this.tooltip = ko.observable(data.tooltip);

        this.options = ko.observableArray(JSON.parse(data.options || data.selectionOptions || '[]'));

        this.isMultilingual = ko.observable(data.isMultilingual && data.isMultilingualSite);

        this.isMultipleValue = ko.observable(data.multipleValue);

        var _validations = JSON.parse(data.validations || '[]'),
            validateRules = {};
        this.validations = ko.observableArray(_validations);
        for (var i = 0, len = _validations.length; i < len; i++) {
            var rule = _validations[i],
                type = (rule.type || rule.validateType),
                msg = (rule.msg || rule.errorMessage);

            switch (type) {
                case 'required':
                    validateRules[type] = msg || Kooboo.text.validation.required;
                    break;
                case 'regex':
                    validateRules[type] = {
                        pattern: new RegExp(rule.pattern),
                        message: msg || Kooboo.text.validation.inputError
                    };
                    break;
                case 'range':
                    validateRules[type] = {
                        from: Number(rule.min),
                        to: Number(rule.max),
                        message: msg || Kooboo.text.validation.inputError
                    };
                    break;
                case 'stringLength':
                    validateRules['stringlength'] = {
                        min: parseInt(rule.min),
                        max: parseInt(rule.max),
                        message: msg || Kooboo.text.validation.inputError
                    };
                    break;
                case 'min':
                case 'max':
                case 'minLength':
                case 'maxLength':
                case 'minChecked':
                case 'maxChecked':
                    validateRules[type] = {
                        value: Number(rule.value),
                        message: msg || Kooboo.text.validation.inputError
                    };
                    break;

            }
        }
        this.fieldValue.extend({ validate: validateRules })
        this.validateRules = validateRules;

        this.isValid = function() {
            return this.fieldValue.isValid();
        }
    }
})()