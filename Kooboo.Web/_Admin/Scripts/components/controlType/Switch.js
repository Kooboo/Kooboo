(function() {
  Vue.component("kb-control-switch", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/Switch.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
