$(function() {

    var CUR_MENU = null,
        IS_MODAL_SHOWEN = false;

    function LangModel(lang) {
        var self = this;

        this.showError = ko.observable(false);

        this.key = lang.key;

        if (lang.verifyField) {
            this.value = ko.validateField(lang.value, {
                required: ""
            })
        } else {
            this.value = ko.observable(lang.value);
        }

        this.verifyField = lang.verifyField;
    }

    var menuViewModel = function(params) {

        var self = this;

        this.loading = ko.observable(true);

        this.defaultLang = ko.observable();
        this.isMultiLangeMenu = ko.observable(false);
        this.multilingualModal = ko.observable(false);
        this.multiNames = ko.observableArray();
        this.onShowMultiModal = function(menu) {
            CUR_MENU = menu;
            Kooboo.Menu.getLang({
                id: menu.id(),
                rootId: menu.rootId()
            }).then(function(res) {
                if (res.success) {
                    var keys = Object.keys(res.model),
                        defaultCultureIdx = keys.indexOf(self.defaultLang());

                    if (defaultCultureIdx > -1) {
                        self.multiNames.push(new LangModel({
                            verifyField: true,
                            key: keys[defaultCultureIdx],
                            value: res.model[keys[defaultCultureIdx]]
                        }))
                        keys.splice(defaultCultureIdx, 1);
                    }

                    keys.forEach(function(key) {
                        self.multiNames.push(new LangModel({
                            verifyField: false,
                            key: key,
                            value: res.model[key]
                        }))
                    })

                    self.multilingualModal(true);
                }
            })

        }
        this.onHideMultilinModal = function() {
            var find = _.find(self.multiNames(), function(name) {
                return name.key == self.defaultLang();
            })
            find.showError(false);

            self.multilingualModal(false);
            self.multiNames([]);
        }
        this.isMultiNameValid = function() {
            var find = _.find(self.multiNames(), function(mn) {
                return mn.key == self.defaultLang();
            })

            return find.value.isValid();
        }
        this.onSaveMultiName = function() {
            if (self.isMultiNameValid()) {
                var menus = _.cloneDeep(self.menuItems()),
                    data = {};
                debugger
                _.forEach(self.multiNames(), function(mn) {
                    data[mn.key] = mn.value();
                })
                Kooboo.Menu.updateLang({
                    id: CUR_MENU.id(),
                    rootId: CUR_MENU.rootId(),
                    values: data
                }).then(function(res) {

                    if (res.success) {
                        var idx = _.findIndex(menus, function(m) {
                            return m.id() == CUR_MENU.id()
                        })
                        var m = menus.splice(idx, 1);
                        m[0].name(res.model);
                        menus.splice(idx, 0, m[0]);
                        self.menuItems(menus);
                        window.info.show(Kooboo.text.info.save.success, true);
                        self.onHideMultilinModal();
                    } else {
                        window.info.show(Kooboo.text.info.save.fail, false);
                    }
                    CUR_MENU = null;
                })
            } else {
                self.showMultiNameError();
            }
        }
        this.showMultiNameError = function() {
            var find = _.find(self.multiNames(), function(mn) {
                return mn.key == self.defaultLang();
            })

            return find.showError(true);
        }

        this.afterMenusRendered = function() {

            $('[data-inline-edit="true"]').editable({
                mode: "inline",
                type: "text",
                validate: function(value) {
                    if ($.trim(value) == '') {
                        return Kooboo.text.validation.required;
                    } else if (value.match(/<[^>]+>/g) && value.match(/<[^>]+>/g).length) {
                        return Kooboo.text.validation.tagNotAllow;
                    }
                },
                send: 'always',
                success: function(res, newName) {
                    var id = $(this).attr("data-id");
                    var _find = _.findLast(self.menuItems(), function(menu) {
                        return menu.id() == id;
                    })
                    _find.name(newName);
                    self.updateRelatedMenu(_find);
                    DataCache.removeRelatedData("menu");
                }
            });

            $(".tree").treegrid({
                expanderExpandedClass: 'fa fa-caret-down',
                expanderCollapsedClass: 'fa fa-caret-right'
            });

        };

        this.name = ko.observable(params.name);

        this.pageList = ko.observableArray();

        var _menus = [];
        _.forEach(params.menuItems, function(menu) {
            _menus.push(new Menu(menu));
        });
        this.menuItems = ko.observableArray(_menus);

        this._menuItems = ko.observableArray();

        this.isNotRoot = function(m) {
            return m.id() !== m.rootId();
        };

        this.className = function(m) {
            var className = [];
            className.push("treegrid-" + m.id());

            if (m.parentId() !== Kooboo.Guid.Empty) {
                className.push("treegrid-parent-" + m.parentId());
            }

            return className.join(" ");
        }

        this.onEditLink = function(m) {
            _.forEach(self.menuItems(), function(menu) {
                menu.addMenu(false);
                menu.urlEditing(false);
            });

            m.urlEditing(true);
            var originalUrl = _.cloneDeep(m.url());
            m._url(originalUrl);
        };

        this.cancelEditLink = function(m) {
            m.url(m._url());
            m.urlEditing(false);
        }

        this.updateLinkUrl = function(m) {

            if (!m.url()) {
                m.url("#");
            }

            if (m.url() !== m._url()) {

                Kooboo.Menu.UpdateUrl({
                    Id: m.id(),
                    RootId: m.rootId(),
                    url: m.url()
                }).then(function(res) {

                    if (res.success) {
                        m.urlEditing(false);
                        window.info.show(Kooboo.text.info.update.success, true);

                        self.updateRelatedMenu(m);
                    } else {
                        m.url(m._url());
                        window.info.show(Kooboo.text.info.update.fail, false);
                    }
                })
            } else {
                self.cancelEditLink(m);
            }
        }

        this.menuWithNoChild = ko.observable(false);
        this.showCurrentTemplate = ko.observable(false);

        this.onShowTemplate = function(m) {
            CUR_MENU = m;
            self.menuWithNoChild(m.children().length == 0);
            self.showCurrentTemplate(m.id() !== m.rootId());
            self.templateModal(true);
        };

        $("#template_modal").on('shown.bs.modal', function(e) {
            var _find = _.findLast(self.menuItems(), function(menu) {
                return menu.id() == CUR_MENU.id();
            });

            self.subItemContainer(_find.subItemContainer() || "");
            self.subItemTemplate(_find.subItemTemplate() || "");
            self.currentTemplate(_find.template() || "");

            if (!self.currentTemplate()) {
                var parent = _.find(self.menuItems(), function(menu) {
                    return menu.id() == _find.parentId();
                })
                parent && self.currentTemplate(parent.subItemTemplate());
            }

            $(".CodeMirror")[1].CodeMirror.setSize("100%", self.showCurrentTemplate() ? "80px" : "230px");
            $(".CodeMirror")[2].CodeMirror.setSize("100%", self.showCurrentTemplate() ? "80px" : "105px");

            _.forEach($(".CodeMirror"), function($cm) {
                // CodeMirror's bug
                $cm.CodeMirror.refresh();
            })

            IS_MODAL_SHOWEN = true;
            self.previewCode();
        });

        $("#template_modal").on('hide.bs.modal', function(e) {
            self.subItemContainer("");
            self.subItemTemplate("");
            self.currentTemplate("");
        });

        this.onAddSubMenu = function(m) {

            _.forEach(self.menuItems(), function(menu) {
                menu.addMenu(false);
                menu.urlEditing(false);
            });

            m.addMenu(true);
        };

        this.onAddDataSourceMneu = function(m) {
            debugger
        }

        this.onRemoveSubMenu = function(m) {

            if (confirm(Kooboo.text.confirm.deleteItem)) {

                Kooboo.Menu.Delete({
                    rootId: m.rootId(),
                    Id: m.id()
                }).then(function(res) {

                    if (res.success) {

                        var newOrder = _.cloneDeep(self.menuItems());

                        var idx = _.findIndex(newOrder, function(me) {
                            return me.id() == m.id();
                        });

                        newOrder.splice(idx, self.getChildrenCount(m) + 1);

                        var parent = _.findLast(newOrder, function(me) {
                                return me.id() == m.parentId();
                            }),
                            idxInParent = _.findIndex(parent.children(), function(child) {
                                return child.id() == m.id();
                            });

                        parent.children().splice(idxInParent, 1);
                        self.updateRelatedMenu(parent);
                        self.menuItems(newOrder);

                        window.info.show(Kooboo.text.info.delete.success, true);

                    } else {
                        window.info.show(Kooboo.text.info.delete.fail, false);
                    }
                })
            }
        };

        this.subMenuValid = function(m) {
            return m.newSubName.isValid();
        }

        this.onSaveSubMenu = function(m) {

            if (self.subMenuValid(m)) {

                m.subNameRequired(false);

                Kooboo.Menu.CreateSub({
                    rootId: m.rootId(),
                    parentId: m.id(),
                    name: m.newSubName(),
                    url: m.newSubUrl() || "#"
                }).then(function(res) {

                    if (res.success) {

                        var newSubMenuModel = new Menu(res.model),
                            _idx = _.findIndex(self.menuItems(), function(menu) {
                                return menu.id() == m.id();
                            });

                        self.menuItems.splice((_idx + 1), 0, newSubMenuModel);
                        m.children.splice(0, 0, newSubMenuModel);

                        self.updateRelatedMenu(m);

                        m.addMenu(false);
                        m.newSubName("");
                        m.newSubUrl("");

                        window.info.show(Kooboo.text.info.save.success, true);
                    } else {
                        window.info.show(Kooboo.text.info.save.fail, false);
                    }
                })
            } else {
                window.info.show(Kooboo.text.info.menuNameRequired, false);
                m.subNameRequired(true);
            }
        };

        this.menuMoveUp = function(m, e) {
            var newOrder = _.cloneDeep(self.menuItems()),
                siblings = _.filter(newOrder, function(me) {
                    return me.parentId() == m.parentId();
                });

            self._menuItems(_.cloneDeep(newOrder));

            // 获取当前的位置
            var idx = _.findIndex(newOrder, function(me) {
                    return me.id() == m.id();
                }),
                idxInSiblings = _.findIndex(siblings, function(me) {
                    return me.id() == m.id();
                });

            // 获取当前菜单
            var current = newOrder.splice(idx, 1 + self.getChildrenCount(m));

            if (idxInSiblings > 0) {
                var siblingIdx = _.findIndex(newOrder, function(me) {
                    return me.id() == siblings[idxInSiblings - 1].id();
                });
                newOrder.splice(siblingIdx, 0, current);
                newOrder = _.flatten(newOrder);

                self.menuItems(newOrder);

                siblings.splice(idxInSiblings, 1);
                siblings.splice(idxInSiblings - 1, 0, m);
                siblings = _.flatten(siblings);

                var parent = _.find(newOrder, function(me) {
                    return me.id() == m.parentId();
                });

                parent.children(siblings);

                self.updateRelatedMenu(parent);

                self.swapOrder(m.rootId(), m.id(), siblings[idxInSiblings].id());
            } else {
                e.preventDefault();
            }
        };

        this.menuMoveDown = function(m, e) {
            var newOrder = _.cloneDeep(self.menuItems()),
                siblings = _.filter(newOrder, function(me) {
                    return me.parentId() == m.parentId();
                });

            self._menuItems(_.cloneDeep(newOrder));

            // 获取 当前的位置
            var idx = _.findIndex(newOrder, function(me) {
                    return me.id() == m.id();
                }),
                idxInSiblings = _.findIndex(siblings, function(me) {
                    return me.id() == m.id();
                });

            // 获取当前菜单
            var current = newOrder.splice(idx, 1 + self.getChildrenCount(m));
            if (idxInSiblings < siblings.length - 1) {
                var siblingIdx = _.findIndex(newOrder, function(me) {
                    return me.id() == siblings[idxInSiblings + 1].id();
                });
                newOrder.splice(siblingIdx + self.getChildrenCount(siblings[idxInSiblings + 1]) + 1, 0, current);
                newOrder = _.flatten(newOrder);

                self.menuItems(newOrder);

                siblings.splice(idxInSiblings, 1);
                siblings.splice(idxInSiblings + 1, 0, m);
                siblings = _.flatten(siblings);

                var parent = _.find(newOrder, function(me) {
                    return me.id() == m.parentId();
                });

                parent.children(siblings);

                self.updateRelatedMenu(parent);

                self.swapOrder(m.rootId(), m.id(), siblings[idxInSiblings].id());
            } else {
                e.preventDefault();
            }
        };

        this.swapOrder = function(rootId, ida, idb) {

            Kooboo.Menu.Swap({
                rootId: rootId,
                ida: ida,
                idb: idb
            }).then(function(res) {

                if (res.success) {

                } else {
                    self.menuItems(self._menuItems());
                }
            });
        }

        this.onCancelSubMenu = function(m) {
            m.newSubName("");
            m.newSubUrl("");
            m.addMenu(false);
            m.subNameRequired(false);
        }

        this.updateRelatedMenu = function(child) {
            var parent = _.find(self.menuItems(), function(me) {
                return me.id() == child.parentId();
            });

            if (parent) {
                var siblings = _.filter(self.menuItems(), function(me) {
                        return me.parentId() == child.parentId();
                    }),
                    idxInSiblings = _.findIndex(siblings, function(sb) {
                        return sb.id() == child.id();
                    });

                siblings.splice(idxInSiblings, 1);
                siblings.splice(idxInSiblings, 0, child);

                parent.children(siblings);

                self.updateRelatedMenu(parent);
            } else {
                return;
            }
        };

        this.getChildrenCount = function(menu) {
            // 获取所有子菜单的数量
            var count = 0,
                children = menu.children();
            if (children && children.length) {
                _.forEach(children, function(child) {
                    count++;
                    count += self.getChildrenCount(child);
                })
                return count;
            } else {
                return 0;
            }
        }

        // Template Modal

        this.templateModal = ko.observable(false);

        this.onHideTemplateModal = function() {
            self.templateModal(false);
            CUR_MENU = null;
        }

        this.onSaveTemplate = function() {

            Kooboo.Menu.UpdateTemplate({
                Id: CUR_MENU.id(),
                RootId: CUR_MENU.rootId(),
                SubItemContainer: self.subItemContainer(),
                SubItemTemplate: self.subItemTemplate(),
                Template: self.currentTemplate()
            }).then(function(res) {

                if (res.success) {
                    var _find = _.findLast(self.menuItems(), function(me) {
                        return me.id() == CUR_MENU.id();
                    });

                    _find.subItemContainer(self.subItemContainer());
                    _find.subItemTemplate(self.subItemTemplate());
                    _find.template(self.currentTemplate());

                    self.updateRelatedMenu(_find);
                    self.onHideTemplateModal();
                }
            });
        }

        var markAnchorText = "{anchortext}";
        var markHref = "{href}";
        var markSubItems = "{items}";
        var markActiveClass = "{activeclass:className}";
        var markParentId = "{parentid}";
        var markCurrentId = "{currentid}";

        this.subItemContainer = ko.observable("");
        this.subItemContainer.subscribe(function() {
            if (IS_MODAL_SHOWEN) {
                self.previewCode();
            }
        });

        this.subItemTemplate = ko.observable("");
        this.subItemTemplate.subscribe(function() {
            if (IS_MODAL_SHOWEN) {
                self.previewCode();
            }
        });

        this.currentTemplate = ko.observable("");
        this.currentTemplate.subscribe(function() {
            if (IS_MODAL_SHOWEN) {
                self.previewCode();
            }
        });

        this.templatePreview = ko.observable("");

        this.previewCode = _.debounce(function() {
            var current = markSubItems;

            if (CUR_MENU) {
                current = html_beautify(self.renderPreview(current, self.menuItems()[0]));
                self.templatePreview(current);
            }
        }, 200);

        this.renderPreview = function(tmpl, context) {
            var self = this;
            if (tmpl.indexOf(markHref) > -1) {
                tmpl = tmpl.split(markHref).join(context.url() ? context.url() : "#");
            }
            if (tmpl.indexOf(markAnchorText) > -1) {
                tmpl = tmpl.split(markAnchorText).join(context.name());
            }
            if (tmpl.indexOf(markCurrentId) > -1) {
                tmpl = tmpl.split(markCurrentId).join(context.id());
            }
            if (tmpl.indexOf(markParentId) > -1) {
                tmpl = tmpl.split(markParentId).join(context.parentId());
            }
            if (tmpl.indexOf(markSubItems) > -1) {
                if (context.children().length > 0) {
                    var _tmplArr = tmpl.split(markSubItems),
                        parentsTemplate = "";

                    tmpl = _tmplArr.join((CUR_MENU.id() == context.id()) ? self.subItemContainer() : context.subItemContainer());
                    parentsTemplate = (CUR_MENU.id() == context.id()) ? self.subItemTemplate() : context.subItemTemplate();

                    var childTemplates = [];
                    _.forEach(context.children(), function(child) {
                        var template = ((CUR_MENU.id() == child.id()) ?
                                self.currentTemplate() :
                                (child.template() == context.subItemTemplate() ? parentsTemplate : child.template())) ||
                            parentsTemplate;
                        childTemplates.push(self.renderPreview(template, child));
                    });

                    tmpl = tmpl.split(markSubItems).join(childTemplates.join(""));
                } else {
                    tmpl = tmpl.split(markSubItems).join("");
                }
            }
            if (tmpl.indexOf("activeclass:") > -1) {
                try {
                    function setActiveClass(elem) {
                        var selfTemplate = $(elem)[0].outerHTML;
                        var hasActiveClass = selfTemplate.match(/{.*?}/);
                        if (hasActiveClass) {
                            var className = hasActiveClass[0].match(/{[\w\W]*:([\w\W]*)}/)[1];
                            if ($(elem)[0].hasAttribute("{activeclass:" + className + "}")) {
                                $(elem)[0].removeAttribute("{activeclass:" + className + "}");
                                $(elem).addClass(className);
                            }

                            $(elem).children().each(function(idx, child) {
                                setActiveClass($(child));
                            })
                        }
                        return elem;
                    }
                    var elem = setActiveClass($(tmpl));
                    tmpl = elem[0].outerHTML;

                } catch (ex) {
                    tmpl = "error";
                }
            }
            return tmpl;
        }

        this.codeHelperSubItem = ko.observableArray([{
            text: "MarkSubItems",
            value: markSubItems,
            for: "SUB_ITEM_CONTAINER"
        }]);

        this.subTmplCodeHelpers = ko.observableArray([{
            text: "MarkAnchorText",
            value: markAnchorText,
            for: "SUB_ITEM_TEMPLATE"
        }, {
            text: "MarkHref",
            value: markHref,
            for: "SUB_ITEM_TEMPLATE"
        }, {
            text: "MarkSubItems",
            value: markSubItems,
            for: "SUB_ITEM_TEMPLATE"
        }, {
            text: "MarkActiveClass",
            value: markActiveClass,
            for: "SUB_ITEM_TEMPLATE"
        }, {
            text: "MarkParentId",
            value: markParentId,
            for: "SUB_ITEM_TEMPLATE"
        }, {
            text: "MarkCurrentId",
            value: markCurrentId,
            for: "SUB_ITEM_TEMPLATE"
        }]);

        this.tmplCodeHelpers = ko.observableArray([{
            text: "MarkAnchorText",
            value: markAnchorText,
            for: "TEMPLATE"
        }, {
            text: "MarkHref",
            value: markHref,
            for: "TEMPLATE"
        }, {
            text: "MarkSubItems",
            value: markSubItems,
            for: "TEMPLATE"
        }, {
            text: "MarkActiveClass",
            value: markActiveClass,
            for: "TEMPLATE"
        }, {
            text: "MarkParentId",
            value: markParentId,
            for: "TEMPLATE"
        }, {
            text: "MarkCurrentId",
            value: markCurrentId,
            for: "TEMPLATE"
        }]);

        this.codeHelp = function(m, e) {
            var editor = null;

            switch (m.for) {
                case "SUB_ITEM_CONTAINER":
                    editor = $(".CodeMirror")[1].CodeMirror;
                    break;
                case "SUB_ITEM_TEMPLATE":
                    editor = $(".CodeMirror")[2].CodeMirror
                    break;
                case "TEMPLATE":
                    editor = $(".CodeMirror")[3].CodeMirror
                    break;
            }

            if (!editor.getCursor().line && !editor.getCursor().ch) {
                editor.setValue(editor.getValue() + m.value);
            } else {
                editor.replaceSelection(m.value);
            }
        }
    }

    var Menu = function(menu) {
        var self = this;

        ko.mapping.fromJS(menu, {}, self);

        this._url = ko.observable();

        this.UpdateText = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Menu._getUrl("UpdateText"), {
                Id: self.id(),
                RootId: self.rootId()
            });
        });

        this.addMenu = ko.observable(false);

        this.urlEditing = ko.observable(false);

        this.newSubName = ko.validateField({
            required: Kooboo.text.validation.required
        });

        this.subNameRequired = ko.observable(false);

        this.newSubUrl = ko.observable();
    }


    $.when(Kooboo.Menu.getFlat({
            id: Kooboo.getQueryString("Id")
        }),
        Kooboo.Site.Langs(),
        Kooboo.Page.getAll()).then(function(menu, lang, page) {
        var r1 = $.isArray(menu) ? menu[0] : menu,
            r2 = $.isArray(lang) ? lang[0] : lang,
            r3 = $.isArray(page) ? page[0] : page;

        if (r1.success && r2.success && r3.success) {
            var vm = new menuViewModel({
                menuItems: r1.model,
                name: r1.model[0].name
            })

            vm.defaultLang(r2.model.default);
            vm.isMultiLangeMenu(Object.keys(r2.model.cultures).length > 1);

            r3.model.pages.forEach(function(page) {
                vm.pageList.push(page.path);
            });

            vm.loading(false);

            ko.applyBindings(vm, document.getElementById("main"));
        }
    })

});