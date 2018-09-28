$(function() {
    var scriptViewModel = function() {

        var self = this;

        this.isNewScript = ko.observable(false);
        this.scriptId = ko.observable("");
        this.scriptId.subscribe(function(id) {
            self.isNewScript(id == Kooboo.Guid.Empty);
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
                url: Kooboo.KScript.isUniqueName(),
                type: "GET",
                data: {
                    name: function() {
                        return self.name()
                    }
                },
                message: Kooboo.text.validation.taken
            }
        });

        this.scriptContent = ko.observable("");
        this.compareTarget = ko.observable("");

        this.formatCode = function() {
            var formatted = js_beautify(self.scriptContent());
            self.scriptContent(formatted);
        }

        this.showError = ko.observable(false);

        this.isValid = function() {

            if (!self.isNewScript()) {
                return true;
            } else {
                return self.name.isValid();
            }
        }

        this.onSubmitScript = function(callback) {

            if (self.isValid()) {
                self.showError(false);

                Kooboo.KScript.Update(JSON.stringify({
                    Id: self.isNewScript() ? Kooboo.Guid.Empty : self.scriptId(),
                    name: self.name(),
                    body: self.scriptContent()
                })).then(function(res) {

                    if (res.success) {

                        if (typeof callback === "function") {
                            callback(res.model);
                        }
                    } else {
                        window.info.show(Kooboo.text.info.save.fail, false);
                    }
                })
            } else {
                self.showError(true);
            }

        }

        this.onSaveAndReturn = function() {
            self.onSubmitScript(function() {
                self.goBack();
            });
        }

        this.onSave = function() {
            self.onSubmitScript(function(id) {
                if (self.isNewScript()) {
                    location.href = Kooboo.Route.Get(Kooboo.Route.KScript.DetailPage, {
                        Id: id
                    });
                } else {
                    self.compareTarget(self.scriptContent());
                    window.info.show(Kooboo.text.info.save.success, true);
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
            return self.scriptContent() !== self.compareTarget();
        }

        this.goBack = function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.KScript.ListPage);
        }
    };

    var vm = new scriptViewModel();

    var scriptId = Kooboo.getQueryString("Id") || Kooboo.Guid.Empty;

    vm.scriptId(scriptId);
    Kooboo.KScript.Get({
        Id: scriptId
    }).then(function(res) {

        if (res.success) {
            vm.name(res.model.name);
            vm.scriptContent(res.model.body || "");
            vm.compareTarget(vm.scriptContent());
        }
    })

    ko.applyBindings(vm, document.getElementById("main"));
});