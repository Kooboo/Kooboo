(function() {
  Vue.component("kb-control-textarea", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/controlType/TextArea.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
