(function () {
    Kooboo.loadJS([
        "/_Admin/Scripts/lib/jquery.textarea_autosize.min.js",
        "/_Admin/Scripts/kobindings.textError.js"
    ])

    var template = Kooboo.getTemplate("/_Admin/Market/Scripts/components/ProposalModal.html");

    ko.components.register('proposal-modal', {
        viewModel: function (params) {
            var self = this;

            this.showError = ko.observable(false);

            this.isShow = params.isShow;
            this.isShow.subscribe(function (show) {
                if (show) {
                    setTimeout(function () {
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
                        self.currencyCode(data.currency);
                        self.currencySymbol(data.symbol);
                        self.attachments(data.attachments || []);
                    }
                }
            })
            this.mode = params.mode;
            this.demandId = params.demandId;
            this.proposalId = ko.observable();
            this.currencyCode = params.currencyCode || ko.observable();
            this.currencySymbol = params.currencySymbol || ko.observable();

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

            this.onHide = function () {
                self.description('');
                self.budget('');
                self.duration('');
                self.showError(false);
                self.attachments([]);
                self.isShow(false);
            }

            this.isValid = function () {
                return self.description.isValid() &&
                    self.budget.isValid() &&
                    self.duration.isValid()
            }

            this.attachments = ko.observableArray();
            this.uploadFile = function (data, files) {
                var fd = new FormData();
                fd.append('filename', files[0].name);
                fd.append('file', files[0]);
                Kooboo.Attachment.uploadFile(fd).then(function (res) {
                    if (res.success) {
                        self.attachments.push(res.model);
                    }
                })
            }
            this.removeFile = function (data, e) {
                Kooboo.Attachment.deleteFile({
                    id: data.id
                }).then(function (res) {
                    if (res.success) {
                        self.attachments.remove(data);
                    }
                })
            }

            this.onSave = function () {
                if (self.isValid()) {
                    var data = {
                        demandId: self.demandId(),
                        description: self.description(),
                        duration: self.duration(),
                        budget: self.budget(),
                        attachments: self.attachments()
                    }

                    if (self.proposalId()) {
                        data.id = self.proposalId()
                    }

                    Kooboo.Demand.addOrUpdateProposal(data).then(function (res) {
                        if (res.success) {
                            self.onHide();
                            Kooboo.EventBus.publish("kb/demand/proposal/update");
                        }
                    })
                } else {
                    self.showError(true);
                }
            }

            this.onAcceptProposal = function () {
                if (confirm(Kooboo.text.confirm.market.sure)) {
                    Kooboo.Demand.acceptProposal({
                        proposalId: self.proposalId()
                    }).then(function (res) {
                        debugger;
                        if (res.success) {
                            self.onHide();
                            Kooboo.EventBus.publish("kb/market/component/cashier/show", res.model);
                            Kooboo.EventBus.publish("kb/demand/proposal/update");
                        }
                    })
                }
            }
        },
        template: template
    })
})()