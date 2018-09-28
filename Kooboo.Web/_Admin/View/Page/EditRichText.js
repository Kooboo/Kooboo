$(function() {
    var contentViewModel = function() {

        var self = this;

        Kooboo.EventBus.subscribe("ko/style/list/pickimage/show", function(ctx) {

            Kooboo.Media.getList().then(function(res) {

                if (res.success) {
                    res.model["show"] = true;
                    res.model["context"] = ctx;
                    res.model["onAdd"] = function(selected) {
                        ctx.settings.file_browser_callback(ctx.field_name, selected.url + "?SiteId=" + Kooboo.getQueryString("SiteId"), ctx.type, ctx.win, true);
                    }
                    self.mediaDialogData(res.model);
                }
            });
        });

        this.isNewPage = ko.observable(false);
        this.showError = ko.observable(false);
        this.name = ko.validateField({
            required: Kooboo.text.validation.required,
            stringlength: {
                min: 1,
                max: 64,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
            },
            remote: {
                url: Kooboo.Page.isUniqueName(),
                data: {
                    name: function() {
                        return self.name()
                    }
                },
                message: Kooboo.text.validation.taken
            }
        });
        this.name.subscribe(function(name) {
            if (!self.url()) {
                if (name.indexOf("/") !== 0) {
                    name = "/" + name;
                }
                self.url(name);
            }
        })
        this.title = ko.validateField({
            required: Kooboo.text.validation.required
        });
        this._title = ko.observable();
        this.url = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /^[^\s|\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\||\[|\]|\;|\:|\"|\'|\,|\<|\>|\?]*$/,
                message: Kooboo.text.validation.urlInvalid
            }
        });
        this._url = ko.observable();
        this.content = ko.observable();
        this._content = ko.observable();

        this.isContentChange = function() {
            var compareContent = "",
                _siteKey = '?SiteId=' + Kooboo.getQueryString('SiteId');
            if (self._content().indexOf(_siteKey) > -1) {
                compareContent = self._content().split(_siteKey).join('');
            } else {
                compareContent = self._content();
            }
            return self.title() !== self._title() || self.content() !== compareContent || self.url() !== self._url();
        };

        this.goBack = function() {
            if (!self.isContentChange()) {
                location.href = Kooboo.Route.Get(Kooboo.Route.Page.ListPage);
            } else {
                if (confirm(Kooboo.text.confirm.beforeReturn)) {
                    location.href = Kooboo.Route.Get(Kooboo.Route.Page.ListPage);
                }
            }
        }

        this.isValid = function() {
            if (self.isNewPage()) {
                return self.name.isValid() && self.title.isValid() && self.url.isValid();
            } else {
                return self.title.isValid() && self.url.isValid();
            }
        }

        this.onSaveAndReturn = function() {
            if (self.isValid()) {
                self.onSubmit(function() {
                    location.href = Kooboo.Route.Get(Kooboo.Route.Page.ListPage)
                })
            } else {
                self.showError(true);
            }
        }

        this.onSave = function() {
            if (self.isValid()) {
                self.onSubmit(function(id) {
                    if (self.isNewPage()) {
                        location.href = Kooboo.Route.Get(Kooboo.Route.Page.EditRichText, {
                            id: id
                        })
                    } else {
                        self._title(self.title());
                        self._url(self.url());
                        self._content(self.content());
                        window.info.show(Kooboo.text.info.save.success, true);
                    }
                })
            } else {
                self.showError(true);
            }
        }

        this.onSubmit = function(cb) {

            self.showError(false);

            Kooboo.Page.PostRichText({
                id: id || Kooboo.Guid.Empty,
                name: self.name(),
                title: self.title(),
                body: self.content(),
                url: self.url()
            }).then(function(res) {

                if (res.success) {

                    if (cb && typeof cb == "function") {
                        cb(res.model);
                    }
                }
            })
        }

        this.mediaDialogData = ko.observable();
    };

    var vm = new contentViewModel();

    var id = Kooboo.getQueryString("Id");
    vm.isNewPage(!id);

    var data = {}
    data[id ? "id" : "type"] = id ? id : "richtext";
    Kooboo.Page.getEdit(data).then(function(res) {
        if (res.success) {
            vm.name(res.model.name);
            vm.title(res.model.title);
            vm.content(res.model.body);
            vm.url(res.model.urlPath);

            vm._title(vm.title());
            vm._content(vm.content());
            vm._url(vm.url());
            ko.applyBindings(vm, document.getElementById("main"));
        }
    })

    Kooboo.EventBus.subscribe("kb/tinymce/initiated", function(editor) {
        vm._content(editor.getContent());
    })

})