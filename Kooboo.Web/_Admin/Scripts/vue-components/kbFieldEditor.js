(function () {
    Kooboo.loadJS([
        "/_Admin/Scripts/vue-components/kbDialog.js",
        "/_Admin/Scripts/vue-components/kbTabs.js",
        "/_Admin/Scripts/vue-components/kbForm.js",
        "/_Admin/Scripts/vue-components/kbFieldValidation.js"

    ]);
    Kooboo.loadJS(["/_Admin/Scripts/Kooboo/ControlType.js"]);

    var self;
    Vue.component("kb-field-editor", {
        template: Kooboo.getTemplate(
            "/_Admin/Scripts/vue-components/kbFieldEditor.html"
        ),
        props: {
            closeHandle: {require: true},
            data: {require: true},
            allItems: {require: true},
            editingIndex: {require: true},
            options: {}
        },
        data: function () {
            return {
                displayNames: [
                    Kooboo.text.component.fieldEditor.basic,
                    Kooboo.text.component.fieldEditor.advanced,
                    Kooboo.text.component.fieldEditor.validation
                ],
                tabIndex: 0,
                isNewField: true,
                d_data: {
                    name: "",
                    displayName: "",
                    controlType: "TextBox",
                    isSummaryField: false,
                    multipleLanguage: false,
                    editable: true,
                    order: 0,
                    tooltip: "",
                    validations: [],
                    multilingual: undefined,
                    modifiedFieldName: undefined,
                    selectionOptions: []
                },
                formRules: {

            },
                validateModel: {},
                optionsValidateModel: undefined,
                _controlTypes: []
            };
        },
        created: function () {
            self = this;

            self._controlTypes = self.getControlTypes(self.options.controlTypes);
            if (self.data && !self.options.isNewField) {
                self.isNewField = false;
                self.d_data = _.cloneDeep(self.data);
                self.d_data.selectionOptions = JSON.parse(self.d_data.selectionOptions);
                var data = self.d_data;
                var multilingual =
                    data.hasOwnProperty("multilingual") ||
                    data.hasOwnProperty("multipleLanguage")
                        ? data.multilingual || data.multipleLanguage
                        : true;
                self.multilingual = multilingual;
            }
            self.generateValidateModel()
        },
        watch: {
            d_data: {
                handler: function (val, oldVal) {
                    self.generateValidateModel()
                },
                deep: true
            }
        },
        methods: {
            getControlTypes: function (types) {
                var _types = [];
                var CONTROL_TYPES = Kooboo.controlTypes;
                types.forEach(function (t) {
                    var _t = CONTROL_TYPES.find(function (c) {
                        return c.value.toLowerCase() == t;
                    });

                    _types.push(_t || {displayName: "NOT_FOUND"});
                });

                return _types;
            },
            getControlTypeByValue: function (value) {
                return self._controlTypes.find(function (item) {
                    return item.value.toLowerCase() === value.toLowerCase();
                });
            },
            addOption: function () {
                self.d_data.selectionOptions.push({key: "", value: ""})
            },
            removeOption: function (event, index) {
                self.d_data.selectionOptions.splice(index, 1)
            },
            generateValidateModel: function () {
                self.validateModel = {};
                var temp = {valid: true, msg: ''};
                self.validateModel.name = temp;
                if (self.d_data.selectionOptions) {
                    self.validateModel.selectionOptions = [];
                    self.d_data.selectionOptions.forEach(function (item) {
                        self.validateModel.selectionOptions.push({key:temp,value:temp})
                    })
                }

            },
            onModalSave: function () {

                var hasError = false;

                var nameValidate = Kooboo.validate({name: self.d_data.name}, {name: [
                        {required: true, message: Kooboo.text.validation.required},
                        {
                            pattern: /^([A-Za-z][\w\-]*)*[A-Za-z0-9]$/,
                            message: Kooboo.text.validation.contentTypeNameRegex
                        },
                        {
                            validate: function (value) {
                                var status = true;
                                self.allItems.forEach(function(item, index){
                                    if(item.name === value && !(index === self.editingIndex)) {
                                        status = false
                                    }
                                });
                                return  status
                            },
                            message: Kooboo.text.validation.taken
                        }

                    ]});
                self.validateModel.name=nameValidate.result.name;
                if(nameValidate.hasError){
                    hasError = true;
                }
                this.d_data.selectionOptions.forEach(function (item,index) {
                    var keyRules = {
                        key:[{required: true, message: Kooboo.text.validation.required}],
                        value:[{required: true, message: Kooboo.text.validation.required}]
                    };
                    var keyValues = {key:item.key,value: item.value};
                    var validate = Kooboo.validate(keyValues, keyRules);
                    if(validate.hasError) {
                        hasError = true;
                    }
                    self.validateModel.selectionOptions[index] = validate.result
                });
                if(hasError){
                    self.tabIndex = 0;
                    this.$forceUpdate();
                }else{

                }


            }
        }
    });
})();

