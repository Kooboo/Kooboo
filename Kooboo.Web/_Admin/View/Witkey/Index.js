$(function() {
    var NAV_DISCUSSIONS_TOP = $('#nav_discussions')[0].getBoundingClientRect().top,
        NAV_DEMAND_TOP = $('#nav_demand')[0].getBoundingClientRect().top;

    var viewModel = function() {
        var self = this;

        this.userName = ko.observable();

        Kooboo.User.get().then(function(res) {
            if (res.success) {
                self.userName(res.model.userName);
            }
        })

        this.discussions = ko.observableArray();

        this.demands = ko.observableArray();

        Kooboo.Discussion.getList({
            pageSize: 8
        }).then(function(res) {
            if (res.success) {
                self.discussions(res.model.list);
            }
        })

        this.deleteDemand = function(m, e) {
            Kooboo.Demand.Delete({
                id: m
            }).then(function(res) {
                if (res.success) {
                    self.getDemandList();
                }
            })
        }

        this.showDiscussionModal = ko.observable(false);
        this.onAddDiscussion = function() {
            self.showDiscussionModal(true);
        }

        this.showDemandModal = ko.observable(false);
        this.onAddDemand = function() {
            self.showDemandModal(true);
        }

        this.getDemandList = function() {
            Kooboo.Demand.getList({
                pageSize: 8
            }).then(function(res) {
                if (res.success) {
                    self.demands(res.model.list.map(function(item) {
                        return {
                            id: item.id,
                            title: item.title,
                            userName: item.userName,
                            url: Kooboo.Route.Get(Kooboo.Route.Demand.DetailPage, {
                                id: item.id
                            }),
                            startDate: new Date(item.startDate).toLocaleDateString(),
                            endDate: new Date(item.endDate).toLocaleDateString(),
                            budget: item.symbol + item.budget,
                            currency: item.currency,
                            skills: item.skills,
                            createdTime: new Date(item.createTime).toDefaultLangString(),
                            proposalCount: item.proposalCount
                        }
                    }));
                }
            })
        }

        this.getDemandList();

        this.isSupplier = ko.observable(false);
        this.mySupplierIntro = ko.observable();

        this.showApplySupplier = ko.observable(false);
        this.onApplySupplier = function() {
            self.showApplySupplier(true);
        }
        this.onEditSupplier = function() {
            Kooboo.Supplier.getByUser().then(function(res) {
                if (res.success) {
                    self.mySupplierIntro(res.model);
                    self.showApplySupplier(true);
                }
            })
        }
        this.suppliers = ko.observableArray();

        this.onViewSupplier = function(data, e) {
            Kooboo.Supplier.get({
                id: data.id
            }).then(function(res) {
                if (res.success) {

                }
            })
        }

        Kooboo.Supplier.getList({
            pageSize: 12
        }).then(function(res) {
            if (res.success) {
                self.suppliers(res.model.list.map(function(item) {
                    item.detailUrl = Kooboo.Route.Get(Kooboo.Route.Supplier.DetailPage, {
                        id: item.id
                    })
                    return item;
                }));
            }
        })

        Kooboo.Supplier.isSupplier().then(function(res) {
            if (res.success) {
                self.isSupplier(res.model);
            }
        })

        Kooboo.EventBus.subscribe('kb/component/demand-modal/saved', function() {
            self.getDemandList();
        })
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));

    window.onpopstate = function(e) {
        e.preventDefault();
    }

    $(window).scroll(function() {
        var discussionsInfo = $('#discussions')[0].getBoundingClientRect(),
            demandInfo = $('#demands')[0].getBoundingClientRect();

        var discussionsRange = discussionsInfo.top + discussionsInfo.height - 15,
            demandRange = demandInfo.top + demandInfo.height - 15;

        $('#side-nav li').removeClass('active');

        if (discussionsRange > NAV_DISCUSSIONS_TOP) {
            $('#nav_discussions').addClass('active');
        } else if (demandRange > NAV_DEMAND_TOP) {
            $('#nav_demand').addClass('active');
        }
    })
})