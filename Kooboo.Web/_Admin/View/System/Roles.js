(function() {
  var Roles = function() {
    var self = this;

    this.showError = ko.observable(false);

    this.onCreate = function() {
      Kooboo.Role.getEdit().then(function(res) {
        if (res.success) {
          self.currentRole(null);
          self.showEditModal(true);
          var treeData = self.getPermissionTree(res.model.subItems);
          self.renderTree(treeData);
        }
      });
    };

    this.roleName = ko.validateField({
      required: Kooboo.text.validation.required,
      remote: {
        url: Kooboo.Role.isUniqueName(),
        message: Kooboo.text.validation.taken,
        type: "get",
        data: {
          name: function() {
            return self.roleName();
          }
        }
      }
    });

    this.showEditModal = ko.observable(false);

    this.onSaveRole = function () {
        debugger;
      var permissions = self.getTreePermission(
        $("#permission-area")
          .jstree(true)
          .get_json()
      );

      if (!self.currentRole()) {
        if (self.roleName.isValid()) {
          update();
        } else {
          self.showError(true);
        }
      } else {
        update();
      }

      function update() {
        Kooboo.Role.post({
          name: self.roleName(),
          selected: false,
          subItems: permissions
        }).then(function(res) {
          window.info.done(Kooboo.text.info.save.success);
          self.getList();
          self.onModalHide();
        });
      }
    };

    this.onModalHide = function() {
      var permissionArea = $("<div>");
      $(permissionArea).attr("id", "permission-area");
      $("#area").empty();
      $("#area").append(permissionArea);
      this.showError(false);
      this.roleName("");
      this.showEditModal(false);
    };

    this.cacheData = ko.observable();
    this.currentRole = ko.observable();

    this.tree = null;

    this.getList = function() {
      Kooboo.Role.getList().then(function(res) {
        if (res.success) {
          self.cacheData(res.model);
          self.tableData({
            docs: res.model.map(function(item) {
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
            }),
            columns: [
              {
                displayName: "Role Name",
                fieldName: "name",
                type: "label"
              }
            ],
            tableActions: [
              {
                fieldName: "edit",
                type: "communication-btn"
              }
            ],
            kbType: Kooboo.Role.name
          });
        }
      });
    };

    this.getList();

    this.getPermissionTree = function(items) {
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
    };

    this.getTreePermission = function(items) {
      if (items && items.length) {
          return items.map(function (item) {
          return {
            name: item.data.value,
            selected: item.state.selected,
            subItems: self.getTreePermission(item.children)
          };
        });
      } else {
        return [];
      }
    };

    Kooboo.EventBus.subscribe("kb/setting/role/edit", function(data) {
      let currentRole = self.cacheData().find(function(item) {
        return item.id == data.id;
      });

      self.currentRole(currentRole);
      self.roleName(currentRole.name);

      var treeData = self.getPermissionTree(currentRole.subItems);
      self.renderTree(treeData);
      self.showEditModal(true);
    });

    this.renderTree = function(treeData) {
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
    };
  };

  Roles.prototype = new Kooboo.tableModel(Kooboo.Role.name);
  var vm = new Roles();
  ko.applyBindings(vm, document.getElementById("main"));
})();
