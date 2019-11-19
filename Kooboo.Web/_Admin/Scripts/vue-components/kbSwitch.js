(function() {
  Vue.component("kb-switch", {
    template: '<input type="checkbox" />',
    props: {
      value: Boolean,
      onText: {
        type: String,
        default: Kooboo.text.common.yes
      },
      offText: {
        type: String,
        default: Kooboo.text.common.no
      }
    },
    mounted: function() {
      var self = this;
      $(self.$el).bootstrapSwitch("state", self.value, self.value);
      $(self.$el).bootstrapSwitch("onText", self.onText);
      $(self.$el).bootstrapSwitch("offText", self.offText);
      $(self.$el).on("switchChange.bootstrapSwitch", function(event, state) {
        self.$emit("input", state);
      });
    },
    watch: {
      value: function(val) {
        $(this.$el).bootstrapSwitch("state", val, val);
      }
    }
  });
})();
