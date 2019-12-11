(function() {
  Vue.component("kb-control-checkbox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/CheckBox.html"
    ),
    props: {
      field: Object
    },
    methods: {
      check: function(value) {
        var list = this.kbFormItem.kbForm.model[this.kbFormItem.prop];
        if (!list) list = [];
        if (typeof list == "string") list = JSON.parse(list);
        if (list.indexOf(value) > -1) {
          list = list.filter(function(f) {
            return f != value;
          });
        } else {
          list.push(value);
        }
        this.kbFormItem.kbForm.model[this.kbFormItem.prop] = list;
      }
    },
    inject: ["kbFormItem"]
  });
})();
