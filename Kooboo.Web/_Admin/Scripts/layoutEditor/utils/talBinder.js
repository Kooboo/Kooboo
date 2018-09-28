(function() {

    var tal2attr = Kooboo.layoutEditor.utils.tal2attr;

    var talBinder = {
        label: function(elem, data) {
            $(elem).attr(tal2attr['label'], data.text);
        },
        unlabel: function(elem) {
            $(elem).removeAttr(tal2attr['label']);
        },
        data: function(elem, data) {
            $(elem).attr(tal2attr['data'], data.text);
        },
        undata: function(elem) {
            $(elem).removeAttr(tal2attr['data']);
        },
        repeat: function(elem, data) {
            $(elem).attr(tal2attr['repeat'], data.text + 'Item' + ' ViewBag.' + data.text);
        },
        unrepeat: function(elem) {
            $(elem).removeAttr(tal2attr['repeat']);
        },
        link: function(elem, data) {
            var href = data.href,
                params = data.params;
            if (params) {
                for (var prop in params) {
                    if (_.has(params, prop) && params[prop]) {
                        href = href.replace('{' + prop + '}', '{' + prop + ':' + params[prop] + '}');
                    }
                }
            }
            $(elem).attr(tal2attr['link'], href);
        },
        unlink: function(elem) {
            $(elem).removeAttr(tal2attr['link']);
        },
        form: function(elem, data) {
            var submitTo = data.submitTo,
                method = data.method,
                redirect = data.redirect,
                callback = data.callback;
            $(elem).attr(tal2attr['form'], [
                "submitTo:" + submitTo,
                "method:" + method,
                "redirect:" + redirect
            ].join(','));
        },
        unform: function(elem, data) {
            $(elem).removeAttr(tal2attr['form']);
        },
        bind: function(elem, data) {
            talBinder[data.type] && talBinder[data.type](elem, data);
        },
        unbind: function(elem, type) {
            talBinder['un' + type] && talBinder['un' + type](elem);
        }
    };

    Kooboo.layoutEditor.utils.talBinder = talBinder;
})();