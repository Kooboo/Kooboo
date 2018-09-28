(function() {
    var _bindings = {};

    var BindingStore = {
        addOrUpdateBindingInfo: function(data) {
            if (data.bindingType !== "attribute") {
                if ($.isArray(data.dataSourceId)) {
                    data.dataSourceId.forEach(function(id) {
                        _AddOrUpdateInfo(id + '', data);
                    });
                } else {
                    _AddOrUpdateInfo(data.dataSourceId + '', data);
                }
            } else {
                data.ids.forEach(function(id) {
                    _AddOrUpdateInfo(id + '', data);
                });
            }

            function _AddOrUpdateInfo(id, data) {
                if (Object.keys(_bindings).indexOf(id) > -1) {
                    var exist = _.find(_bindings[id], function(binding) {
                        return binding.bindingType == data.bindingType &&
                            binding.elem == data.elem;
                    })

                    if (exist) {
                        var idx = _.findIndex(_bindings[id], function(binding) {
                            return _.isEqual(binding, exist);
                        })

                        _bindings[id].splice(idx, 1, data);
                    } else {
                        _bindings[id].push(data);
                    }
                } else {
                    _bindings[id] = [];
                    _bindings[id].push(data);
                }
            }
        },
        getRemoveBindingInfos: function(removedSourceId) {
            return _bindings[removedSourceId];
        },
        removeBindingInfo: function(info) {
            if (info.dataSourceId) {
                if ($.isArray(info.dataSourceId)) {
                    info.dataSourceId.forEach(function(id) {
                        deleteById(id, info);
                    })
                } else { deleteById(info.dataSourceId, info); }
            } else if (info.ids) {
                info.ids.forEach(function(id) {
                    deleteById(id, info);
                })
            }

            function deleteById(id, info) {
                var idx = _.findIndex(_bindings[id], function(binding) {
                    if (typeof info.text == "function") {
                        return binding.bindingType == info.type && binding.elem == info.elem;
                    } else {
                        return _.isEqual(binding, info);
                    }
                })
                _bindings[id].splice(idx, 1);
            }
        },
        removeAttributeBindings: function(ids) {
            ids.forEach(function(id) {
                Object.keys(_bindings).forEach(function(key) {
                    if (key == id) {
                        var len = _bindings[key].length;
                        for (var i = len - 1; i > 0; i--) {
                            if (_bindings[key][i][_bindings[key][i].hasOwnProperty("type") ? "type" : "bindingType"] == "attribute") {
                                _bindings[key].splice(i, 1);
                            }
                        }
                    }
                })
            })
        }
    }

    Kooboo.viewEditor.store.BindingStore = BindingStore;
})()