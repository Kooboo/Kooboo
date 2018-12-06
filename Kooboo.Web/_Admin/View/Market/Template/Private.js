$(function() {
    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.templates = ko.observableArray();

        this.templatesRendered = function() {
            $("img.lazy").lazyload({
                event: "scroll",
                effect: "fadeIn"
            });
        }

        Kooboo.Template.private().then(function(res) {
            if (res.success) {
                self.pager(res.model);
                self.templates(res.model.list);
            }
        })

        this.showTemplateModal = ko.observable(false);
        this.templateData = ko.observable();

        this.onSelectTemplate = function(m, e) {
            Kooboo.Template.Get({
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