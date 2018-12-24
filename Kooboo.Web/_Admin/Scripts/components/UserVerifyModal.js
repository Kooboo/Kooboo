(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/UserVerifyModal.html");

    ko.components.register('user-verify-modal', {
        viewModel: function(params) {
            var self = this;

            this.showError = ko.observable(false);

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    self.email(params.email());
                }
            })

            this.onHide = function() {
                self.showError(false);
                self.email('');
                self.isShow(false);
            }

            this.email = ko.validateField({
                required: '',
                regex: {
                    pattern: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
                    message: Kooboo.text.validation.emailInvalid
                }
            })

            this.onSubmit = function() {
                if (self.email.isValid()) {
                    Kooboo.User.verifyEmail({
                        email: self.email()
                    }).then(function(res) {
                        if (res.success) {
                            alert(Kooboo.text.alert.verificationEmailSent);
                            self.onHide();
                        }
                    })
                } else {
                    self.showError(true);
                }
            }

        },
        template: template
    })
})()