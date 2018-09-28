$(function() {
    var blockViewModel = function() {

        var self = this;

        Kooboo.EventBus.subscribe("ko/style/list/pickimage/show", function(ctx) {

            Kooboo.Media.getList().then(function(res) {

                if (res.success) {
                    res.model["show"] = true;
                    res.model["context"] = ctx;
                    res.model["onAdd"] = function(selected) {
                        ctx.settings.file_browser_callback(ctx.field_name, selected.url + "?SiteId=" + Kooboo.getQueryString("SiteId"), ctx.type, ctx.win, true);
                    }
                    self.mediaDialogData(res.model);
                }
            });
        });

        this.mediaDialogData = ko.observable();

        this._lang = ko.observable(Kooboo.getQueryString("lang"));

        this.showError = ko.observable(true);

        this.name = ko.observable();

        this.contents = ko.observableArray();

        this.contentsValue = ko.observable();

        this.blockId = ko.observable(Kooboo.getQueryString("id"));

        this.isNewBlock = ko.observable(false);

        this.multiLangs = ko.observable();

        this.cancelUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.HtmlBlock.MultiLangListPage, {
                lang: self._lang()
            })
        });

        this.onSubmit = function() {
            Kooboo.HtmlBlock.post({
                id: self.blockId(),
                name: self.name(),
                values: JSON.stringify(self.getMultiConents())
            }).then(function(res) {

                if (res.success) {
                    location.href = Kooboo.Route.Get(Kooboo.Route.HtmlBlock.MultiLangListPage, {
                        lang: self._lang()
                    });
                }
            })
        }

        this.getMultiConents = function() {
            var _values = {};
            _.forEach(self.contents(), function(c) {
                _values[c.abbr] = c.value();
            });

            return _values;
        }

        $.when(Kooboo.HtmlBlock.Get({
            Id: self.blockId()
        }), Kooboo.Site.Langs()).then(function(hbRes, langRes) {

            var r1 = hbRes[0],
                r2 = langRes[0];

            if (r1.success && r2.success) {
                self.name(r1.model.name);
                self.multiLangs(r2.model);

                var values = r1.model.values || {},
                    cultures = Object.keys(r2.model.cultures),
                    defaultCultureIdx = cultures.indexOf(r2.model.default);

                if (defaultCultureIdx > -1) {
                    var defaultCulture = cultures[defaultCultureIdx];
                    self.contents.push({
                        show: ko.observable(true),
                        value: ko.observable(values[defaultCulture]),
                        abbr: defaultCulture
                    })
                    cultures.splice(defaultCultureIdx, 1);
                }

                cultures.forEach(function(c) {
                    self.contents.push({
                        show: ko.observable(c == self._lang()),
                        value: ko.observable(values[c] || ''),
                        abbr: c
                    })
                })
            }
        })
    };

    var vm = new blockViewModel();
    ko.applyBindings(vm, document.getElementById("main"));
})