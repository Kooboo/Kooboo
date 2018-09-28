(function() {
    ko.bindingHandlers.datetimepicker = {
        init: function(element, valueAccessor, allBindings) {
            var options = {
                autoclose: true,
                format: 'yyyy-mm-dd hh:ii',
                minuteStep: 5
            };

            if (allBindings.has('datetimepickerOptions')) {
                $.extend(true, options, ko.toJS(allBindings.get('datetimepickerOptions')));
            }

            var initialDate = ko.utils.unwrapObservable(valueAccessor()) || null;

            $(element).data('value', initialDate).val(initialDate);
            // $(element).datetimepicker(options);
            $(element).datetimepicker(options)
                .on('changeDate', function(e) {
                    // Seems datetimepicker assumes we are choosing utc dates :(
                    // So e.date equals to toLocalDate(selected date)
                    var selectedDate = moment(e.date).utc().format("YYYY-MM-DD HH:mm");
                    valueAccessor()(selectedDate);
                });
        },
        update: function(element, valueAccessor, allBindings) {
            var options = {
                autoclose: true,
                format: 'yyyy-mm-dd hh:ii',
                minuteStep: 5
            };

            if (allBindings.has('datetimepickerOptions')) {
                $.extend(true, options, ko.toJS(allBindings.get('datetimepickerOptions')));
            }

            var formatedDate = ko.utils.unwrapObservable(valueAccessor()) || null;
            if (formatedDate) {
                if (formatedDate.getFullYear) {
                    formatedDate = moment(date).format(options.format);
                }
            }

            $(element).data('value', formatedDate).val(formatedDate);
            $(element).datetimepicker('update');
        }
    };

    return ko;
})();