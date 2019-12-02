$(function() {
  function newId() {
    return Math.ceil(Math.random() * Math.pow(2, 53));
  }

  var Guid = Kooboo.Guid,
    Helper = Kooboo.EditorHelper,
    BindingStore = Kooboo.layoutEditor.store.BindingStore,
    PositionStore = Kooboo.layoutEditor.store.PositionStore,
    BindingPanel = Kooboo.layoutEditor.viewModel.BindingPanel,
    talBinder = Kooboo.layoutEditor.utils.talBinder,
    tal2attr = Kooboo.layoutEditor.utils.tal2attr,
    talParser = Kooboo.layoutEditor.utils.talParser,
    KBFrame = Kooboo.layoutEditor.component.KBFrame;
  var self,
    kbFrame,
    positionKey = "k-placeholder",
    omitTagKey = "k-omit",
    helper;

  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        layoutId: Kooboo.getQueryString("Id") || Guid.Empty,
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
                url: Kooboo.Layout.isUniqueName(),
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
        layoutCode: "",
        _layoutCode: "",
        htmlContent: "",
        _htmlContent: "",
        curType: "preview",
        bindingPanel: new BindingPanel()
      };
    },
    mounted: function() {
      helper = new Helper($(".kb-editor")[0]);

      Kooboo.EventBus.subscribe("kb/lighter/holder", function(elem) {
        self.bindingPanel.elem = elem;
        if (elem !== elem.ownerDocument.body) {
          $(elem.ownerDocument.body).animate(
            {
              scrollTop: $(elem).offset().top
            },
            200,
            function() {
              // Lighter
              helper.hold(elem);
              // HTML Preivew
              Kooboo.EventBus.publish("kb/html/previewer/select", elem);
            }
          );
        } else {
          helper.hold(elem);
          Kooboo.EventBus.publish("kb/html/previewer/select", elem);
        }
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

      Kooboo.EventBus.subscribe("binding/save", function(data) {
        talBinder.bind(data.elem, data);

        if (data.id) {
          BindingStore.update(data.id, data);
        } else {
          BindingStore.add({
            id: newId(),
            elem: data.elem,
            type: data.type,
            text: data.text
          });
        }
        Kooboo.EventBus.publish("kb/frame/dom/update");

        helper.ring(data.elem);
        helper.label(data.elem, data.type);
      });

      Kooboo.EventBus.subscribe("binding/remove", function(data) {
        var item = BindingStore.byId(data.id);

        if (item) {
          talBinder.unbind(item.elem, item.type);
          BindingStore.remove(data.id);
          helper.unring(item.elem);
          helper.unlabel(item.elem);
        }
      });

      Kooboo.EventBus.subscribe("position/add", function(data) {
        self.attrPosition(data.elem, data.name, data.type, data.applyOmit);
        Kooboo.EventBus.publish("kb/html/previewer/select", $(data.elem)[0]);
      });

      Kooboo.EventBus.subscribe("position/update", function(data) {
        self.updatePosition(data.id, data.name, data.applyOmit);
        Kooboo.EventBus.publish("kb/html/previewer/select", $(data.elem)[0]);
      });

      Kooboo.EventBus.subscribe("position:remove", function(pos) {
        var position = PositionStore.byId(pos.id);
        if (position) {
          switch (position.type) {
            case "attr":
            case "prepend":
            case "append":
            case "ap/prepend":
              $(position.elem).removeAttr(positionKey);
              position.elem.hasAttribute(omitTagKey) &&
                $(position.elem).removeAttr(omitTagKey);
              helper.unmask(position.elem);
              break;
          }
          PositionStore.remove(pos.id);
        }
        Kooboo.EventBus.publish(
          "kb/html/previewer/select",
          $(pos.elem).parent()[0]
        );
        Kooboo.EventBus.publish("kb/frame/dom/update");
      });

      kbFrame = new KBFrame(document.getElementById("layout_iframe"), {
        type: "layout"
      });

      $(kbFrame).on("loaded", function() {
        $(window).trigger("resize");
      });

      Kooboo.EventBus.subscribe("kb/frame/loaded", function() {
        $(window).trigger("resize");
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

      $.when(
        Kooboo.Layout.Get({
          Id: self.layoutId
        }),
        Kooboo.Style.getExternalList(),
        Kooboo.Script.getExternalList(),
        Kooboo.ResourceGroup.Style(),
        Kooboo.ResourceGroup.Script()
      ).then(function(layout, styles, scripts, styleGroup, scriptGroup) {
        layout = $.isArray(layout) ? layout[0] : layout;
        styles = $.isArray(styles) ? styles[0] : styles;
        scripts = $.isArray(scripts) ? scripts[0] : scripts;
        styleGroup = $.isArray(styleGroup) ? styleGroup[0] : styleGroup;
        scriptGroup = $.isArray(scriptGroup) ? scriptGroup[0] : scriptGroup;

        var styleList = styles.model.map(function(style) {
            return {
              id: style.id,
              text: style.name,
              url: style.routeName
            };
          }),
          scriptList = scripts.model.map(function(script) {
            return {
              id: script.id,
              text: script.name,
              url: script.routeName
            };
          }),
          styleGroupList = styleGroup.model.map(function(style) {
            return {
              id: style.id,
              text: style.name,
              url: style.relativeUrl
            };
          }),
          scriptGroupList = scriptGroup.model.map(function(script) {
            return {
              id: script.id,
              text: script.name,
              url: script.relativeUrl
            };
          });

        self.bindingPanel.styleResource = {
          styles: styleList,
          styleGroup: styleGroupList
        };

        self.bindingPanel.scriptResource = {
          scripts: scriptList,
          scriptGroup: scriptGroupList
        };

        self.model.name = layout.model.name;
        self.layoutCode = layout.model.body;
        self.setHTML(self.layoutCode, function() {
          self._layoutCode = self.getHTML();
        });
      });

      // $(document).keydown(function(e) {
      //     if (e.keyCode == 83 && e.ctrlKey) {
      //         //Ctrl + S
      //         e.preventDefault();
      //         self.onSave();
      //     }
      // })
    },
    methods: {
      formatCode: function() {
        this.$refs.editor.formatCode();
      },
      changeType: function(type) {
        if (self.curType !== type) {
          self.curType = type;

          if (type == "code") {
            self.layoutCode = self.getHTML();
            self.$nextTick(function() {
              self.formatCode();
            });
          } else {
            var oldHtml = self.htmlContent,
              newHtml = self.layoutCode;

            if (oldHtml !== newHtml) {
              self.setHTML(newHtml);
            }
          }
        }
      },
      getHTML: function() {
        return kbFrame.getHTML();
      },
      setHTML: function(html, callback) {
        BindingStore.clear();

        !kbFrame.hasResource() &&
          kbFrame.setResource(self.bindingPanel.resources);
        kbFrame.setContent(html, function() {
          setTimeout(function() {
            $(window).trigger("resize");
          }, 1500);

          helper.unring();
          helper.unlabel();
          helper.unhold();
          helper.unhover();
          helper.unmask();
          helper.refresh();

          self.scanPositions();
          _.forEach(PositionStore.getAll(), function(it) {
            helper.mask(it.elem);
          });

          self.scanBindings();
          _.forEach(
            _.filter(BindingStore.getAll(), function(it) {
              return ["style", "script"].indexOf(it.type) == -1;
            }),
            function(it) {
              helper.label(it.elem, it.type);
              helper.ring(it.elem);
            }
          );

          $(window).trigger("resize");
          self.htmlContent = self.getHTML();
          self._htmlContent = self._htmlContent || self.getHTML();

          if (callback) callback();
        });
      },
      onSaveAndReturn: function() {
        self.onSubmitLayout(function() {
          self.goBack();
        });
      },
      onSave: function() {
        self.onSubmitLayout(function(id) {
          if (self.isNewLayout) {
            location.href = Kooboo.Route.Get(Kooboo.Route.Layout.DetailPage, {
              Id: id
            });
          } else {
            window.info.show(Kooboo.text.info.save.success, true);
          }
        });
      },
      userCancel: function() {
        if (self.isContentChanged) {
          if (confirm(Kooboo.text.confirm.beforeReturn)) {
            self.goBack();
          }
        } else {
          self.goBack();
        }
      },
      getBodyHtml: function() {
        if (self.isNewLayout) {
          return self.getHTML();
        } else {
          if (self.isContentChanged) {
            return self.getHTML();
          } else {
            return self._layoutCode;
          }
        }
      },
      doSubmit: function(body, callback) {
        Kooboo.Layout.post({
          id: self.layoutId,
          name: self.model.name,
          body: body
        }).then(function(res) {
          if (res.success) {
            self._htmlContent = self.htmlContent;
            self._layoutCode = self.layoutCode;
            if (typeof callback == "function") {
              callback(res.model);
            }
          } else {
            window.info.show(Kooboo.text.info.save.fail, false);
          }
        });
      },
      submit: function(callback) {
        if (self.curType == "code") {
          self.setHTML(self.layoutCode, function() {
            setTimeout(function() {
              var body = self.getBodyHtml();
              self.doSubmit(body, callback);
            }, 600);
          });
        } else {
          var body = self.getBodyHtml();
          self.doSubmit(body, callback);
        }
      },
      onSubmitLayout: function(callback) {
        if (self.isNewLayout) {
          self.$refs.form.validate() && self.submit(callback);
        } else {
          self.submit(callback);
        }
      },
      goBack: function() {
        location.href = Kooboo.Route.Get(Kooboo.Route.Layout.ListPage);
      },
      scanPositions: function() {
        PositionStore.clear();
        self.scanAttrPositions(kbFrame.getDocumentElement());
      },
      scanAttrPositions: function(node) {
        $("[" + positionKey + "]", node).each(function(ix, it) {
          var name = $(it).attr(positionKey),
            position;

          if (!PositionStore.byName(name)) {
            position = {
              id: newId(),
              name: name,
              elem: it,
              type: "attr"
            };
          } else {
            var newName = name,
              i = 1;
            while (PositionStore.byName(newName)) {
              newName = name + "_" + i;
              i++;
            }
            $(it).attr(positionKey, newName);
            position = {
              id: newId(),
              name: newName,
              elem: it,
              type: "attr"
            };
          }

          $(it).attr(omitTagKey) && (position.type = "ap/prepend");

          PositionStore.add(position);
        });
      },
      attrPosition: function(elem, name, loc, applyOmit) {
        switch (loc) {
          case "append":
            var appendBlock = $("<div>");
            $(appendBlock).attr(positionKey, name);
            applyOmit && $(appendBlock).attr(omitTagKey, "");
            $(appendBlock).insertAfter(elem);
            elem = appendBlock[0];
            break;
          case "prepend":
            var prependBlock = $("<div>");
            $(prependBlock).attr(positionKey, name);
            applyOmit && $(prependBlock).attr(omitTagKey, "");
            $(prependBlock).insertBefore(elem);
            elem = prependBlock[0];
            break;
          default:
            $(elem).attr(positionKey, name);
            applyOmit && $(elem).attr(omitTagKey, "");
            break;
        }

        helper.mask(elem);

        var data = {
          id: newId(),
          type: loc,
          elem: elem,
          name: name
        };

        PositionStore.add(data);
        return data;
      },
      updatePosition: function(id, nextName, applyOmit) {
        var pos = PositionStore.byId(id);

        if (pos) {
          $(pos.elem).attr(positionKey, nextName);

          if (applyOmit) {
            $(pos.elem).attr(omitTagKey, "");
          } else {
            pos.elem.hasAttribute(omitTagKey) &&
              pos.elem.removeAttribute(omitTagKey);
          }

          PositionStore.update(id, nextName);
        }

        return this;
      },
      scanBindings: function() {
        self._scanBindings(kbFrame.getDocumentElement());
      },
      _scanBindings: function(elem) {
        var children = elem.children,
          len = children.length,
          parsed = talParser.parse(elem),
          list = this.bindings,
          item;

        _.forEach(parsed, function(val, key) {
          if (val) {
            switch (key) {
              case "label":
                BindingStore.add({
                  id: newId(),
                  elem: elem,
                  type: key,
                  text: val
                });
                break;
              default:
                break;
            }
          }
        });

        for (var i = 0; i < len; i++) {
          self._scanBindings(children[i]);
        }
      }
    },
    computed: {
      isNewLayout: function() {
        return self.layoutId == Guid.Empty;
      },
      isContentChanged: function() {
        if (self.curType == "code") {
          return !_.isEqual(self._layoutCode, self.layoutCode);
        } else {
          return !_.isEqual(self._htmlContent, self.getHTML());
        }
      }
    }
  });
});
