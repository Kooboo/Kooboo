$(function() {
    var codeModel = function() {
        var self = this;

        this.codeCreateUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.Code.EditPage, self.curType() !== 'all' ? {
                codeType: self.curType()
            } : null)
        });

        this.codeTypes = ko.observableArray();

        this.curType = ko.observable();

        this.createBtnText = ko.observable(Kooboo.text.common.create + ' ' + Kooboo.text.common.Code);

        this.changeCodeType = function(type, model) {
            if (self.curType() !== type) {
                self.curType(type);

                if (type == 'all') {
                    self.createBtnText(Kooboo.text.common.create + ' ' + Kooboo.text.common.Code);
                } else if (model) {
                    self.createBtnText(Kooboo.text.common.create + ' ' + model.displayName);
                }

                Kooboo.Code.getListByType({
                    codeType: type
                }).then(function(res) {
                    if (res.success) {
                        handleData(res.model);
                    }
                })
            }
        }

        function handleData(data) {
            var docs = data.map(function(item) {
                var date = new Date(item.lastModified);

                var model = {
                    id: item.id,
                    name: item.name,
                    codeType: {
                        text: item.codeType,
                        class: 'label-sm label-success'
                    },
                    preview: {
                        text: item.previewUrl,
                        url: item.previewUrl,
                        newWindow: true
                    },
                    relationsComm: "kb/relation/modal/show",
                    relationsTypes: item.references ? Object.keys(item.references) : {},
                    relations: item.references,
                    lastModified: date.toDefaultLangString(),
                    edit: {
                        text: Kooboo.text.common.edit,
                        url: Kooboo.Route.Get(Kooboo.Route.Code.EditPage, {
                            id: item.id,
                            codeType: item.codeType
                        })
                    },
                    debug: {
                        text: Kooboo.text.common.Debug,
                        url: Kooboo.Route.Get(Kooboo.Route.Code.DebugPage, {
                            id: item.id
                        }),
                        newWindow: true
                    }
                };

                if (item.codeType.toLowerCase() == 'eventactivity') {
                    model.type = {
                        text: item.eventType,
                        class: 'label-sm label-success'
                    };
                } else if (item.codeType.toLowerCase() == 'pagescript') {
                    model.type = {
                        text: Kooboo.text.common[item.isEmbedded ? 'Embedded' : 'external'],
                        class: 'label-sm label-success'
                    }
                } else {
                    model.type = {}
                }

                return model;
            })

            self.tableData({
                docs: docs,
                columns: [{
                    displayName: Kooboo.text.common.name,
                    fieldName: 'name',
                    type: 'text'
                }, {
                    displayName: Kooboo.text.site.code.codeType,
                    fieldName: 'codeType',
                    type: 'label'
                }, {
                    displayName: Kooboo.text.common.type,
                    fieldName: 'type',
                    type: 'label'
                }, {
                    displayName: Kooboo.text.site.page.references,
                    fieldName: "relations",
                    type: "communication-refer"
                }, {
                    displayName: Kooboo.text.common.preview,
                    fieldName: 'preview',
                    type: 'link'
                }, {
                    displayName: Kooboo.text.common.lastModified,
                    fieldName: 'lastModified',
                    type: 'text'
                }],
                tableActions: [{
                    fieldName: 'edit',
                    type: 'link-btn'
                }, {
                    fieldName: 'debug',
                    type: 'link-btn'
                }],
                kbType: Kooboo.Code.name
            })
        }

        Kooboo.Code.getCodeType().then(function(res) {
            if (res.success) {
                var types = Kooboo.objToArr(res.model, 'value', 'displayName');
                types = _.orderBy(types, [function(o) { return o.value }]);
                types.forEach(function(type) {
                    type.editUrl = Kooboo.Route.Get(Kooboo.Route.Code.EditPage, {
                        codeType: type.value
                    })
                })
                types.splice(0, 0, {
                    displayName: Kooboo.text.common.all,
                    value: "all"
                })
                self.codeTypes(types);
            }
        })

        self.changeCodeType('all');
    }

    codeModel.prototype = new Kooboo.tableModel(Kooboo.Code.name);

    var vm = new codeModel();

    ko.applyBindings(vm, document.getElementById('main'));
})