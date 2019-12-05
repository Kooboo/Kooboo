(function() {
  Vue.component("kb-control-number", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/Number.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
