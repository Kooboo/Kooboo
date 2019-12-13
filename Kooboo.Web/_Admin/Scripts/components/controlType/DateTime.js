(function() {
  Vue.component("kb-control-datetime", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/DateTime.html"
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
      var fieldValue = this.kbFormItem.kbForm.model[this.kbFormItem.prop];
      if (fieldValue) {
        this.formattedDate = moment(fieldValue).format("YYYY-MM-DD HH:mm");
      }
    },
    watch: {
      formattedDate(val) {
        var d = new Date(moment(val));
        this.kbFormItem.kbForm.model[this.kbFormItem.prop] = d.toISOString();
      }
    }
  });
})();
