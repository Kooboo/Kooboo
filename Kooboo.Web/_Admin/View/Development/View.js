(function() {
  Vue.component("data-source-item", {
    template: "#dataSourceTmpl",
    props: ["item", "index"]
  });
})();

$(function() {
  window.viewEditor = {};
  var placeholderKey = "k-placeholder";

  var Helper = Kooboo.EditorHelper,
    DataContext = Kooboo.viewEditor.DataContext,
    BindingPanel = Kooboo.viewEditor.viewModel.BindingPanel,
    DataSourcePanel = Kooboo.viewEditor.viewModel.DataSourcePanel,
    LayoutPosition = Kooboo.viewEditor.viewModel.LayoutPosition,
    Position = Kooboo.viewEditor.widget.Position,
    ActionStore = Kooboo.viewEditor.store.ActionStore,
    DataStore = Kooboo.viewEditor.store.DataStore,
    PageStore = Kooboo.viewEditor.store.PageStore,
    LayoutStore = Kooboo.viewEditor.store.LayoutStore,
    ComparerStore = Kooboo.viewEditor.store.ComparerStore,
    FormBindingStore = Kooboo.viewEditor.store.FormBindingStore,
    k2attr = Kooboo.viewEditor.util.k2attr,
    KBFrame = Kooboo.viewEditor.component.KBFrame;
  var self, kbFrame, helper, pos;
  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        viewId: Kooboo.getQueryString("Id"),
        model: {
          name: ""
        },
        rules: {
          name: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
              message: Kooboo.text.validation.objectNameRegex
            },
            {
              min: 1,
              max: 64,
              message:
                Kooboo.text.validation.minLength +
                1 +
                ", " +
                Kooboo.text.validation.maxLength +
                64
            },
            {
              remote: {
                url: Kooboo.View.isUniqueName(),
                data: function() {
                  return {
                    name: self.model.name
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ]
        },
        bindingPanel: new BindingPanel(),
        dataSourcePanel: new DataSourcePanel(),
        layoutPosition: new LayoutPosition(),
        mediaDialogData: null,
        curType: "preview",
        elem: null,
        viewContent: "",
        load: false,
        positions: {}
      };
    },
    created: function() {
      self.bindingPanel.onRemove = function(binding) {
        if (pos) {
          pos.unbind(binding.elem, binding.bindingType);
          var bindings = pos.getBindings(binding.elem);

          if (bindings.length) {
            helper.rering(binding.elem);
            var text = _.map(bindings, function(item) {
              var bindingType = item.bindingType;
              return bindingType[0].toUpperCase() + bindingType.slice(1);
            }).join(",");
            helper.unlabel(binding.elem);
            helper.label(binding.elem, text);
          } else {
            helper.unring(binding.elem);
            helper.unlabel(binding.elem);
            DataContext.clear(binding.elem);
          }
        }
      };
      self.dataSourcePanel.setViewId(
        self.viewId ? self.viewId : Kooboo.Guid.Empty
      );
    },
    mounted: function() {
      helper = new Helper($(".kb-editor")[0]);
      kbFrame = new KBFrame(document.getElementById("view_iframe"), {
        type: "view"
      });
      Kooboo.EventBus.subscribe("kb/lighter/holder", function(elem) {
        self.elem = elem;
        // Lighter
        helper.hold(elem);
        // HTML Preivew
        Kooboo.EventBus.publish("kb/html/previewer/select", elem);
        $("a[href=#binding_panel]").tab("show");
      });
      Kooboo.EventBus.subscribe("kb/preview/elem/hover", function(elem) {
        // Lighter
        helper.hover(elem);
        // HTML Preivew
        Kooboo.EventBus.publish("kb/html/previewer/hover", elem);
      });
      Kooboo.EventBus.subscribe("kb/html/elem/hover", function(elem) {
        // Lighter
        helper.hover(elem);
        // HTML Preivew
        Kooboo.EventBus.publish("kb/html/previewer/hover", elem);
      });
      Kooboo.EventBus.subscribe("position/change", function(position) {
        self.changePosition(position);
      });
      Kooboo.EventBus.subscribe("layout/change", function(layout) {
        function loadLayout() {
          self.load = true;
          if (window.viewEditor.position) {
            self.changePosition(self.layoutPosition.position);
            helper.shadow(window.viewEditor.position.elem, $(".kb-editor")[0]);
          }
        }

        if (layout.body) {
          kbFrame.setContentWithoutResource(layout.body, function() {
            loadLayout();
          });
        } else {
          Kooboo.Layout.Get({
            id: layout.id
          }).then(function(res) {
            if (res.success) {
              kbFrame.setContentWithoutResource(res.model.body, function() {
                loadLayout();
              });
            }
          });
        }
      });
      Kooboo.EventBus.subscribe("DataStore/change", function(remove) {
        self.scanPositions(kbFrame.getDocumentElement());
      });
      var dummyLayout;
      $.when(
        Kooboo.View.Get({
          id: self.viewId || Kooboo.Guid.Empty
        }),
        Kooboo.DataMethodSetting.byView({
          id: self.viewId || Kooboo.Guid.Empty
        }),
        Kooboo.View.dataMethod({
          id: self.viewId || Kooboo.Guid.Empty
        }),
        Kooboo.Layout.getList()
      ).then(function(viewRes, methodRes, dataRes, layoutRes) {
        viewRes = $.isArray(viewRes) ? viewRes[0] : viewRes;
        dataRes = $.isArray(dataRes) ? dataRes[0] : dataRes;
        layoutRes = $.isArray(layoutRes) ? layoutRes[0] : layoutRes;
        self.model.name = viewRes.model.name;
        self.viewContent = viewRes.model.body || "";
        dummyLayout = viewRes.model.dummyLayout;

        if (methodRes[0].success) {
          ActionStore.init(methodRes[0].model);
        }

        if (dataRes.success) {
          DataStore.init(dataRes.model);
        }

        var dummyLayoutObj = {
          name: "<" + Kooboo.text.site.view.dummy + ">",
          body: dummyLayout
        };

        LayoutStore.init(_.concat([dummyLayoutObj], layoutRes.model));
      });
      Kooboo.Link.All().then(function(res) {
        if (res.success) {
          var links = {};
          Object.keys(res.model).forEach(function(key) {
            links[key] = [];
            res.model[key].forEach(function(link) {
              var isSelf =
                key == "views" && link.url == "/__kb/View/" + self.model.name;
              links[key].push({
                url: link.url,
                displayText:
                  link.url +
                  (isSelf ? " (" + Kooboo.text.site.view.current + ")" : ""),
                parameters: link.parameters
              });
            });
          });
          PageStore.init(links);
        }
      });

      Kooboo.View.CompareType().then(function(res) {
        if (res.success) {
          ComparerStore.init(res.model);
        }
      });

      $(".kb-editor").on(
        Kooboo.BrowserInfo.getBrowser() == "chrome"
          ? "mousewheel"
          : "DOMMouseScroll",
        function(e) {
          var scrollTop =
            kbFrame.getScrollTop() +
            (e.originalEvent.deltaY
              ? e.originalEvent.deltaY
              : e.originalEvent.detail * 50);
          kbFrame.setScrollTop(scrollTop);
        }
      );

      $(window).on("resize", function() {
        helper.refresh();
      });

      $(document).keydown(function(e) {
        if (e.keyCode == 83 && e.ctrlKey) {
          //Ctrl + S
          e.preventDefault();
          self.onSave();
        }
      });
    },
    methods: {
      changeType: function(type) {
        if (self.curType !== type) {
          self.curType = type;
          if (type == "code") {
            if (pos) {
              self.viewContent = pos.getHTML();
            }
          } else if (type == "preview") {
            if (pos) {
              var oldHTML = pos.getHTML(),
                newHTML = self.viewContent;

              if (oldHTML !== newHTML) {
                self.setView(newHTML);
              }
              self.$nextTick(function() {
                $(window).trigger("resize");
              });
            }
          }
        }
      },
      setView: function(html) {
        helper.unring();
        helper.unlabel();
        helper.unshadow();
        helper.unhold();
        helper.unhover();
        self.bindingPanel.reset();
        self.elem = null;

        if (pos) {
          pos.setHTML(html);

          if (pos.firstLoaded) {
            self._viewContent = pos.getHTML();
            delete pos.firstLoaded;
          }

          helper.shadow(pos.elem, $(".kb-editor")[0]);

          $("a", kbFrame.getDocumentElement()).on("click", function(e) {
            e.preventDefault();
          });

          $("img", kbFrame.getDocumentElement())
            .load(function() {
              $(window).trigger("resize");
            })
            .error(function() {
              $(window).trigger("resize");
            });

          var bindings = pos.getBindings();
          _.forEach(bindings, function(it, idx) {
            helper.ring(it.elem);
            var text = pos
              .getBindings(it.elem)
              .map(function(item) {
                var bindingType = item.bindingType;
                return bindingType[0].toUpperCase() + bindingType.slice(1);
              })
              .join(", ");
            helper.label(it.elem, text);
            self.bindingPanel.add(bindings[idx]);
          });

          $(window).trigger("resize");

          for (var i = 0; i < 3; i++) {
            setTimeout(function() {
              $(window).trigger("resize");
            }, 500);
          }
        }
      },
      onSaveAndReturn: function() {
        self.submit(function() {
          self.goBack();
        });
      },
      onSave: function() {
        self.submit(function(id) {
          if (self.isNewView) {
            location.href = Kooboo.Route.Get(Kooboo.Route.View.DetailPage, {
              Id: id
            });
          } else {
            window.info.show(Kooboo.text.info.save.success, true);
            self._viewContent =
              self.curType == "preview" ? pos.getHTML() : self.viewContent;
          }
        });
      },
      isValid: function() {
        return self.isNewView ? self.$refs.form.validate() : true;
      },
      submit: function(callback) {
        if (!self.isValid()) {
          return;
        }
        Kooboo.View.post(JSON.stringify(self.getSubmitData())).then(function(
          res
        ) {
          if (res.success) {
            if (typeof callback === "function") {
              callback(res.model);
            }
          } else {
            window.info.show(Kooboo.text.info.save.fail, false);
          }
        });
      },
      goBack: function() {
        location.href = Kooboo.Route.Get(Kooboo.Route.View.ListPage);
      },
      userCancel: function() {
        if (self.hasValueChanged()) {
          if (confirm(Kooboo.text.confirm.beforeReturn)) {
            self.goBack();
          }
        } else {
          self.goBack();
        }
      },
      hasValueChanged: function() {
        if (self.curType == "preview") {
          if (pos) {
            return pos.getHTML() !== self._viewContent;
          } else {
            return true;
          }
        } else {
          return self.viewContent !== self._viewContent;
        }
      },
      getSubmitData: function() {
        var actions = self.dataSourcePanel.actions;
        _.forEach(actions, function(action) {
          self.removeClientId(action);
        });

        var _data = {
          id: self.isNewView ? Kooboo.Guid.Empty : self.viewId,
          name: self.model.name,
          dataSources: actions,
          body:
            self.curType == "preview"
              ? pos
                ? pos.getHTML()
                : self.viewContent
              : self.viewContent
          /*formBindings: FormBindingStore.all()*/
        };

        if (self.layoutPosition.layout) {
          _data["defaultDisplayLayout"] = self.layoutPosition.layout.name;
        }

        if (self.layoutPosition.position) {
          _data["defaultDisplayPosition"] = self.layoutPosition.position;
        }

        return _data;
      },
      removeClientId: function(action) {
        if (/^\d+$/.test(action.id)) {
          delete action.id;
        }

        if (/^\d+$/.test(action.parentId)) {
          delete action.parentId;
        }

        _.forEach(action.children, function(child) {
          self.removeClientId(child);
        });
      },
      formatCode: function() {
        this.$refs.editor.formatCode();
      },
      bindingSave: function(data) {
        var inst = self.bindingPanel.get(data.elem, data.bindingType);
        self.bindingPanel[inst ? "update" : "add"](data);
        pos.bind(data);
        helper.ring(data.elem);
        var text = pos
          .getBindings(data.elem)
          .map(function(item) {
            var bindingType = item.bindingType;
            return bindingType[0].toUpperCase() + bindingType.slice(1);
          })
          .join(",");
        helper.label(data.elem, text);
        Kooboo.EventBus.publish("kb/frame/dom/update");
        $(window).trigger("resize");
      },
      bindingItemRemove: function(binding) {
        self.bindingPanel.remove(binding);
        Kooboo.EventBus.publish("kb/frame/dom/update");
      },
      bindingItemEdit: function(binding) {
        if (pos) {
          var find = _.findLast(pos.bindings, function(bd) {
            return binding.elem.isEqualNode(bd.elem);
          });
          find && (binding.elem = find.elem);
        }
        self.bindingPanel.edit(binding);
      },
      focusBinding: function(type, binding) {
        Kooboo.EventBus.publish("kb/lighter/holder", binding.elem);
      },
      changePosition: function(position) {
        var oldHTML = self.viewContent,
          newHTML = self.viewContent;

        var isFirstLoaded = false;

        if (pos) {
          oldHTML = pos.getHTML();
          pos.reset();
        } else {
          isFirstLoaded = true;
        }

        pos = self.positions[position];
        isFirstLoaded && (pos.firstLoaded = true);
        window.viewEditor.position = pos;

        self.setView(newHTML);

        setTimeout(function() {
          pos &&
            $(pos.elem.ownerDocument.body).animate(
              {
                scrollTop: pos.elem.offsetTop
              },
              200
            );
        }, 200);
      },
      scanPositions: function(html) {
        self.positions = {};
        self.scanAttrPositions(html);
      },
      scanAttrPositions: function(node) {
        var hash = self.positions;
        $("[" + placeholderKey + "]", node).each(function(ix, it) {
          var name = $(it).attr(placeholderKey);
          hash[name] = new Position(it);
        });
        self.positions = hash;
      },
      changeImage: function() {
        Kooboo.Media.getList().then(function(res) {
          if (res.success) {
            res.model["show"] = true;
            res.model["context"] = self;
            res.model["onAdd"] = function(selected) {
              self.elem.setAttribute("src", selected.url);
              self.elem.setAttribute("style", "max-width: 100%;");
              Kooboo.EventBus.publish("kb/frame/dom/update");
            };
            self.mediaDialogData = res.model;
          }
        });
      },
      editDom: function() {
        if ($(self.elem).is("a")) {
          Kooboo.EventBus.publish("kb/view/edit/node", {
            type: "link",
            html: $(self.elem).html(),
            href: $(self.elem).attr("href"),
            inNewWindow:
              $(self.elem).attr("target") &&
              $(self.elem).attr("target") == "_blank"
          });
        } else {
          Kooboo.EventBus.publish("kb/view/edit/node", {
            type: "normal",
            html: $(self.elem).html()
          });
        }
      },
      editNodeSave: function(data) {
        switch (data.type) {
          case "link":
            $(self.elem).html(data.html);
            $(self.elem).attr("href", data.href);
            if (data.inNewWindow) {
              $(self.elem).attr("target", "_blank");
            } else {
              self.elem.removeAttribute("target");
            }
            break;
          case "normal":
            $(self.elem).html(data.html);
            break;
        }

        $(window).trigger("resize");
      },
      copyDom: function() {
        var cloneDom = $(self.elem).clone();
        $(cloneDom).insertAfter(self.elem);

        if (pos) {
          var bindings = pos.getBindings();
          _.forEach(bindings, function(it, idx) {
            var inst = self.bindingPanel.get(it.elem, it.bindingType);
            if (!inst) {
              helper.ring(it.elem);
              var text = pos
                .getBindings(it.elem)
                .map(function(item) {
                  var bindingType = item.bindingType;
                  return bindingType[0].toUpperCase() + bindingType.slice(1);
                })
                .join(", ");
              helper.label(it.elem, text);
              self.bindingPanel.add(bindings[idx]);
            }
          });
        }
        Kooboo.EventBus.publish("kb/frame/dom/update");
        $(window).trigger("resize");
      },
      removeDom: function() {
        self.removeBindings(self.elem);
        DataContext.clear(self.elem);

        var parent = $(self.elem).parent()[0];
        $(self.elem).remove();
        Kooboo.EventBus.publish(
          "kb/lighter/holder",
          $(pos.elem).children()[0] || pos.elem
        );
        Kooboo.EventBus.publish("kb/frame/dom/update");
        $(window).trigger("resize");
      },
      removeBindings: function(elem) {
        self.removeDesendantBindings(elem);
        self.removeBindingsNonRecursive(elem);
      },
      removeDesendantBindings: function(elem) {
        var storedElem = elem;
        $(self.elem)
          .find("*")
          .each(function(i) {
            self.removeBindingsNonRecursive(this);
          });
        self.elem = storedElem;
      },
      removeBindingsNonRecursive: function(elem) {
        self.elem = elem;
        var bindings = _.cloneDeep(self.bindingPanel.existList);
        _.forEach(bindings, function(binding) {
          if (binding.elem === elem) {
            self.bindingPanel.remove(binding);
          }
        });
      },
      onDataSourceSave: function(data) {
        return self.dataSourcePanel.save(data);
      }
    },
    computed: {
      isNewView: function() {
        return !self.viewId;
      },
      canChangeImg: function() {
        if (self.elem && pos && self.elem.tagName == "IMG") {
          if (
            $(self.elem).parents(k2attr["repeat"]).length ||
            self.elem.hasAttribute(k2attr["attributes"])
          ) {
            return false;
          }
          return !self.elem.hasAttribute(placeholderKey);
        } else {
          return false;
        }
      },
      canEditDom: function() {
        if (
          self.elem &&
          pos &&
          ["img"].indexOf(self.elem.tagName.toLowerCase()) == -1
        ) {
          return !self.elem.hasAttribute(placeholderKey);
        } else {
          return false;
        }
      },
      canRemoveDom: function() {
        if (self.elem && pos) {
          return !self.elem.hasAttribute(placeholderKey);
        } else {
          return false;
        }
      }
    },
    watch: {
      elem: function(elem) {
        self.bindingPanel.elem = elem;
      },
      load: function(load) {
        if (load) {
          helper.unring();
          helper.unlabel();
          helper.unshadow();
          helper.unhold();
          helper.unhover();
          self.bindingPanel.reset();
          self.elem = null;

          self.layoutPosition.positionList = [];

          self.scanPositions(kbFrame.getDocumentElement());

          _.forEach(Object.keys(self.positions), function(position) {
            self.layoutPosition.positionList.push(position);
          });
          self.layoutPosition.position = self.layoutPosition.positionList[0];

          self.load = false;
        }
      }
    }
  });
});
