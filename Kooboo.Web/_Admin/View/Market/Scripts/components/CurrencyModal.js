(function() {
    var template = Kooboo.getTemplate('/_Admin/View/Market/Scripts/components/CurrencyModal.html');

    var currencies = [];

    ko.components.register('currency-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    if (!currencies.length) {
                        Kooboo.Currency.getList().then(function(res) {
                            if (res.success) {
                                currencies = res.model;
                                self.currencies(currencies);
                                self._currentCode(self.code());
                            }
                        })
                    } else {
                        self.currencies(currencies);
                    }
                }
            })

            this._currentCode = ko.observable();
            this.code = params.code;

            this.currencies = ko.observableArray();

            this.onSave = function() {
                if (self.code() !== self._currentCode()) {
                    if (confirm(Kooboo.text.confirm.market.changeCurrency)) {
                        Kooboo.Currency.change({
                            currencyCode: self.code()
                        }).then(function(res) {
                            if (res.success) {
                                self._currentCode(self.code());
                                Kooboo.EventBus.publish('kb/market/balance/update');
                                self.onHide();
                            }
                        })
                    }
                } else {
                    self.onHide();
                }
            }

            this.onHide = function() {
                self.code(self._currentCode());
                self.isShow(false);
            }

        },
        template: template
    })
})()