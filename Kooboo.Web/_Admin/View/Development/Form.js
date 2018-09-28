$(function() {

    var formViewModel = function() {

        var self = this;

        this.isNewForm = ko.observable(false);
        this.formId = ko.observable("");
        this.formId.subscribe(function(id) {
            self.isNewForm(Kooboo.Guid.Empty == id);
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
                url: Kooboo.Form.isUniqueName(),
                data: {
                    name: function() {
                        return self.name()
                    }
                },
                message: Kooboo.text.validation.taken
            }
        });

        this.formContent = ko.observable("");
        this.compareTarget = ko.observable("");

        this.isEmbedded = ko.observable();

        this.formatCode = function() {
            var formatted = html_beautify(self.formContent());
            self.formContent(formatted);
        }

        this.showError = ko.observable(false);

        this.isValid = function() {
            return self.name.isValid();
        }

        this.onSubmitStyle = function(callback) {
            if ((self.isNewForm() && self.isValid()) || !self.isNewForm()) {
                self.showError(false);
                Kooboo.Form.post({
                    id: self.formId(),
                    body: self.formContent(),
                    name: self.isEmbedded() ? "" : self.name(),
                    isEmbedded: self.isEmbedded()
                }).then(function(res) {

                    if (res.success) {

                        if (typeof callback == "function") {
                            callback(res.model);
                        }
                    } else {
                        window.info.show(Kooboo.text.info.save.fail, false);
                    }
                });

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
                if (self.isNewForm()) {
                    location.href = Kooboo.Route.Get(Kooboo.Route.Form.DetailPage, {
                        Id: id
                    })
                } else {
                    window.info.show(Kooboo.text.info.save.success, true);
                    self.compareTarget(self.formContent());
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
            return self.formContent() !== self.compareTarget();
        }

        this.goBack = function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.Form.ListPage) + (self.isEmbedded() ? '#Embedded' : '');
        }

        Kooboo.Form.GetEdit({
            Id: Kooboo.getQueryString("Id") || Kooboo.Guid.Empty
        }).then(function(res) {

            if (res.success) {
                self.name(res.model.name);
                self.formContent(res.model.body || "");
                self.compareTarget(self.formContent());
                self.isEmbedded(res.model.isEmbedded);
            }
        })
    };

    var vm = new formViewModel();

    vm.formId(Kooboo.getQueryString("Id") || Kooboo.Guid.Empty);

    ko.applyBindings(vm, document.getElementById("main"));
});