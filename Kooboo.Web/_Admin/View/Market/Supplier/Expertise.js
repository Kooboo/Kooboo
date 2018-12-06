$(function() {
    var viewModel = function() {
        var self = this;

        this.id = ko.observable(Kooboo.getQueryString('id'));

        this.name = ko.observable();

        this.description = ko.observable();

        this.price = ko.observable();

        this.currency = ko.observable();

        this.isMe = ko.observable(true);

        this.supplierId = ko.observable();
        this.supplierName = ko.observable();
        this.supplierPage = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.Supplier.DetailPage, {
                id: self.supplierId()
            })
        })

        this.getData = function(page) {
            Kooboo.Supplier.getExpertise({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.name(res.model.name);
                    self.description(res.model.description || 'No description provided.');
                    self.price(res.model.symbol + res.model.price);
                    self.currency(res.model.currency);
                    self.supplierId(res.model.supplierId);
                    self.supplierName(res.model.supplierName);

                    Kooboo.Supplier.get({
                        id: self.supplierId()
                    }).then(function(res) {
                        if (res.success) {
                            self.isMe(res.model.userId == localStorage.getItem('_kooboo_api_user'));
                        }
                    })
                }
            })
        }

        this.getData();
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})