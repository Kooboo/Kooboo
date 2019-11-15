(function() {
  Vue.component("kb-control-datetime", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/controlType/DateTime.html"
    ),
    props: {
      field: Object
    },
    data: function() {
      return {
        formattedDate: ""
      };
    },
    inject: ["kbFormItem"],
    mounted: function() {
      var unwatch = this.$watch(
        function() {
          return this.kbFormItem.kbForm.model[this.kbFormItem.prop];
        },
        function(fieldValue) {
          if (fieldValue) {
            this.formattedDate = moment(fieldValue)
              .utc()
              .format("YYYY-MM-DD HH:mm");
            unwatch && unwatch();
          }
        },
        { immediate: true }
      );
    },
    watch: {
      formattedDate(val) {
        var d = new Date(val);
        this.kbFormItem.kbForm.model[this.kbFormItem.prop] = d.toISOString();
      }
    }
  });
})();
