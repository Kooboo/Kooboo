(function() {
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
        addDisable: false,
        d_data: {
          name: "",
          displayName: "",
          controlType: "TextBox",
          isSummaryField: false,
          multipleLanguage: undefined,
          editable: true,
          order: 0,
          tooltip: "",
          validations: [],
          multilingual: undefined,
          modifiedFieldName: undefined,
          selectionOptions: []
        },
        formRules: {},
        firstTabValidate: {},
        controlTypesOptions: [],
        AllControlTypes: []
      };
    },
    created: function() {
      self = this;
      self.AllControlTypes = self.getControlTypes(self.options.controlTypes);
      if (!self.isNew()) {
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
        self.controlTypesOptions = self.getSameTypeControlType(
          self.findControlType(self.d_data.controlType)
        );
      } else {
        self.controlTypesOptions = self.AllControlTypes;
      }

      self.generateValidateModel();
    },
    watch: {
      d_data: {
        handler: function(val, old) {
          if (self.type !== val.controlType) {
            self.controlTypesOptions = self.getSameTypeControlType(
              self.findControlType(self.d_data.controlType)
            );

            self.type = val.controlType;
          }
          self.addDisableHandler();

          self.generateValidateModel();
        },
        deep: true
      }
    },
    methods: {
      getSameTypeControlType: function(item) {
        if(item.value ==='Selection' || item.value ==='Switch') {
          self.d_data.validations = [];
        } else {
          try {
            if (self.data.validations.length > 0) {
              self.d_data.validations = JSON.parse(self.data.validations);
            }
          } catch (e) {
            self.d_data.validations = [];
          }
        }

        if (item.dataType == "Array") {
          try {
            if (self.data.selectionOptions.length > 0) {
              self.d_data.selectionOptions = JSON.parse(
                  self.data.selectionOptions
              );
            }
          } catch (e) {
            self.d_data.selectionOptions = [];
          }
        }else {
          self.d_data.selectionOptions = [];
        }
        if (item.dataType !== "Undefined") {
          delete self.d_data.multipleValue;
        }

        var temp = [];

        self.AllControlTypes.forEach(function(i) {
          if (i.dataType && i.dataType === item.dataType) {
            temp.push(i);
          }
        });
        if (self.isNewField) {
          return self.AllControlTypes;
        }
        return temp;
      },
      getControlTypes: function(types) {
        var _types = [];
        var CONTROL_TYPES = Kooboo.controlTypes;
        types.forEach(function(t) {
          var _t = CONTROL_TYPES.find(function(c) {
            return c.value.toLowerCase() == t;
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
      getSelectionOptions: function() {
        var options = JSON.parse(self.d_data.selectionOptions);
        if (self.data && !self.options.isNewField && self.data.type) {
          var type = self.data.controlType.toLowerCase();
          if (
            ["selection", "checkbox", "radiobox", "fixedspec"].indexOf(type) >
            -1
          ) {
          }
          if (["boolean", "switch", "selection"].indexOf(type) > -1) {
          }
          if (["boolean", "switch", "selection"].indexOf(type) > -1) {
          }
        }
      },
      addOption: function() {
        if(!self.addDisable) {
          self.d_data.selectionOptions.push({ key: "", value: "" });
        }
      },
      removeOption: function(event, index) {
        self.d_data.selectionOptions.splice(index, 1);
      },
      generateValidateModel: function() {
        self.firstTabValidate = {};
        var temp = { valid: true, msg: "" };
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
        if(self.isNewField){
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


        self.d_data.selectionOptions.forEach(function(item, index) {
          var rule = [
            { required: true, message: Kooboo.text.validation.required }
          ];

          var key = Kooboo.validField(item.key, rule);
          var value = Kooboo.validField(item.value, rule);
          if (key.valid === false || value.valid === false)
          self.firstTabValidate.selectionOptions[index] = {
            key: key,
            value: value
          };
        });
        if(self.$refs.fieldValidation) {
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
          data.selectionOptions = JSON.stringify(this.d_data.selectionOptions);
          data.validations = JSON.stringify(this.d_data.validations);
          if (data.displayName === "") {
            data.displayName = data.name;
          }
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
          self.d_data.selectionOptions.forEach(function (i) {
            if(!i.key || !i.value || i.key === '' || i.value === '') {
              self.addDisable = true
            }
          });
      },
      onCancel: function() {
        self.closeHandle();
      }
    }
  });
})();

