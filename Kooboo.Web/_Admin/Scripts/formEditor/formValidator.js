(function() {
  Vue.component("form-validator", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/formEditor/formValidator.html"
    ),
    props: {
      validations: Array,
      fieldIndex: Number,
      errorVisible: Boolean
    },
    data: function() {
      return {
        rules: {},
        types: [
          "message",
          "min",
          "max",
          "minChecked",
          "maxChecked",
          "minLength",
          "maxLength",
          "regex"
        ]
      };
    },
    created: function() {
      var validations = {};
      this.types.forEach(type => {
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
        if (!this.validations.length) {
          return true;
        }
        return this.$refs.validationForm.validate();
      }
    }
  });
})();
