(function() {
  Kooboo.loadJS(["/_Admin/Scripts/vue/components/kbTab/panel/index.js"]);

  Kooboo.vue.component.kbTab = Vue.component("kb-tab", {
    props: {
      tabs: Array
    },
    data: function() {
      return {
        curTab: ""
      };
    },
    methods: {
      onClick: function(value) {
        this.curTab = value;
      }
    },
    watch: {
      curTab: function(cur, orig) {
        this.$emit("change", cur, orig);
      }
    },
    mounted: function() {
      this.curTab = this.tabs && this.tabs.length && this.tabs[0].value;
    },
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue/components/kbTab/index.html"
    )
  });
})();
