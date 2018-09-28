(function() {

    var Label = Kooboo.viewEditor.viewModel.Label,
        Data = Kooboo.viewEditor.viewModel.Data,
        Attribute = Kooboo.viewEditor.viewModel.Attribute,
        Repeat = Kooboo.viewEditor.viewModel.Repeat,
        Link = Kooboo.viewEditor.viewModel.Link,
        Form = Kooboo.viewEditor.viewModel.Form,
        Input = Kooboo.viewEditor.viewModel.Input,
        Condition = Kooboo.viewEditor.viewModel.Condition,
        ActionStore = Kooboo.viewEditor.store.ActionStore,
        BindingStore = Kooboo.viewEditor.store.BindingStore,
        DataStore = Kooboo.viewEditor.store.DataStore,
        kTag = Kooboo.viewEditor.util.k2attrTag,
        DataContext = Kooboo.viewEditor.DataContext;

    function BindingPanel(config) {
        var self = this;
        this.config = _.extend(this, config || {});
        this.const_external_link = "__external_link";

        this.elem = ko.observable();
        this.elem.subscribe(function(elem) {
            refreshExistList();
        })

        this.labelList = ko.observableArray();
        this.dataList = ko.observableArray();
        this.attributeList = ko.observableArray();
        this.repeatList = ko.observableArray();
        this.linkList = ko.observableArray();
        this.formList = ko.observableArray();
        this.inputList = ko.observableArray();
        this.existList = ko.observableArray();
        this.conditionList = ko.observableArray();

        function refreshExistList() {
            var elem = self.elem();
            self.existList.removeAll();

            function cb(item) {
                return item.elem.isSameNode(elem);
            }

            if (elem) {
                var list = [
                    self.labelList.first(cb),
                    self.dataList.first(cb),
                    self.attributeList.first(cb),
                    self.repeatList.first(cb),
                    self.linkList.first(cb),
                    self.formList.first(cb),
                    self.inputList.first(cb),
                    self.existList.first(cb),
                    self.conditionList.first(cb)
                ];
                _.compact(list).forEach(function(li) {
                    self.existList.push(li);
                });
            }
        }

        var ctors = {
            "label": Label,
            "data": Data,
            "attribute": Attribute,
            "link": Link,
            "repeat": Repeat,
            "form": Form,
            "input": Input,
            "condition": Condition
        };

        this.create = function(bindingType, ctx) {
            var elem = self.elem();
            elem && _createProxy[bindingType](elem);
        }

        this.add = function(data) {
            var item = new ctors[data.bindingType](data.elem, data);
            self[data.bindingType + 'List'].push(item);
            BindingStore.addOrUpdateBindingInfo(data);
            refreshExistList();
        };

        this.get = function(elem, bindingType) {
            return self[bindingType + 'List'].first(function(it) {
                return it.elem.isSameNode(elem);
            })
        };

        this.getAll = function() {
            return _.concat(self.labelList(),
                self.dataList(),
                self.attributeList(),
                self.linkList(),
                self.repeatList(),
                self.formList(),
                self.inputList(),
                self.conditionList()
            );
        }

        this.remove = function(item) {
            var _find = _.findLast(self[item.type + 'List'](), function(i) {
                return i.elem == item.elem;
            })

            if (_find) {
                self[item.type + 'List'].remove(_find);

                self.onRemove && self.onRemove({
                    elem: item.elem,
                    bindingType: item.type
                })
                BindingStore.removeBindingInfo(item);
                refreshExistList();
            }
        };

        this.removeBindings = function(bindings) {
            if (bindings && bindings.length) {
                bindings.forEach(function(binding) {
                    !binding.type && (binding.type = binding.bindingType);
                    var _find = _.findLast(self[binding.type + "List"](), function(i) {
                        return i.elem == binding.elem;
                    })

                    if (_find) {
                        self[binding.type + "List"].remove(_find);

                        self.onRemove && self.onRemove({
                            elem: binding.elem,
                            bindingType: binding.type
                        })
                        BindingStore.removeBindingInfo(binding);
                    }
                })
            }
            refreshExistList();
        }

        this.edit = function(item) {
            _editProxy[item.type](item);
        }

        this.update = function(data) {
            var list = self[data.bindingType + 'List'],
                inst = list.first(function(it) {
                    return it.elem.isSameNode(data.elem);
                })

            inst && _updateProxy[data.bindingType](inst, data);
            BindingStore.addOrUpdateBindingInfo(data);
        }

        var _createProxy = {
            label: function(elem) {
                Kooboo.EventBus.publish("binding/edit", {
                    bindingType: 'label',
                    elem: elem,
                    text: ''
                });
            },
            data: function(elem) {
                Kooboo.EventBus.publish("binding/edit", {
                    bindingType: 'data',
                    elem: elem,
                    text: ''
                });
            },
            attribute: function(elem) {
                var attributes = {};
                if (!_.isEmpty($(elem).attr("k-attributes"))) {
                    var rawAttributes = $(elem).attr("k-attributes");
                    var _pairs = _.filter(rawAttributes.split(";"), function(p) {
                        return !_.isEmpty(p);
                    });
                    _.forEach(_pairs, function(pair) {
                        var _key = pair.split(" ")[0];
                        var _val = pair.split(" ")[1];
                        attributes[_key] = _val;
                    });
                }
                Kooboo.EventBus.publish("binding/edit", {
                    bindingType: 'attribute',
                    elem: elem,
                    attributes: _.isEmpty(attributes) ? null : attributes
                });
            },
            repeat: function(elem) {
                Kooboo.EventBus.publish("binding/edit", {
                    bindingType: 'repeat',
                    elem: elem,
                    text: '',
                    repeatSelf: false
                });
            },
            link: function(elem) {
                Kooboo.EventBus.publish("binding/edit", {
                    bindingType: 'link',
                    elem: elem,
                    href: elem.getAttribute("href"),
                    params: {}
                });
            },
            form: function(elem) {
                Kooboo.EventBus.publish("binding/edit", {
                    bindingType: 'form',
                    elem: elem
                });
            },
            input: function(elem) {
                Kooboo.EventBus.publish("binding/edit", {
                    bindingType: 'input',
                    elem: elem,
                    text: ''
                });
            },
            condition: function(elem) {
                Kooboo.EventBus.publish("binding/edit", {
                    bindingType: 'condition',
                    elem: elem,
                    text: ''
                })
            }
        }

        var _editProxy = {
            label: function(item) {
                Kooboo.EventBus.publish("binding/edit", {
                    elem: item.elem,
                    bindingType: 'label',
                    text: item.text()
                });
            },
            data: function(item) {
                Kooboo.EventBus.publish("binding/edit", {
                    elem: item.elem,
                    bindingType: 'data',
                    text: item.text()
                });
            },
            attribute: function(item) {
                var elem = item.elem,
                    attributes = {};
                if (!_.isEmpty($(elem).attr("k-attributes"))) {
                    var rawAttributes = $(elem).attr("k-attributes");
                    var _pairs = _.filter(rawAttributes.split(";"), function(p) {
                        return !_.isEmpty(p);
                    });
                    _.forEach(_pairs, function(pair) {
                        var _tempArr = pair.split(" ");
                        var _key = _tempArr.splice(0, 1)[0];
                        var _val = _tempArr.join(" ");
                        attributes[_key] = _val;
                    });
                }
                Kooboo.EventBus.publish("binding/edit", {
                    bindingType: 'attribute',
                    elem: elem,
                    attributes: _.isEmpty(attributes) ? null : attributes
                });
            },
            repeat: function(item) {
                Kooboo.EventBus.publish("binding/edit", {
                    elem: item.elem,
                    bindingType: 'repeat',
                    text: item.text(),
                    repeatSelf: item.repeatSelf()
                });
            },
            link: function(item) {
                Kooboo.EventBus.publish("binding/edit", {
                    elem: item.elem,
                    bindingType: 'link',
                    href: item.href(),
                    params: item.params,
                    page: item.page
                });
            },
            form: function(item) {
                Kooboo.EventBus.publish("binding/edit", {
                    elem: item.elem,
                    bindingType: 'form',
                    dataSourceMethodId: item.dataSourceMethodId(),
                    dataSourceMethodDisplay: item.dataSourceMethodDisplay(),
                    method: item.method(),
                    redirect: item.redirect(),
                    callback: item.callback()
                });
            },
            input: function(item) {
                Kooboo.EventBus.publish("binding/edit", {
                    elem: item.elem,
                    bindingType: 'input',
                    text: item.text()
                });
            },
            condition: function(item) {
                Kooboo.EventBus.publish("binding/edit", {
                    elem: item.elem,
                    bindingType: 'condition',
                    text: item.text()
                });
            }
        }

        var _updateProxy = {
            label: function(item, data) {
                item.text(data.text);;
            },
            data: function(item, data) {
                item.text(data.text);
            },
            attribute: function(item, data) {
                item.text(data.text);
            },
            repeat: function(item, data) {
                item.text(data.text);
                item.repeatSelf(data.repeatSelf);
            },
            link: function(item, data) {
                item.href(data.href);
                item.params = data.params;
                item.page = data.page;
            },
            form: function(item, data) {
                item.dataSourceMethodId(data.dataSourceMethodId);
                item.method(data.method);
                item.redirect(data.redirect);
                item.callback(data.callback);
            },
            input: function(item, data) {
                item.text(data.text);
            },
            condition: function(item, data) {
                item.text(data.text);
            }
        }

        this.showLabel = ko.pureComputed(function() {
            var elem = self.elem(),
                unableToLabelTags = ["input", "img", "br", "hr"];

            function cb(it) {
                return it.elem.isSameNode(elem);
            }

            function isElemInsideDynamicContent(elem) {
                if ($(elem).parents(kTag["label"]).length ||
                    $(elem).parents(kTag["data"]).length) {
                    return true;
                }
            }

            return elem && !isInput(elem) && !self.labelList.first(cb) && !self.dataList.first(cb) && !self.repeatList.first(cb) &&
                (unableToLabelTags.indexOf(elem.tagName.toLowerCase()) == -1) && !isElemInsideDynamicContent(elem) && !elem.hasAttribute("k-placeholder");
        });

        this.showLink = ko.pureComputed(function() {
            var elem = self.elem();

            function isElemInsideDynamicContent(elem) {
                if ($(elem).parents(kTag["label"]).length ||
                    $(elem).parents(kTag["data"]).length) {
                    return true;
                }
            }

            return elem && elem.tagName.toLowerCase() == "a" && !self.linkList.first(function(it) {
                return it.elem.isSameNode(elem);
            }) && !isElemInsideDynamicContent(elem) && !elem.hasAttribute("k-placeholder");
        });

        this.showData = ko.pureComputed(function() {
            var elem = self.elem(),
                unableToSetDataTag = ["input", "img", "br", "hr"];

            function cb(it) {
                return it.elem.isSameNode(elem);
            }

            function isElemInsideDynamicContent(elem) {
                if ($(elem).parents(kTag["label"]).length ||
                    $(elem).parents(kTag["data"]).length) {
                    return true;
                }
            }

            return elem && !self.labelList.first(cb) && !self.dataList.first(cb) && unableToSetDataTag.indexOf(elem.tagName.toLowerCase()) == -1 &&
                !isElemInsideDynamicContent(elem) && !elem.hasAttribute("k-placeholder");
        });

        this.showAttribute = ko.pureComputed(function() {
            var elem = self.elem();

            function cb(it) {
                return it.elem.isSameNode(elem);
            }

            function isElemInsideDynamicContent(elem) {
                if ($(elem).parents(kTag["label"]).length ||
                    $(elem).parents(kTag["data"]).length) {
                    return true;
                }
            }

            return elem && !self.labelList.first(cb) && !self.attributeList.first(cb) && !isElemInsideDynamicContent(elem) && !elem.hasAttribute("k-placeholder");
        });

        this.showRepeat = ko.pureComputed(function() {
            var elem = self.elem(),
                unableToSetRepeatTag = ["input", "img"];

            function cb(it) {
                return it.elem.isSameNode(elem);
            }

            function isElemInsideDynamicContent(elem) {
                if ($(elem).parents(kTag["label"]).length ||
                    $(elem).parents(kTag["data"]).length) {
                    return true;
                }
            }

            return elem && !self.labelList.first(cb) && !self.dataList.first(cb) && !self.repeatList.first(cb) &&
                unableToSetRepeatTag.indexOf(elem.tagName.toLowerCase()) == -1 && !isElemInsideDynamicContent(elem) && !elem.hasAttribute("k-placeholder");
        });

        this.showForm = ko.pureComputed(function() {
            var elem = self.elem();

            function cb(it) {
                return it.elem.isSameNode(elem);
            }
            return elem && elem.tagName.toLowerCase() == "form" && !self.formList.first(cb) && !elem.hasAttribute("k-placeholder");
        });

        this.showInput = ko.pureComputed(function() {
            var elem = self.elem();

            function cb(it) {
                return it.elem.isSameNode(elem);
            }
            return elem && isInput(elem) && !self.inputList.first(cb) && !elem.hasAttribute("k-placeholder");
        });

        this.showCondition = ko.pureComputed(function() {
            var elem = self.elem();

            function cb(it) {
                return it.elem.isSameNode(elem);
            }

            function isElemInsideDynamicContent(elem) {
                if ($(elem).parents(kTag["label"]).length ||
                    $(elem).parents(kTag["data"]).length) {
                    return true;
                }
            }

            return elem && !self.labelList.first(cb) && !self.conditionList.first(cb) && !isElemInsideDynamicContent(elem) && !elem.hasAttribute("k-placeholder");
        })

        this.showBindingBtns = ko.pureComputed(function() {
            return self.showAttribute() ||
                self.showData() ||
                self.showForm() ||
                self.showInput() ||
                self.showLink() ||
                self.showRepeat() ||
                self.showCondition()
        })

        this.reset = function() {
            self.labelList.removeAll();
            self.dataList.removeAll();
            self.attributeList.removeAll();
            self.repeatList.removeAll();
            self.linkList.removeAll();
            self.formList.removeAll();
            self.inputList.removeAll();
            self.conditionList.removeAll();
            self.elem(null);
        };

        function isInput(elem) {
            return $(elem).is(':text,textarea,select,:radio,:checkbox');
        }

        Kooboo.EventBus.subscribe("DataStore/removed", function(removed) {
            if (confirm(Kooboo.text.confirm.removeBinding)) {
                _.forEach(removed.reverse(), function(r) {
                    self.removeBindings(_.cloneDeep(BindingStore.getRemoveBindingInfos(r.id)));
                    $(window).trigger("resize");
                })
            }
        })
    }

    Kooboo.viewEditor.viewModel.BindingPanel = BindingPanel;
})();