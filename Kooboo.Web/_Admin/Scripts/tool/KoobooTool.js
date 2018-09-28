(function() {
    var KoobooToolTemplateManager = Kooboo.ToolTemplateManager;
    var KoobooTool = {
        text: "",
        baseClass: "",
        owner: null,
        templateString: "",
        inherit: function(Child, Parent) {
            var newPrototype = $.extend({}, Parent.prototype, Child.prototype);
            Child.prototype = newPrototype;
            Child.constructor = Child;
        }
    };
    KoobooTool.prototype = {
        domNode: null,
        appendTo: function(el) {
            el.appendChild(this.domNode);
        },
        insertBefore: function(el) {
            if (el && el.parentNode)
                el.parentNode.insertBefore(this.domNode, el);
        },
        render: function() {
            var koobooToolTemplateManager = new KoobooToolTemplateManager({ "templateName": this.templateName })
            tmpl = koobooToolTemplateManager.renderHtmlWithData(this);
            this.domNode = $.parseHTML(tmpl)[0];
        },
        show: function() {
            $(this.domNode).css("display", "block");
        },
        hide: function() {
            $(this.domNode).css("display", "none");
        },
        //some element height may be zero,so get children element height;
        getRightPos: function(el) {
            var clientRect = el.getBoundingClientRect();

            //reset clientRect.height failed(for example clientRect.height=20)
            var pos = {
                top: clientRect.top,
                left: clientRect.left,
                height: clientRect.height,
                width: clientRect.width
            };
            pos.height = this.doGetRightHeight(el, pos.height);
            return pos;
        },
        doGetRightHeight: function(el, height) {
            if (height == 0) {
                var childrens = $(el).children();
                if (childrens && childrens.length > 0) {
                    var children = childrens[0];

                    var childrenPos = children.getBoundingClientRect();

                    height = childrenPos.height;
                    if (height == 0)
                        height = this.doGetRightHeight(children, height);
                }
            }
            return height;
        }
    };

    /* IN: 
     * params:{
     *      config: {},
     * }
     * example: var lighter= new Widget.Lighter({borderColor: '#51a8ff',zIndex: 6666});
     * lighter.appendTo(container);
     */
    KoobooTool.Lighter = function(config) {
        var self = this;
        var defaults = {
            domNode: null,
            //baseClass: "kb-lighter",
            templateName: "Lighter.html",
            context: null,
            leftRef: null,
            rightRef: null,
            topRef: null,
            bottomRef: null,
            zIndex: 9999,
            borderStyle: "solid",
            borderColor: "rgba(255,0,0,1)",
            borderWidth: 1,
            isMask: false,
            init: function() {
                self.render();
                self.setRef();
            },
            setRef: function() {
                var self = this;
                self.leftRef = $(self.domNode).find(".lighterLeftRef");
                self.rightRef = $(self.domNode).find(".lighterRightRef");
                self.topRef = $(self.domNode).find(".lighterTopRef");
                self.bottomRef = $(self.domNode).find(".lighterBottomRef");
            },
            resetDomNode: function(domNode) {
                self.domNode = domNode;
                self.setRef();
            },
            mask: function(context, offset) {
                self.context = context;
                offset = offset || { top: 0, left: 0 };
                self.domNode.style.display = "none";

                $(self.domNode).data("node", context.el);

                //var pos = self.context.el.getBoundingClientRect();

                var pos = self.getRightPos(self.context.el);

                self._setPosition(pos, offset);

                self.domNode.style.display = 'block';
                self.isMask = true;
                return self;
            },
            unmask: function() {
                self.context = null;
                $(self.domNode).css("display", "none");
                self.isMask = false;
            },
            _setPosition: function(pos, offset) {
                var height = pos.height;
                ///fix overflow bug
                $(self.domNode).css({
                    "height": height ? height : 1,
                    "position": "absolute",
                    "top": pos.top + offset.top,
                    "left": pos.left + offset.left,
                    "overflow": "hidden",
                    "pointer-events": "none"
                });

                $(self.topRef).css({
                    "width": pos.width - 1,
                    "left": 1,
                    "top": 0,
                    "pointer-events": "none"
                });
                $(self.rightRef).css({
                    "width": pos.width,
                    "height": height ? height : 1,
                    "right": 0,
                    "top": 0,
                    "pointer-events": "none"
                });
                $(self.bottomRef).css({
                    "width": pos.width - 1,
                    "left": 1,
                    "top": -2,
                    "pointer-events": "none"
                });
                $(self.leftRef).css({
                    "height": height ? height : 1,
                    "left": 0,
                    "top": -(height + 2),
                    "pointer-events": "none"
                });
            },
            destroy: function() {
                //delete self.context;
                delete this.context;
                delete this.topRef;
                delete this.rightRef;
                delete this.bottomRef;
                delete this.leftRef;
                $(this.domNode).remove();
            }
        }
        $.extend(self, defaults, config);
        self.init();
        return self;
    };
    KoobooTool.inherit(KoobooTool.Lighter, KoobooTool);

    /* IN: 
     * params:{
     *      config: {},
     * }
     * example:var shadow= new Widget.Shadow({
                    zIndex: 9999
                });
     * shadow.appendTo(container);
     */
    KoobooTool.Shadow = function(config) {
        var self = this;
        var defaults = {
            rect: null,
            templateName: "Shadow.html",
            leftRef: null,
            rightRef: null,
            topRef: null,
            bottomRef: null,
            cursor: "not-allowed",
            zIndex: 9999,
            bgColor: "rgba(0,0,0,0.2)",
            init: function() {
                self.render();
                self.setRef();
            },
            setRef: function() {
                var self = this;
                self.leftRef = $(self.domNode).find(".lighterLeftRef");
                self.rightRef = $(self.domNode).find(".lighterRightRef");
                self.topRef = $(self.domNode).find(".lighterTopRef");
                self.bottomRef = $(self.domNode).find(".lighterBottomRef");
            },
            resetDomNode: function(domNode) {
                self.domNode = domNode;
                self.setRef();
            },
            mask: function(context, container, offset) {
                self.context = context;
                var rect = context.el.getBoundingClientRect(),
                    maxWidth, maxHeight;

                offset = offset || { top: 0, left: 0 };
                if (document == context.el.ownerDocument) {
                    maxWidth = container ? container.scrollWidth : document.documentElement.clientWidth;
                } else {
                    maxWidth = context.el.ownerDocument.documentElement.clientWidth;
                }

                maxHeight = container ? container.scrollHeight : document.documentElement.clientHeight;

                $(self.domNode).css("display", "none");

                self._setPosition(rect, maxWidth, maxHeight, offset);

                $(self.domNode).css("display", "block");
            },
            _setPosition: function(rect, maxWidth, maxHeight, offset) {
                $(self.leftRef).css({
                    left: offset.left + "px",
                    top: offset.top + "px",
                    width: rect.left + "px",
                    height: (rect.top + rect.height) + "px"
                });
                $(self.rightRef).css({
                    left: (rect.width + rect.left + offset.left) + "px",
                    top: rect.top + offset.top + "px",
                    width: (maxWidth - rect.left - rect.width) + "px",
                    height: (maxHeight - rect.top) + "px"
                });
                $(self.topRef).css({
                    left: (rect.left + offset.left) + "px",
                    top: offset.top + "px",
                    width: (maxWidth - rect.left) + "px",
                    height: Math.min(rect.top, maxHeight) + "px"
                });
                $(self.bottomRef).css({
                    left: offset.left + "px",
                    top: (rect.top + rect.height + offset.top) + "px",
                    width: (rect.left + rect.width) + "px",
                    height: (maxHeight - rect.top - rect.height) + "px"
                });
            },
            unmask: function() {
                self.context = null;
                $(self.domNode).css("display", "none");
            },
            destroy: function() {
                //delete self.context;
                delete this.context;
                delete this.topRef;
                delete this.rightRef;
                delete this.bottomRef;
                delete this.leftRef;
                $(this.domNode).remove();
            }
        };
        $.extend(self, defaults, config);
        self.init();
        return self;
    }
    KoobooTool.inherit(KoobooTool.Shadow, KoobooTool);

    /* IN: 
     * params:{
     *      config: {},
     * }
     * example:var label = new Widget.Label({
                    text: "test"
                });
     * label.appendTo(container);
     */
    KoobooTool.Label = function(config) {
        var self = this;
        var defaults = {
            templateName: "Label.html",
            el: null,
            text: null,
            init: function() {
                self.render();
            },
            mask: function(context) {
                this.el = context.el;

                var $domNode = $(this.domNode),
                    width = $domNode.outerWidth(),
                    rect = this.el.getBoundingClientRect();

                $domNode.css({
                    top: rect.top + 1,
                    left: (rect.right - width - 1 > 8) ? rect.right - width - 1 : 0,
                    position: "absolute"
                });
            },
            setText: function(text) {
                this.text = text;
                $(this.domNode).text(text).css("position", "fixed");
                this.el && this.mask({ el: this.el });
            },
            destroy: function() {
                delete this.el;
                delete this.text;
                $(this.domNode).remove();
            }
        };
        $.extend(self, defaults, config);
        self.init();
        return self;
    }
    KoobooTool.inherit(KoobooTool.Label, KoobooTool);

    /* IN: 
     * params:{
     *      config: {},
     * }
     * example: var masker= new Widget.Masker({zIndex: 6666});
     * masker.appendTo(container);
     */
    KoobooTool.Masker = function(config) {
        var self = this;
        var defaults = {
            templateName: "Masker.html",
            el: null,
            zIndex: 9999,
            backgroundColor: "rgba(0,0,0,.2)",
            isMask: false,
            init: function() {
                self.render();
            },
            mask: function(context) {
                this.el = context.el;

                var offset = this.el.getBoundingClientRect();

                $(this.domNode).css({
                    left: offset.left,
                    top: offset.top,
                    width: offset.width,
                    height: offset.height
                }).show();

                this.isMask = true;
                return this;
            },
            unmask: function() {
                $(this.domNode).hide();
                this.isMask = false;
                return this;
            },
            destroy: function() {
                delete this.el;
                $(this.domNode).remove();
                delete this;
            }
        };
        $.extend(self, defaults, config);
        self.init();
        return self;
    }
    KoobooTool.inherit(KoobooTool.Masker, KoobooTool);

    Kooboo.Tool = KoobooTool;
})();