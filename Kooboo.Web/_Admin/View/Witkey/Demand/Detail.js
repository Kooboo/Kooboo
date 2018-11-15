$(function() {

    var detailModel = function() {
        var self = this;

        this.id = ko.observable(Kooboo.getQueryString('id'));

        this.isOwner = ko.observable();
        this.showingProposal = ko.observable();
        this.isProposalUser = ko.pureComputed(function() {
            return !!self.showingProposal();
        });
        this.isSelectedProposal = ko.pureComputed(function() {
            return self.showingProposal() && self.showingProposal().winTheBidding;
        })
        this.proposalViewingMode = ko.observable();

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
            return self.status() == 'tendering';
        })
        this.isEndOfTendering = ko.pureComputed(function() {
            return self.status() == 'endoftendering';
        })

        this.proposals = ko.observableArray();
        this.showProposalModal = ko.observable(false);
        this.onShowProposalModal = function() {
            self.proposalViewingMode('edit');
            self.showProposalModal(true);
        }
        this.onDeleteMyProposal = function() {
            if (confirm(Kooboo.text.confirm.recallProposal)) {
                Kooboo.Demand.deleteProposal({
                    proposalId: self.showingProposal().id
                }).then(function(res) {
                    if (res.success) {
                        Kooboo.EventBus.publish("kb/demand/proposal/update")
                    }
                })
            }
        }
        this.onViewProposalDetail = function(data, e) {
            Kooboo.Demand.getProposal({
                proposalId: data.id
            }).then(function(res) {
                if (res.success) {
                    self.showingProposal(res.model);
                    self.proposalViewingMode('view');
                    self.showProposalModal(true);
                }
            })
        }

        this.getDetail = function(cb) {
            Kooboo.Demand.get({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.title(res.model.title);
                    self.description(res.model.description);
                    self.userName(res.model.userName);
                    self.isOwner(res.model.isOwner);
                    self.currency(res.model.currency);
                    self.budget(res.model.symbol + res.model.budget);
                    self.skills(res.model.skills);

                    var start = new Date(res.model.startDate);
                    self.startDate(start.toLocaleDateString());
                    var end = new Date(res.model.endDate);
                    self.endDate(end.toLocaleDateString());
                    self.status(res.model.status.value.toLowerCase());
                    self.displayStatus(res.model.status.displayName);

                    cb && cb();
                }
            })

            self.getMyProposal();
            self.getProposalList();
        }

        this.getMyProposal = function() {
            if (!self.isOwner()) {
                Kooboo.Demand.getUserProposal({
                    demandId: self.id()
                }).then(function(res) {
                    if (res.success) {
                        self.showingProposal(res.model);
                    }
                })
            }
        }

        this.getProposalList = function() {
            Kooboo.Demand.getProposalList({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.proposals(res.model.list.map(function(item) {
                        return {
                            id: item.id,
                            userName: item.userName,
                            duration: item.duration + ' Day' + (item.duration > 1 ? 's' : ''),
                            budget: item.symbol + item.budget,
                            currency: item.currency,
                            winTheBidding: item.winTheBidding
                        }
                    }));
                }
            })
        }

        this.getCommentList = function() {
            Kooboo.Demand.getPublicCommentList({
                demandId: self.id()
            }).then(function(res) {
                if (res.success) {
                    // debugger;
                }
            })

            // if (self.isProposalUser()) {
            Kooboo.Demand.getPrivateCommentList({
                    demandId: self.id()
                }).then(function(res) {
                    if (res.success) {
                        debugger;
                    }
                })
                // }
        }

        this.getCommentList();

        this.getDetail(function() {
            $(".autosize").textareaAutoSize().trigger("keyup");
        });

        Kooboo.EventBus.subscribe("kb/demand/proposal/update", function() {
            self.getMyProposal();
            self.getProposalList();
        })

        Kooboo.EventBus.subscribe("kb/witkey/demand/reply/refresh", function(data) {
            debugger
            if (data.parentCommentId == Kooboo.Guid.Empty) {

            } else {
                self.getCommentList();
            }
        })
    }

    var vm = new detailModel();
    ko.applyBindings(vm, document.getElementById('main'))
})