$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.Contents
          }
        ],
        isSettingShow: false,
        folders: [],
        newContent: Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage),
        tableData: [],
        selected: [],
        newFolderModalShow: false,
        currentId: ""
      };
    },
    mounted: function() {
      self.getList();
      // $("#J_NewFolder").on("show.bs.modal", function() {
      //   var $folder = $(
      //       'table.table > tbody input:checkbox:checked[data-check-model="folders"]'
      //     ),
      //     isExists = !$("#J_NewFolder").data("isnew") && $folder.length > 0,
      //     data = {};
      //   if (isExists) {
      //     data["Id"] = $folder[0].value;
      //     self.init($folder[0].value);
      //   } else {
      //     self.init(null);
      //   }
      // });
    },
    methods: {
      dataMapping: function(data) {
        return data.map(function(item) {
          return {
            id: item.id,
            relationsComm: "kb/relation/modal/show",
            relationsTypes: Object.keys(item.relations),
            relations: item.relations,
            folderName: {
              text: item.displayName,
              url: Kooboo.Route.Get(Kooboo.Route.TextContent.ByFolder, {
                folder: item.id
              })
            },
            edit: {
              text: Kooboo.text.common.setting,
              url: "/textcontent/contentfolder/edit"
            }
          };
        });
      },
      getList: function() {
        Kooboo.ContentFolder.getList().then(function(data) {
          self.tableData = self.dataMapping(data.model);
          self.folders = data.model;
        });
      },
      newFolder: function() {
        this.currentId = "";
        this.newFolderModalShow = true;
      },
      editFolder: function(row) {
        this.newFolderModalShow = true;
        this.currentId = row.id;
      },
      refreshSidebar: function() {
        Kooboo.EventBus.publish("kb/sidebar/refresh");
      },
      afterEdit: function() {
        self.getList();
        self.refreshSidebar();
      },
      onDelete: function() {
        var hasRel = _.some(self.selected, function(doc) {
          return doc.relations && Object.keys(doc.relations).length;
        });
        var confirmStr = hasRel
          ? Kooboo.text.confirm.deleteItemsWithRef
          : Kooboo.text.confirm.deleteItems;
        if (confirm(confirmStr)) {
          var ids = self.selected.map(function(row) {
            return row.id;
          });
          Kooboo.ContentFolder.Deletes({
            ids: JSON.stringify(ids)
          }).then(function(res) {
            if (res.success) {
              self.tableData = _.filter(self.tableData, function(row) {
                return ids.indexOf(row.id) === -1;
              });
              self.selected = [];
              window.info.show(Kooboo.text.info.delete.success, true);
              self.refreshSidebar();
            }
          });
        }
      }
    }
  });
});
