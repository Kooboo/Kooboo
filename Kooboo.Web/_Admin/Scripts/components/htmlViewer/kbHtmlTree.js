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
  Vue.component("kb-html-tree", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/htmlViewer/kbHtmlTree.html"
    ),
    props: {
      elem: undefined,
      rootElem: undefined,
      depth: Number
    },
    data: function() {
      return {
        maxDepth: 5,
        tree: []
      };
    },
    watch: {
      elem: {
        handler: function() {
          var self = this;
          var tree = [];
          if (self.elem) {
            if (Array.isArray(self.elem)) {
              tree = self.elem;
            } else {
              if (self.rootElem) {
                if (
                  $.contains(self.rootElem, self.elem) ||
                  $(self.rootElem).is($(self.elem))
                ) {
                  tree = [new elemModel(self.elem, self.depth)];
                } else {
                  tree = [new elemModel(self.rootElem, self.depth)];
                }
              } else {
                tree = [new elemModel(self.elem, self.depth)];
              }
            }
          }
          self.tree = tree;
        },
        immediate: true
      }
    },
    methods: {
      changeElem: function(m) {
        Kooboo.EventBus.publish("kb/lighter/holder", m.elem);
        // self.$emit("change", m.elem);
      },
      hoverElem: function(m) {
        m.isHovered = true;
        Kooboo.EventBus.publish("kb/html/elem/hover", m.elem);
        // self.$emit("hover", m.elem);
      },
      unhoverElem: function(m) {
        m.isHovered = false;
      }
    }
  });

  function elemModel(elem, depth) {
    var _this = this;
    _this.elem = elem;
    _this.depth = depth;
    _this.name = elem.tagName.toLowerCase();
    _this.isSelfClosed = SELF_CLOSE_TAGS.indexOf(_this.name) > -1;
    _this.attr = "";
    _this.hasAttr = true;

    if ($(elem).attr("k-placeholder")) {
      _this.attr = { key: "placeholder", value: $(elem).attr("k-placeholder") };
    } else if ($(elem).attr("id")) {
      _this.attr = { key: "id", value: $(elem).attr("id") };
    } else if ($(elem).attr("class")) {
      _this.attr = { key: "class", value: $(elem).attr("class") };
    } else {
      _this.hasAttr = false;
    }
    _this.attrText = "";
    if (_this.hasAttr) {
      _this.attrText =
        '<span class="attr-name">' +
        _this.attr.key +
        '</span>="<span class="attr-value">' +
        _this.attr.value +
        '</span>"';
    }
    _this.hasChild = $(elem).children().length > 0;
    if (elem.hasAttribute("k-placeholder") && elem.hasAttribute("k-omit")) {
      _this.hasChild = false;
    }
    _this.children = [];
    $(elem)
      .children()
      .each(function(idx, el) {
        if (
          ["meta", "style", "script"].indexOf(el.tagName.toLowerCase()) == -1
        ) {
          _this.children.push(new elemModel(el, _this.depth + 1));
        }
      });

    _this.isHovered = false;
  }
})();
