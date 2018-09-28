(function() {
    var bindings = [];
    var FormBindingStore = {
        init: function(data) {
            bindings = data;
        },
        all: function() {
            return bindings;
        },
        byId: function(id) {
            return _.find(bindings, function(binding) {
                return binding.id === id;
            });
        },
        save: function(binding) {
            FormBindingStore.remove(binding.id);
            bindings.push(binding);
        },
        remove: function(id) {
            _.remove(bindings, function(it) {
                return it.id === id;
            });
        }
    };
    Kooboo.viewEditor.store.FormBindingStore = FormBindingStore;
})();