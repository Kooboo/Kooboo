(function() {
    Kooboo.loadJS(["/_Admin/Scripts/lib/jquery.qrcode.min.js"]);

    var template = Kooboo.getTemplate('/_Admin/View/Market/Scripts/components/RechargeModal.html');

    var paymentMethods = [],
        paymentPackages = [];

    var interval = null,
        paymentSuccess = false;

    ko.components.register("recharge-modal", {
        viewModel: function(params) {
            var self = this;

            this.showError = ko.observable(false);

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    if (!paymentMethods.length) {
                        // 加载支付方式
                        Kooboo.Balance.getPaymentMethods().then(function(res) {
                            if (res.success) {
                                self.paymentMethods(res.model);
                                self.paymentMethod(res.model[0].type);
                            }
                        })
                    } else {
                        self.paymentMethods(paymentMethods);
                    }

                    if (!paymentPackages.length) {
                        // 加载支付包选项
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
                }
            })

            this.onHide = function() {
                self.showError(false);
                self.payingMode(false);
                self.paymentMethod('wechat');
                self.chargeAmount('');
                self.chargeAmountValue(0);
                self.currentPackage(null);
                self.isShow(false);
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
            this.onPay = function() {
                if (self.currentPackage().type == 'set') {
                    Kooboo.Balance.topup({
                        packageId: self.currentPackage().id,
                        PaymentMethod: self.paymentMethod()
                    }).then(function(res) {
                        if (res.success) {
                            self.onPaying(res.model);
                        }
                    })
                } else {
                    if (self.chargeAmountValue.isValid()) {
                        Kooboo.Balance.topup({
                            price: self.chargeAmountValue(),
                            PaymentMethod: self.paymentMethod()
                        }).then(function(res) {
                            if (res.success) {
                                self.onPaying(res.model);
                            }
                        })
                    } else {
                        self.showError(true);
                    }
                }
            }
            this.onPaying = function(res) {
                if (res.success) {
                    if (res.qrcode) {
                        $("#qr-code").empty().qrcode(res.qrcode);
                        self.paymentId(res.paymentId);
                        self.payingMode(true);
                    } else if (res.approvalUrl) {
                        self.paymentId(res.paymentId);
                        self.payingMode(true);
                        window.open(res.approvalUrl);
                    }

                    interval = setInterval(function() {
                        self.checkPaymentStatus(res.paymentId)
                    }, 500);
                }
            }
            this.changePaymentMethod = function(m, e) {
                e.preventDefault();
                if (m.type !== self.paymentMethod()) {
                    self.paymentMethod(m.type);
                }
            }

            this.checkPaymentStatus = function(paymentId) {
                if (!paymentSuccess) {
                    Kooboo.Balance.getPaymentStatus({
                        paymentId: paymentId
                    }).then(function(res) {
                        if (res.success && (res.model.success || res.model.message == "canceled")) {
                            paymentSuccess = true;
                            clearInterval(interval);
                            self.onHide();
                            if (res.model.success) {
                                window.info.show(Kooboo.text.info.payment.success, true);
                                Kooboo.EventBus.publish('kb/market/balance/update');
                            } else {
                                window.info.show(Kooboo.text.info.payment.cancel, true);
                            }
                        }
                    })
                }
            }

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
                self.checkPaymentStatus(self.paymentId());
            }
            this.onPayingFailed = function() {
                self.payingMode(false);
            }
        },
        template: template
    })
})()