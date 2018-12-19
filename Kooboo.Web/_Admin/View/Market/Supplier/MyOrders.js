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
                    article: {
                        title: item.name,
                        url: Kooboo.Route.Get(Kooboo.Route.Supplier.OrderPage, {
                            id: item.id
                        }),
                        class: "title",
                        newWindow: true
                    },
                    price: {
                        text: symbol + item.totalAmount,
                        class: 'label-sm label-info',
                        tooltip: item.currency
                    },
                    status: {
                        text: item.status,
                        class: 'label-sm label-info'
                    },
                    orgName: {
                        text: item.orgName,
                        class: 'label-sm gray'
                    } 
                }
            })

            self.tableData({
                docs: docs,
                columns: [{
                    displayName: Kooboo.text.common.name,
                    fieldName: 'article',
                    type: 'summary'
                }, {
                    displayName: Kooboo.text.common.price,
                    fieldName: 'price',
                    type: 'label',
                    showClass: "table-short"
                }, {
                    displayName: Kooboo.text.market.supplier.status,
                    fieldName: 'status',
                    type: 'label',
                    showClass: "table-short"
                }, {
                    displayName:  Kooboo.text.common.Supplier,
                    fieldName: 'orgName',
                    type: 'label',
                    showClass: "table-short"
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