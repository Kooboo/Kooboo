(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/controlType/CheckBox.html");

    ko.components.register("checkbox", {
        viewModel: function(params) {
            _.assign(this, params);
        },
        template: template
    })
})()


kooboo.text.market.ok; 

GetValue("OK")