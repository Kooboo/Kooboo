(function() {
    var template = Kooboo.getTemplate('/_Admin/Scripts/components/kbTemplateModal.html'),
        slider = null

    ko.components.register('kb-template-modal', {
        viewModel: function(params) {
            var self = this;

            this.showError = ko.observable(false);

            this.isShow = params.isShow;

            this.template = params.data;
            this.template.subscribe(function(data) {
                var temp = data;
                var date = new Date(data.lastModified),
                    size = Kooboo.bytesToSize(data.size);

                temp.size = size;
                temp.lastModified = date.toDefaultLangString();
                temp.allDynamicCount = temp.layoutCount +
                    temp.menuCount +
                    temp.pageCount +
                    temp.viewCount +
                    temp.imageCount +
                    temp.contentCount;

                self.data(temp);
                setTimeout(function() {
                    slider = $(".bxslider").bxSlider({ auto: false })
                }, 300);
            })

            this.data = ko.observable();

            this.selected = ko.observable(false);

            this.onUse = function() {
                self.selected(true);
            }

            this.onHide = function() {
                slider.destroySlider();
                slider = null;
                self.isShow(false);
            }

            this.isAbleToImport = function() {
                return self.siteName.isValid() && self.subDomain.isValid();
            }

            this.onImport = function() {
                if (self.isAbleToImport()) {
                    Kooboo.Template.Use({
                        siteName: self.siteName(),
                        subDomain: self.subDomain(),
                        rootDomain: self.rootDomain(),
                        downloadCode: self.data().downloadCode
                    }).then(function(res) {

                        if (res.success) {
                            location.href = Kooboo.Route.Get(Kooboo.Route.Site.DetailPage, {
                                SiteId: res.model
                            })
                        }
                    })
                } else {
                    self.showError(true);
                }
            }

            this.onUnuse = function() {
                self.showError(false);
                self.selected(false);

                slider.destroySlider();
                slider = $('.bxslider').bxSlider({ auto: false });
            }

            this.siteName = ko.validateField({
                required: Kooboo.text.validation.required,
                remote: {
                    url: Kooboo.Site.isUniqueName(),
                    message: Kooboo.text.validation.taken,
                    type: "get",
                    data: {
                        name: function() {
                            return self.siteName()
                        }
                    }
                },
                regex: {
                    pattern: /^[A-Za-z][\w\-]*$/,
                    message: Kooboo.text.validation.siteNameInvalid
                }
            })

            this.siteName.subscribe(function(val) {
                // var name = _.words(val).join("-");
                self.subDomain(val);
            });

            this.subDomain = ko.validateField({
                required: Kooboo.text.validation.required,
                remote: {
                    url: Kooboo.Site.CheckDomainBindingAvailable(),
                    message: Kooboo.text.validation.taken,
                    type: "get",
                    data: {
                        SubDomain: function() {
                            return self.subDomain && self.subDomain();
                        },
                        RootDomain: function() {
                            return self.rootDomain && self.rootDomain();
                        }
                    }
                },
                stringlength: {
                    min: 1,
                    max: 63,
                    message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 63
                },
                regex: {
                    pattern: /^[A-Za-z][\w\-]*$/,
                    message: Kooboo.text.validation.siteNameInvalid
                }
            })

            this.domains = ko.observableArray();

            this.rootDomain = ko.observable();

            Kooboo.Domain.getAvailable().then(function(res) {
                if (res.success) {
                    self.domains(res.model.map(function(domain) {
                        return {
                            displayText: '.' + domain.domainName,
                            value: domain.domainName
                        }
                    }));
                    console.log(self.domains());
                }
            })
        },
        template: template
    })
})()