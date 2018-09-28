(function() {
    var _data = {},
        _types = [],
        _tags = [];

    Kooboo.pageEditor.store.ComponentStore = {
        init: function(data) {
            _data = data;
        },
        setTypes: function(type) {
            _types = type;
        },
        getTypes: function() {
            return _types;
        },
        getAll: function() {
            return _data;
        },
        addComponent: function(comp) {

            if (!_data[comp.type]) {
                _data[comp.type] = {};
            }

            _data[comp.type][comp.name] = comp;

            this.onChange();
        },
        getComponent: function(type, name) {
            return _data[type][name];
        },
        hasComponent: function(type, name) {
            return !!(_data[type] && _data[type][name]);
        },
        getMetaBindings: function () {
            var metaBindings = [];
            _.forEach(Object.keys(_data), function(key) {
                var contents = _data[key];
                _.forEach(Object.keys(contents), function(k) {
                    metaBindings = _.concat(metaBindings, contents[k].metaBindings);
                })
            })
            return _.uniq(metaBindings);
        },
        getUrlParamsBindings: function() {
            var _bindings = [];
            _.forEach(Object.keys(_data), function(key) {
                var contents = _data[key];
                _.forEach(Object.keys(contents), function(k) {
                    _bindings = _.concat(_bindings, contents[k].urlParamsBindings);
                })
            });
            return _.uniq(_bindings);
        },
        onChange: function () {
            Kooboo.EventBus.publish("kb/page/ComponentStore/change");
        }
    }
})()