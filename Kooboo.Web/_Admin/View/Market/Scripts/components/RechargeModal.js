(function() {
    var template = Kooboo.getTemplate('/_Admin/View/Market/Scripts/components/RechargeModal.html');

    var paymentMethods = [],
        paymentPackages = [];

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
                                    type: 'mod'
                                })

                                self.paymentPackages(packages);
                            }
                        })
                    }
                }
            })

            this.onHide = function() {
                self.isShow(false);
                self.paymentMethod('wechat');
                self.chargeAmount('');
                self.packageId(0);
            }

            this.paymentMethods = ko.observableArray();
            this.paymentMethod = ko.observable();
            this.chargeAmount = ko.validateField({
                required: '',
                min: 0.01
            })
            this.onPay = function() {
                if (self.packageId() !== 5) {

                } else {
                    if (self.chargeAmount.isValid()) {

                    }
                }
            }
            this.changePaymentMethod = function(m, e) {
                e.preventDefault();
                if (m.method !== self.paymentMethod()) {
                    self.paymentMethod(m.method);
                }
            }

            this.defaultChargeOptions = ko.observableArray()
            this.packageId = ko.observable(0);
            this.changePackage = function(m, e) {
                e.preventDefault();
                if (m.id !== self.packageId()) {
                    self.packageId(m.id);
                }
            }
        },
        template: template
    })
})()