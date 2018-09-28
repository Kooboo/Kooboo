(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/controlType/TextBox.html");

    ko.components.register("text-box", {
        viewModel: function(params) {
            _.assign(this, params);
        },
        template: template
    })
})()