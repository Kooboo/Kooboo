$(function() {

    var menusViewModal = function() {

        var self = this;

        this.showError = ko.observable(false);

        this.menuModal = ko.observable(false);

        this.menuName = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                message: Kooboo.text.validation.objectNameRegex
            },
            stringlength: {
                min: 1,
                max: 64,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
            },
            remote: {
                url: Kooboo.Menu.isUniqueName(),
                data: {
                    name: function() {
                        return self.menuName()
                    }
                },
                message: Kooboo.text.validation.taken
            }
        });

        this.onCreate = function() {
            self.menuModal(true);
            self.menuName("");
        };

        this.hideMenuModal = function() {
            self.resetCreateModal();
        };

        this.resetCreateModal = function() {
            self.menuModal(false);
            self.menuName("");
            self.showError(false);
        }

        this.isCreateValid = function() {
            return self.menuName.isValid();
        }

        this.handleEnter = function(m, e) {

            if (e.keyCode == 13) {
                self.onCreateSubmit();
            }
        }

        this.onCreateSubmit = function() {

            if (self.isCreateValid()) {
                self.showError(false);

                Kooboo.Menu.Create({
                    Name: self.menuName()
                }).then(function(res) {

                    if (res.success) {
                        self.resetCreateModal();
                        renderList();
                    }
                })
            } else {
                self.showError(true);
            }
        }

    }

    menusViewModal.prototype = new Kooboo.tableModel(Kooboo.Menu.name);

    var vm = new menusViewModal();

    function renderList() {

        Kooboo.Menu.getList().then(function(res) {

            if (res.success) {
                var menuList = [];
                _.forEach(res.model, function(menu) {
                    var date = new Date(menu.lastModified);

                    var model = {
                        id: menu.id,
                        name: {
                            text: menu.name,
                            url: Kooboo.Route.Get(Kooboo.Route.Menu.DetailPage, {
                                Id: menu.id
                            })
                        },
                        relationsComm: "kb/relation/modal/show",
                        relationsTypes: Object.keys(menu.relations),
                        relations: menu.relations,
                        date: date.toDefaultLangString(),
                        versions: {
                            iconClass: "fa-clock-o",
                            title: Kooboo.text.common.viewAllVersions,
                            url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
                                KeyHash: menu.keyHash,
                                storeNameHash: menu.storeNameHash
                            }),
                            newWindow: true
                        }
                    };

                    menuList.push(model);
                });

                var columns = [{
                    displayName: Kooboo.text.common.name,
                    fieldName: 'name',
                    type: 'link'
                }, {
                    displayName: Kooboo.text.common.usedBy,
                    fieldName: "relations",
                    type: "communication-refer"
                }, {
                    displayName: Kooboo.text.common.lastModified,
                    fieldName: 'date',
                    type: 'text'
                }];

                var cpnt = {
                    docs: menuList,
                    columns: columns,
                    tableActions: [{
                        fieldName: "versions",
                        type: "link-icon"
                    }],
                    kbType: Kooboo.Menu.name
                };

                vm.tableData(cpnt);
            }
        });
    }

    renderList();

    ko.applyBindings(vm, document.getElementById("main"));
});