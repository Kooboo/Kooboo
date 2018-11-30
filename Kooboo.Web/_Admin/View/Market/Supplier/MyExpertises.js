$(function() {
    var viewModel = function() {
        var self = this;
        this.id = ko.observable();

        this.showError = ko.observable(false);

        this.introduction = ko.validateField({
            required: ''
        })
        this._introduction = ko.observable();
        this.introductionChanged = ko.pureComputed(function() {
            return self.introduction() !== self._introduction();
        })

        this.onResetIntroduction = function() {
            self.introduction(self._introduction());
            $(".autosize").textareaAutoSize().trigger("keyup");
        }

        this.onSaveIntroduction = function() {
            var data = {
                introduction: self.introduction()
            }

            if (self.id()) {
                data.id = self.id()
            }

            Kooboo.Supplier.addOrUpdate(data).then(function(res) {
                if (res.success) {
                    self._introduction(self.introduction());
                    window.info.done(Kooboo.text.info.update.success);
                }
            })
        }

        this.expertises = ko.observableArray();

        this.onAddExpertise = function() {
            self.expertises.push(new Expertise());
            self.isEditingExp(true);

            $(".autosize").textareaAutoSize().trigger("keyup");
        }

        this.onRemoveExpertise = function(data, e) {
            if (confirm(Kooboo.text.confirm.deleteItem)) {
                data.onRemove(function() {
                    self.expertises.remove(data);
                    self.isEditingExp(false);
                })
            }
        }

        this.onSaveExpertise = function(data, e) {
            data.onSave(function() {
                self.isEditingExp(false);
            })
        }

        this.onEditExpertise = function(data, e) {
            data.onEdit(function() {
                self.isEditingExp(true);
            })
        }

        this.isEditingExp = ko.observable(false);

        this.currency = ko.observable();

        this.currencySymbol = ko.observable();

        this.onGet = function() {
            Kooboo.Supplier.getByUser().then(function(res) {
                if (res.success) {
                    var data = res.model;
                    if (data) {
                        self.id(data.id);
                        self.introduction(data.introduction);
                        self._introduction(self.introduction());
                    }

                    setTimeout(function() {
                        $(".autosize").textareaAutoSize().trigger("keyup");
                    }, 250);
                }
            })

            Kooboo.Supplier.getUserExpertiseList().then(function(res) {
                if (res.success) {
                    if (res.model.length) {
                        self.expertises(res.model.map(function(exp) {
                            return new Expertise(exp);
                        }))
                    }
                }
            })

            Kooboo.Currency.get().then(function(res) {
                if (res.success) {
                    self.currency(res.model.code);
                    self.currencySymbol(res.model.symbol);
                }
            })
        }

        this.isAllExpertiseValid = function() {
            var flag = true;
            self.expertises().forEach(function(exp) {
                flag = flag && exp.isValid();
            })

            flag && self.expertises().length;
            return flag;
        }

        this.showExpertiseError = function() {
            self.expertises().forEach(function(exp) {
                !exp.isValid() && exp.showError(true);
            })
        }

        this.getExpertisesData = function() {
            var data = [];
            self.expertises().forEach(function(exp) {
                data.push(exp.getValue());
            })
            return data;
        }

        this.onGet();
    }

    function Expertise(data) {
        var data = data || {};
        var self = this;

        this.id = ko.observable(data.id);

        this.showError = ko.observable(false);

        this.name = ko.validateField(data.name, {
            required: ''
        })

        this.price = ko.validateField(data.price, {
            required: ''
        })

        this.description = ko.observable(data.description);

        this.isValid = function() {
            return self.name.isValid() && self.price.isValid()
        }

        this.getValue = function() {
            var data = {
                name: self.name(),
                price: self.price(),
                description: self.description()
            }
            self.id() && (data.id = self.id());

            return data;
        }

        this.onEdit = function(cb) {
            self.isEditing(true);
            cb && cb();
        }

        this.onRemove = function(cb) {
            self.showError(false);
            if (self.id()) {
                Kooboo.Supplier.deleteExpertise({
                    id: self.id()
                }).then(function(res) {
                    if (res.success) {
                        window.info.done(Kooboo.text.info.delete.success);
                        cb && cb();
                    }
                })
            } else {
                cb && cb();
            }
        }

        this.onSave = function(cb) {
            if (self.isValid()) {
                Kooboo.Supplier.addOrUpdateExpertise(self.getValue()).then(function(res) {
                    if (res.success) {
                        self.isEditing(false);
                        self.showError(false);
                        cb && cb();
                    }
                })
            } else {
                self.showError(true);
            }
        }

        this.isEditing = ko.observable(!data.name);
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})