(function() {

    var tal2attr = Kooboo.layoutEditor.utils.tal2attr;

    var linkParamRe = /{\s*(\S+?)\s*:\s*(\S+?)\s*}/i,
        repeatRe = /\S+\s+ViewBag.(\S+)\s?/;

    var talParser = {
        repeat: function(elem) {
            var str = $(elem).attr(tal2attr['repeat']) || '';
            if (str) {
                var match = str.match(repeatRe);
                return match ? match[1] : null;
            }
        },
        link: function(elem) {
            var str = $(elem).attr(tal2attr['link']);
            if (str) {
                var params = {},
                    m, key, val;

                while (m = str.match(linkParamRe)) {
                    key = m[1];
                    val = m[2];
                    params[key] = val;
                    str = str.replace(':' + val, '');
                }

                return {
                    href: str,
                    params: params
                };
            }
        },
        label: function(elem) {
            return $(elem).attr(tal2attr['label']) || null;
        },
        data: function(elem) {
            return $(elem).attr(tal2attr['data']) || null;
        },
        form: function(elem) {
            var str = $(elem).attr(tal2attr['form']) || null;
            if (str) {
                var args = str.split(','),
                    ret = {};
                _.forEach(args, function(it) {
                    var pair = it.split(':');
                    ret[pair[0]] = pair[1];
                });
                return ret;
            }
        },
        parse: function(elem) {
            return {
                label: talParser.label(elem),
                link: talParser.link(elem),
                repeat: talParser.repeat(elem),
                data: talParser.data(elem),
                form: talParser.form(elem)
            };
        }
    };

    Kooboo.layoutEditor.utils.talParser = talParser;
})();