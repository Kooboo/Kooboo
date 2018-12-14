$(function() {
    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.showDemandModal = ko.observable(false);
        this.onCreateDemand = function() {
            self.showDemandModal(true);
        }

        this.getList = function(page) {
            Kooboo.Demand.getList({
                pageNr: page || 1
            }).then(function(res) {
                if (res.success) {
                    self.handleData(res.model);
                }
            })
        }

        this.handleData = function(data) {
            self.pager(data);

            function getText(str) {
                var dom = $('<div>');
                $(dom).html(str);
                return $(dom).text();
            }

            var docs = data.list.map(function(item) {
                var date = new Date(item.createTime);
                return {
                    id: item.id,
                /*    title: {
                        text: item.title,
                        url: Kooboo.Route.Get(Kooboo.Route.Demand.DetailPage, {
                            id: item.id
                        }),
                        newWindow: true
                    },
                    description: getText(item.description),*/
                    article: {
                        title: item.title,
                        description: getText(item.description),
                        url: Kooboo.Route.Get(Kooboo.Route.Demand.DetailPage, {
                            id: item.id
                        }),
                        class: "title",
                        newWindow: true
                    },
                    demander: {
                        text: item.userName,
                        class: 'label-sm gray'
                    },
                    budget: {
                        text: item.symbol + item.budget,
                        tooltip: item.currency,
                        class: 'label-sm label-info'
                    },
                    date: date.toDefaultLangString(),
                    startDate: {
                        text: new Date(item.startDate).toKBDateString(),
                        class: 'label-sm gray'
                    },
                    endDate: {
                        text: new Date(item.endDate).toKBDateString(),
                        class: 'label-sm gray'
                    },
                    proposalCount: {
                        text: item.proposalCount,
                        class: 'badge badge-info'
                    },
                }
            })

            var data = {
                docs: docs,
                columns: [{
                    displayName: 'Article',
                    fieldName: 'article',
                    type: 'article'
                    }, 
                    /*{
                    displayName: 'Title',
                    fieldName: 'title',
                    type: 'link'
                }, {
                    displayName: 'Description',
                    fieldName: 'description',
                    type: 'text'
                }, */
                {
                    displayName: 'Budget',
                    fieldName: 'budget',
                    type: 'label',
                    showClass: 'table-short'
                }, {
                    displayName: 'Start date',
                    fieldName: 'startDate',
                    type: 'label',
                    showClass: 'table-short'
                }, {
                    displayName: 'End date',
                    fieldName: 'endDate',
                    type: 'label',
                    showClass: 'table-short'
                }, {
                    displayName: 'Proposals',
                    fieldName: 'proposalCount',
                    type: 'badge',
                    showClass: 'table-short'
                }, {
                    displayName: 'Demander',
                    fieldName: 'demander',
                    type: 'label',
                    showClass: 'table-short'
                }, {
                    displayName: 'Create time',
                    fieldName: 'date',
                    type: 'text',
                    showClass: 'table-short'
                }],
                kbType: Kooboo.Demand.name,
                unselectable: true
            };

            self.tableData(data);
        }

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            self.getList(page);
        })

        this.getList();
    }

    viewModel.prototype = new Kooboo.tableModel(Kooboo.Demand.name);
    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})