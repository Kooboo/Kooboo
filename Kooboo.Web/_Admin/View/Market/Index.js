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

        this.getBasicInfo = function() {
            Kooboo.Balance.getBalance().then(function(res) {
                if (res.success) {
                    self.organizationId(res.model.id);
                    self.userName(res.model.name);
                    self.balance(res.model.balance);
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

        /* Coupon START */
        this.couponCode = ko.validateField({
            required: ''
        })
        this.showCouponModal = ko.observable(false);
        this.onShowCouponModal = function() {
            this.showCouponModal(true);
        }
        this.onHideCouponModal = function() {
            this.showError(false);
            this.couponCode('');
            this.showCouponModal(false);
        }
        this.onUseCoupon = function() {
            if (this.couponCode.isValid()) {
                Kooboo.Balance.useCoupon({
                    code: this.couponCode()
                }).then(function(res) {
                    self.onHideCouponModal();
                    if (res.success) {
                        window.info.done(Kooboo.text.info.recharge.success);
                        Kooboo.EventBus.subscribe('kb/market/balance/update')
                    } else {
                        window.info.fail(Kooboo.text.info.recharge.fail);
                    }
                })
            } else {
                this.showError(true);
            }
        }

        /* Coupon END */

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