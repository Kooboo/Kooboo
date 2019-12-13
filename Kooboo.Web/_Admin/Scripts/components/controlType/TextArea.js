(function() {
  Vue.component("kb-control-textarea", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/TextArea.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
