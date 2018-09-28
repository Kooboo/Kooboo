$(function() {
    var databaseModel = function() {
        var self = this;

        this.createTableModal = ko.observable(false);

        this.showError = ko.observable(false);

        this.newTableName = ko.validateField({
            required: '',
            remote: {
                url: Kooboo.Database.isUniqueTableName(),
                type: 'get',
                data: {
                    name: function() {
                        return self.newTableName();
                    }
                },
                message: Kooboo.text.validation.taken
            }
        })

        this.onShowCreateTableModal = function() {
            self.createTableModal(true);
        }

        this.resetTableModal = function() {
            self.showError(false);
            self.newTableName('');
            self.createTableModal(false);
        }

        this.onSaveNewTable = function() {
            if (self.isTableNameVaild()) {
                Kooboo.Database.createTable({
                    name: self.newTableName()
                }).then(function(res) {
                    if (res.success) {
                        self.resetTableModal();
                        getTableList();
                    }
                })
            } else {
                self.showError(true);
            }
        }

        this.isTableNameVaild = function() {
            return self.newTableName.isValid();
        }

        function getTableList() {
            Kooboo.Database.getTables().then(function(res) {
                if (res.success) {
                    var tables = res.model.map(function(name) {
                        return {
                            id: name,
                            name: {
                                text: name,
                                url: Kooboo.Route.Get(Kooboo.Route.Database.DataPage, {
                                    table: name
                                })
                            },
                            edit: {
                                text: Kooboo.text.common.setting,
                                url: Kooboo.Route.Get(Kooboo.Route.Database.ColumnsPage, {
                                    table: name
                                })
                            }
                        }
                    })

                    self.tableData({
                        docs: tables,
                        columns: [{
                            displayName: Kooboo.text.common.name,
                            fieldName: "name",
                            type: "link"
                        }],
                        tableActions: [{
                            fieldName: "edit",
                            type: "link-btn"
                        }],
                        onDelete: function(list) {
                            if (confirm(Kooboo.text.confirm.deleteItems)) {
                                var names = list.map(function(m) {
                                    return m.name.text;
                                })

                                Kooboo.Database.deleteTables({
                                    names: names
                                }).then(function(res) {
                                    if (res.success) {
                                        getTableList();
                                        window.info.done(Kooboo.text.info.delete.success);
                                    }
                                })
                            }
                        },
                        kbType: Kooboo.Database.name
                    })
                }
            })
        }

        getTableList();

        this.columnsModal = ko.observable(false);

        this.resetColumnsModal = function() {
            self.columnsModal(false);
        }

        this.onSaveColumns = function() {
            debugger;
        }

        this.columns = ko.observableArray();

        Kooboo.EventBus.subscribe("kb/database/table/setting", function(arg) {
            Kooboo.Database.getColumns({
                table: arg.id
            }).then(function(res) {
                if (res.success) {
                    self.columns(res.model);
                    self.columnsModal(true);
                }
            })
        })
    }

    databaseModel.prototype = new Kooboo.tableModel(Kooboo.Database.name);

    var vm = new databaseModel();

    ko.applyBindings(vm, document.getElementById("main"));
})