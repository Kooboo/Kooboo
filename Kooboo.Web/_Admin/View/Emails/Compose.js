$(function() {

    var ALLOW_LEAVING = false;

    $("#component_container_header").css("z-index", "200000");
    var mailRegex = /(((("[^"]*")|([^";<>\s]*))\s*<\s*[a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~]+(\.[a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~]+)*@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})\s*>)|([a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~]+(\.[a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~]+)*@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})))(\s*;\s*(((("[^"]*")|([^";<>\s]*))\s*<\s*[a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~]+(\.[a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~]+)*@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})\s*>)|([a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~]+(\.[a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~]+)*@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})))?)*/;

    window.onbeforeunload = function() {
        var contentChanged = vm.isContentChanged();
        if (!contentChanged || (contentChanged && ALLOW_LEAVING)) {
            return undefined;
        } else {
            return false;
        }
    }

    var composeModel = function() {
        var self = this;

        this.contentLoaded = ko.observable(false);

        this.showError = ko.observable(false);

        this.addresses = ko.observableArray();

        this.address = ko.observable();

        this.defaultReceiver = ko.observableArray([]);
        this.defaultCC = ko.observableArray([]);
        this.defaultBCC = ko.observableArray([]);

        this.receiverAddresses = ko.observableArray();
        this.receiverAddressesValidate = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /ADDRESS/,
                message: Kooboo.text.validation.containInvalidEmail
            }
        })
        this._receiverAddresses = ko.observableArray();

        this.ccAddresses = ko.observableArray();
        this.ccAddressesValidate = ko.validateField({
            regex: {
                pattern: /ADDRESS/,
                message: Kooboo.text.validation.containInvalidEmail
            }
        })
        this._ccAddresses = ko.observableArray();

        this.bccAddresses = ko.observableArray();
        this.bccAddressesValidate = ko.validateField({
            regex: {
                pattern: /ADDRESS/,
                message: Kooboo.text.validation.containInvalidEmail
            }
        })
        this._bccAddresses = ko.observableArray();

        this.subject = ko.validateField({
            required: Kooboo.text.validation.required,
        })
        this._subject = ko.observable(null);

        this.attachments = ko.observableArray();
        this._attachments = ko.observableArray();

        this.removeAttachment = function(m) {
            Kooboo.EmailAttachment.DeleteAttachment({
                filename: m.fileName
            }).then(function(res) {

                if (res.success) {
                    self.attachments.remove(m);
                }
            })
        }

        this.content = ko.observable("");
        this._content = ko.observable("");

        this.showCC = ko.observable(false);
        this.showBCC = ko.observable(false);

        this.addCC = function() {
            self.showCC(true);

            if (self.showError()) {
                self.showError(false);
                setTimeout(function() {
                    self.showError(true);
                }, 400);
            }
        }
        this.removeCC = function() {
            // self.ccAddresses.removeAll();
            self.showCC(false);
            // $("#cc_selector").trigger("change");

            if (self.showError()) {
                self.showError(false);
                setTimeout(function() {
                    self.showError(true);
                }, 400);
            }
        }

        this.addBCC = function() {
            self.showBCC(true);

            if (self.showError()) {
                self.showError(false);
                setTimeout(function() {
                    self.showError(true);
                }, 400);
            }
        }
        this.removeBCC = function() {
            // self.bccAddresses.removeAll();
            self.showBCC(false);
            // $("#bcc_selector").trigger("change");

            if (self.showError()) {
                self.showError(false);
                setTimeout(function() {
                    self.showError(true);
                }, 400);
            }
        }

        this.isValid = function() {

            var invalidreceiverAddress = _.find(self.receiverAddresses(), function(address) {
                return !mailRegex.test(address);
            })
            self.receiverAddressesValidate(invalidreceiverAddress ? "INVALID" : self.receiverAddresses().length ? "ADDRESS" : "");

            var invalidCCAddress = _.find(self.ccAddresses(), function(address) {
                return !mailRegex.test(address);
            })
            self.ccAddressesValidate(invalidCCAddress ? "INVALID" : "ADDRESS");

            var invalidBCCAddress = _.find(self.bccAddresses(), function(address) {
                return !mailRegex.test(address);
            })
            self.bccAddressesValidate(invalidBCCAddress ? "INVALID" : "ADDRESS");

            return self.receiverAddressesValidate.isValid() &&
                self.ccAddressesValidate.isValid() &&
                self.bccAddressesValidate.isValid() &&
                self.subject.isValid();
        }

        this.isAbleToSaveDraft = function() {

            self.receiverAddressesValidate("ADDRESS");
            _.forEach(self.receiverAddresses(), function(address) {
                if (!mailRegex.test(address)) {
                    self.receiverAddressesValidate("INVALID");
                }
            })

            self.ccAddressesValidate("ADDRESS");
            _.forEach(self.ccAddresses(), function(address) {
                if (!mailRegex.test(address)) {
                    self.ccAddressesValidate("INVALID");
                }
            })

            self.bccAddressesValidate("ADDRESS");
            _.forEach(self.bccAddresses(), function(address) {
                if (!mailRegex.test(address)) {
                    self.bccAddressesValidate("INVALID");
                }
            })

            return self.receiverAddressesValidate.isValid() &&
                self.ccAddressesValidate.isValid() &&
                self.bccAddressesValidate.isValid();
        }

        this.savingDraft = ko.observable(false);

        this.saveDraft = function() {

            self.savingDraft(true);

            if (self.isAbleToSaveDraft()) {
                Kooboo.EmailDraft.Save(JSON.stringify(self.getMailData())).then(function(res) {

                    if (res.success) {

                        if (!Kooboo.getQueryString("messageId")) {
                            self._subject(self.subject());
                            self._content(self.content());

                            ALLOW_LEAVING = true;
                            location.href = Kooboo.Route.Get(Kooboo.Route.Email.Compose, {
                                messageId: res.model,
                                type: "Drafts",
                                folder: "Draft"
                            })
                        } else {
                            window.info.show(Kooboo.text.info.save.success, true);
                            self.savingDraft(false);

                            self._receiverAddresses(self.receiverAddresses());
                            self._ccAddresses(self.ccAddresses());
                            self._bccAddresses(self.bccAddresses());
                            self._attachments(self.attachments());
                            self._subject(self.subject());
                            self._content(self.content());
                        }
                    } else {
                        window.info.show(Kooboo.text.info.save.fail, false);
                    }
                })
            } else {
                self.showError(true);
            }
        }

        this.send = function() {

            self.savingDraft(false);

            if (self.isValid()) {
                Kooboo.EmailMessage.Send(JSON.stringify(self.getMailData())).then(function(res) {

                    if (res.success) {
                        var address = _.findLast(self.addresses(), function(address) {
                            return self.address() == address.id;
                        })
                        ALLOW_LEAVING = true;
                        location.href = Kooboo.Route.Get(Kooboo.Route.Email.SentPage, {
                            address: address.address
                        })
                    }
                })
            } else {
                if (!self.showError()) {
                    self.showError(true);
                }
            }
        }

        this.getMailData = function() {
            var _data = {
                from: self.address(),
                to: self.receiverAddresses().length ? self.receiverAddresses() : [],
                cc: self.showCC() ? self.ccAddresses() : [],
                bcc: self.showBCC() ? self.bccAddresses() : [],
                subject: self.subject(),
                html: self.content(),
                attachments: self.attachments()
            }

            Kooboo.getQueryString("messageId") && (_data["messageId"] = Kooboo.getQueryString("messageId"));

            return _data;
        }

        this.isContentChanged = function() {
            return !_.isEqual(self.receiverAddresses(), self._receiverAddresses()) ||
                !_.isEqual(self.ccAddresses(), self._ccAddresses()) ||
                !_.isEqual(self.bccAddresses(), self._bccAddresses()) ||
                !_.isEqual(self.attachments(), self._attachments()) ||
                self.content().trim() !== self._content().trim() ||
                (self.subject() ? (self.subject() !== self._subject()) : (!!self.subject() !== !!self._subject()));
        }

        this.cancel = function() {
            if (this.isContentChanged()) {
                ALLOW_LEAVING = true;
                if (confirm(Kooboo.text.confirm.beforeReturn)) {
                    self.goBack();
                }
            } else {
                self.goBack();
            }
        }

        this.goBack = function() {
            var folder = Kooboo.getQueryString("folder");
            location.href = Kooboo.Route.Get(folder ? Kooboo.Route.Email[folder + "Page"] : Kooboo.Route.Email.InboxPage);
        }
    }

    var vm = new composeModel();

    ko.applyBindings(vm, document.getElementById("main"));

    Kooboo.EventBus.subscribe("kb/tinymce/initiated", function(editor) {
        vm._content(vm.content());
        editor.on('focus', function(e) {
            $('#to_selector').select2('close');
            vm.showCC() && $('#cc_selector').select2('close');
            vm.showBCC() && $('#bcc_selector').select2('close');
        })
    })

    Kooboo.EventBus.subscribe("ko/binding/select/close", function(ctx) {
        var possibleValue = ctx.parent().find('.select2-search__field').val();
        if (possibleValue) {
            if (possibleValue.indexOf(' ') == -1) {
                var origValues = ctx.val() || [];

                if (origValues.indexOf(possibleValue) == -1) {
                    origValues.push(possibleValue)
                    ctx.val(origValues).trigger('change');
                }
            }
        }
    })

    $.when(Kooboo.EmailAddress.getList(), Kooboo.EmailDraft.targetAddresses()).then(function(lres, tres) {
        var res1 = lres[0],
            res2 = tres[0];

        if (res1.success) {
            var addresses = res1.model.map(function(o) {
                return {
                    id: o.id,
                    address: o.address
                }
            });
            vm.addresses(addresses);

            if (addresses.length) {
                var _address = Kooboo.getQueryString("address");
                var _a = _.find(addresses, function(addr) {
                    return addr.address == _address;
                });
                vm.address(_a ? _a.id : addresses[0].id);
            } else {
                window.info.fail(Kooboo.text.mail.noAddressYet);
            }
        }

        if (res2.success) {
            res2.model.forEach(function(user) {
                vm.defaultReceiver.push(user.address);
                vm.defaultCC.push(user.address);
                vm.defaultBCC.push(user.address);
            })
        }

        var messageId = Kooboo.getQueryString("messageId"),
            sourceId = Kooboo.getQueryString("sourceId"),
            editId = Kooboo.getQueryString("id"),
            type = Kooboo.getQueryString("type");

        if (type) {
            switch (type) {
                case "Drafts":
                    Kooboo.EmailDraft.Compose({
                        messageId: messageId
                    }).then(function(res) {

                        if (res.success) {
                            var data = res.model;

                            vm.address(data.from);

                            _.forEach(data.to, function(address) {
                                var exist = _.findLast(vm.defaultReceiver(), function(add) {
                                    return add == address;
                                })

                                exist && vm.defaultReceiver.remove(exist);

                                vm.defaultReceiver.push(address);

                                vm.receiverAddresses.push(address);
                            })
                            vm._receiverAddresses(vm.receiverAddresses());

                            if (data.cc.length) {
                                vm.showCC(true);
                                _.forEach(data.cc, function(address) {
                                    var exist = _.findLast(vm.defaultCC(), function(add) {
                                        return add == address;
                                    })

                                    exist && vm.defaultCC.remove(exist);

                                    vm.defaultCC.push(address);

                                    vm.ccAddresses.push(address);
                                })
                                vm._ccAddresses(vm.ccAddresses());
                            }

                            if (data.bcc.length) {
                                vm.showBCC(true);
                                _.forEach(data.bcc, function(address) {
                                    var exist = _.findLast(vm.defaultBCC(), function(add) {
                                        return add == address;
                                    })

                                    exist && vm.defaultBCC.remove(exist);

                                    vm.defaultBCC.push(address);

                                    vm.bccAddresses.push(address);
                                })
                                vm._bccAddresses(vm.bccAddresses());
                            }

                            vm.subject(data.subject);
                            vm._subject(vm.subject());

                            vm.attachments(data.attachments);
                            vm._attachments(vm.attachments());

                            vm.content(data.html);
                            vm.contentLoaded(true);
                        }
                    });

                    break;
                case "Reply":
                    Kooboo.EmailMessage.getReplyContent({
                        sourceId: sourceId
                    }).then(function(res) {

                        if (res.success) {
                            var data = res.model;

                            vm.address(data.from);

                            _.forEach(data.to, function(address) {
                                var exist = _.findLast(vm.defaultReceiver(), function(add) {
                                    return add == address;
                                })

                                exist && vm.defaultReceiver.remove(exist);

                                vm.defaultReceiver.push(address);

                                vm.receiverAddresses.push(address);
                            });
                            vm._receiverAddresses(vm.receiverAddresses());

                            if (data.cc.length) {
                                vm.showCC(true);
                                _.forEach(data.cc, function(address) {
                                    var exist = _.findLast(vm.defaultCC(), function(add) {
                                        return add == address;
                                    })

                                    exist && vm.defaultCC.remove(exist);

                                    vm.defaultCC.push(address);

                                    vm.ccAddresses.push(address);
                                })
                                vm._ccAddresses(vm.ccAddresses());
                            }

                            if (data.bcc.length) {
                                vm.showBCC(true);
                                _.forEach(data.bcc, function(address) {
                                    var exist = _.findLast(vm.defaultBCC(), function(add) {
                                        return add == address;
                                    })

                                    exist && vm.defaultBCC.remove(exist);

                                    vm.defaultBCC.push(address);

                                    vm.bccAddresses.push(address);
                                })
                                vm._bccAddresses(vm.bccAddresses());
                            }

                            vm.subject(data.subject);
                            vm._subject(vm.subject());

                            vm.attachments(data.attachments);
                            vm._attachments(vm.attachments());

                            vm.content(data.html);
                            vm.contentLoaded(true);
                        }
                    })
                    break;
                case "Forward":
                    Kooboo.EmailMessage.getForwardContent({
                        sourceId: sourceId
                    }).then(function(res) {

                        if (res.success) {
                            var data = res.model;

                            vm.address(data.from);

                            _.forEach(data.to, function(address) {
                                var exist = _.findLast(vm.defaultReceiver(), function(add) {
                                    return add == address;
                                })

                                exist && vm.defaultReceiver.remove(exist);

                                vm.defaultReceiver.push(address);

                                vm.receiverAddresses.push(address);
                            })
                            vm._receiverAddresses(vm.receiverAddresses());

                            if (data.cc.length) {
                                vm.showCC(true);
                                _.forEach(data.cc, function(address) {
                                    var exist = _.findLast(vm.defaultCC(), function(add) {
                                        return add == address;
                                    })

                                    exist && vm.defaultCC.remove(exist);

                                    vm.defaultCC.push(address);

                                    vm.ccAddresses.push(address);
                                })
                                vm._ccAddresses(vm.ccAddresses());
                            }

                            if (data.bcc.length) {
                                vm.showBCC(true);
                                _.forEach(data.bcc, function(address) {
                                    var exist = _.findLast(vm.defaultBCC(), function(add) {
                                        return add == address;
                                    })

                                    exist && vm.defaultBCC.remove(exist);

                                    vm.defaultBCC.push(address);

                                    vm.bccAddresses.push(address);
                                })
                                vm._bccAddresses(vm.bccAddresses());
                            }

                            vm.subject(data.subject);
                            vm._subject(vm.subject());

                            vm.attachments(data.attachments);
                            vm._attachments(vm.attachments());

                            vm.content(data.html);
                            vm.contentLoaded(true);
                        }
                    })
                    break;
            }
        } else {
            Kooboo.EmailDraft.Compose().then(function(res) {
                if (res.success) {
                    vm.contentLoaded(true);
                }
            });
        }
    })

    $("#attachments").change(function() {

        function upload(file) {
            var data = new FormData();
            data.append("fileName", file.name);
            data.append("file", file);

            Kooboo.EmailAttachment.AttachmentPost(data).then(function(res) {

                if (res.success) {
                    vm.attachments.push(res.model);
                }
            })
        }

        var files = this.files;

        if (files[0]) {

            if (files[0]["size"]) {

                if (files[0].size / 1024 / 1024 <= 10) {
                    upload(files[0]);
                } else {
                    window.info.show(Kooboo.text.info.fileSizeLessThan + "10MB");
                }
            } else {
                upload(files[0]);
            }
        }

        $(this).val("");
    })
})