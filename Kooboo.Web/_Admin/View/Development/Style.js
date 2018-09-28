$(function() {

    var styleViewModel = function() {

        var self = this;

        this.isNewStyle = ko.observable(false);
        this.styleId = ko.observable("");
        this.styleId.subscribe(function(id) {
            self.isNewStyle(Kooboo.Guid.Empty == id);
        })

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
                url: Kooboo.Style.isUniqueName(),
                data: {
                    name: function() {
                        return self.name()
                    }
                },
                message: Kooboo.text.validation.taken
            }
        });

        this.styleContent = ko.observable("");
        this.compareTarget = ko.observable("");

        this.sourceChange = ko.observable(false);

        this.formatCode = function() {
            var formatted = css_beautify(self.styleContent());
            self.styleContent(formatted);
        }

        this.showError = ko.observable(false);

        this.isValid = function() {
            return self.name.isValid();
        }

        this.supportExtensions = ko.observableArray()

        this.changeExtension = function(m) {
            self.extension(m.value);
        }

        this.extension = ko.observable('css');

        this.onSubmitStyle = function(callback) {
            if ((self.isNewStyle() && self.isValid()) || !self.isNewStyle()) {
                self.showError(false);

                if (self.sourceChange()) {
                    if (confirm(Kooboo.text.confirm.sourceCodeChanged)) {
                        submit();
                    }
                } else {
                    submit();
                }

                function submit() {
                    Kooboo.Style.Update({
                        id: self.isNewStyle() ? Kooboo.Guid.Empty : self.styleId(),
                        name: self.name(),
                        body: self.styleContent(),
                        extension: self.extension()
                    }).then(function(res) {
                        if (res.success) {
                            callback && typeof callback == "function" && callback(res.model);
                        } else {
                            window.info.show(Kooboo.text.info.save.fail, false);
                        }
                    });
                }
            } else {
                self.showError(true);
            }
        }

        this.onSaveAndReturn = function() {
            self.onSubmitStyle(function() {
                self.goBack();
            });
        }

        this.onSave = function() {
            self.onSubmitStyle(function(id) {
                if (self.isNewStyle()) {
                    location.href = Kooboo.Route.Get(Kooboo.Route.Style.DetailPage, {
                        Id: id
                    })
                } else {
                    window.info.show(Kooboo.text.info.save.success, true);
                    self.compareTarget(self.styleContent());
                }
            });
        }

        this.userCancel = function() {
            if (self.isContentChanged()) {
                if (confirm(Kooboo.text.confirm.beforeReturn)) {
                    self.goBack();
                }
            } else {
                self.goBack();
            }
        }

        this.isContentChanged = function() {
            return self.styleContent() !== self.compareTarget();
        }

        this.goBack = function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.Style.ListPage);
        }

        $.when(
            Kooboo.Style.GetEdit({
                id: Kooboo.getQueryString("Id") || Kooboo.Guid.Empty
            }), Kooboo.Style.getExtensions()
        ).then(function(r1, r2) {
            var styleRes = $.isArray(r1) ? r1[0] : r1,
                extensionRes = $.isArray(r2) ? r2[0] : r2;

            if (styleRes.success && extensionRes.success) {
                self.styleId(Kooboo.getQueryString("Id") || Kooboo.Guid.Empty);
                self.name(styleRes.model.displayName);
                self.styleContent(styleRes.model.body || "");
                self.compareTarget(self.styleContent());
                self.sourceChange(styleRes.model.sourceChange);

                self.supportExtensions(extensionRes.model.map(function(ext) {
                    return {
                        displayName: '.' + ext,
                        value: ext
                    }
                }));
                self.extension(styleRes.model.extension || extensionRes.model[0]);
            }
        })
    };

    var vm = new styleViewModel();

    ko.applyBindings(vm, document.getElementById("main"));
});