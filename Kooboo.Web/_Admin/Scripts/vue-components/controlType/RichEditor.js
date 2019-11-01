(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/controlType/RichEditor.html");

    ko.components.register("rich-editor", {
        viewModel: function(params) {
            _.assign(this, params);
        },
        template: template
    })
})()