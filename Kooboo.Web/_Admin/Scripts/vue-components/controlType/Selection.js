(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/controlType/Selection.html");

    ko.components.register("selection", {
        viewModel: function(params) {
            _.assign(this, params);
        },
        template: template
    })
})()