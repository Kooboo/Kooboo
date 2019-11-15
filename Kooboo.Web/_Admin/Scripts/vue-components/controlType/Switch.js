(function() {
  Vue.component("kb-control-switch", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/controlType/Switch.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
