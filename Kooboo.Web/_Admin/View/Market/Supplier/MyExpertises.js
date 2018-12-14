$(function() {
    var viewModel = function() {
        var self = this;

        this.onGet = function() {
            Kooboo.Supplier.myList().then(function(res) {
                if (res.success) {
                    if (res.model.length) {

                        var docs = res.model.map(function(item) {
                            return {
                                id: item.id,
                                article: {
                                    title: item.name,
                                    description: item.description,
                                    url: Kooboo.Route.Get(Kooboo.Route.Supplier.ExpertisePage, {
                                        id: item.id
                                    }),
                                    class: "title",
                                    newWindow: true
                                },
                            /*    price: item.symbol + item.price,*/
                                price: {
                                    text: item.symbol + item.price,
                                    class: 'label-sm label-info',
                                    tooltip: item.currency
                                },
                                orgName: {
                                    text: item.orgName,
                                    class: 'label-sm gray'
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
                                displayName: 'Article',
                                fieldName: 'article',
                                type: 'article'
                            }, {
                                displayName: 'Price',
                                fieldName: 'price',
                                type: 'label',
                                showClass: 'table-short'
                            },{
                                displayName: "Supplier",
                                fieldName: "orgName",
                                type: 'label',
                                showClass: 'table-short'
                            }],
                            tableActions: [{
                                fieldName: 'delete',
                                type: 'communication-icon-btn'
                            }],
                            onDelete: function(docs) {
                                if (confirm(Kooboo.text.confirm.deleteItems)) {
                                    Kooboo.Supplier.deletes({
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
                Kooboo.Supplier.delete(data).then(function(res) {
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