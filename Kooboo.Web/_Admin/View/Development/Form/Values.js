$(function() {

    var formValueModel = function() {
        var self = this;

        this.pager = ko.observable();

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {

            Kooboo.Form.Values({
                id: Kooboo.getQueryString("id"),
                pageNr: page
            }).then(function(res) {
                handleData(res.model);
            })
        })
    }

    formValueModel.prototype = new Kooboo.tableModel(Kooboo.Form.name);
    var vm = new formValueModel();

    ko.applyBindings(vm, document.getElementById("main"));

    Kooboo.Form.Values({
        id: Kooboo.getQueryString("id")
    }).then(function(res) {

        if (res.success) {
            handleData(res.model);
        }
    })

    var handleData = function(data) {
        vm.pager(data);

        var _col = [];
        _.forEach(data.list, function(item) {
            _.forEach(Object.keys(item.values), function(key) {
                (_col.indexOf(key) == -1) && _col.push(key);
            })
        })

        var docs = [];
        _.forEach(data.list, function(item) {
            var model = {
                id: item.id
            }

            _.forEach(_col, function(col) {
                model[col] = item.values[col];
            });

            docs.push(model);
        })

        var columns = [];
        _.forEach(_col, function(col) {
            columns.push({
                displayName: col,
                fieldName: col,
                type: "text"
            })
        })

        var _data = {
            docs: docs,
            columns: columns,
            kbType: Kooboo.Form.name,
            onDelete: function(data) {

                if (confirm(Kooboo.text.confirm.deleteItems)) {

                    var ids = [];
                    _.forEach(data, function(item) {
                        ids.push(item.id);
                    });

                    Kooboo.Form.DeleteValues({
                        ids: JSON.stringify(ids)
                    }).then(function(res) {

                        if (res.success) {
                            location.reload();
                        }
                    })
                }
            }
        }

        vm.tableData(_data);
    }
})