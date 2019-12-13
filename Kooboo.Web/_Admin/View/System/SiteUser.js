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
            name: Kooboo.text.common.SiteUser
          }
        ],
        showAddUserModal: false,
        availableUsers: [],
        newUser: "",
        availableRoles: [],
        newRole: "",
        selected: [],
        tableData: []
      };
    },
    mounted: function() {
      self.getRoles();
      self.getCurrentUsers();
    },
    methods: {
      getRoles: function() {
        Kooboo.SiteUser.getRoles().then(function(res) {
          if (res.success) {
            self.availableRoles = res.model.map(function(name) {
              return {
                name: name,
                value: name
              };
            });
            self.newRole = self.availableRoles[0].value;
          }
        });
      },
      getCurrentUsers: function() {
        Kooboo.SiteUser.getCurrentUsers().then(function(res) {
          if (res.success) {
            var docs = res.model.map(function(user) {
              return {
                id: user.userId,
                username: user.userName,
                role: {
                  text: user.role,
                  class: "label-sm green"
                }
              };
            });
            self.tableData = docs;
          }
        });
      },
      onAddUser: function() {
        Kooboo.SiteUser.getAvailableUsers().then(function(res) {
          if (res.success) {
            self.availableUsers = res.model;
            if (self.availableUsers && self.availableUsers.length) {
              self.newUser = self.availableUsers[0].userId;
            }
            self.showAddUserModal = true;
          }
        });
      },
      onCancel: function() {
        self.showAddUserModal = false;
        self.newUser = "";
      },
      onSaveNewUser: function() {
        Kooboo.SiteUser.addUser({
          userId: self.newUser,
          role: self.newRole
        }).then(function(res) {
          if (res.success) {
            self.onCancel();
            self.getCurrentUsers();
          }
        });
      },
      onDelete: function() {
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var ids = self.selected.map(function(row) {
            return row.id;
          });

          Kooboo.SiteUser.Deletes({
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
});
