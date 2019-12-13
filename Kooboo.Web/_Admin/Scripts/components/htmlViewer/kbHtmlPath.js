(function() {
  Vue.component("kb-html-path", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/htmlViewer/kbHtmlPath.html"
    ),
    data: function() {
      return {
        path: []
      };
    },
    props: ["elem", "rootElem"],
    methods: {
      changeElem: function(m) {
        Kooboo.EventBus.publish("kb/lighter/holder", m.elem);
        // self.$emit("change", m);
      },
      hoverElem: function(m) {
        Kooboo.EventBus.publish("kb/html/elem/hover", m.elem);
        // self.$emit("hover", m);
      }
    },
    watch: {
      elem: {
        handler: function() {
          var self = this;
          var _pathList = [];
          if (self.elem) {
            if (self.rootElem) {
              var parent = null;
              if (
                $.contains(self.rootElem, self.elem) ||
                $(self.rootElem).is($(self.elem))
              ) {
                parent = self.elem;
              } else {
                parent = self.rootElem;
              }
              while (
                $.contains(self.rootElem, parent) ||
                $(self.rootElem).is($(parent))
              ) {
                var displayText = parent.tagName.toLowerCase();

                if ($(parent).attr("id")) {
                  displayText += "#" + $(parent).attr("id");
                } else if ($(parent).attr("class")) {
                  if (
                    $(parent)
                      .attr("class")
                      .indexOf(" ") == -1
                  ) {
                    displayText += "." + $(parent).attr("class");
                  } else {
                    displayText +=
                      "." +
                      $(parent)
                        .attr("class")
                        .split(" ")
                        .join(".");
                  }
                }
                _pathList.push({
                  text: displayText,
                  elem: parent
                });
                parent = $(parent).parent()[0];
              }
            } else {
              var parent = self.elem;
              // TODO: to be reviewed
              while (
                window.viewEditor &&
                $.contains(window.viewEditor.position.elem, parent)
              ) {
                var displayText = parent.tagName.toLowerCase();
                if ($(parent).attr("id")) {
                  displayText += "#" + $(parent).attr("id");
                } else if ($(parent).attr("class")) {
                  displayText += "." + $(parent).attr("class");
                }
                _pathList.push({
                  text: displayText,
                  elem: parent
                });
                parent = $(parent).parent()[0];
              }
            }
          }
          self.path = _pathList.reverse();
        },
        immediate: true
      }
    }
  });
})();
