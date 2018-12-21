$(function() {
    var CURRENT_USER_ID = '';

    var MESSAGE_INTERVAL = null;

    var detailModel = function() {
        var self = this;

        this.id = ko.observable(Kooboo.getQueryString('id'));

        this.isOwner = ko.observable();
        this.showingProposal = ko.observable();
        this.isProposalUser = ko.pureComputed(function() {
            return !!self.showingProposal();
        });
        this.isSelectedProposal = ko.pureComputed(function() {
            return self.showingProposal() && self.showingProposal().isTaken;
        })
        this.proposalId = ko.pureComputed(function() {
            return self.showingProposal().id;
        })
        this.successfulBidding = ko.pureComputed(function() {
            return self.showingProposal() && self.showingProposal().isTaken;
        })
        this.proposalViewingMode = ko.observable();

        this.title = ko.observable();
        this.description = ko.observable();
        this.attachments = ko.observableArray();
        this.userName = ko.observable();
        this.currency = ko.observable();
        this.currencySymbol = ko.observable();
        this.budget = ko.observable();
        this.skills = ko.observableArray();
        this.startDate = ko.observable();
        this.endDate = ko.observable();
        this.duration = ko.pureComputed(function() {
            return (self.startDate() || '') + ' - ' + (self.endDate() || '');
        })
        this.status = ko.observable();
        this.displayStatus = ko.observable();

        this.isOpening = ko.pureComputed(function() {
            return self.status() == 'open';
        })
        this.isTaken = ko.observable();
        this.isClose = ko.observable();
        this.isDemandInvalid = ko.pureComputed(function() {
            return self.status() == 'invalid';
        })
        this.takenProposalId = ko.pureComputed(function() {
            return self.takenProposal() && self.takenProposal().id;
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
            if (confirm(Kooboo.text.confirm.demand.takeProposal)) {
                Kooboo.Demand.acceptProposal({
                    proposalId: data.id
                }).then(function(res) {
                    if (res.success) {
                        Kooboo.EventBus.publish("kb/market/component/cashier/show", res.model);
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
                    self.isTaken(res.model.isTaken);
                    self.isClose(res.model.isClose);
                    self.currency(res.model.currency);
                    self.currencySymbol(res.model.symbol);
                    self.budget(res.model.symbol + res.model.budget);
                    self.skills(res.model.skills);

                    var start = new Date(res.model.startDate);
                    self.startDate(start.toLocaleDateString());
                    var end = new Date(res.model.endDate);
                    self.endDate(end.toLocaleDateString());
                    self.status(res.model.status.value.toLowerCase());
                    self.displayStatus(res.model.status.displayName);

                    cb && cb();

                    self.getMyProposal(function() {
                        self.getProposalList();
                    });
                }
            })
        }

        this.showDemandModal = ko.observable(false);
        this.currentDemand = ko.observable();
        this.onUpdateDemand = function() {
            self.showDemandModal(true);
        }

        this.onFinishTheDemand = function(isAccepted) {
            if (confirm(Kooboo.text.confirm.demand.acceptDelivery)) {
                Kooboo.Demand.complete({
                    id: self.id(),
                    isAccepted: isAccepted
                }).then(function(res) {
                    if (res.success) {
                        location.reload();
                    }
                })
            }
        }

        this.hasCurrentUserMadeABid = ko.observable(false);
        this.isTakenByCurrentUser = ko.observable(false);

        this.myProposalId = ko.observable();
        this.getMyProposal = function(cb) {
            if (!self.isOwner()) {
                Kooboo.Demand.getUserProposal({
                    demandId: self.id()
                }).then(function(res) {
                    if (res.success) {
                        var proposal = res.model;
                        if (proposal.id !== Kooboo.Guid.Empty) {
                            self.hasCurrentUserMadeABid(true);
                            self.showingProposal(res.model);
                            self.myProposalId(res.model && res.model.id);
                        } else {
                            self.hasCurrentUserMadeABid(false);
                        }
                        cb && cb();
                    }
                })
            } else {
                cb && cb();
            }
        }

        this.getProposalList = function() {
            Kooboo.Demand.proposalListByDemand({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.proposals(res.model.list.map(function(item) {
                        if (item.isTaken) {
                            self.takenProposal(item);
                            if (self.isOwner() && !self.isClose()) {
                                MESSAGE_INTERVAL = setInterval(function() {
                                    self.getDeliveryMessages()
                                }, 2000)
                            } else if (self.myProposalId() && (self.myProposalId() == self.takenProposal().id)) {
                                self.isTakenByCurrentUser(true);
                                if (!self.isClose()) {
                                    MESSAGE_INTERVAL = setInterval(function() {
                                        self.getDeliveryMessages()
                                    }, 2000)
                                }
                            }
                        }
                        return {
                            id: item.id,
                            firstLetter: item.userName.split('')[0].toUpperCase(),
                            userName: item.userName,
                            duration: item.duration +' '+ Kooboo.text.component.proposalModal.day + (item.duration > 1 ? Kooboo.text.component.proposalModal.s : ''),
                            budget: item.symbol + item.budget,
                            description: item.description.split('\n').join('<br>'),
                            currency: item.currency,
                            isTaken: item.isTaken,
                            attachments: item.attachments ? item.attachments.map(function(item) {
                                item.url = '/_api/attachment/getFile?id=' + item.id + '&fileName=' + item.fileName;
                                return item;
                            }) : [],
                            isMyProposal: ko.observable(self.myProposalId() ? self.myProposalId() == item.id : false)
                        }
                    }));
                }
            })
        }

        this.takenProposal = ko.observable();

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

        Kooboo.EventBus.subscribe('kb/market/cashier/done', function() {
            location.reload();
        })

        Kooboo.EventBus.subscribe("kb/demand/proposal/update", function() {
            self.getMyProposal();
            self.getProposalList();
        })

        Kooboo.EventBus.subscribe("kb/witkey/demand/reply/refresh", function(data) {
            if (data.parentId == Kooboo.Guid.Empty) {
                self.getCommentList();
            } else {
                var current = _.find(self.publicCommentList(), function(item) {
                    return item.id() == data.parentId
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

        this.messages = ko.observableArray();
        this.getDeliveryMessages = function(cb) {
            Kooboo.Demand.getPrivateCommentList({
                proposalId: self.takenProposal().id
            }).then(function(res) {
                if (res.success) {
                    self.messages(res.model.list.map(function(item) {
                        return new Message(item);
                    }));
                    cb && cb();
                }
            })
        }

        Kooboo.EventBus.subscribe('kb/market/chat/sent', function(cb) {
            self.getDeliveryMessages(cb);
        })

        Kooboo.SPA.beforeUnload = function() {
            MESSAGE_INTERVAL && clearInterval(MESSAGE_INTERVAL);
            return 'refresh';
        }
    }

    var vm = new detailModel();
    ko.applyBindings(vm, document.getElementById('main'))

    function Comment(data) {
        if (data.content && data.content.indexOf('\n') > -1) {
            data.content = data.content.split('\n').join('<br>')
        }
        var date = new Date(data.lastModified);
        this.id = ko.observable(data.id);
        this.firstLetter = data.userName.split('')[0].toUpperCase();
        this.userName = data.userName;
        this.content = data.content || '';
        this.date = date.toDefaultLangString();
        this.commentCount = ko.observable(data.commentCount);
        this.showSubComment = ko.observable(false);
        this.subCommentList = ko.observableArray([]);
    }

    function Message(data) {
        if (!CURRENT_USER_ID) {
            CURRENT_USER_ID = localStorage.getItem('_kooboo_api_user');
        }

        var date = new Date(data.lastModified);

        this.firstLetter = ko.observable(data.userName.split('')[0].toUpperCase());
        this.content = ko.observable(data.content);
        this.isCurrentUser = ko.observable(data.userId == CURRENT_USER_ID);
        this.userName = ko.observable(this.isCurrentUser() ? Kooboo.text.market.supplier.me : data.userName);
        this.date = ko.observable(date.toDefaultLangString());
        this.attachment = ko.observable(data.attachments ? data.attachments.map(function(item) {
            item.url = '/_api/attachment/getFile?id=' + item.id + '&fileName=' + item.fileName;
            return item;
        })[0] : '');
    }
})