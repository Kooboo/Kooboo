$(function() {

    var viewModel = function() {
        var self = this;

        this.id = ko.observable(Kooboo.getQueryString('id'));

        this.currency = ko.observable();
        this.currencySymbol = ko.observable();

        this.username = ko.observable();
        this.introduction = ko.observable();
        this.expertises = ko.observableArray();

        Kooboo.Supplier.get({
            id: self.id()
        }).then(function(res) {
            if (res.success) {
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
    }

    function Expertise(data) {
        ko.mapping.fromJS(data, {}, this);
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'))

})