$(function () {
  var self;

  new Vue({
    el: "#app",
    data: function () {
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.KConfig
          }
        ],
        showConfigModal: false,
        showDelete: true,
        showMediaModal: false,
        tableData: [],
        configItem: undefined,
        tableDataSelected: [],
        mediaDialogData: null,
        editingItem: undefined
      };
    },
    created: function () {
      self = this;
      self.getTableData();
    },
    mounted: function () {
      Kooboo.EventBus.subscribe("kb/config/edit", function (data) {
        self.getTableData();
      });
      Kooboo.EventBus.subscribe("kb/config/attribute/update", function () {
        self.getTableData();
      });
    },
    methods: {
      getConfirmMessage: function (doc) {
        if (doc.relations) {
          var reorderedKeys = _.sortBy(Object.keys(doc.relations));
          doc.relationsTypes = reorderedKeys;
        }
        var find = _.find(doc, function (item) {
          return item.relations && Object.keys(item.relations).length;
        });

        if (!!find) {
          return Kooboo.text.confirm.deleteItemsWithRef;
        } else {
          return Kooboo.text.confirm.deleteItems;
        }
      },
      firstKey: function (item) {
        return Object.keys(item.binding)[0];
      },
      onConfigCancel: function () {
        self.editingItem = null;
        self.configItem = undefined;
        self.showConfigModal = false;
      },
      onConfigSave: function () {
        var item = this.editingItem;
        Kooboo.KConfig.update({
          id: item.id,
          binding: Kooboo.arrToObj(self.configItem)
        }).then(function (res) {
          if (res.success) {
            self.editingItem = null;
            self.configItem = undefined;
            self.showConfigModal = false;
            Kooboo.EventBus.publish("kb/config/attribute/update");
          }
        });
      },
      onEdit: function (item) {

        Kooboo.KConfig.Get({
          id: item.id
        }).then(function (res) {
          if (res.success) {
            switch (res.model.controlType.toLowerCase()) {
              case "textbox":
                self.editingItem = item;
                self.configItem = Kooboo.objToArr(item.binding);
                self.showConfigModal = true;
                break;
              case "mediafile":
                Kooboo.Media.getList().then(function (res) {
                  if (res.success) {
                    res.model["show"] = true;
                    res.model["context"] = self;
                    res.model["onAdd"] = function (selected) {
                      Kooboo.KConfig.update({
                        id: item.id,
                        binding: {
                          src: selected.url
                        }
                      }).then(function (res) {
                        self.getTableData();
                      });
                    };
                    self.mediaDialogData = res.model;
                  }
                });
                break;
            }
          }
        });
      },
      getVersionsUrl: function (keyHash, storeNameHash) {
        return Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
          KeyHash: keyHash,
          storeNameHash: storeNameHash
        });
      },
      getTableData: function () {
        Kooboo.KConfig.getList().then(function (res) {
          if (res.success) {
            self.tableData = res.model;
          }
        });
      },
      rowNameFormat: function (row) {
        var firstKey = Object.keys(row.binding)[0];
        return {
          title: row.name,
          description: firstKey + ": " + row.binding[firstKey]
        };
      },
      linkTo: function (href) {
        window.open(href);
      },
      onShowRelationModal: function (by, id, type) {
        Kooboo.EventBus.publish("kb/relation/modal/show", {
          by: by,
          type: type,
          id: id
        });
      },
      onDelete: function () {
        if (confirm(this.getConfirmMessage(this.tableDataSelected))) {
          var ids = this.tableDataSelected.map(function (m) {
            return m.id;
          });

          Kooboo[Kooboo.KConfig.name]
            .Deletes({
              ids: ids
            })
            .then(function (res) {
              if (res.success) {
                window.info.done(Kooboo.text.info.delete.success);
                self.getTableData();
              } else {
                window.info.fail(Kooboo.text.info.delete.failed);
              }
            });
        }
      }
    }
  });
});
