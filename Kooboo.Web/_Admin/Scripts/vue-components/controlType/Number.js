(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/controlType/Number.html");

    ko.components.register("number", {
        viewModel: function(params) {
            _.assign(this, params);
        },
        template: template
    })
})()