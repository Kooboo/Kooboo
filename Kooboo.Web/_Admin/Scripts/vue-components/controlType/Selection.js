(function() {
  Vue.component("kb-control-selection", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/controlType/Selection.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
