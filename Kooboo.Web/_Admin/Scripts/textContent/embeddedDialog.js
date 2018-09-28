(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/textContent/embeddedDialog.html"),
        modal = Kooboo.viewEditor && Kooboo.viewEditor.component.modal;

    var viewModel = function(params) {
        var self = this;
        var _modal;
        var tableObj = {};
        this.tableData = ko.observable({});
        this.choosedEmbedded = params.choosedEmbedded;
        this.contents = ko.observableArray();

        this.newTextContent = function() {
            _modal = showContentDialog();
        }

        this.close = function() {
            $("#embeddedDialog").modal("hide")
        }

        self.saveIframe = function() {
            if (window.__gl) {
                if (!window.__gl.saveContentFinish) {
                    window.__gl.saveContentFinish = self.saveContentFinishEvent;
                }
                if (window.__gl.saveContent) {
                    window.__gl.saveContent();
                }
            }
        }

        self.saveContentFinishEvent = function(fieldData, textContentId, folderId) {
            var existed = self.choosedEmbedded().contents.some(function(o) {
                return o.id === textContentId
            })
            if (existed) {
                var index = _.findIndex(self.contents(), function(o) {
                    return o.id === textContentId;
                })
                self.contents.splice(index, 1, $.extend({ values: savedContentMapping(fieldData) }, { id: textContentId }))
            } else {
                self.contents.push($.extend({ values: savedContentMapping(fieldData) }, { id: textContentId }));
            }

            _modal.close();
            window.__gl = {};
        }

        this.choosedEmbedded.subscribe(function(choosedEmbedded) {
            self.contents(choosedEmbedded.contents);
        })

        this.contents.subscribe(function(contents) {
            tableObj.columns = [];
            tableObj.kbType = "TextContent";

            var columnName = getDefaultColumnName(contents);
            tableObj.columns.unshift({
                displayName: columnName,
                fieldName: columnName,
                type: "communication-link"
            });
            tableObj.docs = dataMapping(contents);
            tableObj.onDelete = function(deletedContents) {
                deleteContents(deletedContents);
            }
            self.tableData(tableObj);
            Kooboo.EventBus.publish("kb/textContent/embedded/edit", self.choosedEmbedded())
        })

        Kooboo.EventBus.subscribe("kb/textContent/edit", function(contentId) {
            _modal = showContentDialog(contentId);
        })

        function deleteContents(deletedContents) {
            if (confirm(Kooboo.text.confirm.deleteItems)) {
                var ids = deletedContents.map(function(o) {
                    return o.id;
                })
                Kooboo["TextContent"].Deletes({
                    ids: JSON.stringify(ids)
                }).then(function(res) {
                    if (res.success) {
                        _.forEach(ids, function(id) {
                            self.contents.remove(function(o) {
                                return o.id === id
                            })
                        });
                    }
                });
            }
        }

        function getDefaultColumnName(records) {
            if (!!records && records instanceof Array && records.length > 0) {
                return Object.keys(records[0].values)[0];
            }
        }

        function dataMapping(data) {
            var tempArr = [];
            var columnName = getDefaultColumnName(data);
            data.forEach(function(item) {
                var ob = {};
                ob[columnName] = {
                    text: item.values[columnName],
                    url: "kb/textContent/edit"
                }
                ob.id = item.id;
                tempArr.push(ob);
            })
            return tempArr;
        }

        function showContentDialog(contentId) {
            var model = null;
            var iframeUrl = "";

            iframeUrl = Kooboo.Route.Get(Kooboo.Route.TextContent.DialogPage, {
                folder: self.choosedEmbedded().embeddedFolder.id || '',
                id: contentId || Kooboo.Guid.Empty
            })

            model = modal.open({
                title: Kooboo.text.component.modal.embeddedFolder,
                width: 1000,
                url: iframeUrl,
                zIndex: 200003,
                buttons: [{
                    id: 'Save',
                    text: Kooboo.text.common.save,
                    cssClass: 'green',
                    click: function(context) {
                        self.saveIframe()
                    }
                }, {
                    id: 'cancel',
                    text: Kooboo.text.common.cancel,
                    cssClass: 'gray',
                    click: function(context) {
                        context.modal.close();
                    }
                }]
            })
            return model;
        }

        function savedContentMapping(data) {
            var outputObj = {};
            for (var key in data) {
                outputObj[key.toCamelCase()] = data[key];
            }
            return outputObj
        }

        $("#embeddedDialog").on("shown.bs.modal", function() {
            $(".modal-backdrop:last").css("z-index", 200001);
        })
    }

    viewModel.prototype = new Kooboo.tableModel();

    return ko.components.register("embedded-dialog", {
        viewModel: viewModel,
        template: template
    })
})()