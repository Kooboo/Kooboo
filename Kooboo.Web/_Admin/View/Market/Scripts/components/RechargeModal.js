(function() {
    Kooboo.loadJS([
        "/_Admin/Scripts/lib/jquery.qrcode.min.js"
    ]);

    var template = Kooboo.getTemplate('/_Admin/View/Market/Scripts/components/RechargeModal.html');

    var PAYMENT_SUCCESS = false;

    var infoShowed = false;

    ko.components.register("recharge-modal", {
        viewModel: function(params) {
            var self = this;

            this.showError = ko.observable(false);

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    Kooboo.Payment.getMethods().then(function(res) {
                        if (res.success) {
                            var methods = res.model.filter(function(item) {
                                return item.type !== 'balance';
                            })
                            self.paymentMethods(methods);
                            self.paymentMethod(methods[0].type);
                        }
                    })


                    Kooboo.Balance.getChargePackages().then(function(res) {
                        if (res.success) {
                            var packages = res.model.map(function(item) {
                                item.type = 'set';
                                return item;
                            });

                            packages.push({
                                id: 0,
                                type: 'mod'
                            })

                            self.paymentPackages(packages);
                            self.currentPackage(packages[0])
                        }
                    })
                }
            })

            this.onHide = function() {
                self.showError(false);
                self.payingMode(false);
                self.paymentMethod(null);
                self.chargeAmount('');
                self.chargeAmountValue(0);
                self.couponCode('');
                self.currentPackage(null);
                self.isShow(false);
                PAYMENT_SUCCESS = false;
            }

            this.payingMode = ko.observable(false);

            this.paymentMethods = ko.observableArray();
            this.paymentMethod = ko.observable();
            this.chargeAmount = ko.observable();
            this.chargeAmount.subscribe(function(val) {
                self.chargeAmountValue(parseFloat(val));
            })
            this.chargeAmountValue = ko.validateField(0, {
                required: '',
                min: { value: 0.01 }
            })
            this.paymentId = ko.observable();

            this.couponCode = ko.validateField({
                required: ''
            })

            this.onPay = function() {
                if (self.paymentMethod() !== 'coupon') {
                    if (self.currentPackage().type == 'set') {
                        Kooboo.Order.topup({
                            paymentMethod: self.paymentMethod(),
                            packageId: self.currentPackage().id,
                            totalAmount: self.chargeAmountValue(),
                            returnPath: location.pathname
                        }).then(function(res) {
                            if (res.success) {
                                self.onPaying(res.model);
                            }
                        })
                    } else {
                        if (self.chargeAmountValue.isValid()) {
                            Kooboo.Order.topup({
                                paymentMethod: self.paymentMethod(),
                                totalAmount: self.chargeAmountValue(),
                                returnPath: location.pathname
                            }).then(function(res) {
                                if (res.success) {
                                    self.onPaying(res.model);
                                }
                            })
                        } else {
                            self.showError(true);
                        }
                    }
                } else {
                    if (self.couponCode.isValid()) {
                        Kooboo.Order.topup({
                            code: self.couponCode(),
                            paymentMethod: self.paymentMethod()
                        }).then(function(res) {
                            if (res.success) {
                                window.info.done(Kooboo.text.info.recharge.success);
                                Kooboo.EventBus.publish('kb/market/balance/update')
                            } else {
                                window.info.fail(Kooboo.text.info.recharge.fail);
                            }
                            self.onHide();
                        })
                    } else {
                        self.showError(true);
                    }
                }
            }
            this.onPaying = function(res) {
                if (res.actionRequired) {
                    if (res.qrCode) {
                        self.paymentId(res.paymentRequestId);
                        $("#qr-code").empty().qrcode(res.qrCode);
                        self.payingMode(true);
                    } else if (res.redirectUrl) {
                        self.paymentId(res.paymentRequestId);
                        self.payingMode(true);
                        window.open(res.redirectUrl);
                    }

                    self.checkPaymentStatus(res.paymentRequestId)
                }
            }
            this.changePaymentMethod = function(m, e) {
                e.preventDefault();
                if (m.type !== self.paymentMethod()) {
                    self.showError(false);
                    self.paymentMethod(m.type);
                    self.currentPackage(self.paymentPackages()[0]);
                }
            }

            this.checkPaymentStatus = function(paymentId) {
                if (!PAYMENT_SUCCESS && self.payingMode()) {
                    self.getPaymentStatus(paymentId, function(res) {
                        if (res.success && (res.model.success || res.model.message == "canceled")) {
                            PAYMENT_SUCCESS = true;
                            self.onHide();
                            if (res.model.success) {
                                if (!infoShowed) {
                                    infoShowed = true;
                                    window.info.done(Kooboo.text.info.payment.success);
                                }
                                Kooboo.EventBus.publish('kb/market/balance/update');
                            } else {
                                if (!infoShowed) {
                                    window.info.done(Kooboo.text.info.payment.cancel);
                                    infoShowed = true;
                                }
                            }
                        } else {
                            self.checkPaymentStatus(paymentId);
                        }
                    })
                }
            }

            this.getPaymentStatus = function(id, cb) {
                Kooboo.Payment.getStatus({
                    paymentRequestId: id
                }).then(function(res) {
                    cb && cb(res);
                });
            }

            this.confirmBtnText = ko.pureComputed(function() {
                return (self.paymentMethod() == 'coupon' ? Kooboo.text.common.use : Kooboo.text.common.pay);
            })

            this.paymentPackages = ko.observableArray()
            this.currentPackage = ko.observable();
            this.changePackage = function(m, e) {
                e.preventDefault();
                if (m !== self.currentPackage()) {
                    self.currentPackage(m);
                    self.showError(false);
                }
            }

            this.onPayingSuccess = function() {
                self.getPaymentStatus(self.paymentId(), function() {
                    self.onHide();
                    Kooboo.EventBus.publish('kb/market/balance/update');
                })
            }
            this.onPayingFailed = function() {
                self.payingMode(false);
            }
        },
        template: template
    })
})()