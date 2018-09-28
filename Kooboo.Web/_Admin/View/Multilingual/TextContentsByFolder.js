$(function() {
    var ContentType = function() {
        var self = this;
        this.tableData = ko.observable({});
        this.folderName = ko.observable("");
        var folderId = Kooboo.getQueryString("folder");

        this.pager = ko.observable();

        this.lang = ko.observable();
        this._lang = ko.observable();
        this._lang.subscribe(function(l) {
            self.lang(self.multiLangs().cultures[l]);
        });

        this.multiLangs = ko.observableArray();

        function dataMapping(data) {
            var tempArr = [];
            var columnName = getDefaultColumnName(data);
            data.forEach(function(item) {
                var ob = {};
                ob[columnName] = {
                    text: item.values[columnName],
                    url: Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
                        folder: Kooboo.getQueryString("folder") || "",
                        id: item.id || "",
                        lang: Kooboo.getQueryString("lang")
                    })
                }
                ob.lastModifiedDate = new Date(item.lastModified).toDefaultLangString();

                ob.online = {
                    text: item.online ? Kooboo.text.online.yes : Kooboo.text.online.no,
                    class: item.online ? "label-sm label-success" : "label-sm label-default"
                };
                ob.id = item.id;
                ob.edit = {
                    text: Kooboo.text.common.edit,
                    url: Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
                        folder: Kooboo.getQueryString("folder") || "",
                        id: item.id || "",
                        lang: Kooboo.getQueryString("lang")
                    })
                }
                tempArr.push(ob);
            })
            return tempArr;
        }

        function getDefaultColumnName(records) {
            if (!!records && records instanceof Array && records.length > 0) {
                return Object.keys(records[0].values)[0];
            }
        }

        function handleData(data) {
            self.pager(data);
            var ob = {
                columns: [{
                    displayName: Kooboo.text.common.online,
                    fieldName: "online",
                    type: "label"
                }, {
                    displayName: Kooboo.text.common.lastModified,
                    fieldName: "lastModifiedDate",
                    type: "text"
                }],
                kbType: "TextContent"
            }
            var columnName = getDefaultColumnName(data.list);
            ob.columns.unshift({
                displayName: columnName,
                fieldName: columnName,
                type: "link"
            });
            ob.docs = dataMapping(data.list);
            ob.tableActions = [{
                fieldName: "edit",
                type: "link-btn"
            }]
            self.tableData(ob);
        }

        Kooboo.Site.Langs().then(function(res) {

            if (res.success) {
                self.multiLangs(res.model);
                self._lang(Kooboo.getQueryString("lang"));

                Kooboo.TextContent.getByFolder().then(function(res) {
                    if (res.success) {
                        handleData(res.model);
                    }
                })
            }
        })

        Kooboo.ContentFolder.getFolderInfoById({
            id: folderId
        }).then(function(data) {
            self.folderName(data.model.name)
        })

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {

            Kooboo.TextContent.getByFolder({
                pageNr: page
            }).then(function(res) {
                if (res.success) {
                    handleData(res.model);
                }
            })
        })
    }

    ContentType.prototype = new Kooboo.tableModel(Kooboo.Page.name);
    ko.applyBindings(new ContentType, document.getElementById("main"));
})