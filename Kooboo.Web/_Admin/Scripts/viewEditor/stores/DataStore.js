(function() {

    var _datas = [];

    var DataStore = {
        init: function(data) {
            var tempData = _.cloneDeep(data).reverse();
            tempData.forEach(function(td) {
                if (td.parentId !== Kooboo.Guid.Empty) {
                    var parent = _.find(tempData, function(d) {
                        return d.id == td.parentId;
                    })

                    if (parent) {
                        if (!_.isArray(parent.children)) {
                            parent.children = [];
                        }

                        parent.children.push(td);
                    }
                }
            })
            _datas = tempData.reverse();
            this.onChange();
            return this;
        },
        getAll: function() {
            return _datas;
        },
        byId: function(id) {
            return _.find(_datas, function(it) {
                return it.id == id;
            });
        },
        byAlias: function(alias) {
            return _.find(_datas, function(it) {
                return it.aliasName === alias;
            });
        },
        byMethodId: function(id) {
            return _.find(_datas, function(data) {
                return data.methodId === id;
            })
        },
        byParent: function(parentId) {
            parentId = parentId || Kooboo.Guid.Empty;
            return _.filter(_datas, { parentId: parentId });
        },
        getNestedByParent: function(parentId) {
            return _.filter(_.uniq(_datas), { parentId: parentId || Kooboo.Guid.Empty });
        },
        add: function(data) {
            if (!data.id) {
                data.id = +new Date;
            }
            _datas.push(data);
            if (data.parentId !== Kooboo.Guid.Empty) {
                var parent = _.findLast(_datas, { id: data.parentId }),
                    parentIdx = _.findIndex(_datas, { id: parent.id });
                if (!parent["children"]) {
                    parent["children"] = [];
                }
                parent["children"].push(data);
                _datas.splice(parentIdx, 1);
                _datas.splice(parentIdx, 0, parent);
            }
            this.onChange();
            return this;
        },
        updateParameterMappings: function(data) {
            var item = this.byId(data.id);
            item.parameterMappings = data.parameterMappings;
            this.onChange();
        },
        remove: function(ids) {
            var removed = [];
            _datas.forEach(function(data) {
                (ids.indexOf(data.id) > -1) && removed.push(data)
            });
            removed.reverse();
            removed.forEach(function(rm) {
                if (rm.parentId && rm.parentId !== Kooboo.Guid.Empty) {
                    var parentIdx = _.findIndex(_datas, function(d) {
                        return d.id == rm.parentId;
                    })

                    var childIdx = _.findIndex(_datas[parentIdx].children, function(c) {
                        return c.id = rm.id;
                    });

                    _datas[parentIdx].children.splice(childIdx, 1);
                } else {
                    var idx = _.findIndex(_datas, function(d) {
                        return d.id == rm.id;
                    })

                    _datas.splice(idx, 1);
                }
            })

            Kooboo.EventBus.publish("DataStore/removed", removed);
            this.onChange();
            return this;
        },
        onChange: function() {
            Kooboo.EventBus.publish("DataStore/change");
        }
    };
    Kooboo.viewEditor.store.DataStore = DataStore;
})();