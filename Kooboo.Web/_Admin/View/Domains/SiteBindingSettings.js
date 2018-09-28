$(function() {
    function Domain() {
        var self = this;
        this.rootDomain = ko.observableArray([]);
        this.root = ko.observable("");
        this.root.subscribe(function() {
            self.subdomain.valueHasMutated();
        })
        this.subdomain = ko.validateField("", {
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
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
                min: 1,
                max: 63,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 63
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
        this.defaultBinding.subscribe(function(def) {
            self.showError(false);
        })

        this.modalShow = ko.observable(false);
        this.showError = ko.observable(false);

        this.domains = ko.observableArray();

        function isValid() {
            if (self.defaultBinding() == 'port') {
                return self.port.isValid()
            } else {
                return self.subdomain.isValid()
            }
        }

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

        this.save = function() {
            if (!isValid()) {
                self.showError(true)
            } else {
                Kooboo.Binding.post({
                    subdomain: self.subdomain(),
                    rootdomain: self.root(),
                    port: self.port(),
                    defaultBinding: (self.defaultBinding() == 'port')
                }).then(function() {
                    self.cancelDialog();
                    getList();
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
            Kooboo.Binding.listBySite().then(function(res) {
                if (res.success) {
                    var ob = {
                        columns: [{
                            displayName: Kooboo.text.site.domain.name,
                            fieldName: "domain",
                            type: "text"
                        }],
                        kbType: "Binding"
                    }

                    ob.docs = res.model ? dataMapping(res.model) : [];
                    self.tableData(ob);
                    self.domains(res.model);
                }
            })
        }

    }
    Domain.prototype = new Kooboo.tableModel();
    ko.applyBindings(new Domain, document.getElementById("main"));
})