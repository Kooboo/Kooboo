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
        selected: []
      };
    },
    mounted: function() {
      self.getList();
      Kooboo.EventBus.subscribe("/textcontent/contentfolder/edit", function(
        selectedFolder
      ) {
        Kooboo.EventBus.publish(
          "ko/textContent/folderSetting",
          selectedFolder.id
        );
      });

      Kooboo.EventBus.subscribe("kb/textcontents/new/folder", function() {
        self.getList();
        Kooboo.EventBus.publish("kb/sidebar/refresh");
      });

      $("#J_NewFolder").on("show.bs.modal", function() {
        var $folder = $(
            'table.table > tbody input:checkbox:checked[data-check-model="folders"]'
          ),
          isExists = !$("#J_NewFolder").data("isnew") && $folder.length > 0,
          data = {};
        if (isExists) {
          data["Id"] = $folder[0].value;
          self.init($folder[0].value);
        } else {
          self.init(null);
        }
      });

      Kooboo.EventBus.subscribe("kb/table/delete/finish", function() {
        Kooboo.EventBus.publish("kb/sidebar/refresh");
      });
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
          // var ob = {
          //   columns: [
          //     {
          //       displayName: Kooboo.text.common.name,
          //       fieldName: "folderName",
          //       type: "link"
          //     },
          //     {
          //       displayName: Kooboo.text.common.usedBy,
          //       fieldName: "relations",
          //       type: "communication-refer"
          //     }
          //   ],
          //   tableActions: [
          //     {
          //       fieldName: "edit",
          //       type: "communication-btn"
          //     }
          //   ],
          //   kbType: "ContentFolder"
          // };

          self.tableData = self.dataMapping(data.model);
          self.folders = data.model;
        });
      },
      newFolder: function() {
        Kooboo.EventBus.publish("ko/textContent/newFolder");
      },
      onDelete: function() {}
    }
  });
});
