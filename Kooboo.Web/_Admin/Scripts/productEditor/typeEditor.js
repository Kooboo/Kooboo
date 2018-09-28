(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/productEditor/typeEditor.html");

    ko.components.register('type-editor', {
        viewModel: function(params) {
            var self = this;

            this.onShow = params.onShow;
            this.onShow.subscribe(function(show) {
                if (show) {
                    debugger;
                }
            })
            this.field = params.field;
            this.onSave = params.onSave;
            this.onResetModal = function() {
                this.onShow(false);
            }

            this.isValid = function() {
                return false;
            }
            this.onSaveField = function() {
                if (this.isValid()) {
                    this.onSave();
                }
            }
        },
        template: template
    })

    function Option(opt) {
        this.key = ko.validateField(opt.key || '', {
            required: ''
        })

        this.value = ko.validateField(opt.value || '', {
            required: ''
        })

        this.showError = ko.observable(false);

        this.isValid = function() {
            return this.key.isValid() && this.value.isValid();
        }
    }

})