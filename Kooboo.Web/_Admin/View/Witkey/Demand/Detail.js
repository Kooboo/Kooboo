$(function() {

    var detailModel = function() {
        var self = this;

        this.id = ko.observable(Kooboo.getQueryString('id'));

        this.myProposal = ko.observable();
        this.isProposalUser = ko.pureComputed(function() {
            return !!self.myProposal();
        });
        this.isSelectedProposal = ko.pureComputed(function() {
            return self.myProposal() && self.myProposal().winTheBidding;
        })

        this.title = ko.observable();
        this.description = ko.observable();
        this.userName = ko.observable();
        this.currency = ko.observable();
        this.budget = ko.observable();
        this.skills = ko.observableArray();
        this.startDate = ko.observable();
        this.endDate = ko.observable();
        this.duration = ko.pureComputed(function() {
            return (self.startDate() || '') + ' - ' + (self.endDate() || '');
        })
        this.status = ko.observable();
        this.displayStatus = ko.observable();

        this.isTendering = ko.pureComputed(function() {
            return self.status() !== 'Invalid';
        })

        this.proposals = ko.observableArray();
        this.showProposalModal = ko.observable(false);
        this.onShowProposalModal = function() {
            self.showProposalModal(true);
        }
        this.onDeleteMyProposal = function() {
            if (confirm(Kooboo.text.confirm.recallProposal)) {
                Kooboo.Demand.deleteProposal({
                    proposalId: self.myProposal().id
                }).then(function(res) {
                    if (res.success) {
                        Kooboo.EventBus.publish("kb/demand/proposal/update")
                    }
                })
            }
        }

        this.getDetail = function(cb) {
            Kooboo.Demand.get({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.title(res.model.title);
                    self.description(res.model.description);
                    self.userName(res.model.userName);
                    self.currency(res.model.currency);
                    self.budget(res.model.symbol + res.model.budget);
                    self.skills(res.model.skills);

                    var start = new Date(res.model.startDate);
                    self.startDate(start.toLocaleDateString());
                    var end = new Date(res.model.endDate);
                    self.endDate(end.toLocaleDateString());
                    self.status(res.model.status.value);
                    self.displayStatus(res.model.status.displayName);

                    cb && cb();
                }
            })

            self.getMyProposal();
            self.getProposalList();
        }

        this.getMyProposal = function() {
            Kooboo.Demand.getUserProposal({
                demandId: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.myProposal(res.model);
                }
            })
        }

        this.getProposalList = function() {
            Kooboo.Demand.getProposalList({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.proposals(res.model.list.map(function(item) {
                        return {
                            userName: item.userName,
                            duration: item.duration + ' Day' + (item.duration > 1 ? 's' : ''),
                            budget: item.symbol + item.budget,
                            currency: item.currency
                        }
                    }));
                }
            })
        }

        this.getDetail(function() {
            $(".autosize").textareaAutoSize().trigger("keyup");
        });

        Kooboo.EventBus.subscribe("kb/demand/proposal/update", function() {
            self.getMyProposal();
            self.getProposalList();
        })
    }

    var vm = new detailModel();
    ko.applyBindings(vm, document.getElementById('main'))
})