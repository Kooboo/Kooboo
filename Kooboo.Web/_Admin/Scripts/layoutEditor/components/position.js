(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/layoutEditor/components/position.html"),
        PositionStore = Kooboo.layoutEditor.store.PositionStore;

    ko.components.register("kb-layout-placeholder", {
        viewModel: function() {
            var self = this;

            this.elem = null;
            this.id = ko.observable();
            this.name = ko.validateField({
                required: Kooboo.text.validation.required,
                localUnique: {
                    compare: function() {
                        if (self._name()) {
                            return _.concat(_.map(PositionStore.getAll(), function(p) { return p.name }), (self.name() == self._name() ? "" : self.name()));
                        } else {
                            return _.concat(_.map(PositionStore.getAll(), function(p) { return p.name }), self.name())
                        }
                    },
                    message: Kooboo.text.validation.taken
                },
                regex: {
                    pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                    message: Kooboo.text.validation.objectNameRegex
                }
            });
            this._name = ko.observable();
            this.applyOmit = ko.observable(false);

            this.type = ko.observable();

            this.showError = ko.observable(false);
            this.isShow = ko.observable(false);

            Kooboo.EventBus.subscribe("position/edit", function(data) {
                self.id(data.id);
                self.elem = data.elem;
                self.name(data.name);
                self._name(data.name);
                self.type(data.type);
                self.isShow(true);
                self.applyOmit(data.id && self.elem.hasAttribute("k-omit"));
            });

            this.valid = function() {
                return self.name.valid();
            };

            this.save = function() {
                if (self.valid()) {
                    var id = self.id(),
                        elem = self.elem,
                        name = self.name(),
                        type = self.type(),
                        applyOmit = self.applyOmit();
                    if (id) {
                        Kooboo.EventBus.publish("position/update", {
                            id: id,
                            name: name,
                            elem: elem,
                            applyOmit: applyOmit
                        });
                    } else {
                        Kooboo.EventBus.publish("position/add", {
                            elem: elem,
                            name: name,
                            type: type,
                            applyOmit: applyOmit
                        });
                    }
                    Kooboo.EventBus.publish("kb/frame/dom/update")
                    self.reset();
                } else {
                    self.showError(true);
                }
            };

            this.reset = function() {
                self.elem = null;
                self.id(null);
                self.name(null);
                self.type(null);
                self.applyOmit(false);

                self.showError(false);
                self.isShow(false);
            };
        },
        template: template
    });

})();