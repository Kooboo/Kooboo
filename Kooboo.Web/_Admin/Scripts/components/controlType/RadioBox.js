(function() {
  Vue.component("kb-control-radiobox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/RadioBox.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
