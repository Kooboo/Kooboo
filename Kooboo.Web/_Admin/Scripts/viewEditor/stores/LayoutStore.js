(function() {
    var _layouts = [];

    var LayoutStore = {
        init: function(data) {
            _layouts = data;
            Kooboo.EventBus.publish("LayoutStore/change");
        },
        getAll: function() {
            return _layouts;
        }
    };

    Kooboo.viewEditor.store.LayoutStore = LayoutStore;
})();