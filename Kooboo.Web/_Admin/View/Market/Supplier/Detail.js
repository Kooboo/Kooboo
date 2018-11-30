$(function() {

    $('#expertises a').click(function(e) {
        e.preventDefault()
        $(this).tab('show')
    });

    var viewModel = function() {
        var self = this;

        this.id = ko.observable(Kooboo.getQueryString('id'));
        this.isOwner = ko.observable();
        this.isOwner.subscribe(function(isOwner) {
            self.getOrders();
        })

        this.getOrders = function() {
            if (self.isOwner()) {
                Kooboo.Supplier.getOrdersBySupplier().then(function(res) {
                    if (res.success) {
                        self.orders(res.model.list);
                    }
                })
            } else {
                Kooboo.Supplier.getMyOrdersInSupply({
                    supplierId: Kooboo.getQueryString('id')
                }).then(function(res) {
                    if (res.success) {
                        self.orders(res.model.list);
                    }
                })
            }
        }

        this.orders = ko.observableArray();

        this.currency = ko.observable();
        this.currencySymbol = ko.observable();

        this.username = ko.observable();
        this.introduction = ko.observable();
        this.expertises = ko.observableArray();

        Kooboo.Supplier.get({
            id: self.id()
        }).then(function(res) {
            if (res.success) {
                self.isOwner(res.model.isOwner);
                self.username(res.model.userName);
                self.introduction(res.model.introduction.split('\n').join('<br>'));
                self.expertises(res.model.expertises.map(function(item) {
                    return new Expertise(Object.assign(item, {
                        currency: res.model.currency,
                        symbol: res.model.symbol
                    }))
                }));
            }
        })

        this.onOrder = function(data, e) {
            if (confirm('Confirmation')) {
                Kooboo.Supplier.addOrUpdateOrder({
                    supplierId: self.id(),
                    price: data.price(),
                    expertise: data.name()
                }).then(function(res) {
                    if (res.success) {
                        self.getOrders();
                    }
                })
            }
        }

        this.onAcceptOrder = function(data, isAccepted, e) {
            if (confirm('Confirmation')) {
                Kooboo.Supplier.acceptOrder({
                    id: data.id,
                    isAccept: isAccepted
                }).then(function(res) {
                    if (res.success) {
                        self.getOrders();
                    }
                })
            }
        }

        this.onFinishOrder = function(data, isFinished, e) {
            if (confirm('Confirmation')) {
                Kooboo.Supplier.orderFinished({
                    id: data.id,
                    isFinished: isFinished
                }).then(function(res) {
                    if (res.success) {
                        self.getOrders();
                    }
                })
            }
        }
    }

    function Expertise(data) {
        data.description = data.description && data.description.split('\n').join('<br>')
        ko.mapping.fromJS(data, {}, this);
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'))

})