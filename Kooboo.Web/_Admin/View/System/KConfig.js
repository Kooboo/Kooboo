$(function() {
    var configViewModel = function() {
        var self = this;

        this.getList = function() {
            Kooboo.KConfig.getList().then(function(res) {
                if (res.success) {
                    self.tableData(self.getTableData(res.model));
                }
            })
        }

        this.showConfigModal = ko.observable(false);

        this.configItem = ko.observable();

        this.getTableData = function(data) {
            var docs = [];
            data.forEach(function(item) {
                var date = new Date(item.lastModified);

                var obj = {
                    id: item.id,
                    name: item.name,
                    tagName: {
                        text: '<' + item.tagName + '>',
                        tooltip: item.tagHtml,
                        class: 'label-sm blue'
                    },
                    date: date.toDefaultLangString(),
                    relationsComm: "kb/relation/modal/show",
                    relationsTypes: Object.keys(item.relations),
                    relations: item.relations,
                    edit: {
                        iconClass: "fa-pencil",
                        title: Kooboo.text.common.edit,
                        url: 'kb/config/edit'
                    },
                    versions: {
                        iconClass: "fa-clock-o",
                        title: Kooboo.text.common.viewAllVersions,
                        url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
                            KeyHash: item.keyHash,
                            storeNameHash: item.storeNameHash
                        }),
                        newWindow: true
                    }
                }

                docs.push(obj);
            });

            return {
                columns: [{
                    displayName: Kooboo.text.common.name,
                    fieldName: "name",
                    type: "text"
                }, {
                    displayName: Kooboo.text.common.usedBy,
                    fieldName: "relations",
                    type: "communication-refer"
                }, {
                    displayName: 'tagName',
                    fieldName: 'tagName',
                    type: 'label'
                }, {
                    displayName: Kooboo.text.common.lastModified,
                    fieldName: "date",
                    type: "text"
                }],
                docs: docs,
                tableActions: [{
                    fieldName: 'edit',
                    type: 'communication-icon-btn'
                }, {
                    fieldName: "versions",
                    type: "link-icon"
                }],
                kbType: Kooboo.KConfig.name
            };

        }

        this.getList();

        Kooboo.EventBus.subscribe('kb/config/edit', function(data) {
            Kooboo.KConfig.Get({
                id: data.id
            }).then(function(res) {
                if (res.success) {
                    self.configItem(res.model);
                    self.showConfigModal(true);
                }
            })
        })
    }

    configViewModel.prototype = new Kooboo.tableModel(Kooboo.KConfig.name);
    var vm = new configViewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})