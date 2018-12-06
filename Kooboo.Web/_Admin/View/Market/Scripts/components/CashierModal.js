(function() {
    Kooboo.loadJS(["/_Admin/Scripts/lib/jquery.qrcode.min.js"]);

    var template = Kooboo.getTemplate('/_Admin/View/Market/Scripts/components/CashierModal.html')

    var PAYMENT_METHODS = [];

    var interval = null;
    var PAYMENT_SUCCESS = false;
    var infoShowed = false;

    ko.components.register('cashier-modal', {
        viewModel: function() {
            var self = this;

            this.isShow = ko.observable(false);
            this.isShow.subscribe(function(show) {
                if (show) {
                    self.paymentMethod(PAYMENT_METHODS[0].type);
                }
            })

            this.order = ko.observable();

            this.currencySymbol = ko.observable();

            this.price = ko.observable();

            this.paymentMethods = ko.observableArray();
            this.paymentMethod = ko.observable();

            this.displayAmount = ko.pureComputed(function() {
                return self.currencySymbol() + self.price();
            })

            this.onHide = function() {
                interval = null;
                PAYMENT_SUCCESS = false;
                self.order(null);
                self.isPaying(false);
                self.isShow(false);
            }

            this.userCurrencySymbol = ko.observable();
            this.balance = ko.observable();

            this.userBalance = ko.pureComputed(function() {
                return self.userCurrencySymbol() + self.balance();
            })

            this.isPaying = ko.observable(false);

            this.onPay = function() {
                var order = self.order();
                order.paymentMethod = self.paymentMethod();
                Kooboo.Order.pay(order).then(function(res) {
                    if (res.success) {
                        var data = res.model;
                        if (data.actionRequired) {
                            if (data.qrCode) {
                                self.paymentId(data.paymentRequestId);
                                $('#qr-code').empty().qrcode(data.qrCode);
                                self.isPaying(true);
                            } else if (data.redirectUrl) {
                                self.paymentId(data.paymentRequestId);
                                self.isPaying(true);
                                window.open(data.redirectUrl);
                            }

                            interval = setInterval(function() {
                                self.checkPaymentStatus(data.paymentRequestId)
                            }, 500);
                        }
                    }
                })
            }

            this.paymentId = ko.observable();

            this.checkPaymentStatus = function(paymentId) {
                if (!PAYMENT_SUCCESS) {
                    Kooboo.Payment.getStatus({
                        paymentRequestId: paymentId
                    }).then(function(res) {
                        if (res.success && (res.model.success || res.model.message == 'canceled')) {
                            PAYMENT_SUCCESS = true;
                            interval && clearInterval(interval);
                            self.onHide();
                            if (res.model.success) {
                                if (!infoShowed) {
                                    infoShowed = true;
                                    window.info.done(Kooboo.text.info.payment.success);
                                }
                            } else {
                                if (!infoShowed) {
                                    infoShowed = true;
                                    window.info.done(Kooboo.text.info.payment.failed);
                                }
                            }
                        }
                    })
                }
            }

            Kooboo.EventBus.subscribe('kb/market/component/cashier/show', function(data) {
                self.order(data);

                self.currencySymbol(data.symbol);
                self.price(data.totalAmount);

                Kooboo.Balance.getBalance().then(function(res) {
                    if (res.success) {
                        self.userCurrencySymbol(res.model.currency.symbol);
                        self.balance(res.model.balance);
                    }
                })

                if (!PAYMENT_METHODS.length) {
                    Kooboo.Payment.getMethods().then(function(res) {
                        if (res.success) {
                            PAYMENT_METHODS = res.model;
                            self.paymentMethods(res.model);
                            self.isShow(true);
                        }
                    })
                } else {
                    self.isShow(true);
                }
            })
        },
        template: template
    })
})()