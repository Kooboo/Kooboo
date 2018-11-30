$(function() {
    var viewModel = function() {
        var self = this;

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

    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})