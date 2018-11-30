$(function() {
    var viewModel = function() {
        var self = this;

        this.tabs = ko.observableArray([{
            value: 'request',
            displayName: 'Request orders'
        }, {
            value: 'recieve',
            displayName: 'Recieve orders'
        }])

        this.currentTab = ko.observable('request');
        this.currentTab.subscribe(function(tab) {
            switch (tab) {
                case 'request':
                    break;
                case 'recieve':
                    break;
            }
        })

        this.changeTab = function(data, e) {
            if (self.currentTab() !== data.value) {
                self.currentTab(data.value);
            }
        }
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})