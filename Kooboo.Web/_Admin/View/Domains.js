$(function() {
    function Domain() {
        var self = this;
        this.tableData = ko.observable({});
        this.dialogShow = ko.observable(false);
        this.domain = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+\.?/,
                message: Kooboo.text.validation.domainInvalid
            }
        });
        this.showError = ko.observable(false);
        this.registerPageUrl = Kooboo.Route.Domain.Register;

        this.dnsServers = ko.observableArray();
        this.providedIP = ko.observable();
        this.cName = ko.observable(); 

        function dataMapping(data) {
            var arr = [];
            arr = data.map(function(o) {
                o.expires = {
                    text: o.expires,
                    class: "label-sm label-warning"
                }
                o.domainName = {
                    text: o.domainName,
                    url: Kooboo.Route.Get(Kooboo.Route.Domain.DomainBinding, {
                        id: o.id
                    })
                };
                o.sitesCount = o.sites;
                o.sites = {
                    text: o.sites,
                    class: "blue"
                };
                o.useEmail = {
                    text: o.useEmail ? o.emails : Kooboo.text.common.no,
                    class: (o.useEmail ? "badge blue" : "label-sm label-default")
                };
                return o
            });
            return arr;
        }

        this.save = function() {
            if (!self.domain.isValid()) {
                self.showError(true)
            } else {
                var data = JSON.stringify({
                    domainname: self.domain()
                });
                Kooboo.Domain.creatDomain(data).then(function(res) {
                    if (res.success) {
                        self.getList();
                        self.cancelDialog();
                    }
                })
            }
        }

        this.showDialog = function() {
            self.dialogShow(true);
        }

        this.cancelDialog = function() {
            self.domain("");
            self.showError(false);
            self.dialogShow(false);
        }

        this.getList = function() {
            Kooboo.Domain.getList().then(function(data) {
                var ob = {
                    columns: [{
                        displayName: Kooboo.text.site.domain.name,
                        fieldName: "domainName",
                        type: "link"
                    }, {
                        displayName: Kooboo.text.site.domain.expires,
                        fieldName: "expires",
                        type: "label"
                    }, {
                        displayName: Kooboo.text.site.domain.records,
                        fieldName: "records",
                        type: "text"
                    }, {
                        displayName: Kooboo.text.site.domain.site,
                        fieldName: "sites",
                        type: "badge"
                    }, {
                        displayName: Kooboo.text.site.domain.email,
                        fieldName: "useEmail",
                        type: "label"
                    }],
                    kbType: "Domain",
                    onDelete: function(domains) {
                        if (confirm(Kooboo.text.confirm.deleteItems)) {
                            var containSites = false;
                            domains.forEach(function(dm) {
                                (dm.sitesCount > 0) && (containSites = true);
                            })

                            var ids = [];
                            if (containSites) {
                                if (confirm(Kooboo.text.confirm.domains.deleteDomainHasSite)) {
                                    ids = domains.map(function(dm) {
                                        return dm.id;
                                    })
                                } else {
                                    domains.forEach(function(dm) {
                                        (dm.sitesCount == 0) && ids.push(dm.id);
                                    })
                                }
                            } else {
                                ids = domains.map(function(dm) {
                                    return dm.id;
                                });
                            }

                            Kooboo.Domain.Deletes({
                                ids: JSON.stringify(ids)
                            }).then(function(res) {

                                if (res.success) {
                                    self.getList();
                                    window.info.done(Kooboo.text.info.delete.success);
                                }
                            });
                        }
                    }
                }
                ob.docs = dataMapping(data.model)
                self.tableData(ob);
            })
        }

        Kooboo.Domain.serverInfo().then(function(res) {
            if (res.success) {
                self.dnsServers(res.model.dnsServers);
                self.providedIP(res.model.ipAddress);
                self.cName(res.model.cName); 
            }
        })
    }
    Domain.prototype = new Kooboo.tableModel();
    var vm = new Domain();
    ko.applyBindings(vm, document.getElementById("main"));
    vm.getList();
})