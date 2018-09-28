(function() {
    ko.bindingHandlers.select2 = {
        init: function(element, valueAccessor, allBindingsAccessor) {
            var obj = valueAccessor(),
                allBindings = allBindingsAccessor();
            var onSelectChange = allBindings.onSelectChange || null;

            setTimeout(function() {
                var $select2 = $(element).select2(obj);
                if (onSelectChange) {
                    $select2.on('change', onSelectChange.bind(element))
                        .on('select2:closing', function() {
                            Kooboo.EventBus.publish('ko/binding/select/close', $select2)
                        });
                } else {
                    $select2.on('select2:closing', function() {
                        Kooboo.EventBus.publish('ko/binding/select/close', $select2)
                    });
                }
                $(element).trigger('change');
            }, 10);

            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                $(element).select2('destroy');
            });
        },
        update: function(element) {
            $(element).trigger('change');
        }
    };
})();