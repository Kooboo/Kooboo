$(function() {
    var viewModel = function() {
        var self = this;

        this.organizationId = ko.observable();

        this.userName = ko.observable();

        this.balance = ko.observable();

        this.currencySymbol = ko.observable();

        this.currencyCode = ko.observable();

        this.displayBalance = ko.pureComputed(function() {
            return self.currencySymbol() + self.balance();
        })

        this.getBalance = function() {
            Kooboo.Balance.getBalance().then(function(res) {
                if (res.success) {
                    self.organizationId(res.model.id);
                    self.userName(res.model.name);
                    self.balance(res.model.balance);
                    self.currencySymbol(res.model.currency.symbol);
                    self.currencyCode(res.model.currency.code);
                }
            })
        }

        this.getBalance();

        this.showRechargeModal = ko.observable(false);
        this.onShowRechargeModal = function() {
            self.showRechargeModal(true);
        }
        Kooboo.EventBus.subscribe('kb/market/balance/update', function() {
            self.getBalance();
        })

        this.showTopupHistoryModal = ko.observable(false);
        this.onShowTopupHistoryModal = function() {
            self.showTopupHistoryModal(true);
        }

        this.showCurrencyModal = ko.observable(false);
        this.onShowCurrenies = function() {
            self.showCurrencyModal(true);
        }

    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'))
})