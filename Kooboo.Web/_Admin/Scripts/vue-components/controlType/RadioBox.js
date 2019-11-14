(function() {
  Vue.component("kb-control-radiobox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/controlType/RadioBox.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();
