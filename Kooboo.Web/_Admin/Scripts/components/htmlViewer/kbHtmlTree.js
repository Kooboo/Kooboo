(function() {

    var SELF_CLOSE_TAGS = [
        "area", "base", "br", "col",
        "command", "embed", "hr", "img",
        "input", "keygen", "link", "meta",
        "param", "source", "track", "wbr"
    ];
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/htmlViewer/kbHtmlTree.html");

    ko.components.register("kb-html-tree", {
        viewModel: function(params) {

            /*
             * IN:
             * parmas: {
             *     elem: ELEMENT,
             *     depth: (number)
             * }
             * 
             * OUT:
             * TREE_MOUSE_CLICK: {
             *      event: "kb/html/elem/change",
             *      value: ELEMENT
             * },
             * PATH_MOUSE_HOVER: {
             *      event: "kb/html/elem/hover",
             *      value: ELEMENT
             * }
             * 
             */

            var self = this;

            this.rootElem = ko.observable(params && params.rootElem);

            // Display-DOM-tree array
            this.tree = ko.observableArray();

            if (params && params.elem) {

                if (typeof params.elem == "object") {
                    if (self.rootElem()) {
                        if ($.contains(self.rootElem(), params.elem) ||
                            $(self.rootElem()).is($(params.elem))) {
                            self.tree([new elemModel(params.elem, params.depth)]);
                        } else {
                            self.tree([new elemModel(self.rootElem(), params.depth)]);
                        }
                    } else {
                        self.tree([new elemModel(params.elem, params.depth)]);
                    }
                } else {
                    self.tree(params.elem());
                }
            }
            this.maxDepth = ko.observable(5);

            // Modified the display-DOM-tree depth
            this.depth = ko.observable(params && params.depth);

            this.changeElem = function(m) {
                Kooboo.EventBus.publish("kb/lighter/holder", m.elem());
            }

            this.hoverElem = function(m) {
                m.isHovered(true);
                Kooboo.EventBus.publish("kb/html/elem/hover", m.elem());
            }

            this.unhoverElem = function(m) {
                m.isHovered(false);
            }

        },
        template: template
    });

    var elemModel = function(elem, depth) {

        var self = this;

        this.elem = ko.observable(elem);

        this.depth = ko.observable(depth);

        this.name = ko.observable(elem.tagName.toLowerCase());

        this.isSelfClosed = ko.observable(SELF_CLOSE_TAGS.indexOf(self.name()) > -1);

        this.attr = ko.observable();

        this.hasAttr = ko.observable(true);

        if ($(elem).attr("k-placeholder")) {
            self.attr({ key: "placeholder", value: $(elem).attr("k-placeholder") });
        } else if ($(elem).attr("id")) {
            self.attr({ key: "id", value: $(elem).attr("id") });
        } else if ($(elem).attr("class")) {
            self.attr({ key: "class", value: $(elem).attr("class") });
        } else {
            self.hasAttr(false);
        };

        this.attrText = ko.pureComputed(function() {
            return '<span class="attr-name">' + self.attr().key + '</span>="<span class="attr-value">' + self.attr().value + '</span>"';
        })

        this.hasChild = ko.observable($(elem).children().length > 0);

        if (elem.hasAttribute("k-placeholder") && elem.hasAttribute("k-omit")) {
            this.hasChild(false);
        }

        this.getChildren = function(elem) {
            var _children = [];
            $(elem).children().each(function(idx, el) {
                if (["meta", "style", "script"].indexOf(el.tagName.toLowerCase()) == -1) {
                    _children.push(new elemModel(el, self.depth() + 1));
                }
            })
            return _children;
        }

        this.children = ko.observableArray(self.getChildren(elem));

        this.isHovered = ko.observable(false);
    }
})();