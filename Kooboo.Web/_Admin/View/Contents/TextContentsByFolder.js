$(function () {
  var self;
  var folderId = Kooboo.getQueryString("folder") || location.hash;

  new Vue({
    el: "#app",
    data: function () {
      self = this;
      return {
        tableData: [],
        selected: [],
        newTextContent: Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
          folder: Kooboo.getQueryString("folder"),
        }),
        folderName: "",
        contentTypeId: "",
        pager: {},
        searchKey: "",
        cacheData: null,
        isSearching: false,
        defaultColumnName: "",
      };
    },
    mounted: function () {
      Kooboo.TextContent.getByFolder().then(function (res) {
        if (res.success) {
          self.cacheData = res.model;
          self.handleData(res.model);
        }
      });
      Kooboo.ContentFolder.getFolderInfoById({
        id: folderId,
      }).then(function (res) {
        if (res.success) {
          self.folderName = res.model.name;
          self.contentTypeId = res.model.contentTypeId;
        }
      });
    },
    computed: {
      breads: function () {
        return [
          {
            name: "SITES",
          },
          {
            name: "DASHBOARD",
          },
          {
            name: Kooboo.text.common.contentFolders,
            url: Kooboo.Route.Get(Kooboo.Route.TextContent.ListPage),
          },
          {
            name: self.folderName,
          },
        ];
      },
    },
    methods: {
      onEditContentType: function () {
        window.open(
          Kooboo.Route.Get(Kooboo.Route.ContentType.DetailPage, {
            id: self.contentTypeId,
          })
        );
      },
      searchStart: function () {
        if (this.searchKey) {
          Kooboo.TextContent.search({
            folderId: folderId,
            keyword: self.searchKey,
          }).then(function (res) {
            if (res.success) {
              self.handleData(res.model);
              self.isSearching = true;
            }
          });
        } else {
          this.isSearching = false;
          self.handleData(this.cacheData);
        }
      },
      clearSearching: function () {
        this.searchKey = "";
        this.isSearching = false;
        self.handleData(this.cacheData);
      },
      dataMapping: function (data) {
        var columnName = self.getDefaultColumnName(data);
        self.defaultColumnName = columnName;
        return data.map(function (item) {
          var ob = {};
          ob[columnName] = {
            text: item.values[columnName],
            url: Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
              folder: Kooboo.getQueryString("folder"),
              id: item.id,
            }),
          };
          ob.lastModified = new Date(item.lastModified).toDefaultLangString();
          ob.online = {
            text: item.online ? Kooboo.text.online.yes : Kooboo.text.online.no,
            class: item.online
              ? "label-sm label-success"
              : "label-sm label-default",
          };
          ob.id = item.id;
          ob.Edit = {
            text: Kooboo.text.common.edit,
            url: Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
              folder: Kooboo.getQueryString("folder"),
              id: item.id,
            }),
          };
          return ob;
        });
      },
      getDefaultColumnName: function (records) {
        if (!!records && records instanceof Array && records.length > 0) {
          return Object.keys(records[0].values)[0];
        }
      },
      handleData: function (data) {
        self.pager = data;
        self.tableData = self.dataMapping(data.list);
      },
      onDelete: function () {
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var ids = self.selected.map(function (row) {
            return row.id;
          });
          Kooboo.TextContent.Deletes({
            ids: JSON.stringify(ids),
          }).then(function (res) {
            if (res.success) {
              self.tableData = _.filter(self.tableData, function (row) {
                return ids.indexOf(row.id) === -1;
              });
              self.selected = [];
              window.info.show(Kooboo.text.info.delete.success, true);
            }
          });
        }
      },
      onCopy: function () {
        location.href = self.selected[0].Edit.url + "&copy=true";
      },
      changePage: function (page) {
        Kooboo.TextContent.getByFolder({
          pageNr: page,
        }).then(function (res) {
          if (res.success) {
            self.handleData(res.model);
          }
        });
      },
    },
  });
});
