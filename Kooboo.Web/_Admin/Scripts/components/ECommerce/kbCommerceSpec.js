(function() {
  Vue.component("kb-commerce-spec", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/ECommerce/kbCommerceSpec.html"
    ),
    props: {
      fields: Array
    },
    data: function() {
      var self = this;
      return {
        dynamicSpecs: [],
        validateField: {},
        rules: {
          newValue: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              validate: function(val) {
                return self.validateField.options.indexOf(val) == -1;
              },
              message: Kooboo.text.validation.taken
            }
          ]
        }
      };
    },
    methods: {
      dynamicFieldsChange: function() {
        this.$emit("change", this.dynamicSpecs);
      },
      addOption: function(field) {
        field.showNewOptionForm = true;
      },
      saveOption: function(field, index) {
        this.validateField = field;
        if (this.$refs.form[index].validate()) {
          field.options.push(field.newValue);
          this.resetForm(field, index);
          this.dynamicFieldsChange();
        }
      },
      resetForm: function(field, index) {
        field.newValue = "";
        field.showNewOptionForm = false;
        this.$refs.form[index].clearValid();
      },
      removeOption: function(field, index) {
        field.options.splice(index, 1);
        this.dynamicFieldsChange();
      }
    },
    watch: {
      fields: function(fields) {
        if (fields.length) {
          this.dynamicSpecs = fields.map(function(data) {
            if (typeof data.selectionOptions == "string") {
              data.selectionOptions = JSON.parse(data.selectionOptions);
            }
            return {
              name: data.name,
              showNewOptionForm: false,
              options: data.selectionOptions || [],
              newValue: ""
            };
          });
          this.$emit("change", this.dynamicSpecs);
        }
      }
    }
  });
})();
