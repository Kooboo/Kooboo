(function() {
    var eventsModel = function() {
        var self = this;

        Kooboo.BusinessRule.getList().then(function(res) {
            if (res.success) {
                var docs = res.model.map(function(item) {
                    return {
                        type: item.name,
                        count: {
                            text: item.count,
                            class: 'badge-sm blue'
                        },
                        edit: {
                            text: Kooboo.text.common.edit,
                            url: Kooboo.Route.Get(Kooboo.Route.Event.DetailPage, {
                                name: item.name
                            })
                        }
                    }
                })

                self.tableData({
                    docs: docs,
                    columns: [{
                        displayName: Kooboo.text.common.type,
                        fieldName: 'type',
                        type: 'text'
                    }, {
                        displayName: Kooboo.text.site.disk.count,
                        fieldName: 'count',
                        type: 'badge'
                    }],
                    tableActions: [{
                        fieldName: 'edit',
                        type: 'link-btn'
                    }],
                    kbType: Kooboo.BusinessRule.name,
                    unselectable: true
                })
            }
        })
    }

    eventsModel.prototype = new Kooboo.tableModel(Kooboo.BusinessRule.name);

    var vm = new eventsModel();

    ko.applyBindings(vm, document.getElementById('main'));
})()