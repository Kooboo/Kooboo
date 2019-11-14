(function() {
  Vue.component("kb-control-textbox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/controlType/TextBox.html"
    ),
    props: {
      field: Object
    },
    inject:["kbFormItem"]
  });
})();
