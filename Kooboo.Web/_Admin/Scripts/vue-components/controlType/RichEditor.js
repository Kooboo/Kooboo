(function() {
  Vue.component("kb-control-richeditor", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/controlType/RichEditor.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
