(function() {

    var kBinder = Kooboo.viewEditor.util.kBinder,
        kParser = Kooboo.viewEditor.util.kParser,
        k2attr = Kooboo.viewEditor.util.k2attr,
        Label = Kooboo.viewEditor.widget.Label,
        DataStore = Kooboo.viewEditor.store.DataStore,
        ActionStore = Kooboo.viewEditor.store.ActionStore,
        DataContext = Kooboo.viewEditor.DataContext,
        declare = Kooboo.declare;

    var Position = declare(null, {
        init: function(elem) {
            this.posType = "attr";
            this.elem = elem;
            this.origHTML = this.getHTML();
            this.bindings = [];
            this.scanBinding();
            var self = this;
            $(this.elem).on("click mouseover mouseout", function(e) {
                var children = self.getChildren(),
                    elem = e.target,
                    inside = _.some(children, function(it) {
                        return it.contains(elem);
                    });
                if (inside) {
                    // self.emit(e.type, e);
                }
            });
            return this;
        },
        getHTML: function() {
            var html = $(this.elem).data("html");
            // return html ? html : "";
            return this.elem.innerHTML;
        },
        getChildren: function() {
            return $(this.elem).children().toArray();
        },
        getElement: function() {
            return this.elem;
        },
        getRect: function() {
            return this.elem.getBoundingClientRect();
        },
        setHTML: function(html) {
            this.elem.innerHTML = html;
            // $(this.elem).data("html", html);
            this.scanBinding();
            return this;
        },
        reset: function() {
            this.setHTML(this.origHTML);
            return this;
        },
        _scanBinding: function(elem, contextStack) {
            var self = this,
                children = elem.children,
                len = children.length,
                parsed = kParser.parse(elem),
                list = this.bindings,
                item, dataContext;
            _.forEach(parsed, function(val, key) {
                if (val) {
                    switch (key) {
                        case 'label':
                            list.push({
                                elem: elem,
                                bindingType: key,
                                text: val
                            });
                            break;
                        case 'data':
                            var repeatSelf = !!$(elem).attr('k-repeat-self');
                            if(repeatSelf) {
                                dataContext = $(elem).data('kb-datacontext-repeat');
                                var _key = Object.keys(dataContext.value())[0];
                                var dataId = dataContext ? dataContext.value(true)[_key].dataId : null;
                                list.push({
                                    elem: elem,
                                    bindingType: key,
                                    text: val.text,
                                    dataSourceId: dataId
                                });
                            } else {
                                dataContext = DataContext.create(elem, contextStack, false);
                                list.push({
                                    elem: elem,
                                    bindingType: key,
                                    text: val.text,
                                    dataSourceId: dataContext ? dataContext.lookup(val.text).dataId : null
                                });
                            }
                            break;
                        case 'attribute':
                            var ids = [];
                            var repeatSelf = !!$(elem).attr('k-repeat-self');
                            if(repeatSelf) {
                                dataContext = $(elem).data('kb-datacontext-repeat');
                                if(dataContext) {
                                  val.text.split(";").forEach(function(attr) {
                                    if (attr) {
                                            var value = attr.split(" ")[1],
                                                sources = value.match(/[^\{\}]+(?=\})/g);
                                            if (sources && sources.length) {
                                                sources.forEach(function(src) {
                                                    var _key = src.split('.')[0];
                                                    var dataId = dataContext.value(true)[_key].dataId;
                                                    ids.indexOf(dataId) == -1 && ids.push(dataId);
                                                });
                                            }
                                        }
                                    });
                                }
                                list.push({
                                    elem: elem,
                                    bindingType: key,
                                    text: val.text,
                                    ids: ids
                                });
                            } else {
                                dataContext = DataContext.createByAttribute(elem, contextStack);
                                val.text.split(";").forEach(function(attr) {
                                    if (attr) {
                                        var value = attr.split(" ")[1],
                                            sources = value.match(/[^\{\}]+(?=\})/g);
                                        if (sources && sources.length) {
                                            sources.forEach(function(src) {
                                                var find = dataContext.lookup(src);
                                                find && ids.indexOf(find.dataId) == -1 && ids.push(find.dataId);
                                            });
                                        }
                                    }
                                });
                                list.push({
                                    elem: elem,
                                    bindingType: key,
                                    text: val.text,
                                    ids: ids
                                });
                            }
                            break;
                        case 'repeat':
                            dataContext = DataContext.create(elem, contextStack, true);
                            if (!dataContext) {
                                console.warn("Fail to find repeated source:" + val);
                            } else {
                                var repeatSelf = !!$(elem).attr('k-repeat-self');
                                var _key;

                                if (repeatSelf) {
                                    _key = Object.keys(dataContext.value())[0];
                                    $(elem).data('kb-datacontext-repeat', dataContext);
                                } else {
                                    _key = Object.keys(DataContext.get(elem.parentNode).value())[0];
                                }

                                list.push({
                                    elem: elem,
                                    bindingType: key,
                                    text: val,
                                    repeatSelf: repeatSelf,
                                    dataSourceId: dataContext.value(true)[_key].dataId
                                });
                            }
                            break;
                        case 'link':
                            list.push({
                                elem: elem,
                                bindingType: key,
                                href: val.href,
                                params: val.params,
                                page: val.page
                            });
                            break;
                        case "form":
                            var formBinding = {
                                elem: elem,
                                bindingType: key,
                                dataSourceMethodId: val.dataSourceMethodId,
                                method: val.method,
                                redirect: val.redirect,
                                callback: val.callback
                            };
                            list.push(formBinding);
                            $(elem).data('kb-form-binding', formBinding);
                            break;
                        case "input":
                            list.push({
                                elem: elem,
                                bindingType: key,
                                text: val
                            });
                            break;
                        case 'condition':
                            {
                                list.push({
                                    elem: elem,
                                    bindingType: key,
                                    text: val.text
                                });
                            }
                            break;
                        default:
                            break;
                    }
                }
            });
            if (dataContext) {
                DataContext.set(elem, dataContext);
                contextStack.push(dataContext);
            }
            for (var i = 0; i < len; i++) {
                self._scanBinding(children[i], contextStack);
            }
            if (dataContext) {
                contextStack.pop();
            }
        },
        scanBinding: function() {
            this.bindings = [];
            this._scanBinding(this.elem, DataContext.createContextStack());
            return this;
        },
        bind: function(data) {
            if (data.bindingType == "attribute" && _.isEmpty(data.text)) {
                this.unbind(data.elem, data.bindingType);
                Kooboo.EventBus.publish("position/bind/remove", data);
                return this;
            } else {
                kBinder.bind(data.elem, data);
            }
            return this;
        },
        unbind: function(elem, bindingType) {
            kBinder.unbind(elem, bindingType);
            return this;
        },
        getBindings: function(elem) {
            this.scanBinding();
            return elem ? _.filter(this.bindings, function(it) {
                return it.elem == elem;
            }) : _.clone(this.bindings);
        },
        destroy: function() {
            this.unring();
            this.unlabel();
            delete this.elem;
        }
    });

    Kooboo.viewEditor.widget.Position = Position;
})();