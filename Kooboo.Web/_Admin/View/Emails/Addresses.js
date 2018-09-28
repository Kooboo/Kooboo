$(function() {

    var Guid = Kooboo.Guid,
        AddressType = {
            NORMAL: "normal",
            WILDCARD: "wildcard",
            GROUP: "group",
            FORWARD: "forward"
        }

    var addressesModel = function() {
        var self = this;

        this.showError = ko.observable(false);

        this.domains = ko.observableArray();

        this.domain = ko.validateField({
            required: Kooboo.text.validation.required
        });

        this.forwardEditing = ko.observable(false);

        //modal
        this.showModal = ko.observable(false);

        this.modalType = ko.observable();

        this.onShowModal = function(type) {
            self.modalType(type);
            self.showModal(true);
        }

        this.resetModal = function() {
            self.showModal(false);
            self.showError(false);

            _.forEach(self.members(), function(m) {
                m.showError(false);
            })

            // normal
            self.normalAddress(null);

            // wildcard
            self.wildcardAddress(null);

            // group
            self.groupAddress(null);
            self.members([]);

            // forward
            self.forwardName(null);
            self.forwardAddress(null);
            self.forwardEditing(false);

            //members
            self.editingMember(false);

            self.modalType(null);
        }

        this.saveModal = function() {
            switch (self.modalType()) {
                case "normal":
                    if (self.isNormalValid()) {
                        Kooboo.EmailAddress.post(JSON.stringify({
                            local: self.normalAddress(),
                            addressType: AddressType.NORMAL,
                            domain: self.domain() || self.domains().domainName
                        })).then(function(res) {
                            if (res.success) {
                                self.successOperation(res.model);
                            }
                        })
                    } else {
                        self.showError(true);
                    }
                    break;
                case 'wildcard':
                    if (self.isWildcardValid()) {
                        Kooboo.EmailAddress.post(JSON.stringify({
                            local: self.wildcardAddress(),
                            addressType: AddressType.WILDCARD,
                            domain: self.domain() || self.domains().domainName
                        })).then(function(res) {
                            if (res.success) {
                                self.successOperation(res.model);
                            }
                        })
                    } else {
                        self.showError(true);
                    }
                    break;
                case 'group':
                    if (self.isGroupValid()) {
                        Kooboo.EmailAddress.post(JSON.stringify({
                            local: self.groupAddress(),
                            addressType: AddressType.GROUP,
                            domain: self.domain() || self.domains().domainName
                        })).then(function(res) {
                            if (res.success) {
                                self.successOperation(res.model);
                            }
                        })
                    } else {
                        self.showError(true);
                    }
                    break;
                case 'forward':
                    if (self.isForwardValid()) {
                        if (self.forwardEditing()) {
                            Kooboo.EmailAddress.updateForward({
                                id: self.currentMembersParentId(),
                                forwardAddress: self.forwardAddress()
                            }).then(function(res) {
                                if (res.success) {
                                    self.showModal(false);
                                    self.refreshList();
                                    window.info.show(Kooboo.text.info.update.success, true);
                                } else {
                                    window.info.show(Kooboo.text.info.update.fail, false);
                                }
                            })
                        } else {
                            Kooboo.EmailAddress.post(JSON.stringify({
                                local: self.forwardName(),
                                addressType: AddressType.FORWARD,
                                domain: self.domain() || self.domains().domainName,
                                forwardAddress: self.forwardAddress()
                            })).then(function(res) {

                                if (res.success) {
                                    self.successOperation(res.model);
                                }
                            })
                        }
                    } else {
                        self.showError(true);
                    }
                    break;
            }
        }

        this.getMailData = function(data) {
            var useFor = {},
                remark = {},
                rawData = {};

            switch (data.addressType.toLowerCase()) {
                case AddressType.NORMAL:
                    remark = {};
                    useFor = {
                        text: Kooboo.text.mail.address.normal,
                        class: "label-sm green"
                    }
                    break;
                case AddressType.WILDCARD:
                    remark = {};
                    useFor = {
                        text: Kooboo.text.mail.address.wildcard,
                        class: "label-sm green"
                    }
                    break;
                case AddressType.GROUP:
                    useFor = {
                        text: Kooboo.text.mail.address.groupMail,
                        class: "label-sm blue"
                    }
                    remark = {
                        text: data.count + " " + Kooboo.text.mail[data.count > 1 ? "members" : "member"],
                        url: "kb/table/email/group/modal",
                        class: "label-sm blue"
                    }
                    break;
                case AddressType.FORWARD:
                    useFor = {
                        text: Kooboo.text.mail.address.forwarding,
                        class: "label-sm orange"
                    }
                    remark = {
                        text: Kooboo.text.mail.to + ": " + data.forwardAddress,
                        class: "label-sm orange",
                        url: "kb/table/email/forward/modal"
                    }
                    rawData = {
                        address: data.address,
                        forward: data.forwardAddress
                    }
                    break;
            }

            var model = {
                id: data.id,
                name: data.address,
                useFor: useFor,
                remark: remark,
                rawData: rawData,
                Inbox: {
                    iconClass: "fa-inbox",
                    title: Kooboo.text.mail.jumpToInbox,
                    url: Kooboo.Route.Get(Kooboo.Route.Email.InboxPage, {
                        address: data.address
                    })
                }
            };

            return model;
        }

        this.successOperation = function(data) {
            var _table = _.cloneDeep(self.tableData());

            _table.docs.push(self.getMailData(data));

            self.tableData(_table);
            self.resetModal();
            Kooboo.EventBus.publish("kb/mail/menu/refresh");
        }

        // normal
        this.normalAddress = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^[a-zA-Z0-9!#$%&+\-/=?^_`{|}~]+(\.[a-zA-Z0-9!#$%&+\-/=?^_`{|}~]+)*$/,
                message: Kooboo.text.validation.emailInvalid
            },
            remote: {
                url: Kooboo.EmailAddress.isUniqueName(),
                data: {
                    local: function() {
                        return self.normalAddress()
                    },
                    domain: function() {
                        return self.domain() || (self.domains().length && self.domains()[0].domainName) || ""
                    },
                    addressType: AddressType.NORMAL
                },
                message: Kooboo.text.validation.taken
            }
        })

        this.isNormalValid = function() {
            return self.normalAddress.isValid() && self.domain.isValid();
        }

        // wildcard
        this.wildcardAddress = ko.observable();
        this.wildcardAddress = ko.validateField({
            remote: {
                url: Kooboo.EmailAddress.isUniqueName(),
                data: {
                    local: function() {
                        return self.wildcardAddress()
                    },
                    domain: function() {
                        return self.domain() || (self.domains().length && self.domains()[0].domainName) || ""
                    },
                    addressType: AddressType.WILDCARD
                },
                message: Kooboo.text.validation.taken
            }
        })

        this.isWildcardValid = function() {
            return self.wildcardAddress.isValid() && self.domain.isValid();
        }

        // group
        this.groupAddress = ko.validateField({
            required: Kooboo.text.validation.required,
            remote: {
                url: Kooboo.EmailAddress.isUniqueName(),
                data: {
                    local: function() {
                        return self.groupAddress()
                    },
                    domain: function() {
                        return self.domain() || (self.domains().length && self.domains()[0].domainName) || ""
                    },
                    addressType: AddressType.Group
                },
                message: Kooboo.text.validation.taken,
            }
        })

        this.isGroupValid = function() {
            return self.groupAddress.isValid() && self.domain.isValid();
        }

        // forward
        this.forwardName = ko.validateField({
            required: Kooboo.text.validation.required,
            remote: {
                url: Kooboo.EmailAddress.isUniqueName(),
                data: {
                    local: function() {
                        return self.forwardName()
                    },
                    domain: function() {
                        return self.domain() || (self.domains().length && self.domains()[0].domainName) || ""
                    },
                    addressType: AddressType.NORMAL
                },
                message: Kooboo.text.validation.taken,
            }
        })

        this.forwardAddress = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
                message: Kooboo.text.validation.emailInvalid
            }
        })

        this.isForwardValid = function() {
            if (!self.forwardEditing()) {
                return self.forwardName.isValid() && self.forwardAddress.isValid() && self.domain.isValid();
            } else {
                return self.forwardName.isValid() && self.forwardAddress.isValid();
            }
        }

        // members
        this.currentMembersParentId = ko.observable();

        this.members = ko.observableArray();

        this.onEditMember = function() {
            self.editingMember(true);
            self.members.push(new memberModel({
                emailAddressId: self.currentMembersParentId(),
                memberAddress: ""
            }))
        }

        this.editingMember = ko.observable(false);

        this.removeMember = function(m) {
            Kooboo.EmailAddress.removeMember({
                addressId: self.currentMembersParentId(),
                memberAddress: m.memberAddress()
            }).then(function(res) {

                if (res.success) {
                    self.members.remove(m);
                    self.refreshList();
                }
            })
        }

        this.isMembersValid = function() {
            var valid = true;
            _.forEach(self.members(), function(m) {
                valid = valid && m.address.isValid()
            });
            return valid;
        }

        this.saveMember = function(m) {

            if (self.isMembersValid()) {
                Kooboo.EmailAddress.saveMember({
                    addressId: m.emailAddressId(),
                    memberAddress: m.address()
                }).then(function(res) {

                    if (res.success) {
                        self.members.remove(m);
                        self.members.push(new memberModel(res.model));
                        self.editingMember(false);
                        self.refreshList();
                    }
                })

            } else {
                _.forEach(self.members(), function(m) {
                    m.showError(true);
                })
            }
        }

        this.cancelMember = function(m) {
            m.showError(false);
            self.editingMember(false);
            self.members.remove(m);
        }

        this.refreshList = function() {
            Kooboo.EmailAddress.getList().then(function(res) {

                if (res.success) {
                    var addresses = [];
                    _.forEach(res.model, function(model) {
                        addresses.push(self.getMailData(model));
                    });

                    var cpnt = {
                        columns: [{
                            displayName: Kooboo.text.mail.address.name,
                            fieldName: "name",
                            type: "text"
                        }, {
                            displayName: Kooboo.text.common.useFor,
                            fieldName: "useFor",
                            type: "label"
                        }, {
                            displayName: Kooboo.text.common.remark,
                            fieldName: "remark",
                            type: "communication-label"
                        }],
                        docs: addresses,
                        kbType: Kooboo.EmailAddress.name,
                        tableActions: [{
                            fieldName: "Inbox",
                            type: "link-icon"
                        }]
                    }

                    self.tableData(cpnt);
                }
            })
        }
    }

    addressesModel.prototype = new Kooboo.tableModel(Kooboo.EmailAddress.name);
    var vm = new addressesModel();

    ko.applyBindings(vm, document.getElementById("main"));

    Kooboo.EmailAddress.Domains().then(function(res) {
        if (res.success) {
            vm.domains(res.model);
            vm.refreshList();
        }
    })

    Kooboo.EventBus.subscribe("kb/table/delete/finish", function() {
        Kooboo.EventBus.publish("kb/mail/menu/refresh");
    })

    Kooboo.EventBus.subscribe("kb/table/email/group/modal", function(id) {
        vm.currentMembersParentId(id);
        Kooboo.EmailAddress.getMemberList({
            addressId: id
        }).then(function(res) {

            if (res.success) {
                res.model.forEach(function(address) {
                    vm.members.push(new memberModel({
                        memberAddress: address
                    }));
                })
                vm.modalType("members");
                vm.showModal(true);
            }
        })
    })

    Kooboo.EventBus.subscribe("kb/table/email/forward/modal", function(id) {
        var _tableData = _.cloneDeep(vm.tableData());
        var find = _.find(_tableData.docs, function(data) {
            return data.id == id;
        })

        if (find) {
            vm.currentMembersParentId(id);
            vm.modalType("forward");
            vm.showModal(true);
            vm.forwardEditing(true);
            vm.forwardName(find.rawData.address);
            vm.forwardAddress(find.rawData.forward);
        }
    })

    var memberModel = function(member) {
        var self = this;

        ko.mapping.fromJS(member, {}, self);

        self.showError = ko.observable(false);

        self.address = ko.validateField(self.memberAddress(), {
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
                message: Kooboo.text.validation.emailInvalid
            }
        })
    }
})