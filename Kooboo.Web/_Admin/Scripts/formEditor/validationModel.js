(function() {

    window.koobooForm = {
        validationModel: {}
    };

    window.koobooForm.validationModel.required = function(valid) {
        var self = this;

        this.type = 'required';

        this.showError = ko.observable(false);

        this.message = ko.validateField(valid && valid.message, {
            required: ""
        })

        this.isValid = function() {
            return self.message.isValid();
        }
    }

    window.koobooForm.validationModel.min = function(valid) {
        var self = this;

        this.type = 'min';

        this.showError = ko.observable(false);

        this.min = ko.validateField(valid && valid.min, {
            required: ''
        });

        this.message = ko.validateField(valid && valid.message, {
            required: ''
        })

        this.isValid = function() {
            return self.min.isValid() && self.message.isValid();
        }
    }

    window.koobooForm.validationModel.max = function(valid) {
        var self = this;

        this.type = 'max';

        this.showError = ko.observable(false);

        this.max = ko.validateField(valid && valid.max, {
            required: ''
        })

        this.message = ko.validateField(valid && valid.message, {
            required: ''
        })

        this.isValid = function() {
            return self.max.isValid() && self.message.isValid();
        }
    }

    window.koobooForm.validationModel.numberRange = function(valid) {
        var self = this;

        this.type = 'range';

        this.showError = ko.observable(false);

        this.min = ko.validateField(valid && valid.min, {
            required: ''
        })

        this.max = ko.validateField(valid && valid.max, {
            required: ''
        })

        this.message = ko.validateField(valid && valid.message, {
            required: ''
        })

        this.isValid = function() {
            return self.min.isValid() && self.max.isValid() && self.message.isValid();
        }
    }

    window.koobooForm.validationModel.minStringLength = function(valid) {
        var self = this;

        this.type = 'minLength';

        this.showError = ko.observable(false);

        this.minLength = ko.validateField(valid && valid.minLength, {
            required: ''
        })

        this.message = ko.validateField(valid && valid.message, {
            required: ''
        })

        this.isValid = function() {
            return self.minLength.isValid() && self.message.isValid();
        }
    }

    window.koobooForm.validationModel.maxStringLength = function(valid) {
        var self = this;

        this.type = 'maxLength';

        this.showError = ko.observable(false);

        this.maxLength = ko.validateField(valid && valid.maxLength, {
            required: ''
        })

        this.message = ko.validateField(valid && valid.message, {
            required: ''
        })

        this.isValid = function() {
            return self.maxLength.isValid() && self.message.isValid();
        }
    }

    window.koobooForm.validationModel.regex = function(valid) {
        var self = this;

        this.type = 'regex';

        this.showError = ko.observable(false);

        this.regex = ko.validateField(valid && valid.regex, {
            required: ''
        })

        this.message = ko.validateField(valid && valid.message, {
            required: ''
        })

        this.isValid = function() {
            return self.regex.isValid() && self.message.isValid();
        }
    }

    window.koobooForm.validationModel.minChecked = function(valid) {
        var self = this;

        this.type = 'minChecked';

        this.showError = ko.observable(false);

        this.minChecked = ko.validateField(valid && valid.minChecked, {
            required: ''
        });

        this.message = ko.validateField(valid && valid.message, {
            required: ''
        });

        this.isValid = function() {
            return self.minChecked.isValid() && self.message.isValid();
        }
    }

    window.koobooForm.validationModel.maxChecked = function(valid) {
        var self = this;

        this.type = 'maxChecked';

        this.showError = ko.observable(false);

        this.maxChecked = ko.validateField(valid && valid.maxChecked, {
            required: ''
        });

        this.message = ko.validateField(valid && valid.message, {
            required: ''
        });

        this.isValid = function() {
            return self.maxChecked.isValid() && self.message.isValid();
        }
    }

    window.koobooForm.validationModel.email = function(valid) {
        var self = this;

        this.type = 'email';

        this.showError = ko.observable(false);

        this.message = ko.validateField(valid && valid.message, {
            required: ''
        })

        this.isValid = function() {
            return self.message.isValid();
        }
    }
})()