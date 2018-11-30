(function() {
    var template = Kooboo.getTemplate("/_Admin/Witkey/Scripts/components/OrderModal.html");

    ko.components.register('order-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;
        },
        template: template
    })
})()