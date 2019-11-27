(function() {
  var self;
  Vue.component("kb-html-viewer", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/kbHtmlViewer.html"
    ),
    data: function() {
      self = this;
      return {
        elem: Object,
        rootElem: Object
      };
    },
    mounted: function() {
      Kooboo.EventBus.subscribe("kb/html/previewer/select", function(elem) {
        self.elem = elem;
      });
      Kooboo.EventBus.subscribe("kb/html/previewer/hover", function(elem) {
        Kooboo.EventBus.publish("kb/html/tree/elem/hover", elem);
      });
      Kooboo.EventBus.subscribe("kb/html/previewer/rootElem", function(elem) {
        self.rootElem = elem;
      });
    }
  });
})();
