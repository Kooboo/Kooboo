$(function() {
    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.getList = function() {
            Kooboo.Supplier.myOrdersOut().then(function(res) {
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
                    name: {
                        text: item.name,
                        url: Kooboo.Route.Get(Kooboo.Route.Supplier.OrderPage, {
                            id: item.id
                        })
                    },
                    amount: symbol + item.totalAmount,
                    status: {
                        text: item.status,
                        class: 'label-sm label-info'
                    }
                }
            })

            self.tableData({
                docs: docs,
                columns: [{
                    displayName: Kooboo.text.common.name,
                    fieldName: 'name',
                    type: 'link'
                }, {
                    displayName: Kooboo.text.common.amount,
                    fieldName: 'amount',
                    type: 'text'
                }, {
                    displayName: Kooboo.text.market.supplier.status,
                    fieldName: 'status',
                    type: 'label'
                }],
                tableActions: [],
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