$( /* formID */ ).find('input, textarea').not('[type=submit]').jqBootstrapValidation({
    submitSuccess: function($form, event) {
        /* success callback */
    },
    submitError: function($form, event, errors) {
        /* failed callback */
    }
})