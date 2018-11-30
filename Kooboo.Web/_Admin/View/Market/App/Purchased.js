$(function() {
    var viewModel = function() {
        var self = this;
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})