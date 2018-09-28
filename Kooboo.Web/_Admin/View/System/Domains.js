$(function() {
    function Domain() {
        var self = this;
        this.siteName = ko.observable();
        this.tableData = ko.observable({});
        this.rootDomain = ko.observableArray([]);
        this.root = ko.observable("");
        this.root.subscribe(function() {
            self.subdomain.valueHasMutated();
        })
        this.domains = ko.observableArray();
        this.subdomain = ko.observable();
        this.subdomain = ko.validateField('', {
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*$/,
                message: Kooboo.text.validation.objectNameRegex
            },
            localUnique: {
                compare: function() {
                    var exist = _.map(self.domains(), function(dm) {
                        return dm.fullName;
                    });
                    var res = [self.subdomain()];
                    if (exist.indexOf(self.subdomain() + '.' + self.root()) > -1) {
                        res.push(self.subdomain());
                    }
                    return res;
                }
            },
            stringlength: {
                min: 0,
                max: 63,
                message: Kooboo.text.validation.minLength + 0 + ", " + Kooboo.text.validation.maxLength + 63
            },
            remote: {
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                message: Kooboo.text.validation.taken,
                type: "get",
                data: {
                    SubDomain: function() {
                        return self.subdomain();
                    },
                    RootDomain: function() {
                        return self.root();
                    }
                }
            }
        });
        this.port = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^\d*$/,
                message: Kooboo.text.validation.invaildPort
            },
            range: {
                from: 0,
                to: 65535,
                message: Kooboo.text.validation.portRange
            }
        });
        this.defaultBinding = ko.observable('domain');
        this.defaultBinding.subscribe(function(db) {
            self.showError(false);
        })
        this.modalShow = ko.observable(false);
        this.showError = ko.observable(false);

        function dataMapping(data) {
            var arr = [];
            arr = data.map(function(o) {
                return {
                    domain: o.fullName,
                    id: o.id
                }
            })
            return arr;
        }

        function isValid() {
            if (self.defaultBinding() == 'port') {
                return self.port.isValid()
            } else {
                return self.subdomain.isValid()
            }
        }

        this.save = function() {
            if (!isValid()) {
                self.showError(true)
            } else {
                Kooboo.Binding.post({
                    subdomain: self.subdomain(),
                    rootdomain: self.root(),
                    port: self.port(),
                    defaultBinding: self.defaultBinding() == 'port'
                }).then(function() {
                    getList();
                    self.cancelDialog();
                })
            }
        }

        this.showDialog = function() {
            self.modalShow(true);
        }

        this.cancelDialog = function() {
            self.subdomain("");
            self.port("");
            self.defaultBinding('domain');
            self.showError(false);
            self.modalShow(false);
        }

        Kooboo.Domain.getList().then(function(res) {
            self.rootDomain(res.model)
        })

        getList();

        function getList() {
            Kooboo.Binding.listBySite().then(function(data) {
                var ob = {
                    columns: [{
                        displayName: Kooboo.text.site.domain.name,
                        fieldName: "domain",
                        type: "text"
                    }],
                    kbType: "Binding"
                }

                ob.docs = dataMapping(data.model)
                self.tableData(ob);

                self.domains(data.model);
            })
        }

        Kooboo.EventBus.subscribe("kb/table/delete/finish", function(data) {
            var newDomains = self.domains().filter(function(domain) {
                return data.ids.indexOf(domain.id) == -1;
            })

            self.domains(newDomains);
        })

        Kooboo.Site.getName().then(function(res) {
            res.success && self.siteName(res.model);
        })
    }
    Domain.prototype = new Kooboo.tableModel();
    ko.applyBindings(new Domain, document.getElementById("main"));
})