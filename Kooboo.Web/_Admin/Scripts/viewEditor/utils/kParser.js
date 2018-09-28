(function() {

    var k2attr = Kooboo.viewEditor.util.k2attr,
        ActionStore = Kooboo.viewEditor.store.ActionStore,
        PageStore = Kooboo.viewEditor.store.PageStore,
        FormBindingStore = Kooboo.viewEditor.store.FormBindingStore;

    var repeatRe = /\S+\s+(\S+)\s?/;
    var kParser = {
        repeat: function(elem) {
            var str = $(elem).attr(k2attr['repeat']) || '';
            if (str) {
                var match = str.match(repeatRe);
                return match ? match[1] : null;
            }
            return null;
        },
        link: function(elem) {
            var str = $(elem).attr(k2attr['link']);
            if (str) {
                var match = PageStore.match(str);
                if (match) {
                    return {
                        href: str,
                        page: match.page,
                        params: match.params
                    };
                }
                return {
                    href: str,
                    params: []
                };
            }
            return null;
        },
        label: function(elem) {
            return $(elem).attr(k2attr['label']);
        },
        data: function(elem) {
            var text = $(elem).attr(k2attr['data']);
            return text ? { text: text } : null;
        },
        attribute: function(elem) {
            var text = $(elem).attr(k2attr['attributes']);
            return text ? { text: text } : null;
        },
        form: function(elem) {
            var bindingId = $(elem).attr(k2attr['form']);
            if (bindingId) {
                var binding = FormBindingStore.byId(bindingId);
                if (binding) {
                    return {
                        formBindingId: binding.id,
                        dataSourceMethodId: binding.dataSourceMethodId,
                        method: binding.method,
                        redirect: binding.redirect,
                        callback: binding.callback
                    };
                }
            }
            return null;
        },
        input: function(elem) {
            var $elem = $(elem);
            if ((!$elem.is(':text') && !$elem.is('select')) ||
                ($elem.is(":hidden")) ||
                ($elem.attr('name'))) {
                return null;
            }
            var $form = $elem.closest('[' + k2attr['form'] + ']');
            return $form.length === 0 ? null : $elem.attr('name');
        },
        condition: function(elem) {
            var text = $(elem).attr(k2attr['condition']);
            return text ? { text: text } : null;
        },
        parse: function(elem) {
            return {
                label: kParser.label(elem),
                link: kParser.link(elem),
                repeat: kParser.repeat(elem),
                data: kParser.data(elem),
                attribute: kParser.attribute(elem),
                form: kParser.form(elem),
                input: kParser.input(elem),
                condition: kParser.condition(elem)
            };
        }
    };
    Kooboo.viewEditor.util.kParser = kParser;
})();