$(function() {
  var self;

  new Vue({
    el: "#menu-dialog",
    data: function() {
      return {
        name: "",
        menuItems: [],
        breads: [],
        mark: {
          markAnchorText: "{anchortext}",
          markHref: "{href}",
          markSubItems: "{items}",
          markActiveClass: "{activeclass:className}",
          markParentId: "{parentid}",
          markCurrentId: "{currentid}"
        },
        editorOptions: { lineNumbersMinChars: 2, minimap: { enabled: false } },
        defaultLang: undefined,
        isMultiLangMenu: true,
        showMultiLangModal: false,
        showTemplateModal: false,
        multiNames: undefined,
        loading: true,
        CUR_MENU: undefined,
        menuWithNoChild: false,
        showCurrentTemplate: false,
        currentTemplate: "",
        previewCode: undefined,
        subItemContainer: "",
        subItemTemplate: "",
        pageList: []
      };
    },
    created: function() {
      self = this;
      self.getData();
      self.initHelp();
    },
    watch: {
      name: function(val) {
        self.breads = [
          {
            name: Kooboo.text.component.breadCrumb.sites
          },
          {
            name: Kooboo.text.component.breadCrumb.dashboard
          },
          {
            name: Kooboo.text.common.Menus,
            url: Kooboo.Route.Menu.ListPage
          },
          {
            name: val
          }
        ];
      }
    },
    methods: {
      getData: function(callback) {
        var nameOrId = Kooboo.getQueryString("nameOrId");

        $.when(
          Kooboo.Menu.getFlat({
            name: nameOrId
          }),
          Kooboo.Site.Langs(),
          Kooboo.Page.getAll()
        ).then(function(menu, lang, page) {
          var r1 = $.isArray(menu) ? menu[0] : menu,
            r2 = $.isArray(lang) ? lang[0] : lang,
            r3 = $.isArray(page) ? page[0] : page;

          if (r1.success && r2.success && r3.success) {
            self.name = r1.model[0].name;
            var temp = r1.model.map(function(item, index) {
              item.urlEditing = false;
              item.addSubMenuing = false;
              item.tempEditingUrl = item.url;
              item.newSub = { name: undefined, url: undefined };
              item.validate = {
                name: { valid: true, msg: "" },
                url: { valid: true, msg: "" }
              };
              item.UpdateText = Kooboo.Route.Get(
                Kooboo.Menu._getUrl("UpdateText"),
                {
                  Id: item.id,
                  RootId: item.rootId
                }
              );
              return item;
            });
            self.menuItems = [];
            self.$nextTick(function() {
              self.menuItems = temp;
              self.defaultLang = r2.model.default;
              self.isMultiLangMenu = Object.keys(r2.model.cultures).length > 1;

              self.pageList = [];
              r3.model.pages.forEach(function(page) {
                self.pageList.push(page.path);
              });
              self.loading = false;
              self.$nextTick(function() {
                self.afterMenusRendered();
              });
              if (callback) {
                callback();
              }
            });
          }
        });
      },
      isNotRoot: function(m) {
        return m.id !== m.rootId;
      },
      className: function(m) {
        var className = [];
        className.push("treegrid-" + m.id);

        if (m.parentId !== Kooboo.Guid.Empty) {
          className.push("treegrid-parent-" + m.parentId);
        }

        return className.join(" ");
      },
      afterMenusRendered: function() {
        $('[data-inline-edit="true"]').editable({
          mode: "inline",
          type: "text",
          validate: function(value) {
            if ($.trim(value) == "") {
              return Kooboo.text.validation.required;
            } else if (
              value.match(/<[^>]+>/g) &&
              value.match(/<[^>]+>/g).length
            ) {
              return Kooboo.text.validation.tagNotAllow;
            }
          },
          send: "always",
          success: function(res, newName) {
            var id = $(this).attr("data-id");
            var _find = _.findLast(self.menuItems, function(menu) {
              return menu.id === id;
            });
            _find.name = newName;
            self.updateRelatedMenu(_find);
            self.saveMenuCodeEvent(_find);
            DataCache.removeRelatedData("menu");
          }
        });

        $(".tree").treegrid({
          expanderExpandedClass: "fa fa-caret-down",
          expanderCollapsedClass: "fa fa-caret-right"
        });
      },
      editUrl: function(event, item) {
        var toggle = function(item) {
          item.urlEditing = false;
          if (item.children instanceof Array && item.children.length > 0) {
            item.children.forEach(function(i) {
              toggle(i);
            });
          }
        };
        this.menuItems.forEach(function(value) {
          toggle(value);
        });
        self.clearEditStatus();
        item.urlEditing = true;
        $(".glyphicon-remove:visible").click();
      },
      editDisplayText: function() {
        $(".glyphicon-remove:visible").click();
      },
      cancelEditLink: function(event, item) {
        item.tempEditingUrl = item.url;
        item.urlEditing = false;
      },
      updateLinkUrl: function(event, item) {
        if (item.url !== item.tempEditingUrl) {
          if (!item.tempEditingUrl) item.tempEditingUrl = "#";
          item.url = item.tempEditingUrl;
          Kooboo.Menu.UpdateUrl({
            Id: item.id,
            RootId: item.rootId,
            url: item.url
          }).then(function(res) {
            if (res.success) {
              item.urlEditing = false;
              window.info.show(Kooboo.text.info.update.success, true);
              var _find = _.findLast(self.menuItems, function(menu) {
                return menu.id === item.id;
              });
              self.updateRelatedMenu(_find);
              self.saveMenuCodeEvent(_find);
            } else {
              m.url(m._url());
              window.info.show(Kooboo.text.info.update.fail, false);
            }
          });
        } else {
          self.cancelEditLink();
        }
      },
      swapOrder: function(rootId, ida, idb) {
        if (ida && idb) {
          Kooboo.Menu.Swap({
            rootId: rootId,
            ida: ida,
            idb: idb
          }).then(function(res) {
            if (res.success) {
              self.getData(function() {
                self.saveMenuCodeEvent();
              });
            }
          });
        }
      },
      clearValidate: function(item) {
        item.validate = {
          name: { valid: true, msg: "" },
          url: { valid: true, msg: "" }
        };
      },
      updateRelatedMenu: function(child) {
        var parent = _.find(self.menuItems, function(me) {
          return me.id === child.parentId;
        });

        if (parent) {
          var siblings = _.filter(self.menuItems, function(me) {
              return me.parentId === child.parentId;
            }),
            idxInSiblings = _.findIndex(siblings, function(sb) {
              return sb.id === child.id;
            });

          siblings.splice(idxInSiblings, 1);
          siblings.splice(idxInSiblings, 0, child);

          parent.children = siblings;

          self.updateRelatedMenu(parent);
        } else {
          return;
        }
      },
      menuMoveUp: function(e, index, item) {
        var _index = _.findLastIndex(self.menuItems, function(i, idx) {
          if (idx < index && i.parentId === item.parentId) {
            return true;
          }
        });

        if (_index > -1) {
          var idb = self.menuItems[_index].id;
          self.swapOrder(item.rootId, item.id, idb);
        }
      },
      initHelp: function() {
        self.codeHelperSubItem = [
          {
            text: "MarkSubItems",
            value: self.mark.markSubItems,
            for: "SUB_ITEM_CONTAINER"
          }
        ];
        self.subTmplCodeHelpers = [
          {
            text: "MarkAnchorText",
            value: self.mark.markAnchorText,
            for: "SUB_ITEM_TEMPLATE"
          },
          {
            text: "MarkHref",
            value: self.mark.markHref,
            for: "SUB_ITEM_TEMPLATE"
          },
          {
            text: "MarkSubItems",
            value: self.mark.markSubItems,
            for: "SUB_ITEM_TEMPLATE"
          },
          {
            text: "MarkActiveClass",
            value: self.mark.markActiveClass,
            for: "SUB_ITEM_TEMPLATE"
          },
          {
            text: "MarkParentId",
            value: self.mark.markParentId,
            for: "SUB_ITEM_TEMPLATE"
          },
          {
            text: "MarkCurrentId",
            value: self.mark.markCurrentId,
            for: "SUB_ITEM_TEMPLATE"
          }
        ];
        self.tmplCodeHelpers = [
          {
            text: "MarkAnchorText",
            value: self.mark.markAnchorText,
            for: "TEMPLATE"
          },
          {
            text: "MarkHref",
            value: self.mark.markHref,
            for: "TEMPLATE"
          },
          {
            text: "MarkSubItems",
            value: self.mark.markSubItems,
            for: "TEMPLATE"
          },
          {
            text: "MarkActiveClass",
            value: self.mark.markActiveClass,
            for: "TEMPLATE"
          },
          {
            text: "MarkParentId",
            value: self.mark.markParentId,
            for: "TEMPLATE"
          },
          {
            text: "MarkCurrentId",
            value: self.mark.markCurrentId,
            for: "TEMPLATE"
          }
        ];
      },
      menuMoveDown: function(e, index, item) {
        var _index = _.findIndex(self.menuItems, function(i, idx) {
          if (idx > index && i.parentId === item.parentId) {
            return true;
          }
        });
        if (_index > -1) {
          var idb = self.menuItems[_index].id;
          self.swapOrder(item.rootId, item.id, idb);
        }
      },
      onRemoveSubMenu: function(event, index, item) {
        if (confirm(Kooboo.text.confirm.deleteItem)) {
          Kooboo.Menu.Delete({
            rootId: item.rootId,
            Id: item.id
          }).then(function(res) {
            if (res.success) {
              self.getData(function() {
                window.info.show(Kooboo.text.info.delete.success, true);
                self.saveMenuCodeEvent();
              });
            } else {
              window.info.show(Kooboo.text.info.delete.fail, false);
            }
          });
        }
      },
      onAddSubMenu: function(event, index, item) {
        self.clearEditStatus();
        item.addSubMenuing = true;
      },
      clearEditStatus: function() {
        self.menuItems.forEach(function(value, index) {
          if (self.menuItems[index].urlEditing)
            self.menuItems[index].urlEditing = false;
          if (self.menuItems[index].addSubMenuing)
            self.menuItems[index].addSubMenuing = false;
        });
      },
      onCancelSubMenu: function(event, index, item) {
        item.addSubMenuing = false;
      },
      onSaveSubMenu: function(event, index, item) {
        var rules = {
          name: [{ required: Kooboo.text.validation.required }],
          url: []
        };

        var temp = Kooboo.validate(item.newSub, rules);
        item.validate = temp.result;
        self.$forceUpdate();
        if (!item.hasError) {
          Kooboo.Menu.CreateSub({
            rootId: item.rootId,
            parentId: item.id,
            name: item.newSub.name,
            url: item.newSub.url || "#"
          }).then(function(res) {
            if (res.success) {
              item.addSubMenuing = false;
              self.getData(function() {
                self.saveMenuCodeEvent();
                window.info.show(Kooboo.text.info.save.success, true);
              });
            } else {
              window.info.show(Kooboo.text.info.save.fail, false);
            }
          });
        } else {
          window.info.show(Kooboo.text.info.menuNameRequired, false);
        }
      },
      hideMultiLangModal: function() {
        self.showMultiLangModal = false;
      },
      onShowMultiModal: function(event, item) {
        self.CUR_MENU = item;
        Kooboo.Menu.getLang({
          id: item.id,
          rootId: item.rootId
        }).then(function(res) {
          if (res.success) {
            var keys = Object.keys(res.model);
            var defaultCultureIdx = keys.indexOf(self.defaultLang);

            self.multiNames = [];
            keys.forEach(function(key, index) {
              self.multiNames.push({ key: key, value: res.model[key] });
              if (index === defaultCultureIdx)
                self.multiNames[index].validate = { valid: true, msg: "" };
            });
            self.showMultiLangModal = true;
          }
        });
      },
      onSaveMultiName: function() {
        var hasError = false;
        self.multiNames.forEach(function(item) {
          if (item.validate) {
            item.validate = Kooboo.validField(item.value, [
              { required: Kooboo.text.validation.required }
            ]);
            if (item.validate.valid === false) {
              hasError = true;
            }
          }
        });
        self.$forceUpdate();
        if (!hasError) {
          var menus = _.cloneDeep(self.menuItems),
            data = {};
          _.forEach(self.multiNames, function(mn) {
            data[mn.key] = mn.value;
          });
          Kooboo.Menu.updateLang({
            id: self.CUR_MENU.id,
            rootId: self.CUR_MENU.rootId,
            values: data
          }).then(function(res) {
            if (res.success) {
              self.hideMultiLangModal();
              self.getData();
            } else {
              window.info.show(Kooboo.text.info.save.fail, false);
            }
          });
        }
      },
      renderPreview: function(tmpl, context, CUR_MENU) {
        if (!CUR_MENU) {
          CUR_MENU = self.CUR_MENU;
        }
        var relativeUrl = self.getRelativeUrl();

        if (tmpl.indexOf(self.mark.markHref) > -1) {
          tmpl = tmpl
            .split(self.mark.markHref)
            .join(context.url ? context.url : "#");
        }
        if (tmpl.indexOf(self.mark.markAnchorText) > -1) {
          tmpl = tmpl.split(self.mark.markAnchorText).join(context.name);
        }
        if (tmpl.indexOf(self.mark.markCurrentId) > -1) {
          tmpl = tmpl.split(self.mark.markCurrentId).join(context.id);
        }
        if (tmpl.indexOf(self.mark.markParentId) > -1) {
          tmpl = tmpl.split(self.mark.markParentId).join(context.parentId);
        }

        if (tmpl.indexOf(self.mark.markSubItems) > -1) {
          if (context.children.length > 0) {
            var _tmplArr = tmpl.split(self.mark.markSubItems),
              parentsTemplate = "";
            if (!CUR_MENU) {
              tmpl = _tmplArr.join(context.subItemContainer);
              parentsTemplate = context.subItemTemplate;
            } else {
              tmpl = _tmplArr.join(
                CUR_MENU.id === context.id
                  ? self.subItemContainer
                  : context.subItemContainer
              );
              parentsTemplate =
                CUR_MENU.id === context.id
                  ? self.subItemTemplate
                  : context.subItemTemplate;
            }

            var childTemplates = [];
            _.forEach(context.children, function(child) {
              var template = "";

              if (CUR_MENU) {
                template =
                  (CUR_MENU.id === child.id
                    ? self.currentTemplate
                    : child.template === context.subItemTemplate
                    ? parentsTemplate
                    : child.template) || parentsTemplate;
              } else {
                template =
                  (child.template === context.subItemTemplate
                    ? parentsTemplate
                    : child.template) || parentsTemplate;
              }
              childTemplates.push(
                self.renderPreview(template, child, CUR_MENU)
              );
            });
            tmpl = self.removeNoActiveClass(tmpl, context, relativeUrl);
            tmpl = tmpl
              .split(self.mark.markSubItems)
              .join(childTemplates.join(""));
          } else {
            tmpl = tmpl.split(self.mark.markSubItems).join("");
            tmpl = self.removeNoActiveClass(tmpl, context, relativeUrl);
          }
        }

        if (tmpl.indexOf("activeclass:") > -1) {
          try {
            function setActiveClass(elem) {
              var selfTemplate = $(elem)[0].outerHTML;
              var hasActiveClass = selfTemplate.match(/{.*?}/);
              if (hasActiveClass) {
                var className = hasActiveClass[0].match(
                  /{[\w\W]*:([\w\W]*)}/
                )[1];
                if (
                  $(elem)[0].hasAttribute("{activeclass:" + className + "}")
                ) {
                  $(elem)[0].removeAttribute("{activeclass:" + className + "}");
                  $(elem).addClass(className);
                }

                $(elem)
                  .children()
                  .each(function(idx, child) {
                    setActiveClass($(child));
                  });
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
      },
      onHideTemplateModal: function() {
        self.showTemplateModal = false;
        self.subItemContainer = "";
        self.subItemTemplate = "";
        self.currentTemplate = "";
        self.previewCode = "";
        self.templatePreview = "";
        self.menuWithNoChild = false;
      },
      onShowTemplateModal: function(event, item) {
        self.CUR_MENU = item;
        var _find = _.findLast(self.menuItems, function(menu) {
          return menu.id === self.CUR_MENU.id;
        });
        self.subItemContainer = _find.subItemContainer || "";
        self.subItemTemplate = _find.subItemTemplate || "";
        self.currentTemplate = _find.template || "";
        if (!self.currentTemplate) {
          var parent = _.find(self.menuItems, function(menu) {
            return menu.id === _find.parentId;
          });
          if (parent) {
            self.currentTemplate = parent.subItemTemplate;
          }
        }

        var current = self.mark.markSubItems;
        self.previewCode = html_beautify(
          self.renderPreview(current, self.menuItems[0])
        );
        self.templatePreview = item.subItemTemplate;
        if (item.children && item.children.length === 0) {
          self.menuWithNoChild = true;
        }
        if (item.id !== item.rootId) {
          self.showCurrentTemplate = true;
        }
        self.showTemplateModal = true;
      },
      onSaveTemplate: function() {
        self.menuWithNoChild = false;
        Kooboo.Menu.UpdateTemplate({
          Id: self.CUR_MENU.id,
          RootId: self.CUR_MENU.rootId,
          SubItemContainer: self.subItemContainer,
          SubItemTemplate: self.subItemTemplate,
          Template: self.currentTemplate
        }).then(function(res) {
          if (res.success) {
            self.getData(function() {
              self.saveMenuCodeEvent();
              self.onHideTemplateModal();
            });
          }
        });
      },
      codeHelp: function(event, m) {
        var component = undefined;

        switch (m.for) {
          case "SUB_ITEM_CONTAINER":
            component = self.$refs.editor2;
            break;
          case "SUB_ITEM_TEMPLATE":
            component = self.$refs.editor3;
            break;
          case "TEMPLATE":
            component = self.$refs.editor4;
            break;
        }
        component.replace(m.value);
      },
      saveMenuCodeEvent: function() {
        if (window.parent.__gl && window.parent.__gl.saveMenuFinish) {
          var current = self.mark.markSubItems;
          var value = self.renderPreview(current, self.menuItems[0]);
          window.parent.__gl.saveMenuFinish(value);
        }
      },
      getRelativeUrl: function() {
        return window.parent.__gl.relativeUrl;
      },
      isActive: function(context, relativeUrl) {
        var self = this;
        var menuUrl = context.url;
        if (!menuUrl || !relativeUrl) return false;
        if (menuUrl.toLowerCase() == relativeUrl.toLowerCase()) return true;
        var active = false;
        $.each(context.children(), function(i, children) {
          if (self.isActive(children, relativeUrl)) {
            active = true;
            return false;
          }
        });
        return active;
      },
      removeNoActiveClass: function(tmpl, context, relativeUrl) {
        var self = this;
        if (
          tmpl.indexOf("activeclass:") > -1 &&
          !self.isActive(context, relativeUrl)
        ) {
          var _temp = $(tmpl);
          var activeClassName = tmpl
            .match(/{.*?}/)[0]
            .match(/{[\w\W]*:([\w\W]*)}/)[1];
          _temp[0].removeAttribute("{activeclass:" + activeClassName + "}");
          tmpl = _temp[0].outerHTML;
        }
        return tmpl;
      }
    }
  });
});
