(function() {
    var Widget = Kooboo.Tool;

    function Helper(cntr) {

        var self = this;

        this.cntr = cntr;

        try {
            this._ringMap = new Map();

            this._labelMap = new Map();

            this._maskMap = new Map();
        } catch (ex) {
            this._ringMap = new CompMap();

            this._labelMap = new CompMap();

            this._maskMap = new CompMap()
        }

        this._shadow = new Widget.Shadow();
        this._shadow.appendTo(self.cntr);

        this._holder = new Widget.Lighter({
            borderColor: "red",
            zIndex: 9999
        });
        this._holder.appendTo(self.cntr);

        this._hover = new Widget.Lighter({
            borderColor: "#51a8ff",
            zIndex: 8888
        });
        this._hover.appendTo(self.cntr);
    }

    var proto = Helper.prototype;

    proto.shadow = function(elem, container) {
        this._shadow.mask({ el: elem }, container);
        return this;
    }

    proto.unshadow = function() {
        this._shadow.unmask();
        return this;
    }

    proto.hold = function(elem, offset) {
        this._holder.mask({ el: elem }, offset);
        return this;
    }

    proto.rehold = function() {
        this._holder.context && this.hold(this._holder.context.el);
        return this;
    }

    proto.unhold = function() {
        this._holder.unmask();
        return this;
    }

    proto.hover = function(elem) {
        this._hover.mask({ el: elem });
        return this;
    }

    proto.rehover = function() {
        this._hover.context && this.hover(this._hover.context.el);
    }

    proto.unhover = function() {
        this._hover.unmask();
        return this;
    }

    proto.ring = function(elem) {
        var map = this._ringMap,
            w = map.get(elem);

        if (!w) {
            w = new Widget.Lighter({
                context: { el: elem },
                borderColor: "#666",
                borderStyle: "dashed",
                zIndex: 1
            });

            w.appendTo(this.cntr);

            map.set(elem, w);
        }

        w.mask({ el: elem });
    }

    proto.rering = function(elem) {
        var map = this._ringMap;

        if (elem) {
            this.ring(elem);
        } else {
            map.forEach(function(it) {
                it.mask({ el: it.context.el })
            })
        }
    }

    proto.unring = function(elem) {
        var map = this._ringMap;

        if (elem) {

            if (map.has(elem)) {
                var w = map.get(elem);
                w.destroy();
                map['delete'](elem);
            }
        } else {
            map.forEach(function(it) {
                it.destroy();
            });
            map.clear();
        }
    }

    proto.label = function(elem, text) {
        var map = this._labelMap,
            w = map.get(elem);

        if (!w) {
            w = new Widget.Label({
                el: elem,
                text: text
            });

            w.appendTo(this.cntr);
            map.set(elem, w);
        }

        w.setText(text);
        w.mask({ el: elem });

        return this;
    }

    proto.relabel = function(elem) {
        var map = this._labelMap;

        if (elem) {
            var w = map.get(elem);
            w && w.mask({ el: elem });
        } else {
            map.forEach(function(it) {
                it.mask({ el: it.el });
            })
        }

        return this;
    }

    proto.unlabel = function(elem) {
        var map = this._labelMap;

        if (elem) {

            if (map.has(elem)) {
                var w = map.get(elem);
                w.destroy();
                map['delete'](elem);
            }
        } else {
            map.forEach(function(it) {
                it.destroy();
            });
            map.clear();
        }

        return this;
    }

    proto.mask = function(elem) {
        var self = this,
            map = this._maskMap,
            w = map.get(elem);

        if (!w) {
            w = new Widget.Masker({
                context: { el: elem },
                zIndex: 2
            });

            w.appendTo(this.cntr);

            $(w.domNode).on("click", function(e) {

                self._maskMap.forEach(function(it) {

                    if ($(e.target).is(it.domNode)) {
                        Kooboo.EventBus.publish("kb/lighter/holder", it.el);
                    }
                })
            })

            map.set(elem, w);
        }

        w.mask({ el: elem });
    }

    proto.remask = function() {
        this._maskMap.forEach(function(it) {
            it.mask({ el: it.context.el });
        })
    }

    proto.unmask = function(elem) {
        var map = this._maskMap;

        if (elem) {

            if (map.has(elem)) {
                var w = map.get(elem);
                w.destroy();
                map['delete'](elem);
            }
        } else {
            map.forEach(function(it) {
                it.destroy();
            });
            map.clear();
        }
    }

    proto.refresh = function() {
        this.rehold();
        this.rehover();
        this.rering();
        this.relabel();
        this.remask();
        this.unshadow();

        if (window.viewEditor && window.viewEditor.position) {
            this.shadow(window.viewEditor.position.getElement(), this.cntr);
        }
    }


    function CompMap() {
        this.elements = new Array();
        this.size = function() {
                return this.elements.length;
            },
            this.isEmpty = function() {
                return (this.elements.length < 1);
            },
            this.clear = function() {
                this.elements = new Array();
            },
            this.put = function(_key, _value) {
                if (this.containsKey(_key) == true) {
                    if (this.containsValue(_value)) {
                        if (this.remove(_key) == true) {
                            this.elements.push({
                                key: _key,
                                value: _value
                            });
                        }
                    } else {
                        this.elements.push({
                            key: _key,
                            value: _value
                        });
                    }
                } else {
                    this.elements.push({
                        key: _key,
                        value: _value
                    });
                }
            },
            this.set = function(_key, _value) {
                if (this.containsKey(_key) == true) {
                    if (this.containsValue(_value)) {
                        if (this.remove(_key) == true) {
                            this.elements.push({
                                key: _key,
                                value: _value
                            });
                        }
                    } else {
                        this.elements.push({
                            key: _key,
                            value: _value
                        });
                    }
                } else {
                    this.elements.push({
                        key: _key,
                        value: _value
                    });
                }
            },
            this.remove = function(_key) {
                var bln = false;
                try {
                    for (i = 0; i < this.elements.length; i++) {
                        if (this.elements[i].key == _key) {
                            this.elements.splice(i, 1);
                            return true;
                        }
                    }
                } catch (e) {
                    bln = false;
                }
                return bln;
            },
            this.delete = function(_key) {
                var bln = false;
                try {
                    for (i = 0; i < this.elements.length; i++) {
                        if (this.elements[i].key == _key) {
                            this.elements.splice(i, 1);
                            return true;
                        }
                    }
                } catch (e) {
                    bln = false;
                }
                return bln;
            },

            this.get = function(_key) {
                try {
                    for (i = 0; i < this.elements.length; i++) {
                        if (this.elements[i].key == _key) {
                            return this.elements[i].value;
                        }
                    }
                } catch (e) {
                    return null;
                }
            },

            this.setValue = function(_key, _value) {
                var bln = false;
                try {
                    for (i = 0; i < this.elements.length; i++) {
                        if (this.elements[i].key == _key) {
                            this.elements[i].value = _value;
                            return true;
                        }
                    }
                } catch (e) {
                    bln = false;
                }
                return bln;
            },

            this.element = function(_index) {
                if (_index < 0 || _index >= this.elements.length) {
                    return null;
                }
                return this.elements[_index];
            },

            this.containsKey = function(_key) {
                var bln = false;
                try {
                    for (i = 0; i < this.elements.length; i++) {
                        if (this.elements[i].key == _key) {
                            bln = true;
                        }
                    }
                } catch (e) {
                    bln = false;
                }
                return bln;
            },

            this.has = function(_key) {
                var bln = false;
                try {
                    for (i = 0; i < this.elements.length; i++) {
                        if (this.elements[i].key == _key) {
                            bln = true;
                        }
                    }
                } catch (e) {
                    bln = false;
                }
                return bln;
            },

            this.containsValue = function(_value) {
                var bln = false;
                try {
                    for (i = 0; i < this.elements.length; i++) {
                        if (this.elements[i].value == _value) {
                            bln = true;
                        }
                    }
                } catch (e) {
                    bln = false;
                }
                return bln;
            },

            this.keys = function() {
                var arr = new Array();
                for (i = 0; i < this.elements.length; i++) {
                    arr.push(this.elements[i].key);
                }
                return arr;
            },

            this.values = function() {
                var arr = new Array();
                for (i = 0; i < this.elements.length; i++) {
                    arr.push(this.elements[i].value);
                }
                return arr;
            };

        this.forEach = function forEach(callback, context) {
            context = context || window;

            var newAry = new Array();
            for (var i = 0; i < this.elements.length; i++) {
                if (typeof callback === 'function') {
                    var val = callback.call(context, this.elements[i].value, this.elements[i].key, this.elements);
                    newAry.push(this.elements[i].value);
                }
            }
            return newAry;
        }

    }

    Kooboo.EditorHelper = Helper;
})();