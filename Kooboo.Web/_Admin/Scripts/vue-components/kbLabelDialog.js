(function() {
    var bindingType = "label",
        KB_NEW_LABEL = "__KB__NEW__LABEL";
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbLabelDialog.html");
    ko.components.register("kb-label-dialog", {
        viewModel: function(params) {

            var self = this;

            this.onSave = params.onSave;

            this.elem = null;

            this.id = ko.observable();
            this.title = ko.observable();

            this.isShow = ko.observable(false);

            this.existLabels = ko.observableArray();

            this.showInput = ko.observable(false);
            this.showInput.subscribe(function(show) {
                show ? self.text("") : self.showError(false);
            })

            this.labelValue = ko.observable();
            this.labelValue.subscribe(function(value) {
                self.showInput(value == KB_NEW_LABEL);
            });

            this.text = ko.validateField({
                required: Kooboo.text.validation.required,
                regex: {
                    pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                    message: Kooboo.text.validation.objectNameRegex
                }
            });
            this.placeholder=ko.observable();
            this.showError = ko.observable(false);

            this.reset = function() {
                self.elem = null;
                self.id("");
                self.text("");
                self.showError(false);
                self.isShow(false);
            }

            this.isValid = function() {
                if (self.showInput()) {
                    return self.text.isValid();
                } else {
                    return true;
                }
            }

            this.save = function() {

                if (self.isValid()) {
                    if (self.onSave) {
                        self.onSave({
                            elem: self.elem,
                            text: self.showInput() ? self.text() : self.labelValue(),
                            bindingType: bindingType
                        });
                    } else {
                        Kooboo.EventBus.publish("binding/save", {
                            id: self.id(),
                            elem: self.elem,
                            text: self.showInput() ? self.text() : self.labelValue(),
                            type: bindingType
                        })
                    }
                    self.reset();
                } else {
                    self.showError(true);
                }
            }

            Kooboo.EventBus.subscribe("binding/edit", function(data) {

                if (data.bindingType == bindingType || data.type == bindingType) {

                    Kooboo.Label.getKeys().then(function(res) {

                        if (res.success) {
                            var list = [];
                            _.forEach(res.model, function(label) {
                                list.push({
                                    name: label,
                                    value: label
                                })
                            })
                            
                            var name=data && data.name?
                                    data.name :
                                    Kooboo.text.site.label.createANewLabel; 
                            list.push({
                                name: name,
                                value: KB_NEW_LABEL
                            })
                            self.existLabels(list);

                            var find = _.find(res.model, function(m) {
                                return m == data.text;
                            })

                            self.labelValue(find ? data.text : KB_NEW_LABEL);
                            data.text && self.text(data.text);

                            var placeholder=data && data.placeholder?data.placeholder:Kooboo.text.site.label.placeholder;
                            self.placeholder(placeholder);

                            self.elem = data.elem;
                            self.id(data.id);
                            self.isShow(true);
                            self.title(data.title || Kooboo.text.common.Label);
                        }
                    })
                }
            })
        },
        template: template
    })
})();