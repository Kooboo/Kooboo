$(function() {
    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.getList = function(page) {
            Kooboo.Supplier.getList({
                pageNr: page || 1
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
                    name: item.userName,
                    introduction: item.introduction,
                    expertises: {
                        data: item.expertises,
                        class: 'label label-sm label-info'
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

            var data = {
                docs: docs,
                columns: [{
                    displayName: 'User',
                    fieldName: 'name',
                    type: 'text'
                }, {
                    displayName: 'Expertises',
                    fieldName: 'expertises',
                    type: 'array'
                }, {
                    displayName: 'Introduction',
                    fieldName: 'introduction',
                    type: 'text'
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