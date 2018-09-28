$(function() {
    var blockViewModel = function() {

        var self = this;
        var blockId = Kooboo.getQueryString("id") || Kooboo.Guid.Empty;

        Kooboo.EventBus.subscribe("kb/multilang/change", function(target) {
            var content = _.findLast(self.contents(), function(c) {
                return c.abbr == target.name;
            });

            if (content) {
                content.show(target.selected);
            }
        });

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

        this.showError = ko.observable(false);

        this.name = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                message: Kooboo.text.validation.objectNameRegex
            },
            stringlength: {
                min: 1,
                max: 64,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
            },
            remote: {
                url: Kooboo.HtmlBlock.isUniqueName(),
                data: {
                    name: function() {
                        return self.name()
                    }
                },
                message: Kooboo.text.validation.taken
            }
        });

        this.isMultilingual = ko.observable(false);

        this.contents = ko.observableArray();

        this.blockId = ko.observable(blockId);

        this.isNewBlock = ko.observable(blockId == Kooboo.Guid.Empty);

        this.multiLangs = ko.observable();

        this.cancelUrl = Kooboo.Route.Get(Kooboo.Route.HtmlBlock.ListPage);

        this.isValid = function() {
            if (!self.name()) self.name("");
            return self.name.isValid();
        }

        this.onSubmit = function() {

            function submit() {
                self.showError(false);

                Kooboo.HtmlBlock.post({
                    id: self.blockId(),
                    name: self.name(),
                    values: JSON.stringify(self.getMultiConents())
                }).then(function(res) {

                    if (res.success) {
                        location.href = Kooboo.Route.Get(Kooboo.Route.HtmlBlock.ListPage);
                    }
                })
            }

            if (self.isNewBlock()) {
                if (self.isValid()) {
                    submit();
                } else {
                    self.showError(true);
                }
            } else {
                submit();
            }
        }

        this.getMultiConents = function() {
            var _values = {};
            self.contents().forEach(function(c) {
                _values[c.abbr] = c.value();
            });

            return _values;
        }


        $.when(Kooboo.HtmlBlock.Get({
            Id: blockId
        }), Kooboo.Site.Langs()).then(function(hbRes, langRes) {
            var r1 = hbRes[0],
                r2 = langRes[0];

            if (r1.success && r2.success) {
                self.multiLangs(r2.model);
                self.name(r1.model.name);

                self.isMultilingual(_.keys(r2.model.cultures).length > 1);

                var values = r1.model.values || {};

                var cultures = Object.keys(r2.model.cultures),
                    defaultCultureIdx = cultures.indexOf(r2.model.default);

                if (defaultCultureIdx > -1) {
                    var defaultCulture = cultures[defaultCultureIdx];
                    self.contents.push({
                        show: ko.observable(true),
                        value: ko.observable(values[defaultCulture] || ''),
                        abbr: defaultCulture
                    })
                    cultures.splice(defaultCultureIdx, 1);
                }

                cultures.forEach(function(c) {
                    self.contents.push({
                        show: ko.observable(false),
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