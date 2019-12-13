(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        showEditModal: false,
        cacheData: [],
        tree: null,
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.Roles
          }
        ],
        tableData: [],
        selected: [],
        roleForm: {
          name: ""
        },
        roleRules: {
          name: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              remote: {
                url: Kooboo.Role.isUniqueName(),
                data: function() {
                  return {
                    name: self.roleForm.name
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ]
        },
        isEdit: false
      };
    },
    mounted: function() {
      this.getList();
    },
    methods: {
      onCreate: function() {
        self.isEdit = false;
        Kooboo.Role.getEdit().then(function(res) {
          if (res.success) {
            self.roleForm.name = "";
            self.showEditModal = true;
            var treeData = self.getPermissionTree(res.model.subItems);
            self.renderTree(treeData);
          }
        });
      },
      onEdit: function(data) {
        self.isEdit = true;
        let currentRole = self.cacheData.find(function(item) {
          return item.id == data.id;
        });

        self.roleForm.name = currentRole.name;
        var treeData = self.getPermissionTree(currentRole.subItems);
        self.renderTree(treeData);
        self.showEditModal = true;
      },
      onSaveRole: function() {
        // debugger;
        var isValid = self.isEdit || self.$refs.roleForm.validate();
        if (isValid) {
          var permissions = self.getTreePermission(
            $("#permission-area")
              .jstree(true)
              .get_json()
          );

          Kooboo.Role.post({
            name: self.roleForm.name,
            selected: false,
            subItems: permissions
          }).then(function(res) {
            window.info.done(Kooboo.text.info.save.success);
            self.getList();
            self.onModalHide();
          });
        }
      },
      onModalHide: function() {
        var permissionArea = $("<div>");
        $(permissionArea).attr("id", "permission-area");
        $("#area").empty();
        $("#area").append(permissionArea);
        this.roleForm.name = "";
        self.$refs.roleForm.clearValid();
        this.showEditModal = false;
      },
      getList: function() {
        Kooboo.Role.getList().then(function(res) {
          if (res.success) {
            self.cacheData = res.model;
            var docs = res.model.map(function(item) {
              return {
                id: item.id,
                name: {
                  class: "label-sm blue",
                  text: item.name
                },
                edit: {
                  text: Kooboo.text.common.edit,
                  url: "kb/setting/role/edit"
                }
              };
            });
            self.tableData = docs;
          }
        });
      },
      getPermissionTree: function(items) {
        if (items && items.length) {
          return items.map(function(item) {
            return {
              text: item.displayName || item.name,
              data: {
                value: item.name
              },
              state: {
                selected: item.selected
              },
              children: self.getPermissionTree(item.subItems)
            };
          });
        } else {
          return [];
        }
      },
      getTreePermission: function(items) {
        if (items && items.length) {
          return items.map(function(item) {
            return {
              name: item.data.value,
              selected: item.state.selected,
              subItems: self.getTreePermission(item.children)
            };
          });
        } else {
          return [];
        }
      },
      renderTree: function(treeData) {
        $("#permission-area").jstree({
          plugins: ["wholerow", "checkbox"],
          checkbox: {
            keep_selected_style: false
          },
          core: {
            strings: { "Loading ...": Kooboo.text.common.loading + " ..." },
            data: treeData
          }
        });
      },
      onDelete: function() {
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var ids = self.selected.map(function(row) {
            return row.id;
          });
          Kooboo.Role.Deletes({
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
      }
    }
  });
})();
