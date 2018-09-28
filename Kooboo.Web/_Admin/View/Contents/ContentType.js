$(function() {
    var ContentType = function() {
        var contentTypeId = Kooboo.getQueryString("id") || Kooboo.Guid.Empty;
        var self = this;

        this.isNewField = ko.observable();
        this.isNewContentType = (contentTypeId == Kooboo.Guid.Empty);
        this.showError = ko.observable(false);
        this.fields = ko.observableArray();
        this.systemField = ko.pureComputed(function() {
            return self.fields().filter(function(item) {
                return item.isSystemField();
            })
        })

        this.onFieldModalShow = ko.observable(false);
        this.showSystemFields = ko.observable(false);

        this.fieldData = ko.observable();
        this.onAddField = function() {
            this.isNewField(true);
            this.fieldData({});
            this.onFieldModalShow(true);
        }
        this.onEditField = function(m, e) {
            self.isNewField(false);
            self.fieldData(ko.mapping.toJS(m));
            self.onFieldModalShow(true);
        }

        this.onFieldSave = function(fm) {
            var _fields = _.cloneDeep(self.fields()),
                idx = _.findIndex(_fields, function(f) {
                    return f.name() == (self.isNewField() ? "Online" : fm.name);
                });
            if (idx > -1) {
                _fields.splice(idx, self.isNewField() ? 0 : 1, ko.mapping.fromJS(fm));
                self.fields(_fields);
            }
        }

        this.getFieldNames = function() {
            return _.map(self.fields(), function(f) {
                return f.name();
            })
        }

        this.removeField = function(field) {
            self.fields.remove(field);
        }

        this.toggleSystemFields = function() {
            this.showSystemFields(!this.showSystemFields())
        }

        this.name = ko.validateField({
            required: Kooboo.text.validation.required,
            stringlength: {
                min: 1,
                max: 64,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
            },
            remote: {
                url: Kooboo.ContentType.isUniqueName(),
                data: {
                    name: function() { return self.name() }
                },
                message: Kooboo.text.validation.taken
            }
        });

        this.contentTypesPageUrl = Kooboo.Route.ContentType.ListPage;

        this.isValid = function() {
            if (this.isNewContentType) {
                return self.name.isValid();
            } else {
                return true;
            }
        }

        this.onSave = function() {
            if (this.isValid()) {
                var properties = this.fields().map(function(f) {
                    return ko.mapping.toJS(f);
                });

                var data = {
                    id: contentTypeId,
                    name: this.name(),
                    properties: properties
                }
                Kooboo.ContentType.save(data).then(function(res) {
                    if (res.success) {
                        location.href = self.contentTypesPageUrl;
                    }
                })
            } else {
                this.showError(true);
            }
        }

        Kooboo.ContentType.Get({
            id: contentTypeId
        }).then(function(res) {
            if (res.success) {
                self.name(res.model.name);

                self.fields(res.model.properties.map(function (o) {
                    if (o.controlType.toLowerCase() == 'tinymce') {
                        o.controlType = 'RichEditor';
                    }
                    return ko.mapping.fromJS(o)
                }));

            }
        })
    }

    ko.applyBindings(new ContentType, document.getElementById("main"));
})