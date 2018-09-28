(function() {

    var _bindings = [];

    var BindingStore = {
        getAll: function() {
            return _bindings;
        },
        add: function(data) {
            _bindings.push(data);
            this._onChange();
        },
        remove: function(id) {
            var item = this.byId(id);
            if (item) {
                _.remove(_bindings, item);
                this._onChange();
            }
        },
        update: function(id, data) {
            var item = this.byId(id);
            if (item) {
                item.text = data.text;
                this._onChange();
            }
        },
        replace: function(id, data, pos) {
            var item = this.byId(id);
            if (item) {
                _.remove(_bindings, item);
                var _headScripts = _.filter(_bindings, { head: true });
                _bindings.splice(data.head ? pos : pos + _headScripts.length, 0, data);
                this._onChange();
            }
        },
        byId: function(id) {
            return _.find(_bindings, function(it) {
                return it.id == id;
            });
        },
        byName: function(name) {
            return _.find(_bindings, function(it) {
                return it.name == name;
            });
        },
        byType: function(type) {
            return _.filter(_bindings, function(it) {
                return it.type == type;
            })
        },
        clear: function() {
            _bindings = [];
            this._onChange();
        },
        _onChange: _.throttle(function() {
            Kooboo.EventBus.publish("BindingStore/change");
        }, 50)
    };

    if (Kooboo.layoutEditor) {
        Kooboo.layoutEditor.store.BindingStore = BindingStore;
    }

    if (Kooboo.pageEditor) {
        Kooboo.pageEditor.store.BindingStore = BindingStore;
    }
})();