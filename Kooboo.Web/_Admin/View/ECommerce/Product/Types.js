$(function() {

    var typesModel = function() {
        var self = this;

        this.newTypeUrl = Kooboo.Route.Product.Type.DetailPage;

        getList();

        function getList() {
            Kooboo.ProductType.getList().then(function(res) {
                if (res.success) {
                    var docs = res.model.map(function(item) {
                        return {
                            id: item.id,
                            name: item.name,
                            edit: {
                                text: Kooboo.text.common.edit,
                                url: Kooboo.Route.Get(Kooboo.Route.Product.Type.DetailPage, {
                                    id: item.id
                                })
                            }
                        }
                    })

                    self.tableData({
                        docs: docs,
                        columns: [{
                            displayName: Kooboo.text.common.name,
                            fieldName: 'name',
                            type: 'text'
                        }],
                        tableActions: [{
                            fieldName: 'edit',
                            type: 'link-btn'
                        }],
                        kbType: Kooboo.ProductType.name
                    })
                }
            })
        }
    }

    typesModel.prototype = new Kooboo.tableModel(Kooboo.ProductType.name);
    var vm = new typesModel();
    ko.applyBindings(vm, document.getElementById('main'));
})