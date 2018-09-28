(function() {

    var colorProperty = [
            'color',
            'background-color',
            'border',
            'border-top',
            'border-right',
            'border-bottom',
            'border-left',
            'border-color',
            'border-top-color',
            'border-right-color',
            'border-bottom-color',
            'border-left-color',
            'box-shadow',
            'text-shadow',
            "outline",
            "outline-color"
        ],
        imageProperty = [
            "background-image"
        ],
        colorAndImageProperty = [
            "background"
        ]
    var template = Kooboo.getTemplate("/_Admin/Scripts/styleEditor/components/kb-style-list.html");
    ko.components.register("kb-style-list", {
        viewModel: function(params) {
            /*
             * parmas: {
             *  rule: array,
             *  showRelationBtn: boolean (default: false),
             *  getRules: function (send current list)
             * }
             * 
             */

            var self = this;

            var _styleRules = [];
            _.forEach(params.rules, function(rule) {
                _styleRules.push(new StyleRuleModel(rule, self));
            });
            this.styleRules = ko.observableArray(_styleRules);
            this.styleRules.subscribe(function() {
                self.saveTrigger(new Date());
            })

            this.showRelationBtn = ko.observable(params.showRelationBtn || false);

            this.addDeclaration = function(rule) {
                rule.declarations.push(new decModel({
                    name: "",
                    value: "",
                    important: false
                }, rule));
            }

            // Posible property name
            this._propertyName = ko.observableArray(["accelerator", "azimuth", "background",
                "background-attachment", "background-color", "background-image", "background-position",
                "background-position-x", "background-position-y", "background-repeat", "behavior",
                "border", "border-bottom", "border-bottom-color", "border-bottom-style", "border-bottom-width",
                "border-collapse", "border-color", "border-left", "border-left-color", "border-left-style",
                "border-left-width", "border-right", "border-right-color", "border-right-style",
                "border-right-width", "border-spacing", "border-style", "border-top", "border-top-color",
                "border-top-style", "border-top-width", "border-width", "bottom", "caption-side", "clear",
                "clip", "color", "content", "counter-increment", "counter-reset", "cue", "cue-after",
                "cue-before", "cursor", "direction", "display", "elevation", "empty-cells", "filter",
                "float", "font", "font-family", "font-size", "font-size-adjust", "font-stretch", "font-style",
                "font-variant", "font-weight", "height", "ime-mode", "include-source", "layer-background-color",
                "layer-background-image", "layout-flow", "layout-grid", "layout-grid-char",
                "layout-grid-char-spacing", "layout-grid-line", "layout-grid-mode", "layout-grid-type", "left",
                "letter-spacing", "line-break", "line-height", "list-style", "list-style-image", "list-style-position",
                "list-style-type", "margin", "margin-bottom", "margin-left", "margin-right", "margin-top",
                "marker-offset", "marks", "max-height", "max-width", "min-height", "min-width", "-moz-binding",
                "-moz-border-radius", "-moz-border-radius-topleft", "-moz-border-radius-topright",
                "-moz-border-radius-bottomright", "-moz-border-radius-bottomleft", "-moz-border-top-colors",
                "-moz-border-right-colors", "-moz-border-bottom-colors", "-moz-border-left-colors", "-moz-opacity",
                "-moz-outline", "-moz-outline-color", "-moz-outline-style", "-moz-outline-width", "-moz-user-focus",
                "-moz-user-input", "-moz-user-modify", "-moz-user-select", "orphans", "outline", "outline-color",
                "outline-style", "outline-width", "overflow", "overflow-X", "overflow-Y", "padding", "padding-bottom",
                "padding-left", "padding-right", "padding-top", "page", "page-break-after", "page-break-before",
                "page-break-inside", "pause", "pause-after", "pause-before", "pitch", "pitch-range", "play-during",
                "position", "quotes", "-replace", "richness", "right", "ruby-align", "ruby-overhang", "ruby-position",
                "-set-link-source", "size", "speak", "speak-header", "speak-numeral", "speak-punctuation",
                "speech-rate", "stress", "scrollbar-arrow-color", "scrollbar-base-color", "scrollbar-dark-shadow-color",
                "scrollbar-face-color", "scrollbar-highlight-color", "scrollbar-shadow-color",
                "scrollbar-3d-light-color", "scrollbar-track-color", "table-layout", "text-align", "text-align-last",
                "text-decoration", "text-indent", "text-justify", "text-overflow", "text-shadow", "text-transform",
                "text-autospace", "text-kashida-space", "text-underline-position", "top", "unicode-bidi",
                "-use-link-source", "vertical-align", "visibility", "voice-family", "volume", "white-space", "widows",
                "width", "word-break", "word-spacing", "word-wrap", "writing-mode", "z-index", "zoom"
            ]);

            this.addNewRule = function() {
                var rule = {
                    selector: "selector",
                    rules: [],
                    id: Kooboo.Guid.Empty,
                    declarations: []
                };
                self.styleRules.push(new StyleRuleModel(rule, self));
            }

            this.getRules = params.getRules;

            this.showRelation = function(id) {
                Kooboo.EventBus.publish("ko/style/list/relation/show", id());
            }

            this.saveTrigger = ko.observable(0);
            this.saveTrigger.subscribe(_.debounce(function() {
                var _rules = self.styleRules();
                self.getRules(ko.mapping.toJS(_rules));
            }, 100));

        },
        template: template
    });

    var StyleRuleModel = function(rule, parent) {
        var self = this;

        rule.ruleType = Kooboo.styleEditor.CSSRule.STYLE_RULE;

        ko.mapping.fromJS(rule, {}, self);

        var _decs = [];
        _.forEach(rule.declarations, function(dec) {
            _decs.push(new decModel(dec, self));
        });

        this._selector = ko.observable(rule.selector);
        this.selector = ko.observable(rule.selector);
        this.selector.subscribe(function() {

            if (!self.selector()) {
                self.selector(self._selector());
            } else {
                self._selector(self.selector());
            }

            parent.saveTrigger(new Date());
        })

        this.declarations = ko.observableArray(_decs);

        this.removeSelector = function() {

            if (confirm(Kooboo.text.confirm.deleteItem)) {
                parent.styleRules.remove(self);
                parent.saveTrigger(new Date());
            }
        }

        // selector
        this.showSelectorInput = ko.observable(!self.selector());

        this.onShowSelectorInput = function() {
            self.showSelectorInput(true);
        }

        this.selectorInputBlur = function() {
            self.showSelectorInput(false);

            if (!self.selector()) {
                parent.styleRules.remove(this);
            }
        }

        this.saveTrigger = ko.observable();
        this.saveTrigger.subscribe(_.debounce(function() {
            parent.saveTrigger(new Date());
        }, 500));
    }

    var decModel = function(dec, rule) {
        var self = this;

        ko.mapping.fromJS(dec, {}, self);

        this.valueString = ko.pureComputed({
            read: function() {
                return self.value() + (self.important() ? " !important" : "")
            },
            write: function(newValue) {
                var valueArr = newValue.match(/([\w_/\?]*)( !important)?$/);

                if (valueArr[2]) {
                    self.important(true);
                    self.value(valueArr[1]);
                } else {
                    self.value(newValue);
                    self.important(false);
                }
            }
        }, this);
        this.valueString.subscribe(function(val) {
            self.emptyValue(!val);
            rule.saveTrigger(new Date());
        });

        this.timer = ko.observable(0);
        this._timer = ko.observable(0);

        this.name.subscribe(function() {

            setTimeout(function() {

                self.timer(self.timer() + 1);

                if (self.timer() % 2 == 0) {

                    if (!self.name()) {
                        rule.declarations.remove(self);
                    }
                    rule.saveTrigger(new Date());
                }
            }, 100);
        })

        // declaration name
        this.showDecName = ko.observable((!self.name()) || false);
        this.showDecName.subscribe(function(val) {

            setTimeout(function() {

                self._timer(self._timer() + 1);

                if (self._timer()) {

                    if (!val) {

                        if (!self.name()) {
                            rule.declarations.remove(self);
                            rule.saveTrigger(new Date());
                        }
                    }
                }
            }, 100);
        });

        this.onShowDecName = function() {
            self.showDecName(true);
        }

        this.decNameInputBlur = function() {
            self.showDecName(false);
        }

        // declaration value
        this.showDecValue = ko.observable(false);

        this.onShowDecValue = function() {
            self.showDecValue(true);
        }

        this.decValueInputBlur = function() {
            var _properties = _.concat(colorProperty, imageProperty, colorAndImageProperty);
            if (_properties.indexOf(self.name()) > -1) {

            } else {
                self.showDecValue(false);
            }
        }

        this.emptyValue = ko.observable(_.isEmpty(self.valueString()));

        this.switchToValueInput = function(m, e) {

            if (e.keyCode == 9 || e.keyCode == 13) {
                m.showDecName(false);
                m.showDecValue(true);
            }

            return true;
        }

        this.handleValueInputKeyPress = function(m, e) {
            // it doesn't work!
            if (e.keyCode == 9 && !e.shiftKey) {
                m.showDecValue(false);
            } else if (e.keyCode == 9 && e.shiftKey) {
                m.showDecValue(false);
                m.showDecName(true);
            } else if (e.keyCode == 13) {
                m.showDecValue(false);
            }

            return true;
        }

        this.btns = ko.observable(false);

        this.enableChangeBtns = function(dec) {
            self.btns(true);
        }

        this.disableChangesBtns = function(dec) {
            self.btns(false);
        }
        this.ableToShowColorPicker = ko.pureComputed(function() {
            return colorProperty.indexOf(self.name()) > -1 ||
                colorAndImageProperty.indexOf(self.name()) > -1;
        })

        this.ableToShowChangeImgBtn = ko.pureComputed(function() {
            return imageProperty.indexOf(self.name()) > -1 ||
                colorAndImageProperty.indexOf(self.name()) > -1
        })

        this.onPickImageBtnClick = function() {
            Kooboo.EventBus.publish("ko/style/list/pickimage/show", self);
        }
    }
})();