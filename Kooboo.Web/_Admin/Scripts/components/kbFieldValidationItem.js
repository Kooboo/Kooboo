(function() {
  var self;
  Vue.component("kb-field-validation-item", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/kbFieldValidationItem.html"
    ),
    props: {
      item: { require: true },
      index: { require: true },
      showValidateError: { default: true },
      validateModel: {
        default: function() {
          self = this;
          var model = {};
          var keys = Object.keys(self.item);
          keys.forEach(function(key) {
            if (!(key === "type" || key === "msg" || key === "required"))
              model[key] = { valid: true, msg: "" };
          });
          return model;
        }
      }
    },
    watch: {
      item: {
        handler: function(value) {
          this.$emit("update:item", value);
        },
        deep: true
      }
    }
  });
})();
