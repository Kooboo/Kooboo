$(function() {

    var createSiteViewModel = function() {
        var self = this;

        self.showError = ko.observable(false);

        self.siteName = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                message: Kooboo.text.validation.siteNameInvalid
            },
            remote: {
                url: Kooboo.Site.isUniqueName(),
                message: Kooboo.text.validation.taken,
                type: "get",
                data: {
                    SiteName: function() {
                        return self.siteName();
                    }
                }
            }
        });
        self.siteName.subscribe(function(val) {
            // var name = _.words(val).join("-");
            self.subDomain(val);
        });

        self.subDomain = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                message: Kooboo.text.validation.siteNameRegex
            },
            stringlength: {
                min: 1,
                max: 63,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 63
            },
            remote: {
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                message: Kooboo.text.validation.taken,
                type: "get",
                data: {
                    SubDomain: function() {
                        return self.subDomain();
                    },
                    RootDomain: function() {
                        return self.rootDomain();
                    }
                }
            }
        });

        self.domains = ko.observableArray();

        self.rootDomain = ko.observable("");

        self.SPAClick = function(m, e) {
            e.preventDefault();
            self.showError(false);
            Kooboo.SPA.getView(Kooboo.Route.Site.ListPage, {
                container: '[layout="default"]'
            })
        }

        self.isValid = function() {
            return self.siteName.isValid() && self.subDomain.isValid()
        }

        self.onCreateSubmit = function() {

            if (self.isValid()) {
                self.showError(false);

                Kooboo.Site.Create({
                    SiteName: self.siteName(),
                    SubDomain: self.subDomain(),
                    RootDomain: self.rootDomain()
                }).then(function(res) {

                    if (res.success) {
                        location.href = Kooboo.Route.Get(Kooboo.Route.Site.DetailPage, {
                            SiteId: res.model.id
                        });
                    }
                });

            } else {
                self.showError(true);
            }
        }
    };

    var vm = new createSiteViewModel();
    $.when(Kooboo.Domain.getAvailable()).then(function(availRes) {
        if (availRes.success) {
            vm.domains(availRes.model);
            ko.applyBindings(vm, document.getElementById("main"));
        }
    })
})