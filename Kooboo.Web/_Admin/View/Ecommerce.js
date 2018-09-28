$(function() {
    var ecomModel = function() {

        var self = this;

        this.showEnableBtn = ko.observable(false);
        this.showDisableBtn = ko.observable(false);

        this.enableSites = function() {
            var ids = this.selectedSites().map(function(site) {
                return site.id();
            })

            Kooboo.Commerce.enableSites({ ids: ids }).then(function(res) {
                if (res.success) {
                    getList();
                }
            })
        }

        this.disableSites = function() {
            var ids = this.selectedSites().map(function(site) {
                return site.id();
            })

            Kooboo.Commerce.disableSites({ ids: ids }).then(function(res) {
                if (res.success) {
                    getList();
                }
            })
        }

        this.selectedSites = ko.observableArray();

        getList();

        function getList() {
            Kooboo.Commerce.getList().then(function(res) {
                if (res.success) {
                    var docs = res.model.map(function(site) {
                        return {
                            id: site.id,
                            name: site.name,
                            enabled: {
                                text: Kooboo.text.common[site.enable ? 'yes' : 'no'],
                                class: 'label-sm ' + (site.enable ? 'green' : 'label-default')
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
                            displayName: Kooboo.text.commerce.isEnabled,
                            fieldName: 'enabled',
                            type: 'label'
                        }],
                        tableActions: [],
                        kbType: Kooboo.Commerce.name
                    })
                }
            })
        }


        Kooboo.EventBus.subscribe("ko/table/docs/selected", function(selected) {
            self.showEnableBtn(!!selected.length);
            self.showDisableBtn(!!selected.length);
            self.selectedSites(selected.length ? selected : []);
        })
    }

    ecomModel.prototype = new Kooboo.tableModel(Kooboo.Commerce.name);
    var vm = new ecomModel();
    ko.applyBindings(vm, document.getElementById('main'));
})