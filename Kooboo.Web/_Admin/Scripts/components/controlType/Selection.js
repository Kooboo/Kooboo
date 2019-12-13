(function() {
  Vue.component("kb-control-selection", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/Selection.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"],
    mounted: function() {
      // select the first option by default
      var unwatch = this.$watch(
        function() {
          return this.kbFormItem.kbForm.model[this.kbFormItem.prop];
        },
        function(val) {
          if (val !== null && val != "") {
            unwatch && unwatch();
          } else if (this.field.options[0]) {
            this.kbFormItem.kbForm.model[
              this.kbFormItem.prop
            ] = this.field.options[0].value;
            unwatch && unwatch();
          }
        },
        {
          immediate: true
        }
      );
      this.kbFormItem.kbForm.model[this.kbFormItem.prop] && unwatch();
    }
  });
})();
