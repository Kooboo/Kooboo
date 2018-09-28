(function() {
    ko.bindingHandlers.editable = {
        _ignoreUpdate: false,
        init: function(element, valueAccessor, allBindings) {
            var options = {};
            if (allBindings.has('editableOptions')) {
                options = $.extend(true, options, ko.toJS(allBindings.get('editableOptions')));
            }
            $(element).editable(options)
                .on('save', function(e, params) {
                    ko.bindingHandlers.editable._ignoreUpdate = true;
                    valueAccessor()(params.newValue);
                    ko.bindingHandlers.editable._ignoreUpdate = false;
                });
        },
        update: function(element, valueAccessor) {
            if (ko.bindingHandlers.editable._ignoreUpdate) {
                return;
            }
            $(element).editable('setValue', ko.utils.unwrapObservable(valueAccessor()));
        }
    };
})();