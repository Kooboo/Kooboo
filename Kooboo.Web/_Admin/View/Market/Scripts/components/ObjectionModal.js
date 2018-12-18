(function() {
    Kooboo.loadJS([
        "/_Admin/Scripts/kobindings.textError.js"
    ]);

    var template = Kooboo.getTemplate("/_Admin/Market/Scripts/components/ObjectionModal.html");

    ko.components.register('objection-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    Kooboo.Demand.getDemandObjection({
                        demandId: self.demandId()
                    }).then(function(res) {
                        if (res.success) {
                            self._isShow(true);
                            if (res.model) {
                                self.mode('preview');
                                self.description(res.model.description);
                                self.contact(res.model.contact);
                            } else {
                                self.mode('edit');
                            }
                        }
                    })
                } else {
                    self._isShow(false);
                }
            })

            this._isShow = ko.observable(false);

            this.demandId = params.demandId;

            this.mode = ko.observable('preview');

            this.showError = ko.observable(false);

            this.contact = ko.validateField({
                required: ''
            })

            this.description = ko.validateField({
                required: ''
            })

            this.onHide = function() {
                self.contact('');
                self.description('');
                self.showError(false);
                self.isShow(false);
            }

            this.isValid = function() {
                return self.contact.isValid() && self.description.isValid();
            }

            this.onSubmit = function() {
                if (self.isValid()) {
                    Kooboo.Demand.raiseObjection({
                        demandId: self.demandId(),
                        description: self.description(),
                        contact: self.contact()
                    }).then(function(res) {
                        if (res.success) {
                            window.info.done();
                            self.onHide()
                        }
                    })
                } else {
                    self.showError(false);
                }
            }
        },
        template: template
    })
})()