(function() {
  Vue.component("kb-control-textbox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/controlType/TextBox.html"
    ),
    inject:["kbFormItem"],
    props: {
      field: Object
    }
  });
})();
