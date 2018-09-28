$(function() {

    var importSiteViewModel = function() {
        var self = this;

        self.showError = ko.observable(false);

        self.siteName = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^[A-Za-z][\w\-]*$/,
                message: Kooboo.text.validation.siteNameInvalid
            },
            remote: {
                //TODO
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
                pattern: /^[A-Za-z][\w\-]*$/,
                message: Kooboo.text.validation.siteNameInvalid
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

        self.importing = ko.observable(false);
        self.importing.subscribe(function(now) {
            $('.import-form').readOnly(now);
        })
        self.uploadPercentage = ko.observable(0);

        self.domains = ko.observableArray();

        self.rootDomain = ko.observable("");

        self.isValid = function() {
            return self.siteName.isValid() && self.subDomain.isValid() && self.fileExist();
        }

        self.startSubmit = function() {

            if (self.isValid()) {

                self.showError(false);
                self.importing(true);

                var data = new FormData();

                data.append("SiteName", self.siteName());
                data.append("SubDomain", self.subDomain());
                data.append("RootDomain", self.rootDomain());
                data.append("Package", self.file());

                Kooboo.Site.Import(data, self.uploadPercentage).then(function(res) {
                    if (res.success) {
                        location.href = Kooboo.Route.Get(Kooboo.Route.Site.DetailPage, {
                            SiteId: res.model
                        });
                    }
                });

            } else {
                self.showError(true);
            }
        }

        self.fileExist = ko.observable(false);

        self.fileRequire = ko.validateField({
            required: Kooboo.text.validation.required
        })

        self.fileName = ko.observable();

        self.file = ko.observable();
        self.file.subscribe(function(file) {
            $("input:file").val("");

            if (file) {
                self.fileExist(true);
                self.fileRequire(true);
                self.fileName(file.name);

                if (!self.siteName()) {
                    self.siteName(file.name.split(".zip")[0]);
                }
            } else {
                self.fileExist(false);
                self.fileRequire(null);
            }
        })

        self.clearFile = function() {
            self.file(null);
            self.siteName("");
        }

        self.ableToDragFile = ko.observable(false);

        self.showDropArea = ko.observable(false);

        self.SPAClick = function(m, e) {
            e.preventDefault();
            self.showError(false);
            Kooboo.SPA.getView(Kooboo.Route.Site.ListPage, {
                container: '[layout="default"]'
            })
        }
    };

    var vm = new importSiteViewModel();

    $.when(Kooboo.Domain.getAvailable()).then(function(availRes) {
        if (availRes.success) {
            vm.domains(availRes.model);
            ko.applyBindings(vm, document.getElementById("main"));
        }
    })

    $("input:file").change(function() {
        var files = this.files;

        if (files.length > 0) {
            var name = files[0].name;

            if (name.indexOf(".") > -1 && name.split(".").reverse()[0] == "zip") {
                vm.file(files[0]);
            } else {
                alert(Kooboo.text.alert.uploadZipFile);
            }
        }
    })

    if (typeof Worker !== undefined) {

        vm.ableToDragFile(true);

        $(document).on({
            dragleave: _.debounce(function(e) {
                e.preventDefault();
                vm.showDropArea(false);
            }, 500),
            dragenter: function(e) {
                e.preventDefault();
                vm.showDropArea(true);
            },
            dragover: function(e) {
                e.preventDefault();
                vm.showDropArea(true);
            },
            drop: function(e) {
                e.preventDefault();
                vm.showDropArea(false);
            }
        })

        var box = document.getElementById("dropArea");
        box.addEventListener("drop", function(e) {
            e.preventDefault();

            var files = e.dataTransfer.files;

            if (files.length) {
                var name = files[0].name;

                if (name.indexOf(".") > -1 && name.split(".").reverse()[0] == "zip") {
                    vm.file(files[0]);
                } else {
                    alert(Kooboo.text.alert.uploadZipFile);
                }
            }
        })
    }
})