(function() {
  Kooboo.loadJS([
    "/_Admin/Scripts/components/kbDialog.js",
    "/_Admin/Scripts/components/kbTabs.js",
    "/_Admin/Scripts/components/kbForm.js",
    "/_Admin/Scripts/components/kbFieldValidation.js"
  ]);
  Kooboo.loadJS(["/_Admin/Scripts/Kooboo/ControlType.js"]);

  var self;
  Vue.component("kb-field-editor", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/kbFieldEditor.html"
    ),
    props: {
      closeHandle: { require: true },
      data: { require: true },
      allItems: { require: true },
      editingIndex: { require: true },
      options: {}
    },
    data: function() {
      return {
        showValidateTab: true,
        showOptionsPanel: false,
        showMultilvalue: false,
        tabIndex: 0,
        isNewField: true,
        type: undefined,
        isInit: true,
        addDisable: false,
        d_data: {
          name: "",
          displayName: "",
          dataType: "String",
          controlType: "TextBox",
          isSummaryField: false,
          multipleLanguage: undefined,
          editable: true,
          order: 0,
          tooltip: "",
          validations: [],
          multilingual: true,
          modifiedFieldName: undefined,
          multipleValue: undefined,
          selectionOptions: [],
          isSpecification: false
        },
        formRules: {},
        firstTabValidate: {},
        controlTypesOptions: [],
        AllControlTypes: Kooboo.controlTypes,
        multilinguable: false,
        changeModifiedField: false
      };
    },
    created: function() {
      self = this;
      if (!self.isNew()) {
        if (self.d_data.controlType.toLowerCase() === "tinymce") {
          self.d_data.controlType = "RichEditor";
        }
        try {
          if (self.data.validations.length > 0) {
            self.d_data.validations = JSON.parse(self.data.validations);
          }
        } catch (e) {
          self.d_data.validations = [];
        }
        try {
          if (self.data.selectionOptions.length > 0) {
            self.d_data.selectionOptions = JSON.parse(
              self.data.selectionOptions
            );
          }
        } catch (e) {
          self.d_data.selectionOptions = [];
        }
        var multilingual = self.data.multilingual || self.data.multipleLanguage;
        if (multilingual !== undefined) {
          self.d_data.multilingual = multilingual;
        }
        self.initDataByType(self.findControlType(self.d_data.controlType));
      } else {
        self.controlTypesOptions = self.getAllControlTypes(
          self.options.controlTypes
        );
      }
      var type = self.findControlType(self.d_data.controlType);
      if (type && type.dataType === "String") {
        self.multilinguable = true;
      }
      self.generateValidateModel();
    },
    watch: {
      d_data: {
        handler: function(val, old) {
          if (self.changeModifiedField) {
            val.controlType = val[self.options.modifiedField]
              ? "dynamicSpec"
              : "TextBox";
            self.changeModifiedField = false;
          }
          if (self.type !== val.controlType) {
            self.initDataByType(self.findControlType(self.d_data.controlType));
            self.type = val.controlType;
          }
          self.addDisableHandler();

          self.generateValidateModel();
        },
        deep: true
      }
    },
    methods: {
      initDataByType: function(item) {
        self.d_data.dataType = item.dataType;
        if (item.dataType === "String") {
          self.multilinguable = true;
        } else {
          self.multilinguable = false;
        }
        if (item.value === "Selection" || item.value === "Switch" || item.dataType == "Spec") {
          self.d_data.validations = [];
        } else {
          if (self.isInit) {
            self.d_data.validations = [];
            try {
              self.d_data.validations = JSON.parse(self.data.validations);
              if (!self.d_data.validations) self.d_data.validations = [];
            } catch (e) {
              self.d_data.validations = [];
            }
          }
        }
        if (item.dataType === "Array" || item.value === "fixedSpec") {
          if (self.isInit) {
            self.d_data.selectionOptions = [];
            try {
              self.d_data.selectionOptions = JSON.parse(
                self.data.selectionOptions
              );
              if (!self.data.selectionOptions) self.data.selectionOptions = [];
            } catch (e) {
              self.d_data.selectionOptions = [];
            }
          }
        } else {
          self.d_data.selectionOptions = [];
        }

        if (item.dataType !== "Undefined") {
          delete self.d_data.multipleValue;
        }

        self.initControlTypesOptions(item);
        self.isInit = false;
      },
      initControlTypesOptions: function(item) {
        if (self.isNewField) {
          self.controlTypesOptions = self.getAllControlTypes(
            self.isProductType && self.d_data[self.options.modifiedField]
              ? self.options.specControlTypes
              : self.options.controlTypes
          );
        } else {
          var options = self.AllControlTypes.filter(function(i) {
            if (i.dataType && i.dataType === item.dataType && i.displayName) {
              return i;
            }
          });
          self.controlTypesOptions = options;
        }
      },
      getAllControlTypes: function(types) {
        var _types = [];
        var CONTROL_TYPES = Kooboo.controlTypes;
        types.forEach(function(t) {
          var _t = CONTROL_TYPES.find(function(c) {
            return c.value.toLowerCase() === t;
          });

          _types.push(_t || { displayName: "NOT_FOUND" });
        });
        return _types;
      },
      isNew: function() {
        if (!self.data || !self.data.name || self.data.name === "") {
          self.isNewField = true;
        } else {
          self.isNewField = false;
          var data = {};
          if (self.data) {
            data = self.data;
          }
          _.assign(self.d_data, data);
        }
        return self.isNewField;
      },
      findControlType: function(value) {
        return self.AllControlTypes.find(function(item) {
          return item.value.toLowerCase() === value.toLowerCase();
        });
      },
      addOption: function() {
        if (!self.addDisable) {
          self.d_data.selectionOptions.push({ key: "", value: "" });
        }
      },
      removeOption: function(event, index) {
        self.d_data.selectionOptions.splice(index, 1);
      },
      generateValidateModel: function() {
        var temp = { valid: true, msg: "" };
        self.firstTabValidate = {};
        self.firstTabValidate.selectionOptionsNull = temp;
        self.firstTabValidate.name = temp;
        if (self.d_data.selectionOptions) {
          self.firstTabValidate.selectionOptions = [];
          self.d_data.selectionOptions.forEach(function(item) {
            self.firstTabValidate.selectionOptions.push({
              key: temp,
              value: temp
            });
          });
        }
      },
      validate: function() {
        var firstTabHasError = false;

        if (self.isNewField) {
          self.firstTabValidate.name = Kooboo.validField(self.d_data.name, [
            { required: true, message: Kooboo.text.validation.required },
            {
              pattern: /^([A-Za-z][\w\-]*)*[A-Za-z0-9]$/,
              message: Kooboo.text.validation.contentTypeNameRegex
            },
            {
              validate: function(value) {
                var status = true;
                self.allItems.forEach(function(item, index) {
                  if (item.name === value && !(index === self.editingIndex)) {
                    status = false;
                  }
                });
                return status;
              },
              message: Kooboo.text.validation.taken
            }
          ]);
          if (!self.firstTabValidate.name.valid) {
            firstTabHasError = true;
          }
        }
        if (self.d_data.selectionOptions.length < 1) {
          if (
            ["selection", "checkbox", "radiobox", "fixedspec"].includes(
              self.d_data.controlType.toLowerCase()
            )
          ) {
            self.firstTabValidate.selectionOptionsNull = {
              valid: false,
              msg: Kooboo.text.validation.required
            };
            firstTabHasError = true;
          }
        } else {
          self.d_data.selectionOptions.forEach(function(item, index) {
            var rule = [
              { required: true, message: Kooboo.text.validation.required }
            ];

            var key = Kooboo.validField(item.key, rule);
            var value = Kooboo.validField(item.value, rule);
            if (key.valid === false || value.valid === false) {
              self.firstTabValidate.selectionOptions[index] = {
                key: key,
                value: value
              };
              firstTabHasError = true;
            }
          });
        }

        if (self.$refs.fieldValidation) {
          var threeTabValidation = self.$refs.fieldValidation.validate();
          if (threeTabValidation.hasError) {
            self.tabIndex = 2;
          }
        }
        if (firstTabHasError) {
          self.tabIndex = 0;
        }
        var els = document.getElementsByClassName("has-error");
        if (els.length > 0) {
          els[0].scrollIntoView();
        }
        self.showValidateError = true;
        this.$forceUpdate();
        return !(firstTabHasError || threeTabValidation.hasError);
      },
      onSave: function() {
        if (self.validate()) {
          var data = this.d_data;
          data.isSystemField =
            self.d_data.isSystemField || self.options.isSystemField;
          data.displayName =
            !this.d_data.displayName || this.d_data.displayName === ""
              ? this.d_data.name
              : this.d_data.displayName;
          data.editable =
            self.d_data.isSystemField || self.options.isSystemField
              ? this.d_data.editable
              : true;
          if (!self.multilinguable) {
            self.d_data.multilingual = false;
          }
          data.multipleLanguage = data.multilingual = self.d_data.multilingual;
          data.multipleValue = self.d_data.multipleValue;
          data.selectionOptions = JSON.stringify(this.d_data.selectionOptions);
          data.validations = JSON.stringify(this.d_data.validations);

          var temp = {
            isNewField: self.isNewField,
            data: data,
            editingIndex: self.editingIndex
          };
          self.$emit("on-save", temp);
          self.closeHandle();
        }
      },
      addDisableHandler: function() {
        self.addDisable = false;
        self.d_data.selectionOptions.forEach(function(i) {
          if (!i.key || !i.value || i.key === "" || i.value === "") {
            self.addDisable = true;
          }
        });
      },
      onCancel: function() {
        self.closeHandle();
      },
      changeModified: function() {
        if (this.isProductType) {
          this.changeModifiedField = true;
        }
      }
    },
    computed: {
      isProductType: function() {
        return this.options.modifiedField == "isSpecification";
      }
    }
  });
})();
