(function() {
    var template = Kooboo.getTemplate("/_Admin/Witkey/Scripts/components/DemandModal.html");

    ko.components.register('demand-modal', {
        viewModel: function(params) {
            var self = this;

            this.showError = ko.observable(false);

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    Kooboo.Currency.get().then(function(res) {
                        if (res.success) {
                            self.currencyCode(res.model.code.toLowerCase());
                            self.currencySymbol(res.model.symbol);
                        }
                    })
                }
            })

            this.currencyCode = ko.observable();
            this.currencySymbol = ko.observable();

            this.title = ko.validateField({
                required: '',
                maxLength: {
                    value: 140,
                    message: Kooboo.text.validation.maxLength + 140
                }
            })

            this.skills = ko.observable();

            this.description = ko.validateField({
                required: ''
            })

            this.startDate = ko.validateField({
                required: ''
            })

            this.endDate = ko.validateField({
                required: ''
            })

            this.budget = ko.validateField({
                required: '',
                min: 0
            })

            this.attachments = ko.observableArray();

            this.uploadFile = function(data, files) {
                var fd = new FormData();
                fd.append('filename', files[0].name);
                fd.append('file', files[0]);
                Kooboo.Demand.uploadFile(fd).then(function(res) {
                    if (res.success) {
                        self.attachments.push(res.model);
                    }
                })
            }

            this.removeFile = function(data, e) {
                Kooboo.Demand.deleteFile({
                    id: data.id
                }).then(function(res) {
                    if (res.success) {
                        self.attachments.remove(data);
                    }
                })
            }

            // this.descriptionLoaded = ko.observable(false);

            this.isValid = function() {
                return self.title.isValid() &&
                    self.description.isValid() &&
                    self.startDate.isValid() &&
                    self.endDate.isValid() &&
                    self.budget.isValid();
            }

            this.onHide = function() {
                self.title('');
                self.description('');
                self.startDate('');
                self.endDate('');
                self.budget('');
                self.attachments([]);
                self.showError(false);
                self.isShow(false);
            }

            this.onPublish = function() {
                if (self.isValid()) {
                    Kooboo.Demand.addOrUpdate(self.getData()).then(function(res) {
                        if (res.success) {
                            Kooboo.EventBus.publish("kb/component/demand-modal/saved")
                            self.onHide();
                        }
                    })
                } else {
                    self.showError(true);
                }
            }

            this.getData = function() {
                return {
                    title: self.title(),
                    skills: self.skills().split(','),
                    description: self.description(),
                    attachments: self.attachments(),
                    startDate: self.startDate(),
                    endDate: self.endDate(),
                    budget: self.budget()
                }
            }
        },
        template: template
    })
})()