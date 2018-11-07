(function() {
    var template = Kooboo.getTemplate('/_Admin/View/Market/Scripts/components/CashierModal.html')

    ko.components.register('cashier-modal', {
        viewModel: function(params) {

            var self = this;

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    Kooboo.Balance.getBalance().then(function(res) {
                        if (res.success) {
                            self.currencySymbol(res.model.currency.symbol);
                            self.balance(res.model.balance);
                        }
                    })
                }
            })

            this.item = params.item;
            this.currencySymbol = ko.observable();
            this.balance = ko.observable(0);

            this.insuffisant = ko.pureComputed(function() {
                if (self.item()) {
                    return self.balance() < self.item().totalPrice;
                } else {
                    return true;
                }
            })

            this.onHide = function() {
                self.item(null);
                this.isShow(false);
            }

            this.onPay = function() {
                Kooboo.EventBus.publish("kb/component/rechargeModal/show")
                self.onHide();
            }

            this.onBuy = function() {
                Kooboo.Domain.payDomain({
                    name: self.item().domain,
                    years: self.item().year,
                    paymentMethod: 'balance'
                }).then(function(res) {
                    if (res.success) {
                        Kooboo.EventBus.publish('kb/market/balance/update')
                        window.info.done(Kooboo.text.info.purchase.success);
                        self.onHide();
                    } else {
                        window.info.fail(Kooboo.text.info.purchase.failed);
                    }
                })
            }
        },
        template: template
    })
})()