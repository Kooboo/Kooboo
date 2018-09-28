(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/formEditor/formValidator.html");

    ko.components.register("form-validator", {
        viewModel: function(params) {
            var self = this;

            this.validations = params.data;

            this.onRemove = params.onRemove;

            this.showError = ko.observable(false);

            this.removeValiteRule = function(rule) {
                self.onRemove(rule);
            }

            this.inputNumberOnly = function(m, e) {
                if (e.keyCode >= 48 && e.keyCode <= 57 /*number*/ ) {
                    return true;
                } else if (e.keyCode >= 96 && e.keyCode <= 105 /*number*/ ) {
                    return true;
                } else if (e.keyCode == 190 /*.*/ ||
                    e.keyCode == 69 /*e*/ ||
                    e.keyCode == 8 /*BACKSPACE*/ ||
                    e.keyCode == 189 /*-*/ ) {
                    return true;
                } else {
                    return false;
                }
            }
        },
        template: template
    })
})();