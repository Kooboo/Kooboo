(function() {
    var _positions = [];

    var PositionStore = {
        getAll: function() {
            return _positions;
        },
        add: function(data) {
            _positions.push(data);
            this._onChange();
        },
        remove: function(id) {
            var pos = this.byId(id);
            if (pos) {
                _.remove(_positions, pos);
                this._onChange();
            }
        },
        update: function(id, name) {
            var pos = this.byId(id);
            if (pos) {
                pos.name = name;
                this._onChange();
            }
        },
        byId: function(id) {
            return _.find(_positions, function(it) {
                return it.id == id;
            });
        },
        byName: function(name) {
            return _.find(_positions, function(it) {
                return it.name == name;
            });
        },
        clear: function() {
            _positions = [];
            this._onChange();
        },
        _onChange: _.throttle(function() {
            Kooboo.EventBus.publish("PositionStore/change");
        }, 50)
    };

    if (Kooboo.layoutEditor) {
        Kooboo.layoutEditor.store.PositionStore = PositionStore;
    }

    if (Kooboo.pageEditor) {
        Kooboo.pageEditor.store.PositionStore = PositionStore;
    }
})();