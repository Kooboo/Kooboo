$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        tableData: [],
        selected: [],
        folderId: Kooboo.getQueryString("folder") || location.hash,
        lang: undefined,
        folderName: "",
        pager: {},
        breads: undefined,
        defaultColumnName: "",
        multiLangs: undefined
      };
    },
    mounted: function() {
      Kooboo.TextContent.getByFolder().then(function(res) {
        if (res.success) {
          self.handleData(res.model);
        }
      });
      Kooboo.ContentFolder.getFolderInfoById({
        id: self.folderId
      }).then(function(res) {
        if (res.success) {
          self.folderName = res.model.name;
        }
      });
      self.getLangs();
    },
    watch: {
      lang: function(value) {
        self.breads = [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: self.lang,
            url: "#"
          },
          {
            name: Kooboo.text.common.contentFolder
          }
        ];
      }
    },
    methods: {
      getLangs: function() {
        Kooboo.Site.Langs().then(function(res) {
          if (res.success) {
            self.multiLangs = res.model;
            self.lang = self.multiLangs.cultures[Kooboo.getQueryString("lang")];
          }
        });
      },
      dataMapping: function(data) {
        var columnName = self.getDefaultColumnName(data);
        self.defaultColumnName = columnName;
        return data.map(function(item) {
          var ob = {};
          ob[columnName] = {
            text: item.values[columnName],
            url: Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
              folder: Kooboo.getQueryString("folder") || "",
              id: item.id || "",
              lang: Kooboo.getQueryString("lang")
            })
          };
          ob.lastModified = new Date(item.lastModified).toDefaultLangString();
          ob.online = {
            text: item.online ? Kooboo.text.online.yes : Kooboo.text.online.no,
            class: item.online
              ? "label-sm label-success"
              : "label-sm label-default"
          };
          ob.id = item.id;
          ob.Edit = {
            text: Kooboo.text.common.edit,
            url: Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
              folder: Kooboo.getQueryString("folder") || "",
              id: item.id || "",
              lang: Kooboo.getQueryString("lang")
            })
          };
          return ob;
        });
      },
      getDefaultColumnName: function(records) {
        if (!!records && records instanceof Array && records.length > 0) {
          return Object.keys(records[0].values)[0];
        }
      },
      handleData: function(data) {
        self.pager = data;
        self.tableData = self.dataMapping(data.list);
      },
      onDelete: function() {
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var ids = self.selected.map(function(row) {
            return row.id;
          });
          Kooboo.TextContent.Deletes({
            ids: JSON.stringify(ids)
          }).then(function(res) {
            if (res.success) {
              self.tableData = _.filter(self.tableData, function(row) {
                return ids.indexOf(row.id) === -1;
              });
              self.selected = [];
              window.info.show(Kooboo.text.info.delete.success, true);
            }
          });
        }
      },
      changePage: function(page) {
        Kooboo.TextContent.getByFolder({
          pageNr: page
        }).then(function(res) {
          if (res.success) {
            self.handleData(res.model);
          }
        });
      }
    }
  });
});
