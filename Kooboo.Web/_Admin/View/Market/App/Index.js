$(function() {
    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.apps = ko.observableArray();

        this.appsRendered = function() {
            $("img.lazy").lazyload({
                event: "scroll",
                effect: "fadeIn"
            });
        }

        Kooboo.App.getList().then(function(res) {
            if (res.success) {
                self.pager(res.model);
                self.apps(res.model.list);
            }
        })

        this.showTemplateModal = ko.observable(false);
        this.appData = ko.observable();

        this.onSelectApp = function(m, e) {
            Kooboo.App.Get({
                id: m.id
            }).then(function(res) {
                if (res.success) {
                    self.templateData(res.model);
                    self.showTemplateModal(true);
                }
            })
        }
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})