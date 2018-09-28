(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbUploadProgressor.html");
    ko.components.register("kb-upload-progressor", {
        viewModel: function(params) {
            var self = this;
            this.percentage = ko.observable(params.percentage);
            this.percentageString = ko.observable((params.percentage * 100).toFixed(2) + '%');

        },
        template: template
    })

})()