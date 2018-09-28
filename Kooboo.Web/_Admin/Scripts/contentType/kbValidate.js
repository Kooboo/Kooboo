(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/contentType/kbValidate.html");

    ko.components.register("kb-validate", {
        viewModel: function(params) {
            var self = this;
            this.validateData = params.validateData;
            this.removeValiteRule = function(rule) {
                Kooboo.EventBus.publish("ko/contentType/field/removeValidate", rule);
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
})()