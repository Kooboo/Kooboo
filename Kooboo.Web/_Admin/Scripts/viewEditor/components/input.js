(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/viewEditor/components/input.html"),
        ActionStore = Kooboo.viewEditor.store.ActionStore;

    var bindingType = "input";

    ko.components.register("kb-view-input", {
        viewModel: function(params) {

            var self = this;
            self.elem = null;

            this.isShow = ko.observable(false);

            this.text = ko.validateField({
                required: Kooboo.text.validation.required
            });

            this.showError = ko.observable(false);

            this.fields = ko.observableArray();

            this.onSave = params.onSave;

            this.reset = function() {
                self.elem = null;
                self.isShow(false);
                self.showError(false);
                self.text(null);
            }

            this.isValid = function() {

                if (self.fields().length > 0) {
                    return true;
                } else {
                    return self.text.isValid();
                }
            }

            this.save = function() {

                if (self.isValid()) {
                    self.onSave({
                        bindingType: bindingType,
                        elem: self.elem,
                        text: self.text()
                    })
                } else {
                    self.showError(false);
                }
            }

            Kooboo.EventBus.subscribe("binding/edit", function(data) {

                if (data.bindingType == bindingType) {
                    self.elem = data.elem;
                    self.isShow(true);
                    self.text(data.text);

                    var _fields = [],
                        formBinding = null;

                    if ($(data.elem).is(":text,textarea,select")) {
                        var $form = $(data.elem).closest("form");
                        formBinding = $form.data("kb-form-binding");
                    }

                    if (formBinding) {
                        var action = ActionStore.byId(formBinding.dataSourceMethodId);
                        if (action) {
                            if (action.parameters) {
                                _.forEach(action.parameters, function(p) {
                                    fields.push({
                                        dataName: p.name,
                                        groupedDisplay: action.parameters.length > 1,
                                        fields: DataContext.flatternFields(p, p.name + '.', null, function(field) {
                                            return !field.enumerable;
                                        })
                                    })
                                });
                            }
                        }
                    }

                    self.fields(_fields);
                }
            })
        },
        template: template
    })
})();