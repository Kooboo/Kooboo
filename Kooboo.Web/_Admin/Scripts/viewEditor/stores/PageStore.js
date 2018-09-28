(function() {
    var _pages = [];
    var _pattern = /\{([^\}]+)\}/g;
    var PageStore = {
        init: function(data) {
            _pages = data;
            Kooboo.EventBus.publish("PageStore/change");
            return this;
        },
        getAll: function() {
            return _pages;
        },
        getByName: function(name) {
            return _.find(_pages, function(p) {
                return p.name === name;
            });
        },
        getByRoute: function(route) {
            return _.find(_pages, function(p) {
                return p.route === route;
            });
        },
        match: function(route) {
            for (var i = 0, page; page = _pages[i]; i++) {
                var params = [],
                    j1 = 0,
                    j2 = 0,
                    success = true;
                var pageRoute = page.route;
                if (!pageRoute) {
                    continue;
                }
                while (j1 < pageRoute.length && j2 < route.length) {
                    var ch1 = pageRoute[j1];
                    var ch2 = route[j2];
                    if (ch1 === '{' && ch2 === '{') {
                        j1++;
                        j2++;
                        var param1 = pageRoute.substr(j1, pageRoute.indexOf('}', j1) - j1);
                        var param2 = route.substr(j2, route.indexOf('}', j2) - j2);
                        params.push({
                            name: param1,
                            value: param2
                        });
                        j1 += param1.length + 1;
                        j2 += param2.length + 1;
                    } else if (ch1.toLowerCase() === ch2.toLowerCase()) {
                        j1++;
                        j2++;
                    } else {
                        success = false;
                        break;
                    }
                }
                if (success) {
                    return {
                        page: page,
                        params: params
                    };
                }
            }
            return null;
        },
        getParameters: function(route) {
            var params = [],
                match;
            while (match = _pattern.exec(route)) {
                params.push({
                    name: match[1],
                    value: null
                });
            }
            _pattern.lastIndex = 0;
            return params;
        },
        replaceParameters: function(route, params) {
            for (var name in params) {
                route = route.replace('{' + name + '}', params[name]);
            }
            return route;
        }
    };
    Kooboo.viewEditor.store.PageStore = PageStore;
})();