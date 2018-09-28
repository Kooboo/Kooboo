$(function() {
    var typeModel = function() {
        var self = this;

        this.id = Kooboo.getQueryString('id') || Kooboo.Guid.Empty;

        this.isNewProductType = ko.observable(true);
        this.productTypesUrl = Kooboo.Route.Product.Type.ListPage;

        this.typename = ko.validateField({
            required: '',
            stringlength: {
                min: 1,
                max: 64,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
            },
            remote: {
                url: Kooboo.ProductType.isUniqueName(),
                data: {
                    name: function() { return self.typename() }
                },
                message: Kooboo.text.validation.taken
            }
        })

        this.showError = ko.observable(false);

        this.fields = ko.observableArray([]);

        this.isNewField = ko.observable();
        this.onFieldModalShow = ko.observable(false);
        this.fieldData = ko.observable();

        this.addNewField = function() {
            self.isNewField(true);
            self.fieldData({});
            self.onFieldModalShow(true);
        }

        this.editField = function(m, e) {
            self.isNewField(false);
            self.fieldData(ko.mapping.toJS(m));
            self.onFieldModalShow(true);
        }

        this.removeField = function(m, e) {
            if (confirm(Kooboo.text.confirm.deleteItem)) {
                this.fields.remove(m);
            }
        }

        this.normalControlTypes = ko.observableArray(['textbox', 'textarea', 'richeditor', 'radiobox', 'switch', 'mediafile', 'number']);
        this.specControlTypes = ko.observable(['dynamicspec', 'fixedspec']);
        this.controlTypes = ko.observableArray(this.normalControlTypes());

        this.onFieldSave = function(field) {

            var idx = _.findIndex(self.fields(), function(f) {
                return f.name() == field.name;
            })

            if (idx > -1) {
                var _fields = _.cloneDeep(self.fields());
                _fields.splice(idx, 1, ko.mapping.fromJS(field));
                self.fields(_fields);
            } else {
                self.fields.push(ko.mapping.fromJS(field));
            }
        };

        this.getFieldNames = function() {
            return self.fields().map(function(f) {
                return f.name()
            })
        }

        this.specSelect = function(field, select) {
            field.emit('controlTypes/change', select ? self.specControlTypes() : self.normalControlTypes());
        }

        this.isValid = function() {
            if (this.isNewProductType()) {
                return this.typename.isValid()
            } else {
                return true;
            }
        }

        this.onSave = function() {
            if (this.isValid()) {
                var props = this.fields().map(function(f) {
                    return ko.mapping.toJS(f);
                })

                Kooboo.ProductType.post({
                    id: self.id,
                    name: self.typename(),
                    properties: props
                }).then(function(res) {
                    if (res.success) {
                        location.href = self.productTypesUrl;
                    }
                })
            } else {
                this.showError(true);
            }
        }

        Kooboo.ProductType.get({
            id: self.id
        }).then(function(res) {
            if (res.success) {
                self.isNewProductType(!res.model.name);
                self.typename(res.model.name);
                self.fields(res.model.properties.map(function(p) {
                    return ko.mapping.fromJS(p);
                }))
            }
        })
    }

    var vm = new typeModel();
    ko.applyBindings(vm, document.getElementById('main'));
})