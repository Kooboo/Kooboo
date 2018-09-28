$(function() {

    var shareModel = function() {
        var self = this;

        this.siteName = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^[A-Za-z][\w\-]*$/,
                message: Kooboo.text.validation.siteNameInvalid
            },
        });

        this.makePrivate = ko.observable(false);

        this.showError = ko.observable(false);

        this.tags = ko.observableArray([Kooboo.text.site.share.corporation, Kooboo.text.site.share.article, Kooboo.text.site.share.category]);
        this.selectedTags = ko.observableArray();

        this.description = ko.observable();

        this.link = ko.observable();

        this.files = ko.observableArray();
        this.files.subscribe(function(files) {
            var origSelected = _.findIndex(self.fileList(), function(f) {
                return f.selected();
            });

            self.fileList.removeAll();

            _.forEach(files, function(file) {
                self.fileList.push(new fileModel(file));
            })

            if (self.fileList().length) {
                self.fileList()[origSelected == -1 ? 0 : origSelected].selected(true);
            }
        })

        this.chooseAsDefault = function(m, e) {
            if (m.selected()) {
                e.preventDefault();
            } else {
                self.fileList().forEach(function(f) {
                    f.selected(false);
                })
                m.selected(true);
            }
        }

        this.fileList = ko.observableArray();

        this.removeFile = function(file) {

            self.fileList.remove(file);

            if (file.selected() && self.fileList().length) {
                self.fileList()[0].selected(true);
            }

            var _find = _.find(self.files(), function(f) {
                return f == file.target;
            })
            _find && self.files.remove(_find);
        }

        this.isValid = function() {
            return self.siteName.isValid();
        }

        this.onShare = function() {

            if (self.isValid()) {
                var defaultImg = _.findIndex(self.fileList(), function(f) {
                    return f.selected();
                });

                var data = new FormData();

                data.append("siteName", self.siteName());
                data.append("isPrivate", /*self.makePrivate()*/ false);
                data.append("tags", self.selectedTags());
                data.append("description", self.description() ? self.description() : "");
                data.append("link", self.link() ? self.link() : "");
                _.forEach(self.fileList(), function(file, idx) {
                    data.append("thumbnail_" + idx, file.target);
                });
                data.append("defaultImg", defaultImg);

                Kooboo.Template.Share(data).then(function(res) {

                    if (res.success) {
                        location.href = Kooboo.Route.Get(Kooboo.Route.Site.TemplatePage, {
                            type: "personal"
                        })
                    }
                })
            } else {
                self.showError(true);
            }
        }

        this.SPAClick = function(m, e) {
            e.preventDefault();
            self.showError(false);
            Kooboo.SPA.getView(Kooboo.Route.Site.ListPage, {
                container: '[layout="default"]'
            })
        }
    }

    var vm = new shareModel();

    var fileModel = function(file) {
        var self = this;
        this.name = file.name;
        this.value = ko.observable();
        this.selected = ko.observable(false);
        this.target = file;

        var reader = new FileReader();
        reader.onloadend = function() {
            self.value(reader.result);
        }

        reader.readAsDataURL(file);
    }

    ko.applyBindings(vm, document.getElementById("main"));
    $(".autosize").textareaAutoSize().trigger("keyup");

    Kooboo.Site.getName().then(function(res) {
        res.success && vm.siteName(res.model);
    })

    $("input:file").change(function() {
        var files = this.files,
            filesLength = files.length,
            uploadLength = 0;

        if (filesLength) {
            _.forEach(files, function(file) {
                let suffix = file.name.split('.').reverse()[0].toLowerCase();
                if (['bmp', 'png', 'jpg', 'jpeg'].indexOf(suffix) > -1) {
                    vm.files.push(file);
                    uploadLength++;
                }
            })
        }

        if (uploadLength !== filesLength) {
            alert(Kooboo.text.alert.imageFileUploaded);
        }

        $(this).val("");
    })
})