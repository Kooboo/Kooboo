(function() {

    var Label = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].viewModel.Label,
        Script = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].viewModel.Script,
        Style = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].viewModel.Style,
        Position = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].viewModel.Position,
        PositionStore = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].store.PositionStore,
        BindingStore = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].store.BindingStore;

    var positionKey = "k-placeholder";

    function newId() {
        return Math.ceil(Math.random() * Math.pow(2, 53));
    }

    function getAllPositions() {
        return PositionStore.getAll().map(function(it) {
            return new Position(it);
        });
    }

    function getAllLabels() {
        var list = BindingStore.getAll(),
            ret = [];
        _.forEach(list, function(it) {
            if (it.type == 'label') {
                ret.push(new Label(it));
            }
        });
        return ret;
    }

    function getAllStyles() {
        return _.filter(BindingStore.getAll(), function(style) {
            return style.type === "style" && style.text() !== "";
        });
    }

    function BindingPanel() {
        var self = this;
        this.elem = ko.observable();
        this.elem.subscribe(function() {
            refreshExistList();
        });

        this.styleResource = ko.observable({});
        this.scriptResource = ko.observable({});
        this.resources = ko.pureComputed(function() {
            var _res = {};
            _.forEach(Object.keys(self.styleResource()), function(key) {
                _res[key] = self.styleResource()[key];
            })
            _.forEach(Object.keys(self.scriptResource()), function(key) {
                _res[key] = self.scriptResource()[key];
            });
            return _res;
        })

        this.positionList = ko.observableArray(getAllPositions());
        Kooboo.EventBus.subscribe("PositionStore/change", function() {
            self.positionList(getAllPositions());
            refreshExistList();
        })

        this.labelList = ko.observableArray();
        this.styleList = ko.observableArray();

        this.headScriptList = ko.observableArray();
        this.bodyScriptList = ko.observableArray();

        this.headScriptList.onReceive = function(sortedList, context) {
            context.viewModel.head = true;
            context = _.assignIn(context, { list: sortedList });
            publishSortEventToKbFrame('script', context, 'head');
        };
        this.bodyScriptList.onReceive = function(sortedList, context) {
            context.viewModel.head = false;
            context = _.assignIn(context, { list: sortedList });
            publishSortEventToKbFrame('script', context, 'body');
        };
        this.headScriptList.onSorted = function(sortedList, context) {
            context = _.assignIn(context, { list: sortedList });
            publishSortEventToKbFrame('script', context);
        };
        this.bodyScriptList.onSorted = function(sortedList, context) {
            context = _.assignIn(context, { list: sortedList });
            publishSortEventToKbFrame('script', context);
        };
        this.styleList.onSorted = function(sortedList, context) {
            context = _.assignIn(context, { list: sortedList });
            publishSortEventToKbFrame('style', context)
        };

        function publishSortEventToKbFrame(type, context) {
            Kooboo.EventBus.publish("kb/frame/resource/sort", {
                type: type,
                targetIdx: context.targetIndex,
                elem: context.viewModel.elem,
                list: context.list
            })
        }

        Kooboo.EventBus.subscribe("BindingStore/change", function() {
            self.labelList(getAllLabels());
            self.headScriptList(_.filter(BindingStore.getAll(), { type: "script", head: true }));
            self.bodyScriptList(_.filter(BindingStore.getAll(), { type: "script", head: false }));
            self.styleList(getAllStyles());
            refreshExistList();
        });

        this.removeScript = function(m, e) {
            e.preventDefault();
        }

        this.existList = ko.observableArray();

        function refreshExistList() {
            var elem = self.elem();
            self.existList.removeAll();

            function cb(item) {
                return item.elem == elem;
            }
            if (elem) {
                var list = [
                    self.positionList.first(cb),
                    self.labelList.first(cb)
                ];
                _.forEach(list, function(it) {
                    it && self.existList.push(it);
                });
            }
        }

        this.showConvert = ko.pureComputed(function() {
            var elem = self.elem();
            return elem && elem.tagName && elem.tagName.toLowerCase() != positionKey && !self.positionList.first(function(it) {
                var el = it.placeholder || it.elem;
                return el.contains(elem) || elem.contains(el);
            }) && !self.labelList.first(function(it) {
                return it.elem.contains(elem);
            }) && elem !== elem.ownerDocument.body;
        });

        this.showPrepend = ko.pureComputed(function() {
            var elem = self.elem();
            return elem && elem.tagName && elem.tagName.toLowerCase() != positionKey && !self.labelList.first(function(it) {
                return it.elem.contains(elem);
            }) && elem !== elem.ownerDocument.body;
        });

        this.showAppend = ko.pureComputed(function() {
            var elem = self.elem();
            return elem && elem.tagName && elem.tagName.toLowerCase() != positionKey && !self.labelList.first(function(it) {
                return it.elem.contains(elem);
            }) && elem !== elem.ownerDocument.body;
        });

        this.showLabel = ko.pureComputed(function() {
            var elem = self.elem();
            return elem && elem.tagName && elem.tagName.toLowerCase() != positionKey && !self.positionList.first(function(it) {
                return (it.placeholder || it.elem).contains(elem);
            }) && !self.labelList.first(function(it) {
                return elem.contains(it.elem) || it.elem.contains(elem);
            }) && elem !== elem.ownerDocument.body && !$("[" + positionKey + "]", $(elem)).length;
        });

        this.createLabel = _.bind(function() {
            Kooboo.EventBus.publish("binding/edit", {
                type: 'label',
                elem: self.elem()
            });
        }, this);

        this.editLabel = _.bind(function(item) {
            self.elem(item.elem);
            Kooboo.EventBus.publish("binding/edit", {
                id: item.id,
                elem: item.elem,
                type: item.type,
                text: item.text()
            });
        });

        this.removeLabel = _.bind(function(item) {
            Kooboo.EventBus.publish("binding/remove", {
                id: item.id
            });

            Kooboo.EventBus.publish("kb/frame/dom/update")
        });

        this.createScript = _.bind(function(isHead) {
            var self = this;

            var _choosenScriptList = _.concat(self.headScriptList(), self.bodyScriptList()),
                _resources = _.cloneDeep(self.scriptResource());

            ["scripts", "scriptGroup"].map(function(key) {
                var _choosenScripts = [];

                _choosenScriptList.map(function(script) {
                    _choosenScripts.push(script.name());
                });
                _choosenScripts = _.compact(_choosenScripts);

                var _filterScripts = _.remove(_resources[key], function(script) {
                    return (_choosenScripts.indexOf(script.text) == -1);
                });

                _resources[key] = _filterScripts;
            });

            Kooboo.EventBus.publish("binding/edit", {
                type: 'script',
                elem: self.elem(),
                resources: _resources,
                isHead: isHead
            });
        }, this);

        this.createStyle = _.bind(function() {
            var self = this;

            var _resources = _.cloneDeep(self.styleResource());

            ["styles", "styleGroup"].map(function(key) {
                var _choosenStyleList = [];

                self.styleList().map(function(style) {
                    _choosenStyleList.push(style.name());
                });
                _choosenStyleList = _.compact(_choosenStyleList);

                var _filterStyles = _.remove(_resources[key], function(style) {
                    return (_choosenStyleList.indexOf(style.text) == -1);
                })

                _filterStyles = _.remove(_filterStyles, function(style) {
                    return (!_.isEmpty(style.text));
                });

                _resources[key] = _filterStyles;
            });

            Kooboo.EventBus.publish("binding/edit", {
                type: 'style',
                elem: self.elem(),
                resources: _resources
            });
        }, this);

        this.editJsCss = _.bind(function(item) {
            self.elem(item.elem);
            Kooboo.EventBus.publish("binding/edit", {
                id: item.id,
                elem: item.elem,
                type: item.type,
                text: item.text()
            });
        });

        this.removeScript = _.bind(function(item) {
            item && item.elem && item.elem.tagName === "SCRIPT" && $(item.elem).remove();
            Kooboo.EventBus.publish("binding/remove", {
                id: item.id
            });
            Kooboo.EventBus.publish("kb/page/field/change", {
                type: "resource"
            })
            Kooboo.EventBus.publish("kb/frame/resource/remove", {
                type: "script",
                tag: item.elem
            })
        });

        this.removeStyle = _.bind(function(item) {
            item && item.elem &&
                ((item.elem.tagName === "LINK" && item.elem.getAttribute("rel") === "stylesheet") || item.elem.tagName === "STYLE") &&
                $(item.elem).remove();
            Kooboo.EventBus.publish("binding/remove", {
                id: item.id
            });
            Kooboo.EventBus.publish("kb/page/field/change", {
                type: "resource"
            })
            Kooboo.EventBus.publish("kb/frame/resource/remove", {
                type: "style",
                tag: item.elem
            })
        });

        this.convert = _.bind(function() {
            var elem = this.elem();
            var foundItem = this.labelList.first(function(it) {
                if (it.elem != elem && elem.contains(it.elem)) {
                    return it;
                }
            });
            if (!foundItem || confirm(Kooboo.text.confirm.layoutEditor.labelInside)) {
                foundItem && self.removeLabel(foundItem);
                Kooboo.EventBus.publish("position/edit", {
                    elem: this.elem(),
                    type: 'attr'
                });
            }
        }, this);

        this.prepend = _.bind(function() {
            Kooboo.EventBus.publish("position/edit", {
                elem: this.elem(),
                type: 'prepend'
            });
        }, this);

        this.append = _.bind(function() {
            Kooboo.EventBus.publish("position/edit", {
                elem: this.elem(),
                type: 'append'
            });
        }, this);

        this.edit = _.bind(function(item) {
            if (item.hasOwnProperty("name")) {
                Kooboo.EventBus.publish("position/edit", {
                    id: item.id,
                    elem: item.elem,
                    name: item.name(),
                    type: item.type
                });
            } else {
                Kooboo.EventBus.publish("binding/edit", {
                    id: item.id,
                    elem: item.elem,
                    text: item.text(),
                    type: item.type
                });
            }
        }, this);

        this.remove = _.bind(function(item) {
            Kooboo.EventBus.publish("position:remove", {
                id: item.id,
                elem: item.elem
            });
            Kooboo.EventBus.publish("kb/frame/layout/resource/update")
        }, this);

        this.focusPosition = function(item) {
            Kooboo.EventBus.publish("position:focus", {
                id: item.id,
                elem: item.elem,
                name: item.name(),
                type: item.type
            });
        }

    }

    if (Kooboo.layoutEditor) {
        Kooboo.layoutEditor.viewModel.BindingPanel = BindingPanel;
    }

    if (Kooboo.pageEditor) {
        Kooboo.pageEditor.viewModel.BindingPanel = BindingPanel;
    }
})();