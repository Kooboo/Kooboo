$(function() {

    var viewModel = function() {
        var self = this;

        this.id = ko.observable(Kooboo.getQueryString('id'));

        this.name = ko.observable();

        this.expertises = ko.observableArray();

        this.getDetail = function() {
            Kooboo.Supplier.get({
                id: this.id()
            }).then(function(res) {
                if (res.success) {
                    self.name(res.model.userName);
                    Kooboo.Supplier.getUserExpertiseList({
                        userId: res.model.userId
                    }).then(function(res) {
                        if (res.success) {
                            self.expertises(res.model.map(function(item) {
                                item.pageUrl = Kooboo.Route.Get(Kooboo.Route.Supplier.ServicePage, {
                                    id: item.id
                                })
                                return item;
                            }));
                        }
                    })
                }
            })

        }

        this.getDetail();
    }
    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'))

})