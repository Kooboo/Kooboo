$(function() {
    ko.bindingHandlers.typeahead = {
        init: function(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            ko.bindingHandlers.typeahead.update(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
        },
        update: function(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var $element = $(element);
            var allBindings = allBindingsAccessor();
            //var value = ko.utils.unwrapObservable(allBindings.value);
            var source = ko.utils.unwrapObservable(valueAccessor());
            var items = ko.utils.unwrapObservable(allBindings.items) || 4;
            var showHintOnFocus = ko.utils.unwrapObservable(allBindings.defaultShow) && true;

            var valueChange = function(item) {
                return item;
            };

            var highlighter = function(item) {
                var matchSpan = '<span style="color: blue;font-weight:bold">';
                var query = this.query.replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, '\\$&');
                return item.replace(new RegExp('(' + query + ')', 'ig'), function($1, match) {
                    return matchSpan + match + '</span>';
                });
            };

            var options = {
                source: source,
                items: items,
                updater: valueChange,
                minLength: 0,
                showHintOnFocus: showHintOnFocus,
                afterSelect: function() {
                    this.blur();
                    this.hide();
                }
            };

            if (source && source.length) {
                $element
                    .attr('autocomplete', 'off')
                    .typeahead(options);
            }
        }
    };

    return ko;
});