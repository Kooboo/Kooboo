$(function() {
    var ContentType = function() {
        var self = this;
        this.tableData = ko.observable({});
        this.newTextContent = Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
            folder: Kooboo.getQueryString("folder")
        });
        this.folderName = ko.observable("");
        this.contentTypeId = ko.observable("");
        var folderId = Kooboo.getQueryString("folder") || location.hash;
        this.pager = ko.observable();

        this.onEditContentType = function() {
            window.open(Kooboo.Route.Get(Kooboo.Route.ContentType.DetailPage, {
                id: this.contentTypeId()
            }))
        }

        this.cacheData = null;

        this.searchKey = ko.observable("");
        this.handleEnter = function(m, e) {
            if (e.keyCode == 13) {
                this.searchStart();
            }
        }
        this.isSearching = ko.observable(false);
        this.searchStart = function() {
            if (this.searchKey()) {
                Kooboo.TextContent.search({
                    folderId: folderId,
                    keyword: self.searchKey()
                }).then(function(res) {
                    if (res.success) {
                        handleData(res.model);
                        self.isSearching(true);
                    }
                })
            } else {
                this.isSearching(false);
                handleData(this.cacheData);
            }
        }

        this.clearSearching = function() {
            this.searchKey("");
            this.isSearching(false);
            handleData(this.cacheData);
        }

        function dataMapping(data) {
            var tempArr = [];
            var columnName = getDefaultColumnName(data);
            data.forEach(function(item) {
                var ob = {};
                ob[columnName] = {
                    text: item.values[columnName],
                    url: Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
                        folder: Kooboo.getQueryString("folder"),
                        id: item.id
                    })
                }
                ob.lastModified = new Date(item.lastModified).toDefaultLangString();

                ob.online = {
                    text: item.online ? Kooboo.text.online.yes : Kooboo.text.online.no,
                    class: item.online ? "label-sm label-success" : "label-sm label-default"
                };
                ob.id = item.id;
                ob.Edit = {
                    text: Kooboo.text.common.edit,
                    url: Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
                        folder: Kooboo.getQueryString("folder"),
                        id: item.id
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
                    fieldName: "lastModified",
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
                fieldName: "Edit",
                type: "link-btn",
            }]
            self.tableData(ob);
        }

        Kooboo.TextContent.getByFolder().then(function(res) {
            if (res.success) {
                self.cacheData = res.model;
                handleData(res.model);
            }
        })

        Kooboo.ContentFolder.getFolderInfoById({
            id: folderId
        }).then(function(res) {
            if (res.success) {
                self.folderName(res.model.name)
                self.contentTypeId(res.model.contentTypeId);
            }
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