$(function() {
    var scriptViewModel = function() {

        var self = this;

        this.isNewCode = ko.observable(false);
        this.codeId = ko.observable("");
        this.codeId.subscribe(function(id) {
            self.isNewCode(id == Kooboo.Guid.Empty);
        })

        this.availableCodeType = ko.observableArray();
        this.codeType = ko.validateField({
            required: ''
        });
        this.availableEventType = ko.observableArray();
        this.eventType = ko.validateField({
            required: ''
        });

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
                url: Kooboo.Code.isUniqueName(),
                type: "GET",
                data: {
                    name: function() {
                        return self.name()
                    }
                },
                message: Kooboo.text.validation.taken
            }
        });
        this.url = ko.observable();

        this.codeContent = ko.observable("");
        this.compareTarget = ko.observable("");

        this.configContent = ko.observable("");

        this.formatCode = function() {
            var formatted = "";
            if (self.curType() == 'code') {
                formatted = js_beautify(self.codeContent());
                self.codeContent(formatted);
            } else {
                formatted = js_beautify(self.configContent());
                self.configContent(formatted);
            }
        }

        this.curType = ko.observable('code');
        this.curType.subscribe(function() {
            setTimeout(function() {
                $('.CodeMirror').each(function(idx, el) {
                    var cm = el.CodeMirror;
                    cm.refresh()
                })
            }, 10);
        })

        this.changeType = function(type) {
            if (type !== self.curType()) {
                self.curType(type);
            }
        }

        this.showError = ko.observable(false);

        this.isValid = function() {

            if (!self.isNewCode()) {
                return true;
            } else {
                if (self.codeType() == 'event') {
                    return self.name.isValid() && self.codeType.isValid() && self.eventType.isValid();
                } else if (self.codeType() == 'api') {
                    return self.name.isValid() && self.codeType.isValid();
                } else {
                    return self.name.isValid() && self.codeType.isValid();
                }
            }
        }

        this.onSubmitCode = function(callback) {

            if (self.isValid()) {
                self.showError(false);

                var data = {
                    Id: self.isNewCode() ? Kooboo.Guid.Empty : self.codeId(),
                    name: self.name(),
                    body: self.codeContent(),
                    config: self.configContent(),
                    codeType: self.codeType(),
                    eventType: self.eventType(),
                    url: self.codeType().toLowerCase() == 'api' ? self.url() : ''
                };

                Kooboo.Code.post(data).then(function(res) {

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
            self.onSubmitCode(function() {
                self.goBack();
            });
        }

        this.onSave = function() {
            self.onSubmitCode(function(id) {
                if (self.isNewCode()) {
                    location.href = Kooboo.Route.Get(Kooboo.Route.Code.EditPage, {
                        Id: id
                    });
                } else {
                    self.compareTarget(self.codeContent());
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
            return self.codeContent() !== self.compareTarget();
        }

        this.goBack = function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.Code.ListPage);
        }
    };

    var vm = new scriptViewModel();

    var codeId = Kooboo.getQueryString("Id") || Kooboo.Guid.Empty;

    vm.codeId(codeId);
    Kooboo.Code.getEdit({
        Id: codeId,
        codeType: Kooboo.getQueryString("codeType") || "all"
    }).then(function(res) {

        if (res.success) {
            vm.name(res.model.name);
            vm.codeContent(res.model.body || "");
            vm.compareTarget(vm.codeContent());
            vm.url(res.model.url);

            if (res.model.availableCodeType) {
                vm.availableCodeType(res.model.availableCodeType.map(function(item) {
                    return {
                        displayText: item,
                        value: item.toLowerCase()
                    }
                }));
            } else {
                vm.availableCodeType([]);
                vm.codeType(res.model.codeType);
            }

            if (res.model.availableCodeType) {
                vm.availableEventType(res.model.availableEventType.map(function(item) {
                    return {
                        displayText: item,
                        value: item
                    }
                }));

                var codeTypeParams = Kooboo.getQueryString('codeType'),
                    eventTypeParma = Kooboo.getQueryString('eventType');

                if (codeTypeParams) {
                    vm.codeType(codeTypeParams.toLowerCase());
                }
                if (eventTypeParma) {
                    vm.codeType('event');
                    vm.eventType(eventTypeParma);
                }

            } else {
                vm.availableEventType([]);
                vm.eventType(res.model.eventType);
            }

            vm.configContent(res.model.config || "");
        }
    })

    ko.applyBindings(vm, document.getElementById("main"));

    $(document).keydown(function(e) {
        if (e.keyCode == 83 && e.ctrlKey) {
            //Ctrl + S
            e.preventDefault();
            vm.onSave();
        }

        if (e.keyCode == 70 && e.altKey && e.shiftKey) {
            //Shift + Alt + F
            e.preventDefault();
            vm.formatCode();
        }
    })
});