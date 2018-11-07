(function() {

    Kooboo.loadJS([
        "/_Admin/Scripts/components/kbPager.js",
        "/_Admin/Scripts/tableModel.js",
        "/_Admin/Scripts/components/kbTable.js"
    ])

    var template = Kooboo.getTemplate('/_Admin/View/Market/Scripts/components/TopupHistoryModal.html');

    ko.components.register('topup-history-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    self.getHistory();
                }
            })


            this.onHide = function() {
                self.tableData({
                    kbType: Kooboo.Balance.name
                });
                self.pager(null);
                self.isShow(false);
            }

            this.tableData = ko.observable({
                kbType: Kooboo.Balance.name
            });
            this.pager = ko.observable();

            this.getHistory = function(page) {
                Kooboo.Balance.getTopupHistory({
                    pageNr: page || 1
                }).then(function(res) {
                    if (res.success) {
                        self.pager(res.model);
                        self.tableData(self.getTableData(res.model));
                    }
                })
            }

            this.getTableData = function(data) {
                var docs = data.list.map(function(log) {
                    var date = new Date(log.orderDate);
                    return {
                        amount: log.amount,
                        date: date.toDefaultLangString()
                    }
                });

                var columns = [{
                    displayName: Kooboo.text.common.amount,
                    fieldName: "amount",
                    type: "text"
                }, {
                    displayName: Kooboo.text.component.topupModal.orderDate,
                    fieldName: "date",
                    type: "text"
                }];

                return {
                    docs: docs,
                    columns: columns,
                    tableActions: [],
                    kbType: Kooboo.Balance.name,
                    unselectable: true
                }
            }

            Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
                self.getHistory(page);
            })

        },
        template: template
    })
})()