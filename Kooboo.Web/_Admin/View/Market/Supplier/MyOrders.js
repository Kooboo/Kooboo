$(function() {
    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.getList = function() {
            Kooboo.Supplier.getOrdersByUser().then(function(res) {
                if (res.success) {
                    self.handleData(res.model);
                }
            })
        }

        this.handleData = function(data) {
            self.pager(data);

            var docs = data.list.map(function(item) {
                return {
                    id: item.id,
                    expertise: item.expertise,
                    price: item.symbol + item.price,
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
                    displayName: 'Expertise',
                    fieldName: 'expertise',
                    type: 'text'
                }, {
                    displayName: 'Price',
                    fieldName: 'price',
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