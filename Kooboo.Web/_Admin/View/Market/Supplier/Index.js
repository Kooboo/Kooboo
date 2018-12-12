$(function() {
    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.getList = function(page) {
            Kooboo.Supplier.list({
                page: page || 1
            }).then(function(res) {
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
                    name: {
                        text: item.name,
                        url: Kooboo.Route.Get(Kooboo.Route.Supplier.ExpertisePage, {
                            id: item.id
                        }),
                        newWindow: true
                    },
                    description: item.description,
                    price: {
                        text: item.symbol + item.price,
                        class: 'label-sm label-info',
                        tooltip: item.currency
                    },
                    orgName: {
                        text: item.orgName,
                        class: 'label-sm gray'
                    },
                    view: {
                        iconClass: 'fa-eye',
                        url: Kooboo.Route.Get(Kooboo.Route.Supplier.ExpertisePage, {
                            id: item.id
                        }),
                        newWindow: true
                    }
                }
            })

            var data = {
                docs: docs,
                columns: [{
                    displayName: 'Name',
                    fieldName: 'name',
                    type: 'link',
                    showClass: 'table-short'
                }, {
                    displayName: 'Description',
                    fieldName: 'description',
                    type: 'text'
                }, {
                    displayName: 'Price',
                    fieldName: 'price',
                    type: 'label',
                    showClass: 'table-short'
                }, {
                    displayName: "Supplier",
                    fieldName: "orgName",
                    type: 'label',
                    showClass: 'table-short'
                }],
                tableActions: [{
                    fieldName: 'view',
                    type: 'link-icon'
                }],
                kbType: Kooboo.Supplier.name,
                unselectable: true
            };

            self.tableData(data);
        }

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            self.getList(page);
        })

        this.getList();
    }

    viewModel.prototype = new Kooboo.tableModel(Kooboo.Supplier.name);
    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})