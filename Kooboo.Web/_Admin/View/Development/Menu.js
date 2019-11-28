$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      return {
        name: "",
        menuItems: [],
        breads: [
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
            name: name
          }
        ],
        defaultLang: undefined,
        isMultiLangeMenu: undefined,
        loading: undefined,
        _menuItems: undefined
      };
    },
    created: function() {
      self = this;
      self.getData();
    },
    mounted: function() {},
    methods: {
      getData: function() {
        $.when(
          Kooboo.Menu.getFlat({
            id: Kooboo.getQueryString("Id")
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
              return item;
            });
            self.menuItems = temp;
            self.defaultLang = r2.model.default;
            self.isMultiLangeMenu = Object.keys(r2.model.cultures).length > 1;

            self.pageList = [];
            r3.model.pages.forEach(function(page) {
              self.pageList.push(page.path);
            });

            self.loading = false;
            self.$nextTick(function() {
              self.afterMenusRendered();
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
            var _find = _.findLast(self.menuItems(), function(menu) {
              return menu.id == id;
            });
            _find.name(newName);
            self.updateRelatedMenu(_find);
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
        item.urlEditing = true;
      },
      cancelEditLink: function(event, item) {
        item.tempEditingUrl = item.url;
        item.urlEditing = false;
      },
      updateLinkUrl: function(event, item) {
        item.url = item.tempEditingUrl;
        item.urlEditing = false;
      },
      swapOrder: function(rootId, ida, idb) {
        Kooboo.Menu.Swap({
          rootId: rootId,
          ida: ida,
          idb: idb
        }).then(function(res) {
          if (res.success) {
          } else {
            self.menuItems = self._menuItems;
          }
        });
      },
      clearValidate: function(item) {
        item.validate = {
          name: { valid: true, msg: "" },
          url: { valid: true, msg: "" }
        };
      },
      getAllChildrenMumber: function(item) {
        var mum = 0;
        if (item.children instanceof Array && item.children.length > 0) {
          item.children.forEach(function(i) {
            var childMum = self.getAllChildrenMumber(i);
            mum = mum + childMum;
            mum++;
          });
        }
        return mum;
      },
      menuMoveUp: function(e, index, item) {
        var count = self.getAllChildrenMumber(self.menuItems[index - 1]);
        self.swapOrder(
          item.rootId,
          item.id,
          self.menuItems[index - count - 1].id
        );
        self.getData();
      },
      menuMoveDown: function(e, index, item) {
        var count = self.getAllChildrenMumber(item);
        self.swapOrder(
          item.rootId,
          item.id,
          self.menuItems[index + count + 1].id
        );
        self.getData();
      },
      onRemoveSubMenu: function(event, index, item) {
        if (confirm(Kooboo.text.confirm.deleteItem)) {
          Kooboo.Menu.Delete({
            rootId: item.rootId,
            Id: item.id
          }).then(function(res) {
            if (res.success) {
              self.getData();
              window.info.show(Kooboo.text.info.delete.success, true);
            } else {
              window.info.show(Kooboo.text.info.delete.fail, false);
            }
          });
        }
      },
      onAddSubMenu: function(event, index, item) {
        item.addSubMenuing = true;
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
              self.getData();
              window.info.show(Kooboo.text.info.save.success, true);
            } else {
              window.info.show(Kooboo.text.info.save.fail, false);
            }
          });
          item.addSubMenuing = false;
        } else {
          window.info.show(Kooboo.text.info.menuNameRequired, false);
        }
      }
    }
  });
});
