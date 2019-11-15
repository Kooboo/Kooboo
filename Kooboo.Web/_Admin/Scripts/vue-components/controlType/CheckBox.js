(function() {
  Vue.component("kb-control-checkbox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/controlType/CheckBox.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
