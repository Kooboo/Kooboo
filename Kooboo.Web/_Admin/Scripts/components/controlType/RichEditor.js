(function() {
  Vue.component("kb-control-richeditor", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/RichEditor.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
