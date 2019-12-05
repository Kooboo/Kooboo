(function() {
  Kooboo.loadJS([
    "/_Admin/Scripts/components/htmlViewer/kbHtmlTree.js",
    "/_Admin/Scripts/components/htmlViewer/kbHtmlPath.js"
  ]);
  Kooboo.loadCSS([
    "/_Admin/Scripts/components/htmlViewer/kbHtmlTree.css",
    "/_Admin/Scripts/components/htmlViewer/kbHtmlPath.css"
  ]);
  Vue.component("kb-html-viewer", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/kbHtmlViewer.html"
    ),
    data: function() {
      return {
        elem: undefined,
        rootElem: undefined
      };
    },
    mounted: function() {
      var self = this;
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
