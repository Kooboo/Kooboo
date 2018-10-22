$(function() {

    var profileModel = function() {
        var self = this;

        this.tabs = ko.observableArray();

        var adminTabs = [{
                displayName: Kooboo.text.site.profile.Account,
                value: "Account"
            }, {
                displayName: Kooboo.text.site.profile.Password,
                value: "Password"
            }, {
                displayName: Kooboo.text.site.profile.Organization,
                value: "Organization"
            }, {
                displayName: Kooboo.text.site.profile.Balance,
                value: "Balance"
            }, {
                displayName: Kooboo.text.site.profile.Users,
                value: "Users"
            }, {
                displayName: Kooboo.text.site.profile.DataCenter,
                value: "DataCenter"
            }],
            noneAdminTabs = [{
                displayName: Kooboo.text.site.profile.Account,
                value: "Account"
            }, {
                displayName: Kooboo.text.site.profile.Password,
                value: "Password"
            }, {
                displayName: Kooboo.text.site.profile.Organization,
                value: "Organization"
            }];

        this.curType = ko.observable();

        this.isAdmin = ko.observable(false);

        this.changeType = function(type) {
            if (self.curType() !== type) {
                self.curType(type);
                self.showError(false);
                switch (type) {
                    case "Account":
                        if (!self.isUserTabInit()) {
                            Kooboo.User.get().then(function(res) {
                                if (res.success) {
                                    self.username(res.model.userName);
                                    self.email(res.model.emailAddress);
                                    self.language(res.model.language);
                                    self._language(self.language());

                                    self.isUserTabInit(true);
                                    self.curType(type);

                                    self.isAdmin(res.model.isAdmin);
                                    self.tabs(res.model.isAdmin ? adminTabs : noneAdminTabs);
                                }
                            })
                        } else {
                            self.curType(type);
                        }
                        break;
                    case "Password":
                        self.curType(type);
                        break;
                    case "Organization":
                        if (!self.isOrganizationInit()) {
                            Kooboo.Organization.getOrganizations().then(function(res) {

                                if (res.success) {
                                    self.organizationOptions(res.model);

                                    Kooboo.Organization.getOrg().then(function(r) {

                                        if (r.success) {
                                            self.organizationName(r.model.name);
                                            self.organization(r.model.id);
                                            self.organizationId(r.model.id);
                                            self.organizationBalance(r.model.balance);
                                            self.curType(type);
                                            self.isOrganizationInit(true);
                                        }
                                    })
                                }
                            })
                        } else {
                            self.curType(type);
                        }
                        break;
                    case "Balance":
                        Kooboo.Organization.getOrg().then(function(res) {
                            if (res.success) {
                                self.organizationName(res.model.name);
                                self.organizationBalance(res.model.balance);
                                self.organization(res.model.id);
                                self.organizationId(res.model.id);
                                self.curType(type);
                            }
                        })
                        break;
                    case "Users":
                        if (!self.isUserInit()) {
                            Kooboo.Organization.getOrg().then(function(res) {
                                if (res.success) {
                                    self.organizationName(res.model.name);
                                    self.organizationBalance(res.model.balance);
                                    self.organization(res.model.id);
                                    self.organizationId(res.model.id);
                                    self.newUser("");

                                    Kooboo.Organization.getUsers({
                                        organizationId: self.organizationId()
                                    }).then(function(r) {
                                        if (r.success) {
                                            var users = r.model.map(function(o) {
                                                return o.userName;
                                            })
                                            self.organizationUsers(users);
                                        }
                                    })
                                }
                            })
                        } else {
                            self.curType(type);
                        }
                        break;
                    case 'DataCenter':
                        Kooboo.Organization.getDataCenter().then(function(res) {
                            if (res.success) {
                                self.dataCenterOrganizationName(res.model.organizationName);
                                self.dataCenterCurrent(res.model.currentDataCenter);
                                self.avaliableDataCenter(Kooboo.objToArr(res.model.availableDataCenters, 'value', 'name'));
                            }
                        })
                        break;
                }
            }
        }

        this.dataCenterOrganizationName = ko.observable();
        this.dataCenterCurrent = ko.observable();
        this.avaliableDataCenter = ko.observableArray();
        this.showUpdateDataCenterBtn = ko.observable(false);
        this.newDataCenter = ko.observable();
        this.newDataCenter.subscribe(function(changed) {
            self.showUpdateDataCenterBtn(!!changed);
        })

        this.updateDataCenter = function() {
            if (confirm(Kooboo.text.confirm.changeDataCenter)) {
                Kooboo.Organization.updateDataCenter({
                    datacenter: self.newDataCenter()
                }).then(function(res) {
                    if (res.success) {
                        window.info.done(Kooboo.text.info.update.success);
                        window.location.href = res.model.redirectUrl || Kooboo.Route.User.LoginPage;
                    }
                })
            }
        }

        this.languageOptions = ko.observableArray();

        this.showUpdateSuccess = function(success) {
            window.info.show(Kooboo.text.info.update[success ? "success" : "fail"], success);
        }

        // User
        this.isUserTabInit = ko.observable(false);

        this.username = ko.observable();

        this.email = ko.observable();

        this.newEmail = ko.validateField({
            required: '',
            regex: {
                pattern: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
                message: Kooboo.text.validation.emailInvalid
            }
        });

        this.language = ko.observable();

        this._language = ko.observable();

        this.isUserProfileValid = function() {
            return this.newEmail.isValid();
        }

        this.saveUser = function() {
            var self = this,
                data = {
                    username: this.username(),
                    language: this.language(),
                };

            if (!this.email()) {
                if (this.isUserProfileValid()) {
                    data.emailAddress = this.newEmail();
                    update(data);
                } else {
                    this.showError(true);
                }
            } else {
                data.emailAddress = this.email();
                update(data);
            }

            function update(data) {
                Kooboo.User.updateProfile(data).then(function(res) {
                    self.showUpdateSuccess(res.success);
                    if (res.success) {
                        if (self._language() !== self.language()) {
                            setTimeout(function() {
                                location.reload();
                            }, 300);
                        } else {
                            self.email(self.newEmail());
                        }
                    } else {
                        self.language(self._language());
                    }
                })
            }
        }

        // Password
        this.showError = ko.observable(false);

        this.oldPassword = ko.validateField({
            required: Kooboo.text.validation.required
        });

        this.newPassword = ko.validateField({
            required: Kooboo.text.validation.required
        });
        self.newPassword.subscribe(function() {
            self.confirmPassword.valueHasMutated();
        })

        this.confirmPassword = ko.validateField({
            required: Kooboo.text.validation.required,
            equals: {
                value: function() {
                    return self.newPassword()
                },
                message: Kooboo.text.validation.notEqual
            }
        })

        this.isPasswordValid = function() {
            return self.newPassword.isValid() &&
                self.oldPassword.isValid() &&
                self.confirmPassword.isValid();
        }

        this.savePassword = function() {
            if (self.isPasswordValid()) {
                self.showError(false);
                Kooboo.User.changePassword({
                    userName: self.username(),
                    oldPassword: self.oldPassword(),
                    newPassword: self.newPassword()
                }).then(function(res) {

                    self.oldPassword("");
                    self.newPassword("");
                    self.confirmPassword("");
                    self.showUpdateSuccess(res.success);
                })
            } else {
                self.showError(true);
            }
        }


        // Organization
        this.isOrganizationInit = ko.observable(false);

        this.organizationOptions = ko.observableArray();

        this.organization = ko.observable();
        this.organizationId = ko.observable();

        this.organizationName = ko.observable();

        this.saveOrganization = function() {
            Kooboo.Organization.changeUserOrg({
                organizationId: self.organization()
            }).then(function(res) {
                self.showUpdateSuccess(res.success);

                if (res.success) {
                    self.organizationName(res.model.currentOrgName);
                    localStorage.clear();
                    location.href = res.model.redirectUrl || Kooboo.Route.User.LoginPage;
                }
            })
        }

        // Balance
        this.isBalanceInit = ko.observable(false);

        this.organizationBalance = ko.observable();

        this.inputNumberOnly = function(m, e) {
            if (e.keyCode >= 48 && e.keyCode <= 57 /*number*/ ) {
                return true;
            } else if (e.keyCode >= 96 && e.keyCode <= 105 /*number*/ ) {
                return true;
            } else if (e.keyCode == 190 /*.*/ || e.keyCode == 69 /*e*/ || e.keyCode == 8 /*BACKSPACE*/ ) {
                return true;
            } else {
                return false;
            }
        }

        this.paymentModal = ko.observable(false);
        this.paypalModal = ko.observable(false);

        this.couponCode = ko.observable();

        this.startRecharge = function() {

            if (!self.couponCode()) {
                alert(Kooboo.text.alert.voucherCode);
            } else {
                Kooboo.Organization.useCoupon({
                    organizationId: self.organizationId(),
                    code: self.couponCode()
                }).then(function(res) {
                    if (res.success) {
                        self.organizationName(res.model.name);
                        self.organizationBalance(res.model.balance);
                        self.organization(res.model.id);
                        self.organizationId(res.model.id);
                        self.couponCode("");
                    } else {
                        self.couponCode("");
                        window.info.show(Kooboo.text.info.invalidCoupon, false);
                    }
                })
            }
        }

        this.paymentMethod = ko.observable("wechat");

        this.cardNumber = ko.validateField({
            required: Kooboo.text.validation.required
        });

        this.expMonth = ko.validateField({
            required: Kooboo.text.validation.required
        })
        this.expYear = ko.validateField({
            required: Kooboo.text.validation.required
        })

        this.cvv = ko.validateField({
            required: Kooboo.text.validation.required
        })

        this.recharge = ko.validateField({
            required: Kooboo.text.validation.required,
            min: {
                value: 0.01
            },
            max: {
                value: 9999999.99
            }
        });

        this.isRechargeValid = function() {
            return self.recharge.isValid();
        }

        this.isCreditCardValid = function() {
            return self.cardNumber.isValid() &&
                self.expMonth.isValid() &&
                self.expYear.isValid() &&
                self.cvv.isValid();
        }

        var interval = null,
            paymentSuccess = false;

        this.paymentId = ko.observable();

        this.paynow = function() {
            if (!self.isRechargeValid()) {
                self.showError(true);
            } else {
                self.showError(false);
                switch (self.paymentMethod()) {
                    case "wechat":
                    case "paypal":
                        Kooboo.Organization.payRecharge({
                            organizationId: self.organizationId(),
                            money: Number(self.recharge()),
                            paymentMethod: self.paymentMethod(),

                        }).then(function(res) {
                            if (res.success) {
                                if (self.paymentMethod() == "paypal") {
                                    // debugger;
                                    // self.paypalModal(true);
                                    // $("#paypalIframe").attr("src",res.model.approvalUrl);
                                    window.location.href = res.model.approvalUrl;
                                } else {
                                    if (res.model.success) {
                                        $("#qr-code").empty().qrcode(res.model.qrcode);
                                        self.paymentModal(true);
                                        self.paymentId(res.model.paymentId);

                                        interval = setInterval(function() {
                                            checkStatus(res.model.paymentId);
                                        }, 10000);
                                    } else {
                                        window.info.show(res.model.errorMessage, false);
                                        self.recharge("");
                                    }
                                }

                            }
                        })
                        break;
                    case "alipay":
                        break;
                    case "creditCard":
                        if (self.isCreditCardValid()) {
                            var args = {
                                sellerId: "203374964",
                                publishableKey: "9ACF7A79-61CD-4C1B-8A88-DC5B8888C375",
                                ccNo: self.cardNumber(),
                                cvv: self.cvv(),
                                expMonth: self.expMonth(),
                                expYear: self.expYear()
                            };
                            TCO.loadPubKey('production');
                            $(".page-loading").show();
                            TCO.requestToken(function(data) {
                                // success callback
                                $(".page-loading").hide();
                                Kooboo.Organization.payRecharge({
                                    organizationId: self.organizationId(),
                                    money: Number(self.recharge()),
                                    paymentMethod: self.paymentMethod(),
                                    token: data.response.token.token
                                }).then(function(res) {
                                    if (res.success) {

                                    }
                                })
                            }, function(err) {
                                // failure callback
                                $(".page-loading").hide();
                                alert(err.errorMsg);
                            }, args);
                        } else {
                            self.showError(true);
                        }
                        break;
                }
            }
        }

        this.successAttempt = 0;
        this.cancelPaypal = function() {
            self.paypalModal(false);
            //clearInterval(interval);
            window.info.show(Kooboo.text.info.payment.cancel, false);
            self.recharge("");
        }
        this.userConfirmPaymentStatus = function(status) {
            if (status == "success") {
                Kooboo.Domain.getPaymentStatus({
                    organizationId: self.organizationId(),
                    paymentId: self.paymentId()
                }).then(function(res) {

                    if (res.success && res.model.success) {
                        self.paymentModal(false);
                        window.info.show(Kooboo.text.info.payment.success, true);
                        Kooboo.Organization.getOrg().then(function(r) {
                            self.organizationName(r.model.name);
                            self.organization(r.model.id);
                            self.organizationId(r.model.id);
                            self.organizationBalance(r.model.balance);
                            self.recharge("");
                        })
                        self.paymentId("");
                    } else {
                        if (self.successAttempt > 1) {
                            clearInterval(interval);
                            window.info.show(Kooboo.text.info.payment.fail, false);
                            self.paymentModal(false);
                            self.successAttempt = 0;
                        } else {
                            window.info.show(Kooboo.text.info.payment.tryAgain, false);
                            self.successAttempt++;
                        }
                    }
                })
            } else if (status == "cancel") {
                self.paymentModal(false);
                clearInterval(interval);
                window.info.show(Kooboo.text.info.payment.cancel, false);
                self.recharge("");
            } else if (status == "fail") {
                self.paymentModal(false);
                clearInterval(interval);
                self.recharge("");
            }
        }

        function checkStatus(paymentId) {
            if (!paymentId) {
                return;
            }

            if (!paymentSuccess) {
                Kooboo.Domain.getPaymentStatus({
                    organizationId: self.organizationId(),
                    paymentId: paymentId
                }).then(function(res) {

                    if (res.success && (res.model.success || res.model.message == "canceled")) {
                        paymentSuccess = true;
                        clearInterval(interval);
                        self.paymentModal(false);
                        if (res.model.success) {
                            window.info.show(Kooboo.text.info.payment.success, true);
                            Kooboo.Organization.getOrg().then(function(r) {
                                self.organizationName(r.model.name);
                                self.organization(r.model.id);
                                self.organizationId(r.model.id);
                                self.organizationBalance(r.model.balance);
                                self.recharge("");
                            })
                        } else {
                            window.info.show(Kooboo.text.info.payment.cancel, true);
                        }

                    }
                })
            }
        }


        // Users
        this.isUserInit = ko.observable(false);

        this.organizationUsers = ko.observableArray();

        this.newUser = ko.validateField({
            required: Kooboo.text.validation.required
        })

        this.isNewUserNameValid = function() {
            return self.newUser.isValid();
        }

        this.addNewUser = function() {
            if (!self.isNewUserNameValid()) {
                self.showError(true);
            } else {
                Kooboo.Organization.addUser({
                    organizationId: self.organizationId(),
                    userName: self.newUser()
                }).then(function(res) {
                    self.showError(false);
                    if (!res.model) {
                        self.organizationUsers.push(self.newUser());
                        self.newUser("");
                        window.info.show(Kooboo.text.info.update["success"], true);
                    } else {
                        self.newUser("");
                        window.info.show(res.model, false);
                    }

                })
            }
        }

        this.deletableUser = function(username) {
            return username !== self.organizationName();
        }

        this.deleteUser = function(username) {
            Kooboo.Organization.deleteUser({
                organizationId: self.organizationId(),
                userName: username
            }).then(function(res) {
                if (res.success) {
                    self.organizationUsers.remove(username);
                }
            })
        }

        Kooboo.User.getCulture().then(function(res) {

            if (res.success) {
                self.languageOptions(Kooboo.objToArr(res.model));
                self.changeType("Account");
            }
        })
    }

    var vm = new profileModel();
    ko.applyBindings(vm, document.getElementById("main"));
    //add 2co.mine
    // var script = document.createElement("script");
    // script.src = "https://www.paypalobjects.com/api/checkout.js";
    //script.src = "https://www.2checkout.com/checkout/api/2co.min.js";
    // document.getElementsByTagName("head")[0].appendChild(script);
})