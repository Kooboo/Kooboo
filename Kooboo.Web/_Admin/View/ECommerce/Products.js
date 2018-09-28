$(function() {

    var productsModel = function() {
        var self = this;

        this.productTypes = ko.observableArray();

        this.pager = ko.observable();

        function getListByPage(page) {

            Kooboo.Product.getList({
                page: page || 1
            }).then(function(res) {
                if (res.success) {
                    handleData(res.model);
                }
            })
        }

        function handleData(data) {
            self.pager(data);

            var docs = data.list.map(function(d) {
                var date = new Date(d.lastModified);
                var type = _.find(self.productTypes(), function(t) {
                    return t.id == d.productTypeId;
                })

                return _.assign({
                    id: d.id,
                    lastModified: date.toDefaultLangString(),
                    online: d.online,
                    productType: {
                        text: type ? type.name : '',
                        class: 'label-sm ' + (type ? 'blue' : 'label-default')
                    },
                    edit: {
                        text: Kooboo.text.common.edit,
                        url: Kooboo.Route.Get(Kooboo.Route.Product.DetailPage, {
                            id: d.id,
                            type: d.productTypeId
                        })
                    }
                }, d.values);
            })

            var columns = [];
            if (data.list.length) {
                var keys = Object.keys(data.list[0].values);
                keys.forEach(function(key) {
                    columns.push({
                        displayName: key,
                        fieldName: key,
                        type: 'text'
                    })
                })

                columns.push({
                    displayName: Kooboo.text.common.ProductType,
                    fieldName: 'productType',
                    type: 'label'
                })
            }

            self.tableData({
                docs: docs,
                columns: columns,
                tableActions: [{
                    fieldName: 'edit',
                    type: 'link-btn'
                }],
                kbType: Kooboo.Product.name,
                class: 'table-bordered'
            })
        }

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            getListByPage(page);
        })

        Kooboo.ProductType.getList().then(function(res) {
            if (res.success) {
                self.productTypes(res.model.map(function(t) {
                    return {
                        id: t.id,
                        name: t.name
                    }
                }))

                getListByPage();
            }
        })
    }

    productsModel.prototype = new Kooboo.tableModel(Kooboo.Product.name);
    var vm = new productsModel();
    ko.applyBindings(vm, document.getElementById('main'));
})