(function() {
    var template = Kooboo.getTemplate("/_Admin/Witkey/Scripts/components/DemandModal.html");

    ko.components.register('demand-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;
            this.onHide = function() {
                self.isShow(false);
            }

            this.onPublish = function() {}
        },
        template: template
    })
})()