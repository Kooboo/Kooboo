$(function() {

    window.__pageEditor = {};

    var Guid = Kooboo.Guid,
        BindingStore = Kooboo.pageEditor.store.BindingStore,
        BindingPanel = Kooboo.pageEditor.viewModel.BindingPanel,
        KBFrame = Kooboo.pageEditor.component.KBFrame;

    var cm, kbFrame = new KBFrame(document.getElementById('page_iframe'), {
            type: 'normal_page'
        }),
        bindingPanel = new BindingPanel();

    window.__pageEditor.kbFrame = kbFrame;

    Kooboo.EventBus.subscribe("binding/remove", function(data) {
        var item = BindingStore.byId(data.id);

        if (item) {
            BindingStore.remove(data.id);
        }
    });

    var pageViewModel = function() {

        var self = this;

        this.multiLangs = ko.observable();

        this.isNewPage = ko.observable();

        this.pageId = ko.observable();
        this.pageId.subscribe(function(id) {
            self.isNewPage(id == Guid.Empty);
        });

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
            Kooboo.EventBus.publish("kb/page/url/route/set", name);
        })

        this.pageContent = ko.observable("");
        this._pageContent = ko.observable("");

        this.formatCode = function() {
            var formatted = html_beautify(self.pageContent());
            self.pageContent(formatted);
        }

        this.setHTML = function(html, callback) {
            BindingStore.clear();
            !kbFrame.hasResource() && kbFrame.setResource(self.bindingPanel().resources());
            kbFrame.setContent(html, function() {
                self.bindingPanel().elem(kbFrame.getDocumentElement());
                if (callback) callback();
            });

        }

        this.getHTML = function() {
            return html_beautify(kbFrame.getHTML());
        }

        this.bindingPanel = ko.observable(bindingPanel);

        this.showError = ko.observable(false);

        this.curType = ko.observable("preview");

        this.changeType = function(type) {

            if (self.curType() !== type) {

                if (type == "code") {
                    self.curType(type);
                    self.pageContent(self.getHTML());
                    self._pageContent(self.pageContent());
                    cm.refresh();
                } else {

                    if (self._pageContent() !== self.pageContent()) {
                        self.isBodyChanged(true);
                        self.setHTML(self.pageContent(), function() {
                            Kooboo.EventBus.publish("kb/page/title/set", kbFrame.getTitle());
                        });

                    }
                    self.curType(type);
                }
            }
        }

        this.settings = ko.observable();

        this.isTitleChanged = ko.observable(false);
        this.isMetaChanged = ko.observable(false);
        this.isUrlChanged = ko.observable(false);
        this.isBodyChanged = ko.observable(false);

        this.isContentChanged = function() {
            return self.isTitleChanged() ||
                self.isMetaChanged() ||
                self.isUrlChanged() ||
                self.isBodyChanged() ||
                (self.curType() == "code" ? (self.pageContent() !== self._pageContent()) : false);
        }

        this.isValid = function() {

            if (!self.isNewPage()) {
                return true;
            } else {
                return self.name.isValid();
            }
        }

        this.isSaveAndReturn = ko.observable();

        this.onSaveAndReturn = function() {
            self.isSaveAndReturn(true);
            if (self.curType() == "code") {
                self.setHTML(self.pageContent(), function() {
                    Kooboo.EventBus.publish("kb/page/title/set", kbFrame.getTitle());
                    setTimeout(function() {
                        Kooboo.EventBus.publish("kb/page/save", {});
                    }, 250);
                });
            } else {
                Kooboo.EventBus.publish("kb/page/save", {});
            }
        }

        this.onSave = function() {
            self.isSaveAndReturn(false);
            if (self.curType() == "code") {
                self.setHTML(self.pageContent(), function() {
                    Kooboo.EventBus.publish("kb/page/title/set", kbFrame.getTitle());
                    setTimeout(function() {
                        Kooboo.EventBus.publish("kb/page/save", {});
                    }, 250);
                });
            } else {
                Kooboo.EventBus.publish("kb/page/save", {});
            }
        }

        this.submitData = ko.observable();

        this.onSubmit = function(cb) {

            function submit() {
                Kooboo.Page.post(
                    JSON.stringify(self.getSubmitData())
                ).then(function(res) {

                    if (res.success) {

                        if (typeof cb == "function") {
                            cb(res.model);
                        }
                    }
                })
            }

            if (self.isValid()) {
                submit();
            } else {
                self.showError(true);
            }

        }

        this.userCancel = function() {

            if (self.isContentChanged()) {

                if (confirm(Kooboo.text.confirm.beforeReturn)) {
                    self.goBack();
                }
            } else {
                self.goBack();
            }
        }

        this.goBack = function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.Page.ListPage);
        }

        this.getSubmitData = function() {
            return self.submitData();
        }

        Kooboo.EventBus.subscribe("kb/page/field/change", function(data) {
            switch (data.type) {
                case "title":
                    self.isTitleChanged(true);
                    break;
                case "meta":
                    self.isMetaChanged(true);
                    break;
                case "url":
                    self.isUrlChanged(true);
                    break;
                case "resource":
                    self.isBodyChanged(true);
                    break;
            }
        })

        Kooboo.EventBus.subscribe("kb/page/final/save", function(res) {
            if (!res.errorCount || res.errorCount == 0) {
                res["body"] = (self.curType() == "preview") ? self.getHTML() : self.pageContent();
                res["name"] = self.name();
                res["id"] = self.pageId();

                self.submitData(res);

                if (self.isSaveAndReturn()) {
                    self.onSubmit(function() {
                        self.goBack();
                    })
                } else {
                    self.onSubmit(function(id) {

                        if (self.isNewPage()) {
                            location.href = Kooboo.Route.Get(Kooboo.Route.Page.EditPage, {
                                id: id
                            })
                        } else {
                            self.isTitleChanged(false);
                            self.isMetaChanged(false);
                            self.isUrlChanged(false);
                            self.isBodyChanged(false);
                            self._pageContent(self.pageContent())
                            window.info.show(Kooboo.text.info.save.success, true);
                        }
                    });
                }
            }
        })
    }

    var vm = new pageViewModel();

    var pageId = Kooboo.getQueryString("Id") || Guid.Empty;

    $.when(Kooboo.Page.getEdit({
            Id: pageId
        }),
        Kooboo.Site.Langs(),
        Kooboo.Style.getExternalList(),
        Kooboo.Script.getExternalList(),
        Kooboo.ResourceGroup.Style(),
        Kooboo.ResourceGroup.Script()
    ).then(function(page, langs, styles, scripts, styleGroup, scriptGroup) {
        var styleList = [],
            styleGroupList = [],
            scriptList = [],
            scriptGroupList = [];

        page = $.isArray(page) ? page[0] : page;
        styles = $.isArray(styles) ? styles[0] : styles;
        scripts = $.isArray(scripts) ? scripts[0] : scripts;
        styleGroup = $.isArray(styleGroup) ? styleGroup[0] : styleGroup;
        scriptGroup = $.isArray(scriptGroup) ? scriptGroup[0] : scriptGroup;

        styles.model.forEach(function(style) {
            styleList.push({
                id: style.id,
                text: style.name,
                url: style.routeName
            })
        })

        scripts.model.forEach(function(script) {
            scriptList.push({
                id: script.id,
                text: script.name,
                url: script.routeName
            })
        })

        styleGroup.model.forEach(function(style) {
            styleGroupList.push({
                id: style.id,
                text: style.name,
                url: style.relativeUrl
            })
        })

        scriptGroup.model.forEach(function(script) {
            scriptGroupList.push({
                id: script.id,
                text: script.name,
                url: script.relativeUrl
            })
        })

        vm.bindingPanel().styleResource({
            styles: styleList,
            styleGroup: styleGroupList
        });

        vm.bindingPanel().scriptResource({
            scripts: scriptList,
            scriptGroup: scriptGroupList
        })

        vm.multiLangs(langs[0].model);
        vm.pageId(pageId);
        vm.pageContent(page.model.body || "");
        vm._pageContent(vm.pageContent());
        vm.setHTML(vm.pageContent(), function() {
            vm.name(page.model.name);
            vm.settings(page.model);
            ko.applyBindings(vm, document.getElementById("main"));
            cm = $(".CodeMirror")[0].CodeMirror;
        });

    });
})