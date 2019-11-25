(function() {
  Vue.component("form-validator", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/formEditor/formValidator.html"
    ),
    props: {
      validations: Array,
      fieldIndex: Number
    },
    data: function() {
      return {
        rules: {}
      };
    },
    created: function() {
      var validations = {};
      [
        "message",
        "min",
        "max",
        "minChecked",
        "maxChecked",
        "minLength",
        "maxLength",
        "regex"
      ].forEach(type => {
        validations[type] = [
          {
            required: true,
            message: Kooboo.text.validation.required
          }
        ];
      });
      this.rules = {
        "validations[]": validations
      };
    },
    computed: {
      model: function() {
        return {
          validations: this.validations
        };
      }
    },
    methods: {
      remove: function(index) {
        this.$emit("remove", index);
      },
      validate: function() {
        return this.$refs.validationForm.validate();
      }
    }
  });
})();
