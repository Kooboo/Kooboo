(function() {
  var Roles = function() {
    var self = this;

    this.showError = ko.observable(false);

    this.onCreate = function() {
      this.currentRole(null);
      this.showEditModal(true);
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

    this.onSaveRole = function() {
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
            docs: res.model.map(function(item, idx) {
              return {
                id: idx,
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
            kbType: Kooboo.Role.name,
            unselectable: true
          });
        }
      });
    };

    this.getList();

    this.getPermissionTree = function(items) {
      if (items && items.length) {
        return items.map(function(item) {
          return {
            text: item.name,
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
        return items.map(function(item) {
          return {
            name: item.text,
            selected: item.state.selected,
            subItems: self.getTreePermission(item.children)
          };
        });
      } else {
        return [];
      }
    };

    Kooboo.EventBus.subscribe("kb/setting/role/edit", function(data) {
      let idx = data.id;

      self.currentRole(self.cacheData()[idx]);
      self.roleName(self.currentRole().name);

      console.log(self.getPermissionTree(self.currentRole().subItems));
      var treeData = self.getPermissionTree(self.currentRole().subItems);
      $("#permission-area")
        .jstree({
          plugins: ["wholerow", "checkbox"],
          checkbox: {
            keep_selected_style: false
          },
          core: {
            strings: { "Loading ...": Kooboo.text.common.loading + " ..." },
            data: treeData
          }
        })
        .on("loaded.jstree", function(event, data) {
          $("#permission-area")
            .jstree()
            .open_all();
        });
      self.showEditModal(true);
    });
  };

  Roles.prototype = new Kooboo.tableModel(Kooboo.Role.name);
  var vm = new Roles();
  ko.applyBindings(vm, document.getElementById("main"));
})();
