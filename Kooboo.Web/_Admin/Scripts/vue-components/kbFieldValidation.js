(function() {
  var self;
  Kooboo.loadJS(["/_Admin/Scripts/vue-components/kbFieldValidationItem.js"]);
  Vue.component("kb-field-validation", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/kbFieldValidation.html"
    ),
    props: {
      data: {}
    },
    data: function() {
      return {
        showValidate: false,
        validations: this.data.validations
          ? JSON.parse(this.data.validations)
          : [],
        hasError: false,
        options: [],
        selectedValue: undefined,
        allOptions: {
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
            default: { value: "", type: "maxLength" }
          },
          maxChecked: {
            name: Kooboo.text.validationRule.maxChecked,
            type: '"maxChecked"',
            default: { value: "", type: "maxLength" }
          }
        },
        validateModels: undefined
      };
    },
    created: function() {
      self = this;
      self.options = self.getOptionsBytype(self.data.controlType);
      self.shiftExistRule();

      this.createValidateModels();
    },
    watch: {
      data: function(value) {
        this.d_data = value;
      },
      validations: function(value) {
        this.createValidateModels();
      }
    },
    methods: {
      createValidateModels: function() {
        self.validateModels = [];
        self.validations.forEach(function(item) {
          var model = {};
          var keys = Object.keys(item);
          keys.forEach(function(key) {
            if (!(key === "type" || key === "msg" || key === "required"))
              model[key] = { valid: true, msg: "" };
          });
          self.validateModels.push(model);
        });
      },
      getOptionsBytype: function(type) {
        var options = [];
        var a = self.allOptions;
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
      onAdd: function() {
        this.validations.push(self.allOptions[self.selectedValue].default);
      },
      itemChange: function(e, item, index) {},
      shiftExistRule: function() {
        for (var i = 0; i < self.options.length; i++) {
          var hasSame = false;
          self.validations.forEach(function(rule) {
            if (
              self.options[i].type &&
              rule.type &&
              self.options[i].type === rule.type
            ) {
              self.options.splice(i, 1);
              hasSame = true;
            }
          });
          if (hasSame) i--;
        }
        if (self.options.length > 0 && self.options[0].type) {
          self.selectedValue = self.options[0].type;
        }
      }
    }
  });
})();

