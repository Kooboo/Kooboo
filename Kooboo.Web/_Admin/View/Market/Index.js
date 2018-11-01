$(function() {

    var NAV_APP_TOP = $('#nav_app')[0].getBoundingClientRect().top,
        NAV_HARDWARE_TOP = $('#nav_hardware')[0].getBoundingClientRect().top,
        NAV_TEMPLATE_TOP = $('#nav_template')[0].getBoundingClientRect().top,
        NAV_DOMAIN_TOP = $('#nav_domain')[0].getBoundingClientRect().top;

    var Market = function() {
        var self = this;
        this.showError = ko.observable(false);

        /* UserInfo START */
        this.organizationId = ko.observable();
        this.userName = ko.observable();
        this.balance = ko.observable();

        this.currencySymbol = ko.observable();
        this.currencyCode = ko.observable();
        this.showCurrencyModal = ko.observable(false);
        this.onShowCurrenies = function() {
            self.showCurrencyModal(true);
        }

        this.getBasicInfo = function() {
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
        this.getBasicInfo();

        /* UserInfo END */

        /* Recharge START */
        this.showRechargeModal = ko.observable(false);
        this.onShowRechargeModal = function() {
            self.showRechargeModal(true);
        }
        Kooboo.EventBus.subscribe('kb/market/balance/update', function() {
            self.getBasicInfo();
        })

        /* Recharge END */

        /* Topup History START */
        this.showTopupHistoryModal = ko.observable(false);
        this.onShowTopupHistoryModal = function() {
            self.showTopupHistoryModal(true);
        }

        /* Topup History END */

        /* Hardware START */
        this.hardwares = ko.observableArray();

        Kooboo.Infrastructure.getSalesItems()
            .then(function(res) {
                if (res.success) {
                    self.hardwares(res.model);
                }
            })

        this.showHardwareModal = ko.observable(false);
        this.hardwareData = ko.observable();

        this.onSelectHardware = function(m, e) {
            self.hardwareData(m);
            self.showHardwareModal(true);
        }

        /* Hardware END */

        /* Template START */
        this.templates = ko.observableArray();

        this.templatesRendered = function() {
            $("img.lazy").lazyload({
                event: "scroll",
                effect: "fadeIn"
            });
        }

        Kooboo.Template.getList({
            pageSize: 12
        }).then(function(res) {
            if (res.success) {
                self.templates(res.model.list);
            }
        })

        this.showTemplateModal = ko.observable(false);
        this.templateData = ko.observable();

        this.onSelectTemplate = function(m, e) {
            Kooboo.Template.Get({
                id: m.id
            }).then(function(res) {
                if (res.success) {
                    self.templateData(res.model);
                    self.showTemplateModal(true);

                }
            })
        }

        /* Template END */

        /* Domian START */
        this.showCashierModal = ko.observable(false);
        this.checkoutItem = ko.observable();
        this.domainIWant = ko.validateField({
            required: ''
        })
        this.searched = ko.observable(false);
        this.availableDomains = ko.observableArray();
        this.onSearchDomain = function() {
            if (self.domainIWant.isValid()) {
                Kooboo.Domain.searchDomain({
                    domain: self.domainIWant()
                }).then(function(res) {
                    if (res.success) {
                        self.searched(true);
                        self.availableDomains(res.model.map(function(item) {
                            return {
                                domain: item.domain,
                                currencySymbol: item.currency.symbol,
                                price: item.price,
                                displayPrice: item.currency.symbol + item.price,
                                options: item.options.map(function(opt) {
                                    return {
                                        year: opt.year,
                                        price: opt.price,
                                        displayPrice: item.currency.symbol + opt.price
                                    }
                                })
                            }
                        }));
                    }
                })
            }
        }
        this.buyDomain = function(domain, idxOrEvent) {
            var selected = {
                domain: domain.domain,
                currencySymbol: domain.currencySymbol,
                unitPrice: domain.price
            }

            if (typeof idxOrEvent == "number") {
                var idx = idxOrEvent;
                var opt = domain.options[idx];
                selected.totalPrice = opt.price;
                selected.year = opt.year;
            } else {
                selected.totalPrice = domain.price;
                selected.year = 1;
            }

            self.checkoutItem(selected);
            self.showCashierModal(true);
        }

        /* Domain END */

        Kooboo.EventBus.subscribe("kb/component/rechargeModal/show", function() {
            self.showRechargeModal(true);
        })
    }

    var vm = new Market();
    ko.applyBindings(vm, document.getElementById('main'));

    window.onpopstate = function(e) {
        e.preventDefault();
    }

    $(window).scroll(function() {
        var appInfo = $('#app')[0].getBoundingClientRect(),
            hardwareInfo = $('#hardware')[0].getBoundingClientRect(),
            templateInfo = $('#template')[0].getBoundingClientRect(),
            domainInfo = $('#domain')[0].getBoundingClientRect();

        var appRange = appInfo.top + appInfo.height - 15,
            hardwareRange = hardwareInfo.top + hardwareInfo.height - 15,
            templateRange = templateInfo.top + templateInfo.height - 15,
            domainRange = domainInfo.top + domainInfo.height;

        $('#side-nav li').removeClass('active');

        if (hardwareRange > NAV_HARDWARE_TOP) {
            $('#nav_hardware').addClass('active');
        } else if (appRange > NAV_APP_TOP) {
            $('#nav_app').addClass('active');
        } else if (templateRange > NAV_TEMPLATE_TOP) {
            $('#nav_template').addClass('active');
        } else if (domainRange > NAV_DOMAIN_TOP) {
            $('#nav_domain').addClass('active');
        }
    })
})