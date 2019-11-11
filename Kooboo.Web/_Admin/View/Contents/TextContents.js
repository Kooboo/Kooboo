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
      Kooboo.EventBus.subscribe("kb/table/delete/finish", function() {
        Kooboo.EventBus.publish("kb/sidebar/refresh");
      });
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
      afterEdit: function() {
        self.getList();
        Kooboo.EventBus.publish("kb/sidebar/refresh");
      },
      onDelete: function() {}
    }
  });
});
