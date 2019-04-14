(function() {
  Kooboo.vue.component.kbBreadcrumb = Vue.component("kb-breadcrumb", {
    props: {
      items: Array
    },
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue/components/kbBreadcrumb/index.html"
    )
  });
})();
