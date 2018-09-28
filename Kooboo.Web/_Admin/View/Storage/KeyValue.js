$(function() {

    var KeyValueStoreModel = function() {

        var self = this;

        this.pairs = ko.observable();

        this.showEditModal = ko.observable(false);

        this.isNewStore = ko.observable(false);

        this.editingName = ko.validateField({
            required: '',
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                message: Kooboo.text.validation.objectNameRegex
            },
            remote: {
                url: Kooboo.KeyValue.isUniqueName(),
                message: Kooboo.text.validation.taken,
                type: "get",
                data: {
                    name: function() {
                        return self.editingName();
                    }
                }
            }
        });
        this.editingValue = ko.validateField({
            required: ''
        });
        this.isValid = function() {
            if (self.isNewStore()) {
                return self.editingName.isValid() && self.editingValue.isValid();
            } else {
                return self.editingValue.isValid();
            }
        }

        this.showError = ko.observable(false);

        this.onCreate = function() {
            self.isNewStore(true);
            self.showEditModal(true);
        }

        this.hideEditModal = function() {
            self.showError(false);
            self.editingName('');
            self.editingValue('')
            self.showEditModal(false);
        }

        this.saveEditing = function() {
            if (self.isValid()) {
                Kooboo.KeyValue.Update({
                    key: self.editingName(),
                    value: self.editingValue()
                }).then(function(res) {
                    if (res.success) {
                        window.info.done(Kooboo.text.info.update.success);
                        self.hideEditModal();
                        getList();
                    }
                })
            } else {
                self.showError(true);
            }
        }

        getList();

        function getList() {
            Kooboo.KeyValue.getList().then(function(res) {
                if (res.success) {
                    self.pairs(res.model);
                    var docs = Kooboo.objToArr(res.model).map(function(item) {
                        return {
                            id: item.key,
                            key: item.key,
                            value: item.value,
                            edit: {
                                text: Kooboo.text.common.edit,
                                url: 'kb/KeyValueStore/edit'
                            }
                        }
                    })

                    self.tableData({
                        docs: docs,
                        columns: [{
                            fieldName: 'key',
                            displayName: Kooboo.text.site.label.key,
                            type: 'text'
                        }, {
                            fieldName: 'value',
                            displayName: Kooboo.text.site.label.value,
                            type: 'text'
                        }],
                        tableActions: [{
                            fieldName: 'edit',
                            type: 'communication-btn'
                        }],
                        kbType: Kooboo.KeyValue.name
                    })
                }
            })
        }

        Kooboo.EventBus.subscribe('kb/KeyValueStore/edit', function(obj) {
            var name = obj.id;
            self.isNewStore(false);
            self.editingName(name);
            self.editingValue(self.pairs()[name]);
            self.showEditModal(true);
        })

    }

    $('#modal').on('shown.bs.modal', function() {
        $(".autosize").textareaAutoSize().trigger("keyup");
    });

    KeyValueStoreModel.prototype = new Kooboo.tableModel(Kooboo.KeyValue.name);

    var vm = new KeyValueStoreModel();
    ko.applyBindings(vm, document.getElementById('main'));
})