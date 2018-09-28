$(function() {

    var siteUserModel = function() {
        var self = this;

        this.onAddUser = function() {

            Kooboo.SiteUser.getAvailableUsers().then(function(res) {
                if (res.success) {
                    self.availableUsers(res.model);
                    self.showAddUserModal(true);
                }
            })
        }

        this.showAddUserModal = ko.observable(false);

        this.onCancel = function() {
            this.showAddUserModal(false);
        }

        this.onSaveNewUser = function() {
            Kooboo.SiteUser.addUser({
                userId: self.newUser(),
                role: self.newRole()
            }).then(function(res) {
                if (res.success) {
                    self.onCancel();
                    getCurrentUsers();
                }
            })
        }

        this.availableUsers = ko.observableArray();
        this.newUser = ko.observable();

        this.availableRoles = ko.observableArray();
        this.newRole = ko.observable();

        Kooboo.SiteUser.getRoles().then(function(res) {
            if (res.success) {
                self.availableRoles(res.model.map(function(name) {
                    return {
                        name: name,
                        value: name
                    }
                }))
            }
        })

        getCurrentUsers();

        function getCurrentUsers() {
            Kooboo.SiteUser.getCurrentUsers().then(function(res) {
                if (res.success) {
                    var docs = res.model.map(function(user) {
                        return {
                            id: user.userId,
                            username: user.userName,
                            role: {
                                text: user.role,
                                class: 'label-sm green'
                            }
                        }
                    })

                    self.tableData({
                        docs: docs,
                        columns: [{
                            displayName: Kooboo.text.common.username,
                            fieldName: 'username',
                            type: 'text'
                        }, {
                            displayName: Kooboo.text.site.siteUser.role,
                            fieldName: 'role',
                            type: 'label'
                        }],
                        actions: [],
                        kbType: Kooboo.SiteUser.name
                    })
                }
            })
        }
    }

    siteUserModel.prototype = new Kooboo.tableModel(Kooboo.SiteUser.name);
    var vm = new siteUserModel();

    ko.applyBindings(vm, document.getElementById('main'));
})