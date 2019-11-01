(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/controlType/RadioBox.html");

    ko.components.register("radiobox", {
        viewModel: function(params) {
            _.assign(this, params);

            this.clearValue = function() {
                this.fieldValue("");
            }
        },
        template: template
    })
})()