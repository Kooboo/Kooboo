(function() {
    var k2attr = Kooboo.viewEditor.util.k2attr;

    var kBinder = {
        label: function(elem, data) {
            $(elem).attr(k2attr['label'], data.text);
        },
        unlabel: function(elem) {
            $(elem).removeAttr(k2attr['label']);
        },
        data: function(elem, data) {
            $(elem).attr(k2attr['data'], data.text);
        },
        undata: function(elem) {
            $(elem).removeAttr(k2attr['data']);
        },
        attribute: function(elem, data) {
            $(elem).attr(k2attr['attributes'], data.text);
        },
        unattribute: function(elem) {
            $(elem).removeAttr(k2attr['attributes']);
        },
        repeat: function(elem, data) {
            $(elem).attr(k2attr['repeat'], data.text.replace(/\./, '_') + '_Item' + ' ' + data.text);
            if (data.repeatSelf) {
                $(elem).attr('k-repeat-self', 'true');
            } else {
                $(elem).removeAttr('k-repeat-self');
            }
        },
        unrepeat: function(elem) {
            $(elem).removeAttr(k2attr['repeat']).removeAttr('k-repeat-self');
        },
        link: function(elem, data) {
            $(elem).attr(k2attr['link'], data.href);
        },
        unlink: function(elem) {
            $(elem).removeAttr(k2attr['link']);
        },
        form: function(elem, data) {
            $(elem).attr(k2attr['form'], data.formBindingId);
        },
        unform: function(elem, data) {
            $(elem).removeAttr(k2attr['form']);
        },
        input: function(elem, data) {
            $(elem).attr('name', data.text);
        },
        uninput: function(elem, data) {
            $(elem).removeAttr('name');
        },
        condition: function(elem, data) {
            $(elem).attr(k2attr['condition'], data.text);
        },
        uncondition: function(elem, data) {
            $(elem).removeAttr(k2attr['condition']);
        },
        bind: function(elem, data) {
            kBinder[data.bindingType](elem, data);
        },
        unbind: function(elem, bindingType) {
            kBinder['un' + bindingType](elem);
        }
    };
    Kooboo.viewEditor.util.kBinder = kBinder;
})();