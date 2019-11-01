(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/controlType/TextArea.html");

    ko.components.register("text-area", {
        viewModel: function(params) {
            _.assign(this, params);
        },
        template: template
    })
})()