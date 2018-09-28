$(function() {
    var Domain = function() {
        var self = this;
        this.domains = ko.observableArray();
        this.subDomain = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                message: Kooboo.text.validation.objectNameRegex
            },
            localUnique: {
                compare: function() {
                    return _.concat(_.map(self.domains(), function(dm) {
                        return dm.subDomain
                    }), self.subDomain())
                }
            },
            stringlength: {
                min: 1,
                max: 63,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 63
            }
        });
        this.rootDomain = ko.observable("");
        this.siteId = ko.observable("");

        this.sites = ko.observableArray([]);

        this.showNewBindingModal = ko.observable(false);

        this.onShowModal = function() {
            self.showNewBindingModal(true);
        }

        function dataMapping(domainData) {
            var arr = [];
            arr = domainData.map(function(o) {
                return {
                    domain: o.fullName,
                    id: o.id,
                    site: {
                        text: o.siteName,
                        title: Kooboo.text.common.edit,
                        url: Kooboo.Route.Get(Kooboo.Route.Site.DetailPage, {
                            SiteId: o.webSiteId
                        }),
                        newWindow: true
                    },
                    preview: {
                        text: Kooboo.text.common.preview,
                        url: "//" + o.fullName,
                        newWindow: true
                    }
                }
            })

            return arr;
        }

        this.isValid = function() {
            return self.subDomain.isValid();
        }

        this.showError = ko.observable(false);

        this.save = function() {
            if (self.isValid()) {
                Kooboo.Binding.post({
                    SubDomain: self.subDomain(),
                    RootDomain: self.rootDomain(),
                    SiteId: self.siteId()
                }).then(function(res) {
                    if (res.success) {
                        self.reset();
                        self.getList();
                    }
                })
            } else {
                self.showError(true);
            }
        }

        this.reset = function() {
            self.showNewBindingModal(false);
            self.subDomain("");
            self.showError(false);
        }

        Kooboo.Domain.get({
            id: Kooboo.getQueryString("id")
        }).then(function(res) {
            if (res.success) {
                self.rootDomain(res.model.domainName);
                Kooboo.Site.getList().then(function(data) {
                    self.sites(data.model);
                    self.getList();
                })
            }
        })

        this.getList = function() {
            Kooboo.Binding.ListByDomain({
                domainid: Kooboo.getQueryString("id")
            }).then(function(res) {
                if (res.success) {
                    var ob = {
                        columns: [{
                            displayName: Kooboo.text.common.name,
                            fieldName: "domain",
                            type: "text"
                        }, {
                            displayName: Kooboo.text.site.domain.site,
                            fieldName: "site",
                            type: "link"
                        }],
                        tableActions: [{
                            fieldName: "preview",
                            type: "link-btn"
                        }],
                        kbType: "Binding"
                    }

                    ob.docs = dataMapping(res.model)
                    self.tableData(ob);

                    self.domains(res.model);
                }
            })
        }

    }

    Domain.prototype = new Kooboo.tableModel();
    ko.applyBindings(new Domain, document.getElementById("main"));
})