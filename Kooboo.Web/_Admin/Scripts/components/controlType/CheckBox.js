(function() {
  Vue.component("kb-control-checkbox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/CheckBox.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
