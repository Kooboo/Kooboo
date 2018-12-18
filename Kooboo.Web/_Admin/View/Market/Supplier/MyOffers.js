$(function() {
    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.getList = function() {
            Kooboo.Supplier.myOrdersIn().then(function(res) {
                if (res.success) {
                    self.handleData(res.model);
                }
            })
        }

        this.handleData = function(data) {
            self.pager(data);

            var docs = data.list.map(function(item) {
                var symbol = item.symbol ? item.symbol : item.currency;
                return {
                    id: item.id,
                    name: item.name,
                    amount: symbol + item.totalAmount,
                    status: {
                        text: item.status,
                        class: 'label-sm label-info'
                    },
                    user: {
                        text: item.buyerOrgName,
                        class: 'label-sm gray'
                    },
                    view: {
                        iconClass: 'fa-eye',
                        url: Kooboo.Route.Get(Kooboo.Route.Supplier.OrderPage, {
                            id: item.id
                        }),
                        newWindow: true
                    }
                }
            })

            self.tableData({
                docs: docs,
                columns: [{
                    displayName: Kooboo.text.common.name,
                    fieldName: 'name',
                    type: 'text'
                }, {
                    displayName: Kooboo.text.common.amount,
                    fieldName: 'amount',
                    type: 'text'
                }, {
                    displayName: Kooboo.text.market.supplier.status,
                    fieldName: 'status',
                    type: 'label'
                }, {
                    displayName: Kooboo.text.market.supplier.orderUser,
                    fieldName: 'buyerOrgName',
                    type: 'label',
                }],
                tableActions: [{
                    fieldName: 'view',
                    type: 'link-icon'
                }],
                unselectable: true,
                kbType: Kooboo.Supplier.name
            })
        }

        this.getList();
    }

    viewModel.prototype = new Kooboo.tableModel(Kooboo.Supplier.name);
    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})