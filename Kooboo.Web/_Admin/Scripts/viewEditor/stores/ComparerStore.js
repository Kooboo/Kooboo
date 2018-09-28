(function() {
    var compares = {};
    Kooboo.viewEditor.store.ComparerStore = {
        init: function(cfg) {
            compares = cfg;
        },
        all: function() {
            return compares;
        },
        getByType: function(type) {
            return compares[type] || [];
        }
    }
})();