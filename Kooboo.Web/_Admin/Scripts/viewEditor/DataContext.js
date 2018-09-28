(function() {
    var DataStore = Kooboo.viewEditor.store.DataStore,
        ActionStore = Kooboo.viewEditor.store.ActionStore,
        k2attr = Kooboo.viewEditor.util.k2attr;

    var DataContextStack = function() {
        var self = this;
        var stack = [];
        self.lookup = function(key) {
            for (var i = stack.length - 1, item; item = stack[i]; i--) {
                var context = stack[i];
                var value = context.lookup(key);
                if (value) {
                    return value;
                }
            }
            return null;
        };
        self.push = function(context) {
            stack.push(context);
        };
        self.pop = function() {
            if (stack.length > 0) {
                var context = stack[stack.length - 1];
                stack.length = stack.length - 1;
                return context;
            }
            return null;
        };
    };
    DataContextStack.create = function() {
        var stack = new DataContextStack();
        stack.push(DataContext.global());
        return stack;
    };
    var DataContext = function(element, value) {
        var self = this,
            $element = element == null ? null : $(element),
            value = value || {};
        self.$element = function() {
            return $element;
        };
        self.lookup = function(key) {
            return value[key] || null;
        };
        self.value = function(includeBase) {
            var result = $.extend(true, {}, value);
            if (includeBase) {
                if ($element) {
                    var parentDataContext = null;
                    var $parent = $element.parent();
                    if ($parent.length === 0) {
                        parentDataContext = DataContext.global();
                    } else {
                        parentDataContext = DataContext.getParent($parent);
                    }
                    if (parentDataContext) {
                        var baseValues = parentDataContext.value(true);
                        for (var name in baseValues) {
                            if (!result[name]) {
                                result[name] = baseValues[name];
                            }
                        }
                    }
                }
            }
            return result;
        };
        self.flatternFields = function(options) {
            var result = [];
            var value = self.value(options.includeBase);
            var names = Object.keys(value);
            for (var i = 0, len = names.length; i < len; i++) {
                var fields = [];
                fields = DataContext.flatternFields(value[names[i]], names[i] + '.', fields, options.predicate);
                result.push({
                    dataName: names[i],
                    enumerable: value[names[i]].enumerable,
                    fields: fields,
                    children: value[names[i]].children
                });
            }
            return result;
        };

        self.getDataSource = function() {
            var result = [];
            var value = self.value(true);
            var names = Object.keys(value);

            _.forEach(names, function(name) {
                result.push($.extend(true, value[name], { name: name }));
            })

            return result;
        }

        self.getBindingFields = function(options) {
            var result = [],
                value = self.value(true),
                names = Object.keys(value);

            _.forEach(names, function(name) {
                var fields = [];

                fields = DataContext.getBindingFields(value[name], name + ".", fields, options);

                result.push({
                    dataName: name,
                    list: fields,
                    enumerable: value[name].enumerable
                })
            });

            return result;
        }
    };
    DataContext.getBindingFields = function(data, prefix, fields, options) {
        var dataFields = data.fields;

        if (typeof dataFields == "function") {
            dataFields = dataFields();
        }

        if (!dataFields || !dataFields.length) {
            dataFields = data.itemFields || [];
        }

        prefix = prefix || "";
        fields = fields || [];
        _.forEach(dataFields, function(field) {

            if (options && options.predicate(field)) {
                if (field.isComplexType) {
                    _.forEach(field.fields, function(f) {
                        fields.push({
                            name: prefix + field.name + "." + f.name,
                            type: f.type
                        });
                    })
                } else {
                    fields.push({
                        name: field.name ? prefix + field.name : (prefix.indexOf(".") > -1 ? prefix.split(".")[0] : prefix),
                        type: field.type
                    })
                }
            }

            if (field.fields && field.fields.length) {
                var newPrefix = prefix + field.name + ".";

                if ((!field.enumerable)) {
                    DataContext.getBindingFields(field, newPrefix, fields);
                }
            }

            if (data.children && data.children.length) {
                _.forEach(data.children, function(child) {
                    DataContext.getBindingFields(child, prefix + child.name + ".", fields);
                })
            }
        });

        return fields;
    }

    DataContext.flatternFields = function(data, prefix, fields, predicate) {
        var dataFields = data.fields;
        if (typeof dataFields === 'function') {
            dataFields = dataFields();
        }
        if (!dataFields || !dataFields.length) {
            dataFields = data.itemFields || [];
        }
        prefix = prefix || '';
        fields = fields || [];
        for (var i = 0, len = dataFields.length; i < len; i++) {
            var field = dataFields[i];
            if (!predicate || predicate(field)) {
                fields.push({
                    name: prefix + field.name,
                    type: field.type
                });
            }
            if (field.fields) {
                var newPrefix = prefix + field.name + '.';
                DataContext.flatternFields(field, newPrefix, fields, predicate);
            }
        }
        return fields;
    };
    DataContext.get = function(element) {
        var $element = !!$(element).attr("k-repeat-self") ? $(element) : $(element).parent();
        while ($element.length > 0) {
            var context = $element.data('kb-datacontext');
            if (context) {
                return context;
            }
            $element = $element.parent();
        }
        return DataContext.global();
    };
    DataContext.getParent = function(element) {
        var $element = $(element);
        while ($element.length > 0) {
            var context = $element.data('kb-datacontext');
            if (context) {
                return context;
            }
            $element = $element.parent();
        }
        return DataContext.global();
    };
    DataContext.set = function(container, dataContext) {
        $(container).data('kb-datacontext', dataContext);
    };
    DataContext.clear = function(container) {
        DataContext.clearDesendants(container);
        $(container).data('kb-datacontext', null);
    };
    DataContext.clearDesendants = function(container) {
        $(container).find('*').each(function() {
            $(this).data('kb-datacontext', null);
        });
    };
    DataContext.createContextStack = function(pushGlobal) {
        if (arguments.length === 0) {
            pushGlobal = true;
        }
        var stack = new DataContextStack();
        if (pushGlobal) {
            stack.push(DataContext.global());
        }
        return stack;
    };
    DataContext.create = function(element, contextStack, isRepeat) {
        if (!contextStack)
            throw new Error("Context stack is required to create a data context.");
        var $element = $(element);
        var identifier = '',
            bindingExp, isRepeater = isRepeat;

        if (isRepeat) {
            bindingExp = $element.attr(k2attr['repeat']);
            if (bindingExp) {
                isRepeater = true;
                var parts = bindingExp.split(' ');
                identifier = parts[0];
                bindingExp = parts[1];
            }
        } else {
            bindingExp = $element.attr(k2attr['data']);
            identifier = bindingExp;
        }

        var data = parseBindingExpression(bindingExp, contextStack);
        if (data) {
            var value = {};
            var repeatData = _.cloneDeep(data);
            repeatData.enumerable = false;
            value[identifier] = repeatData;
            return new DataContext(element, value);
        }
        return null;
    };
    DataContext._global = null;
    DataContext.global = function() {
        // if (DataContext._global) {
        //     return DataContext._global;
        // }
        var value = {};
        $.each(DataStore.getNestedByParent(null), function() {
            var aliasName = this.aliasName;
            value[aliasName] = createDataContextValueFromDataSource(this);
        });
        DataContext._global = new DataContext(null, value);
        return DataContext._global;
    };
    DataContext.createByAttribute = function(element, contextStack) {
        if (!contextStack) {
            throw new Error("Context stack is required to create a data context.");
        }
        var dataContext = DataContext.get(element),
            values = dataContext ? dataContext.value() : {};

        $(element).attr(k2attr['attributes']).split(";").forEach(function(attr) {
            if (attr) {
                var value = attr.split(" ")[1],
                    sources = value.match(/[^\{\}]+(?=\})/g);
                if (sources && sources.length) {
                    sources.forEach(function(src) {
                        var data = parseBindingExpression(src, contextStack);
                        if (data) {
                            var value = {},
                                repeatData = _.cloneDeep(data);
                            repeatData.enumerable = false;
                            values[src] = repeatData;
                        }
                    })
                }
            }
        })

        return new DataContext(element, values);
    }

    function parseBindingExpression(exp, contextStack) {
        var segments = exp.split('.');
        var data = contextStack.lookup(segments[0]);
        var find;
        if (data) {
            var dataFields = data.fields,
                children = data.children;

            if (typeof dataFields === 'function') {
                dataFields = dataFields();
            }

            if (!dataFields || !dataFields.length) {
                dataFields = data.itemFields || [];
            }

            for (var i = 1, item; item = segments[i]; i++) {
                var fieldName = segments[i];
                find = _.find(dataFields, function(f) {
                    return f.name === fieldName;
                });
                if (!find) {
                    break;
                } else {
                    !find.dataId && (find.dataId = data.dataId);
                    return find;
                }

            }

            if (!find) {
                find = _.find(children, function(child) {
                    return segments.indexOf(child.name) > -1;
                });
                find && (data = find);
            }
        }
        return data;
    }

    function createDataContextValueFromDataSource(dataSource) {
        var action = ActionStore.byId(dataSource.methodId);
        var result = {
            type: 'datasource',
            methodId: dataSource.methodId,
            dataId: dataSource.id,
            parentMethodId: dataSource.parentId,
            fields: _.cloneDeep(action ? action.fields || [] : []),
            itemFields: _.cloneDeep(action ? action.itemFields || [] : []),
            enumerable: action ? action.enumerable : true,
            isComplexType: action ? (action.hasOwnProperty("isComplexType") ? action.isComplexType : true) : false,
            isPagedResult: action ? action.isPagedResult : false,
            paras: action ? action.paras : null
        };
        if (action) {
            if (!action.enumerable) {
                var children = DataStore.byParent(dataSource.id);
                _.forEach(children, function(child) {
                    var childDataContext = createDataContextValueFromDataSource(child);
                    var newChildData = _.map(childDataContext.itemFields, function(item) {
                        item.name = child.aliasName + "." + item.name;
                        return item;
                    });
                    result.itemFields = _.concat(result.itemFields, newChildData);
                });
            }

            if (dataSource.children && dataSource.children.length) {
                result.children = [];
                _.forEach(dataSource.children, function(child) {
                    result.children.push($.extend(true, { name: child.aliasName }, createDataContextValueFromDataSource(child)));
                })
            }
        }
        return result;
    }

    Kooboo.EventBus.subscribe("DataStore/change", function() {
        DataContext._global = null;
    })

    Kooboo.viewEditor.DataContext = DataContext;
})();