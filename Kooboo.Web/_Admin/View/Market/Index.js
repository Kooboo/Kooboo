$(function() {
    var viewModel = function() {
        var self = this;

        this.organizationId = ko.observable();

        this.userName = ko.observable();

        this.balance = ko.observable();

        this.currencySymbol = ko.observable();

        this.currencyCode = ko.observable();

        this.displayBalance = ko.pureComputed(function() {
            if (self.currencySymbol() !== undefined && self.balance() !== undefined) {
                return self.currencySymbol() + self.balance();
            } else {
                return Kooboo.text.common.loading;
            }
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

        this.username = ko.observable();
        this.userEmail = ko.observable();
        this.isUserVerified = ko.observable(false);
        this.currentDataCenter = ko.observable();
        this.availableDataCenters = ko.observableArray();
        this.getUser = function() {
            Kooboo.Organization.getDataCenter().then(function(res) {
                if (res.success) {
                    self.username(res.model.organizationName);
                    self.currentDataCenter(res.model.currentDatacenterName);
                    self.availableDataCenters(Kooboo.objToArr(res.model.availableDataCenters, 'value', 'displayName'));
                }
            })
            Kooboo.User.getUser().then(function(res) {
                if (res.success) {
                    self.userEmail(res.model.emailAddress);
                    self.isUserVerified(res.model.isEmailVerified);
                }
            })
        }
        this.showDataCenterModal = ko.observable(false);
        this.onChangeDC = function() {
            self.showDataCenterModal(true);
        }

        this.showUserVerifyModal = ko.observable(false);
        this.onVerifyUser = function() {
            if (!self.isUserVerified()) {
                self.showUserVerifyModal(true);
            }
        }

        this.getBalance();
        this.getUser();

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
                            moreUrl = Kooboo.Route.Supplier.ListPage;
                            detailUrl = Kooboo.Route.Supplier.ServicePage;
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

        Kooboo.EventBus.subscribe('kb/market/datacenter/updated', function(data) {
            self.currentDataCenter(data.loc);
        })
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