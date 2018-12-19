$(function() {
    var viewModel = function() {

        var self = this;

        this.pager = ko.observable();

        this.getList = function() {
            Kooboo.Infrastructure.getMyOrders().then(function(res) {
                if (res.success) {
                    self.handleData(res.model);
                }
            })
        }

        this.getList();

        this.handleData = function(data) {
            self.pager(data);

            var docs = data.list.map(function(doc) {
                return {
                    id: doc.salesItemId,
                    name: doc.displayName,
                    
                    type: {
                        text: doc.type,
                        class: 'label-sm label-info'
                    },
                    amount: {
                        text: doc.amount,
                        class: 'label-sm label-info'
                    },
                    startTime: {
                        text: new Date(doc.startTime).toDefaultLangString(),
                        class: 'label-sm green'
                    },
                    endMonth: {
                        text: doc.endMonth,
                        class: 'label-sm green'
                    },
                    renew: {
                        text: Kooboo.text.common.renew,
                        url: 'kb/market/hardware/orders/renew'
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
                    displayName: Kooboo.text.common.type,
                    fieldName: 'type',
                    type: 'label',
                    showClass: 'table-short'
                }, {
                    displayName: Kooboo.text.common.amount,
                    fieldName: 'amount',
                    type: 'label',
                    showClass: 'table-short'
                }, {
                    displayName: Kooboo.text.common.startTime,
                    fieldName: 'startTime',
                    type: 'label',
                    showClass: 'table-short'
                }, {
                    displayName: Kooboo.text.common.endMonth,
                    fieldName: 'endMonth',
                    type: 'label',
                    showClass: 'table-short'
                }],
                tableActions: [{
                    fieldName: 'renew',
                    type: 'communication-btn'
                }],
                unselectable: true
            })
        }

        this.showHardwareModal = ko.observable(false);
        this.hardwareData = ko.observable();

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            self.getList(page);
        })

        Kooboo.EventBus.subscribe("kb/market/hardware/orders/renew", function(data) {
            Kooboo.Infrastructure.getSalesItem({
                id: data.id
            }).then(function(res) {
                if (res.success) {
                    self.hardwareData(res.model);
                    self.showHardwareModal(true);
                }
            })
        })
    }

    viewModel.prototype = new Kooboo.tableModel(Kooboo.Infrastructure.name);
    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})