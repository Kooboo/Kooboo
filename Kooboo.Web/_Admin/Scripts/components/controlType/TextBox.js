(function() {
  Vue.component("kb-control-textbox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/TextBox.html"
    ),
    props: {
      field: Object
    },
    inject:["kbFormItem"]
  });
})();
