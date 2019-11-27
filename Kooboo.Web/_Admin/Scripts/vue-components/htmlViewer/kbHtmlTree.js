(function() {
  var SELF_CLOSE_TAGS = [
    "area",
    "base",
    "br",
    "col",
    "command",
    "embed",
    "hr",
    "img",
    "input",
    "keygen",
    "link",
    "meta",
    "param",
    "source",
    "track",
    "wbr"
  ];
  var self;
  Vue.component("kb-html-tree", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/htmlViewer/kbHtmlTree.html"
    ),
    props: {
      elem: [Object, Array],
      rootElem: [Object, Array],
      depth: Number
    },
    data: function() {
      self = this;
      return {
        tree: [],
        maxDepth: 5
      };
    },
    mounted: function() {
      if (self.elem) {
        if (typeof self.elem == "object") {
          if (self.rootElem) {
            if (
              $.contains(self.rootElem, self.elem) ||
              $(self.rootElem).is($(self.elem))
            ) {
              self.tree = [new elemModel(self.elem, self.depth)];
            } else {
              self.tree = [new elemModel(self.rootElem, self.depth)];
            }
          } else {
            self.tree = [new elemModel(self.elem, self.depth)];
          }
        } else {
          self.tree = self.elem;
        }
      }
    },
    methods: {
      changeElem: function(m) {
        Kooboo.EventBus.publish("kb/lighter/holder", m.elem);
        self.$emit("change", m.elem);
      },
      hoverElem: function(m) {
        m.isHovered = true;
        Kooboo.EventBus.publish("kb/html/elem/hover", m.elem);
        self.$emit("hover", m.elem);
      },
      unhoverElem: function(m) {
        m.isHovered = false;
      }
    }
  });

  function elemModel(elem, depth) {
    this.elem = elem;
    this.depth = depth;
    this.name = elem.tagName.toLowerCase();
    this.isSelfClosed = SELF_CLOSE_TAGS.indexOf(self.name) > -1;
    this.attr = "";
    this.hasAttr = true;

    if ($(elem).attr("k-placeholder")) {
      this.attr = { key: "placeholder", value: $(elem).attr("k-placeholder") };
    } else if ($(elem).attr("id")) {
      this.attr = { key: "id", value: $(elem).attr("id") };
    } else if ($(elem).attr("class")) {
      this.attr = { key: "class", value: $(elem).attr("class") };
    } else {
      this.hasAttr = false;
    }
    this.attrText = "";
    if (this.hasAttr) {
      '<span class="attr-name">' +
        self.attr.key +
        '</span>="<span class="attr-value">' +
        self.attr.value +
        '</span>"';
    }
    this.hasChild = $(elem).children().length > 0;
    if (elem.hasAttribute("k-placeholder") && elem.hasAttribute("k-omit")) {
      this.hasChild = false;
    }
    this.children = [];
    $(elem)
      .children()
      .each(function(idx, el) {
        if (
          ["meta", "style", "script"].indexOf(el.tagName.toLowerCase()) == -1
        ) {
          this.children.push(new elemModel(el, self.depth + 1));
        }
      });

    this.isHovered = false;
  }
})();
