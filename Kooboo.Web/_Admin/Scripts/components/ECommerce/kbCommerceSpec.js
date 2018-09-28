(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/ECommerce/kbCommerceSpec.html");

    ko.components.register('kb-commerce-spec', {
        viewModel: function(params) {
            var self = this;

            this.fields = params.fields;
            this.fields.subscribe(function(fields) {
                if (fields.length) {
                    var fixedSpecs = [],
                        dynamicSpecs = [];
                    fields.forEach(function(f) {
                        if (f.controlType.toLowerCase() == 'fixedspec') {
                            fixedSpecs.push(f);
                        } else {
                            var dy = new DynamicSpec(f);
                            dynamicSpecs.push(dy);

                            dy.on('change', function() {
                                self.dynamicFieldsChange(self.dynamicSpecs());
                            })
                        }
                    })

                    self.fixedSpecs(fixedSpecs);
                    self.dynamicSpecs(dynamicSpecs);
                }
            })

            this.fixedSpecs = ko.observableArray();
            this.dynamicSpecs = ko.observableArray();

            this.dynamicFieldsChange = params.dynamicFieldsChange;
        },
        template: template
    })

    function DynamicSpec(data) {
        var self = this;

        this.showError = ko.observable(false);

        this.name = ko.observable(data.name);

        if (typeof data.selectionOptions == 'string') {
            data.selectionOptions = JSON.parse(data.selectionOptions);
        }

        this.options = ko.observableArray(data.selectionOptions || []);

        if (data.selectionOptions && data.selectionOptions.length) {
            setTimeout(function() { self.emit('change') }, 100);
        }

        this.addOption = function() {
            self.showNewOptionForm(true);
        }

        this.newValue = ko.validateField({
            required: '',
            localUnique: {
                compare: function() {
                    return _.concat(self.options(), self.newValue());
                }
            }
        });

        this.saveOption = function() {
            if (self.newValue.isValid()) {
                self.options.push(self.newValue());
                self.resetForm();
                this.emit('change');
            } else {
                self.showError(true);
            }
        }

        this.resetForm = function() {
            self.showError(false);
            self.newValue('');
            self.showNewOptionForm(false);
        }

        this.removeOption = function(m, e) {
            self.options.remove(m);
            self.emit('change');
        }

        this.showNewOptionForm = ko.observable(false);

        this.events = {};
        this.on = function(ev, fn) {
            if (!this.events[ev]) this.events[ev] = [];
            this.events[ev].push(fn);
        }
        this.emit = function(ev, data) {
            if (!this.events[ev] || !this.events[ev].length) return;
            this.events[ev].forEach(function(fn) {
                fn(data);
            })
        }
    }
})()