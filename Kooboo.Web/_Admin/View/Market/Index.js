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
        Kooboo.EventBus.subscribe('kb/market/cashier/done', function() {
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

        this.panels = ko.observableArray();

        Kooboo.Market.getMy().then(function(res) {
            if (res.success) {
                debugger; 
                self.panels(res.model.map(function(item) {
                    var moreUrl = '',
                        detailUrl = '';

                    switch (item.title.value.toLowerCase()) {
                        case 'discussion':
                            moreUrl = Kooboo.Route.Discussion.MyPage;
                            detailUrl = Kooboo.Route.Discussion.DetailPage;
                            break;
                        case 'demand':
                            moreUrl = Kooboo.Route.Demand.MyDemandPage;
                            detailUrl = Kooboo.Route.Demand.DetailPage;
                            break;
                        case 'service':
                            moreUrl = "/_admin/market/supplier/index";
                            url = "/_admin/market/supplier/service";
                            break;
            
                    }

                    return {
                        title: item.title.displayName,
                        showMoreUrl: moreUrl,
                        list: item.list.map(function(li) {
                            return {
                                title: li.title,
                                url: Kooboo.Route.Get(detailUrl, {
                                    id: li.id
                                })
                            }
                        })
                    }
                }))
            }
        })

        this.afterRender = function() {
            waterfall('#waterfall')
        }

    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'))

    $(window).on('resize', function() {
        try {
            waterfall('#waterfall')
        } catch (e) {
            // console.error(e);
        }
    })
})