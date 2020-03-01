(function () {
  var self;
  Kooboo.loadJS(["/_Admin/Scripts/components/kbFieldValidationItem.js"]);
  Vue.component("kb-field-validation", {
    template: Kooboo.getTemplate(
        "/_Admin/Scripts/components/kbFieldValidation.html"
    ),
    props: {
      data: {},
      showValidateError: { default: false }
    },
    data: function () {
      return {
        validations: [],
        hasError: false,
        options: [],
        allOptions: [],
        selectedValue: undefined,
        defaultoptions: {
          required: {
            name: Kooboo.text.validationRule.required,
            type: "required",
            default: { type: "required" }
          },
          stringLength: {
            name: Kooboo.text.validationRule.stringLength,
            type: "stringLength",
            default: { min: "", max: "", type: "stringLength" }
          },
          range: {
            name: Kooboo.text.validationRule.range,
            type: "range",
            default: { min: "", max: "", type: "range" }
          },
          regex: {
            name: Kooboo.text.validationRule.regex,
            type: "regex",
            default: { pattern: "", type: "regex" }
          },
          min: {
            name: Kooboo.text.validationRule.min,
            type: "min",
            default: { value: "", type: "min" }
          },
          max: {
            name: Kooboo.text.validationRule.max,
            type: "max",
            default: { value: "", type: "max" }
          },
          minLength: {
            name: Kooboo.text.validationRule.minLength,
            type: "minLength",
            default: { value: "", type: "minLength" }
          },
          maxLength: {
            name: Kooboo.text.validationRule.maxLength,
            type: "maxLength",
            default: { value: "", type: "maxLength" }
          },
          minChecked: {
            name: Kooboo.text.validationRule.minChecked,
            type: "minChecked",
            default: { value: "", type: "minChecked" }
          },
          maxChecked: {
            name: Kooboo.text.validationRule.maxChecked,
            type: "maxChecked",
            default: { value: "", type: "maxChecked" }
          }
        },
        validateModels: undefined
      };
    },
    created: function () {
      self = this;
      self.allOptions = self.getOptionsBytype(self.data.controlType);
      self.getOptions();
      if (this.data.validations) {
        this.validations = this.data.validations;
      }
      this.createValidateModels();
    },
    watch: {
      data: {
        handler: function (value, old) {
          this.d_data = value;
        },
        deep: true
      },
      "data.controlType": {
        handler: function (value, old) {
          self.allOptions = self.getOptionsBytype(self.data.controlType);
          self.getOptions();
          self.validations = [];
        },
        deep: true
      },
      validations: {
        handler: function (value) {
          self.getOptions();
          self.createValidateModels();
          self.data.validations = value;
        },
        deep: true
      }
    },
    methods: {
      createValidateModels: function () {
        self.validateModels = [];
        self.validations.forEach(function (item) {
          var model = {};
          var keys = Object.keys(item);
          keys.forEach(function (key) {
            if (!(key === "type" || key === "msg" || key === "required"))
              model[key] = { valid: true, msg: "" };
          });
          self.validateModels.push(model);
        });
      },
      getOptionsBytype: function (type) {
        var options = [];
        var a = self.defaultoptions;
        switch (type.toLowerCase()) {
          case "textbox":
          case "textarea":
            options = [
              a.required,
              a.stringLength,
              a.minLength,
              a.maxLength,
              a.regex
            ];
            break;
          case "richeditor":
          case "mediafile":
          case "datetime":
          case "file":
          case "radiobox":
            options = [a.required];
            break;
          case "checkbox":
            options = [a.required, a.minChecked, a.maxChecked];
            break;
          case "number":
            options = [a.required, a.range, a.regex, a.min, a.max];
            break;
        }
        return options;
      },
      onAdd: function () {
        this.validations.push(self.defaultoptions[self.selectedValue].default);
      },
      onRemove: function (index) {
        this.validations.splice(index, 1);
      },
      validate: function () {
        var hasError = false;
        self.validations.forEach(function (item, index) {
          var model = {};
          var keys = Object.keys(item);
          keys.forEach(function (key) {
            if (!(key === "type" || key === "msg" || key === "required"))
              model[key] = Kooboo.validField(item[key], [
                { required: true, message: Kooboo.text.validation.required }
              ]);
            if (
                model[key] &&
                model[key].hasOwnProperty("valid") &&
                !model[key].valid
            ) {
              hasError = true;
            }
          });
          self.validateModels[index] = model;
        });
        this.$forceUpdate();
        return { hasError: hasError, result: self.validateModels };
      },
      getOptions: function () {
        self.options = _.clone(self.allOptions);
        var count = 0;
        self.allOptions.forEach(function (item, index) {
          self.validations.forEach(function (rule) {
            if (item.type && rule.type && item.type === rule.type) {
              self.options.splice(index - count, 1);
              count++;
            }
          });
        });
        if (self.options.length > 0 && self.options[0].type) {
          self.selectedValue = self.options[0].type;
        }
      }
    }
  });
})();
