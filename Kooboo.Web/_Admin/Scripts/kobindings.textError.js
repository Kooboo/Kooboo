(function() {
    ko.bindingHandlers.textError = {
        init: function(element, valueAccessor, allBindingsAccessor, bindingContext) {
            ko.bindingHandlers.error.update(element, valueAccessor, allBindingsAccessor, bindingContext);
        },
        update: function(element, valueAccessor, allBindingsAccessor, bindingContext) {
            var isShow = bindingContext.showError(),
                value = valueAccessor(),
                error = value.error(),
                $el = $(element);

            if ($el.closest(".input-group").length > 0) {
                $el.closest(".input-group").removeClass('has-error');
            } else {
                $el.closest('.form-group').removeClass('has-error');
            }

            if (error && isShow && $el.is(':visible')) {

                var helper = $('<span>');
                $(helper).addClass('help-block kb-text-error').text(error);

                $el.parent().find('.kb-text-error').each(function(idx, el) {
                    el.remove();
                });

                $el.parent().append(helper);

                if ($el.closest(".input-group").length > 0) {
                    $el.closest(".input-group").addClass('has-error');
                } else {
                    $el.closest('.form-group').addClass('has-error');
                }

            } else {
                $('.kb-text-error', $el.parent()).remove();
            }
        }
    };
})()