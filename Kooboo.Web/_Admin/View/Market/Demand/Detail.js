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
        this.proposalId = ko.pureComputed(function() {
            return self.showingProposal().id;
        })
        this.successfulBidding = ko.pureComputed(function() {
            return self.showingProposal() && self.showingProposal().winTheBidding;
        })
        this.proposalViewingMode = ko.observable();

        this.title = ko.observable();
        this.description = ko.observable();
        this.attachments = ko.observableArray();
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
        this.isEndOfTender = ko.pureComputed(function() {
            return self.status() == 'endoftender';
        })
        this.isDemandClosed = ko.pureComputed(function() {
            return ['finished', 'unfinished'].indexOf(self.status()) > -1;
        })
        this.isDemandInvalid = ko.pureComputed(function() {
            return self.status() == 'invalid';
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
        this.onAcceptProposal = function(data, e) {
            if (confirm('you sure?')) {
                Kooboo.Demand.acceptProposal({
                    proposalId: data.id
                }).then(function(res) {
                    if (res.success) {
                        Kooboo.EventBus.publish("kb/demand/proposal/update");
                    }
                })
            }
        }

        this.getDetail = function(cb) {
            Kooboo.Demand.get({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.currentDemand(res.model);
                    self.title(res.model.title);
                    self.description(res.model.description);
                    self.attachments(res.model.attachments ? res.model.attachments.map(function(item) {
                        item.downloadUrl = '/_api/attachment/getFile?id=' + item.id + '&fileName=' + item.fileName;
                        return item;
                    }) : [])
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

        this.showDemandModal = ko.observable(false);
        this.currentDemand = ko.observable();
        this.onUpdateDemand = function() {
            self.showDemandModal(true);
        }

        this.onFinishTheDemand = function(isFinished) {
            debugger;
            if (confirm('You sure?')) {
                Kooboo.Demand.confirmDemandStatus({
                    id: self.id(),
                    isFinished: isFinished
                }).then(function(res) {
                    if (res.success) {
                        location.reload();
                    }
                })
            }
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
                        if (item.winTheBidding) {
                            self.winningProposal(item);
                        }

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

        this.winningProposal = ko.observable();

        this.publicCommentList = ko.observableArray();
        this.getCommentList = function() {
            Kooboo.Demand.getPublicCommentList({
                demandId: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.publicCommentList(res.model.list.map(function(item) {
                        return new Comment(item);
                    }));
                }
            })
        }
        this.getNestedCommentList = function(m) {
            Kooboo.Demand.getNestedPublicCommentList({
                commentId: m.id()
            }).then(function(res) {
                if (res.success) {
                    m.showSubComment(true);
                    m.subCommentList(res.model.map(function(item) {
                        return new Comment(item);
                    }));
                    $(".autosize").textareaAutoSize().trigger("keyup");
                }
            })
        }
        this.onToggleSubComment = function(m, e) {
            if (m.showSubComment()) {
                m.showSubComment(false);
            } else {
                self.getNestedCommentList(m);
            }
        }
        this.commentRendered = function() {
            Holder.run();
        }

        this.getCommentList();

        this.getDetail(function() {
            $(".autosize").textareaAutoSize().trigger("keyup");
        });

        this.showChatModal = ko.observable(false);
        this.onShowChatModal = function() {
            self.showChatModal(true);
        }
        this.onShowProposalChatModal = function(m) {
            if (m) {
                self.showingProposal(m);
            } else {
                self.showingProposal(self.winningProposal());
            }
            self.onShowChatModal();
        }

        this.showObjectionModal = ko.observable(false);
        this.onShowObjectionModal = function() {
            self.showObjectionModal(true);
        }

        Kooboo.EventBus.subscribe("kb/demand/proposal/update", function() {
            self.getMyProposal();
            self.getProposalList();
        })

        Kooboo.EventBus.subscribe("kb/witkey/demand/reply/refresh", function(data) {
            if (data.parentCommentId == Kooboo.Guid.Empty) {
                self.getCommentList();
            } else {
                var current = _.find(self.publicCommentList(), function(item) {
                    return item.id() == data.parentCommentId
                })
                if (current) {
                    current.commentCount(current.commentCount() + 1);
                    self.getNestedCommentList(current);
                }
            }
        })

        Kooboo.EventBus.subscribe("kb/component/demand-modal/saved", function() {
            location.reload();
        })
    }

    var vm = new detailModel();
    ko.applyBindings(vm, document.getElementById('main'))

    function Comment(data) {
        if (data.content && data.content.indexOf('\n') > -1) {
            data.content = data.content.split('\n').join('<br>')
        }
        var date = new Date(data.createTime);
        this.id = ko.observable(data.id);
        this.firstLetter = data.userName.split('')[0].toUpperCase();
        this.userName = data.userName;
        this.content = data.content || '';
        this.date = date.toDefaultLangString();
        this.commentCount = ko.observable(data.commentCount);
        this.showSubComment = ko.observable(false);
        this.subCommentList = ko.observableArray([]);
    }
})