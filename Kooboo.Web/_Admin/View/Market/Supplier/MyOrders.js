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
                var symbol=item.symbol?item.symbol:item.currency;
                return {
                    id: item.id,
                    name: item.name,
                    amount: symbol + item.totalAmount,
                    status: {
                        text: item.status.displayName,
                        class: 'label-sm label-info'
                    },
                    view: {
                        iconClass: 'fa-eye',
                        url: Kooboo.Route.Get(Kooboo.Route.Supplier.DetailPage, {
                            id: item.id
                        }),
                        newWindow: true
                    }
                }
            })

            self.tableData({
                docs: docs,
                columns: [{
                    displayName: 'Name',
                    fieldName: 'name',
                    type: 'text'
                }, {
                    displayName: 'Amount',
                    fieldName: 'amount',
                    type: 'text'
                }, {
                    displayName: 'Status',
                    fieldName: 'status',
                    type: 'label'
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