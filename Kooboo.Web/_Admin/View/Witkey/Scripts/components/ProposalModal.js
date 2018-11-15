(function() {
    var template = Kooboo.getTemplate("/_Admin/View/Witkey/Scripts/components/ProposalModal.html");

    ko.components.register('proposal-modal', {
        viewModel: function(params) {
            var self = this;

            this.showError = ko.observable(false);

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    setTimeout(function() {
                        $(".autosize").textareaAutoSize().trigger("keyup");
                    }, 280);

                    if (params.data()) {
                        var data = params.data();
                        self.proposalId(data.id);
                        self.userName(data.userName);
                        self.description(data.description);
                        self.duration(data.duration);
                        self.budget(data.budget);
                        self.displayBudget(data.symbol + data.budget);
                        self.displayDuration(data.duration + ' Day(s)');
                    }
                }
            })
            this.mode = params.mode;
            this.demandId = params.demandId;
            this.proposalId = ko.observable();
            this.currencyCode = params.currencyCode;
            this.currencySymbol = ko.pureComputed(function() {
                return self.currencyCode() && self.currencyCode().toLowerCase();
            })

            this.userName = ko.observable();
            this.displayBudget = ko.observable();
            this.displayDuration = ko.observable();

            this.description = ko.validateField({
                required: ''
            })

            this.budget = ko.validateField({
                required: ''
            })

            this.duration = ko.validateField({
                required: ''
            })

            this.onHide = function() {
                self.description('');
                self.budget('');
                self.duration('');
                self.showError(false);
                self.isShow(false);
            }

            this.isValid = function() {
                return self.description.isValid() &&
                    self.budget.isValid() &&
                    self.duration.isValid()
            }

            this.onSave = function() {
                if (self.isValid()) {
                    var data = {
                        demandId: self.demandId(),
                        description: self.description(),
                        duration: self.duration(),
                        budget: self.budget()
                    }

                    if (self.proposalId()) {
                        data.id = self.proposalId()
                    }

                    Kooboo.Demand.addOrUpdateProposal(data).then(function(res) {
                        if (res.success) {
                            self.onHide();
                            Kooboo.EventBus.publish("kb/demand/proposal/update");
                        }
                    })
                } else {
                    self.showError(true);
                }
            }

            this.onAcceptProposal = function() {
                if (confirm('you sure?')) {
                    Kooboo.Demand.acceptProposal({
                        proposalId: self.proposalId()
                    }).then(function(res) {
                        if (res.success) {
                            self.onHide();
                            Kooboo.EventBus.publish("kb/demand/proposal/update");
                        }
                    })
                }
            }
        },
        template: template
    })
})()