$(function() {
    var model = {
        "domain": null,
        "available": false,
        "status": null,
        "balance": 0.0,
        "prices": null,
        "currencySymbol": "&yen;",
        "years": null,
        "orderId": "00000000-0000-0000-0000-000000000000",
        "suggests": null
    };

    function viewModel(data) {
        var self = this,
            paySuccess = false;
        self.domain = ko.observable(data.domain);
        self.available = ko.observable(data.available);
        self.suggests = ko.observableArray(data.suggests);
        self.statusCode = ko.observable(data.statusCode);
        self.balance = ko.observable(data.balance);
        self.currencySymbol = ko.observable(data.currencySymbol);

        this.handleEnter = function(m, e) {
            if (e.keyCode == 13) {
                self.search(m, e);
            }
        }

        self.payment = ko.observable('balance');
        self.years = ko.observable(1);
        self.prices = ko.observable({});
        self.target = ko.observable();
        self.confirm = ko.observable(false);
        self.showQrcode = ko.observable(false);
        self.paymentId = ko.observable();

        this.organizationId = ko.observable();
        self.select = function(m, e) {
            e.preventDefault();
            var o = this.item,
                amount = this.amount;
            if (!o) {
                return;
            }
            self.confirm(true);
            self.available(true);
            self.domain(o.domain);
            self.years(amount);
            self.unitPrice(o.price);
            if (self.balance() >= self.totalPrice()) {
                self.payment("balance");
            } else {
                self.payment("wechat");
            }
        }

        self.cancel = function(m, e) {
            e.preventDefault();
            self.confirm(false);
        }

        self.submit = function(m, e) {
            e.preventDefault();
            Kooboo.Domain.payDomain({
                name: self.domain(),
                years: self.years(),
                organizationId: self.organizationId(),
                paymentMethod: self.payment()
            }).then(function(data) {
                var m = data.model;
                if (m.success === false) {
                    alert(m.errorMessage);
                } else if (m.success === true && m.paid === true) {
                    alert("Paid by balance!");
                    location.href = "/_Admin/Domains";
                } else if(self.payment()=="paypal"){
                    window.location.href=m.approvalUrl;
                }else {
                    $('#qrcode').qrcode(m.qrcode);
                    self.showQrcode(m.success);
                    self.paymentId(m.paymentId);
                    checkStatus(m.paymentId);

                    interval = setInterval(function() {
                        checkStatus(m.paymentId);
                    }, 3000);
                }
            });
        }

        self.checkPay = function(m, e) {
            e.preventDefault();
            checkStatus(self.paymentId());
        }
        var interval;

        function checkStatus(paymentId) {
            if (!paymentId) {
                return;
            }
            if (!paySuccess) {
                Kooboo.Domain.getPaymentStatus({
                    organizationId: self.organizationId,
                    paymentId: paymentId
                }).then(function(data) {
                    if (data.model.success) {
                        paySuccess = true;
                        clearInterval(interval);
                        location.href = "/_Admin/Domains";
                    }
                });
            }
        }

        self.formatYearPrice = function(v) {
            return self.currencySymbol() + (+v.key * +v.value) + ' / ' + v.key + ' ' + (v.key > 1 ? Kooboo.text.site.domain.years : Kooboo.text.site.domain.year)
        }

        self.formatedYear = function() {
            var y = self.years();
            return y > 1 ? y + ' ' + Kooboo.text.site.domain.years : y + ' ' + Kooboo.text.site.domain.year;
        }

        self.yearOptions = function(prices) {
            console.log(prices);
            var arr = [];
            for (var i = 1, l = 3; i <= l; i++) {
                arr.push({
                    value: prices,
                    key: i
                });
            }
            return arr;
            //return Kooboo.objToArr(prices);
        }

        self.unitPrice = ko.observable(-1);

        self.totalPrice = ko.pureComputed(function() {
            return +(this.unitPrice() * this.years());
        }, self);

        self.search = function(m, e) {
            e.preventDefault();
            Kooboo.Domain.searchDomain({
                domain: self.domain()
            }).then(function(data) {
                var m = data.model;
                //self.domain(m.domain);
                //self.available(m);
                self.suggests(m);
                //self.statusCode(m.statusCode);
            });
        }

        Kooboo.Organization.getOrg().then(function(res) {
            if (res.success) {
                self.organizationId(res.model.id);
                self.balance(res.model.balance);
            }
        })
    }
    var vm = new viewModel(model);
    ko.applyBindings(vm, document.getElementById('main'));
    ko.applyBindings(vm, document.getElementById('payment-qrcode-container'));
});