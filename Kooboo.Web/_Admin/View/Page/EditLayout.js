$(function() {
    window.__pageEditor = {};
    var cm,
        BindingStore = Kooboo.pageEditor.store.BindingStore,
        bindingPanel = new Kooboo.pageEditor.viewModel.BindingPanel(),
        Style = Kooboo.pageEditor.viewModel.Style,
        Script = Kooboo.pageEditor.viewModel.Script,
        KBFrame = Kooboo.pageEditor.component.KBFrame,
        ComponentTool = Kooboo.pageEditor.util.ComponentTool;

    var kbFrame = new KBFrame(document.getElementById("page_iframe"), {
        type: "layout_page"
    })
    window.__pageEditor.kbFrame = kbFrame;

    var savedStyles, savedScripts;

    Kooboo.EventBus.subscribe("binding/remove", function(data) {
        var item = BindingStore.byId(data.id);

        item && BindingStore.remove(data.id);
    });

    var pageViewModel = function() {

        var self = this;

        this.multiLangs = ko.observable();

        this.isNewPage = ko.observable(true);

        this.cachedPositionsInfo = null;

        this.pageId = ko.observable();
        this.pageId.subscribe(function(id) {
            self.isNewPage(id == Kooboo.Guid.Empty);
        });

        this.layoutName = ko.observable();

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

        this.positions = ko.observableArray();

        this.pageContent = ko.observable("");
        this.pageContent.subscribe(function(html) {
            if (html) {
                var layout = $.parseHTML(html);

                var positions = [];
                positions = self.getPositions(layout, positions, false);

                self.positions(positions);
            }
        })
        this._pageContent = ko.observable("");

        this.getPositions = function(dom, positions, fromNestedLayout, layoutName) {

            $(dom).children("placeholder").each(function(idx, position) {
                var name = $(position).attr("id"),
                    contents = [];

                $(Kooboo.pageEditor.util.ComponentTool.getComponentTags().join(','), position).each(function(idx, content) {
                    if ($(content).parents("placeholder").length == 1 || fromNestedLayout) {
                        contents.push({
                            type: content.tagName.toLowerCase(),
                            name: $(content).attr("id"),
                            engine: content.hasAttribute("engine") ? $(content).attr("engine") : false,
                            id: Math.ceil(Math.random() * Math.pow(2, 53))
                        })
                    }

                    if (content.tagName.toLowerCase() == "layout") {
                        positions = self.getPositions(content, positions, true, $(content).attr("id"));
                    }
                })

                var pos = {
                    name: name,
                    contents: contents,
                    fromLayout: fromNestedLayout
                }

                layoutName && (pos.layoutName = layoutName);

                positions.push(pos);
            })

            return positions;
        }

        this.formatCode = function() {
            var formatted = html_beautify(self.pageContent());
            self.pageContent(formatted);
        }

        this.setHTML = function(html, callback) {
            !kbFrame.hasResource() && kbFrame.setResource(self.bindingPanel().resources());
            kbFrame.setExistResource({
                scripts: savedScripts,
                styles: savedStyles
            })
            kbFrame.setContent(html, function() {
                self.bindingPanel().elem(kbFrame.getDocumentElement());
                if (!self.cachedPositionsInfo) {
                    self.cachedPositionsInfo = {
                        fdoc: kbFrame.getDocumentElement(),
                        container: $(".kb-editor")[0],
                        positions: self.positions(),
                        styles: savedStyles,
                        scripts: savedScripts
                    }
                }

                setTimeout(function() {
                    Kooboo.EventBus.publish("fdoc/load", self.cachedPositionsInfo);
                    $(window).trigger('resize');
                }, 500);
                if (callback) callback();
            });

        }

        this.getLayoutHTML = function() {
            var vDom = $("<layout>");
            $(vDom).attr("id", self.layoutName());

            vDom = self.getLayout(kbFrame.getDocumentElement(), vDom);

            self._pageContent(vDom[0].outerHTML);
            return html_beautify(self._pageContent());
        }

        this.getLayout = function(node, dom) {

            dom = self.getSaveLayout(node, dom);

            handleSavedLayout(dom[0]);

            function handleSavedLayout(dom) {
                if ($(dom)[0].tagName.toLowerCase() == "placeholder") {
                    if (dom.hasAttribute("k-omit")) {
                        if ($(dom).children().length) {
                            var children = $(dom).children().get().reverse();
                            children.forEach(function(child) {
                                $(child).insertAfter(dom);
                            })
                            $(dom).remove();
                            children.forEach(function(child) {
                                handleSavedLayout(child);
                            })
                        } else {
                            dom.removeAttribute("k-omit");
                        }
                    } else {
                        $(dom).children().each(function(idx, component) {
                            handleSavedLayout(component);
                        })
                    }
                } else {
                    $(dom).children().each(function(idx, component) {
                        handleSavedLayout(component);
                    })
                }
            }

            return dom;
        }

        this.getSaveLayoutHTML = function() {
            var vDom = $("<layout>");
            $(vDom).attr("id", self.layoutName());

            vDom = self.getSaveLayout(kbFrame.getDocumentElement(), vDom);

            return html_beautify(vDom[0].outerHTML);
        }

        this.getSaveLayout = function(node, dom) {
            $(node).children().each(function(idx, component) {
                if (component.getAttribute('k-placeholder')) {
                    var el = $("<placeholder>");
                    $(el).attr("id", component.getAttribute("k-placeholder"));

                    if (component.hasAttribute("k-omit")) {
                        $(el).attr("k-omit", "");
                    }

                    self.getSaveLayout(component, el);
                    $(dom).append(el);
                } else if (ComponentTool.isNormalComponent(component)) {
                    var el = $('<' + component.tagName.toLowerCase() + '>'),
                        data = $(component).data('kb-comp');
                    $(el).attr('id', data ? data.name : component.getAttribute('id'));
                    if (component.tagName.toLowerCase() == 'layout') {
                        self.getSaveLayout(component, el);
                    }
                    $(dom).append(el);
                } else if (ComponentTool.isEnginedComponent(component)) {
                    var data = $(component).data('kb-comp'),
                        el = $('<' + data.type.toLowerCase() + '>');
                    $(el).attr('engine', data.engine)
                        .attr('id', data.name);
                    if (data.type.toLowerCase() == 'layout') {
                        self.getSaveLayout(component, el);
                    }
                    $(dom).append(el);
                } else {
                    self.getSaveLayout(component, dom);
                }
            })

            return dom;
        }

        this.setHTMLByLayout = function(layout) {
            var layoutDOM = $.parseHTML(layout),
                newDOM = $("<body>");
            $(layout).children().each(function(idx, node) {

            })
        }

        this.getNodeDOMByLayout = function(node) {

        }

        this.bindingPanel = ko.observable(bindingPanel);

        $(window).on('resize', _.debounce(function() {
            Kooboo.EventBus.publish("kb/page/layout/component/remask");
        }, 50))

        this.showError = ko.observable(false);

        this.curType = ko.observable("preview");

        this.changeType = function(type) {

            if (self.curType() !== type) {

                if (type == "code") {
                    self.curType(type);
                    self.pageContent(self.getLayoutHTML());
                    cm.refresh();
                } else {
                    if (self.isPageContentEqual()) {
                        self.curType(type);
                    } else {
                        self.isBodyChanged(true);
                        Kooboo.EventBus.publish("fdoc/load", {
                            fdoc: kbFrame.getDocumentElement(),
                            container: $(".kb-editor")[0],
                            positions: self.positions()
                        });
                        self.curType(type);
                    }
                }
            }
        }

        this.isPageContentEqual = function() {

            return plainify(self.pageContent()) == plainify(self._pageContent());

            function plainify(p) {
                var lineBreaker = (p.indexOf('\r\n') > -1) ? '\r\n' : '\n';
                return p.split(lineBreaker).map(function(line) {
                    return line.trim();
                }).join('');
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
            Kooboo.EventBus.publish("kb/page/save", {});
        }

        this.onSave = function() {
            self.isSaveAndReturn(false);
            Kooboo.EventBus.publish("kb/page/save", {});
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

        Kooboo.EventBus.subscribe("kb/pageeditor/component/position/ready", function() {
            Kooboo.EventBus.publish("fdoc/load", self.cachedPositionsInfo);
        })

        Kooboo.EventBus.subscribe("kb/page/final/save", function(res) {
            if (!res.errorCount || res.errorCount == 0) {
                var styleList = [],
                    scriptList = [];
                self.bindingPanel().styleList().forEach(function(style) {
                    style.url() && styleList.push(style.url());
                })
                self.bindingPanel().bodyScriptList().forEach(function(script) {
                    script.url() && scriptList.push(script.url());
                });
                res["styles"] = styleList;
                res["scripts"] = scriptList;

                res["body"] = self.curType() == "preview" ? self.getSaveLayoutHTML() : self.pageContent();
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
                            location.href = Kooboo.Route.Get(Kooboo.Route.Page.EditLayout, {
                                id: id,
                                layoutId: layoutId
                            })
                        } else {
                            self.isTitleChanged(false);
                            self.isMetaChanged(false);
                            self.isUrlChanged(false);
                            self.isBodyChanged(false);
                            self._pageContent(self.pageContent());
                            window.info.show(Kooboo.text.info.save.success, true);
                        }
                    });
                }
            }
        })

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
    }

    var vm = new pageViewModel();

    var pageId = Kooboo.getQueryString("Id") || Kooboo.Guid.Empty,
        layoutId = Kooboo.getQueryString("layoutId");

    $.when(Kooboo.Page.getEdit({
            Id: pageId,
            layoutId: layoutId
        }), Kooboo.Layout.get({
            Id: layoutId
        }),
        Kooboo.Site.Langs(),
        Kooboo.Component.getList(),
        Kooboo.Style.getExternalList(),
        Kooboo.Script.getExternalList(),
        Kooboo.ResourceGroup.Style(),
        Kooboo.ResourceGroup.Script()
    ).then(function(page, layout, langs, component, styles, scripts, styleGroup, scriptGroup) {
        var styleList = [],
            styleGroupList = [],
            scriptList = [],
            scriptGroupList = [];

        page = $.isArray(page) ? page[0] : page;
        layout = $.isArray(layout) ? layout[0] : layout;
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

        Kooboo.pageEditor.store.ComponentStore.setTypes(component[0].model);
        vm.pageId(pageId);
        vm.pageContent(page.model.body || "");
        vm.name(page.model.name);
        vm.settings(page.model);
        vm.multiLangs(langs[0].model);
        vm.layoutName(layout.model.name);
        savedStyles = page.model.styles;
        savedScripts = page.model.scripts;
        vm.setHTML(layout.model.body, function() {
            ko.applyBindings(vm, document.getElementById("main"));
            cm = $(".CodeMirror")[0].CodeMirror;
        });

    })

    $(window).on("resize", function() {
        Kooboo.EventBus.publish("kb/page/layout/component/remask");
    })
})