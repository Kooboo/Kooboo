$(function() {
    var siteViewModel = function() {

        var self = this;

        this.widgets = ko.observableArray();

        this.afterRender = function() {
            waterfall('.block-dashboard-stat')
        }

        Kooboo.Dashboard.getItems().then(function(res) {

            if (res.success) {
                res.model.forEach(function(item, idx) {
                    self.widgets.push(item);
                })
            }
        })
    }

    var vm = new siteViewModel();

    ko.applyBindings(vm, document.getElementById('main'));

    $(window).on('resize', function() {
        try {
            waterfall('.block-dashboard-stat')
        } catch (e) {
            // console.error(e);
        }
    })
})