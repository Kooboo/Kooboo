$(function() {
    var viewModel = function() {
        var self = this;

        this.onGet = function() {
            Kooboo.Supplier.getUserExpertiseList().then(function(res) {
                if (res.success) {
                    if (res.model.length) {

                        var docs = res.model.map(function(item) {
                            return {
                                id: item.id,
                                name: item.name,
                                price: item.symbol + item.price,
                                description: item.description,
                                edit: {
                                    iconClass: 'fa-pencil',
                                    url: 'kb/expertise/edit'
                                },
                                delete: {
                                    class: 'red',
                                    iconClass: 'fa-trash',
                                    url: 'kb/expertise/delete'
                                }
                            }
                        })

                        self.tableData({
                            docs: docs,
                            columns: [{
                                displayName: 'Name',
                                fieldName: 'name',
                                type: 'text',
                                showClass: 'table-short'
                            }, {
                                displayName: 'Price',
                                fieldName: 'price',
                                type: 'text',
                                showClass: 'table-short'
                            }, {
                                displayName: 'Description',
                                fieldName: 'description',
                                type: 'text'
                            }],
                            tableActions: [{
                                fieldName: 'edit',
                                type: 'communication-icon-btn'
                            }, {
                                fieldName: 'delete',
                                type: 'communication-icon-btn'
                            }],
                            onDelete: function(docs) {
                                if (confirm(Kooboo.text.confirm.deleteItems)) {
                                    Kooboo.Supplier.deleteExpertises({
                                        ids: docs.map(function(doc) {
                                            return doc.id
                                        })
                                    }).then(function(res) {
                                        if (res.success) {
                                            self.onGet();
                                        }
                                    })
                                }
                            }
                        })
                    }
                }
            })
        }

        this.onAdd = function() {
            Kooboo.EventBus.publish('kb/market/component/expertise-modal/show');
        }

        this.onGet();

        Kooboo.EventBus.subscribe('kb/expertise/edit', function(data) {
            Kooboo.EventBus.publish('kb/market/component/expertise-modal/show', data.id);
        })

        Kooboo.EventBus.subscribe('kb/market/component/expertise-modal/updated', function() {
            self.onGet();
        })

        Kooboo.EventBus.subscribe('kb/expertise/delete', function(data) {
            if (confirm(Kooboo.text.confirm.deleteItem)) {
                Kooboo.Supplier.deleteExpertise(data).then(function(res) {
                    if (res.success) {
                        window.info.done(Kooboo.text.info.delete.success);
                        self.onGet();
                    }
                })
            }
        })
    }

    viewModel.prototype = new Kooboo.tableModel(Kooboo.Supplier.name);
    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})