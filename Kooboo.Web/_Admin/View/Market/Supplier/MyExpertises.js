$(function() {
    var viewModel = function() {
        var self = this;
        this.id = ko.observable();

        this.showError = ko.observable(false);

        this.introduction = ko.validateField({
            required: ''
        })
        this.expertises = ko.observableArray();

        this.onAddExpertise = function() {
            self.expertises.push(new Expertise())
        }

        this.onRemoveExpertise = function(data, e) {
            data.showError(false);
            self.expertises.remove(data)
        }

        this.onSave = function() {
            if (self.introduction.isValid()) {
                if (self.isAllExpertiseValid()) {
                    var data = {
                        introduction: self.introduction(),
                        expertises: self.getExpertisesData()
                    };

                    if (self.id()) {
                        data.id = self.id()
                    }

                    Kooboo.Supplier.addOrUpdate(data).then(function(res) {
                        if (res.success) {
                            Kooboo.EventBus.publish("kb/witkey/supplier/update");
                            self.onGet();
                        }
                    })
                } else {
                    self.showExpertiseError();
                }
            } else {
                self.showError(true);
            }
        };

        this.onGet = function() {
            Kooboo.Supplier.getByUser().then(function(res) {
                if (res.success) {
                    var data = res.model;
                    if (data) {
                        self.id(data.id);
                        self.introduction(data.introduction);
                        self.expertises(data.expertises ? data.expertises.map(function(exp) {
                            return new Expertise(exp);
                        }) : [])
                    }
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
            return {
                name: self.name(),
                price: self.price(),
                description: self.description()
            }
        }

        setTimeout(function() {
            $(".autosize").textareaAutoSize().trigger("keyup");
        }, 250);
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})