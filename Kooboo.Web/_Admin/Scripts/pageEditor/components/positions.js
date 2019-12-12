(function() {
  var template = Kooboo.getTemplate(
      "/_Admin/Scripts/pageEditor/components/positions.html"
    ),
    scaner = Kooboo.pageEditor.util.PositionScanner,
    ComponentStore = Kooboo.pageEditor.store.ComponentStore;

  Vue.component("kb-page-positions", {
    template: template,
    data: function() {
      return {
        positions: [],
        components: [],
        curPosition: null,
        container: null,
        context: null
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.EventBus.subscribe("fdoc/load", function(context) {
        var cachePostions = _.cloneDeep(self.positions);
        self.positions = [];
        self.container = context.container;
        self.context = context;
        var positions = scaner.scan(context.fdoc, context.container);
        _.forEach(Object.keys(positions), function(key) {
          var _find = _.find(cachePostions, function(cp) {
            return cp.name == key;
          });

          if (_find) {
            self.positions.push(_find);
          } else {
            var position = positions[key];
            position.contents = [];
            position.contents.onSorted = function(sortedList) {
              var self = this;
              Kooboo.EventBus.publish("kb/page/layout/component/sort", {
                context: self,
                list: sortedList
              });
              Kooboo.EventBus.publish("kb/page/field/change", {
                type: "resource"
              });
            };

            self.positions.push(position);
          }
        });

        _.forEach(context.positions, function(pos) {
          var _find = self.findPosition(pos.name);

          if (_find) {
            _find.contents = pos.contents;
          }
        });

        self.components = ComponentStore.getTypes();
        self.reload();
      });

      Kooboo.EventBus.publish("kb/pageeditor/component/position/ready");

      Kooboo.EventBus.subscribe("kb/page/layout/component/sort", function(
        data
      ) {
        var pos = _.find(self.positions, function(pos) {
          return pos.contents == data.context;
        });
        self.reload(pos);
      });

      Kooboo.EventBus.subscribe("kb/page/layout/components/save", function(
        components
      ) {
        var pos = self.curPosition;
        _.forEach(components, function(c) {
          pos.contents.push(c);
        });
        self.reload(pos);
        $(window).trigger("resize");
      });

      Kooboo.EventBus.subscribe("kb/page/layout/component/remask", function() {
        $(".kb-lighter").each(function(idx, lighter) {
          var node = $(lighter).data("node");
          if (!$(node).parent().length || !$(node).is(":visible")) {
            $(lighter).remove();
          }
        });
        _.forEach(self.positions, function(pos) {
          pos.resize();
        });
      });
    },
    methods: {
      remove: function(pos, id, item) {
        var self = this;
        pos.remove(id);
        pos.contents = pos.contents.filter(function(f) {
          return f.id != item.id;
        });
        Kooboo.EventBus.publish("kb/page/field/change", {
          type: "resource"
        });

        if (pos.type.toLowerCase() !== "layout") {
          var positions = [];
          _.forEach(self.positions, function(p) {
            if (p.layoutId && p.layoutId == id) {
              positions.push(p);
            }
          });
          if (positions.length) {
            _.forEach(positions, function(p) {
              _.forEach(p.contents, function(item) {
                self.remove(p, item.id, item);
              });
              self.positions = _.without(self.positions, p);
            });
          }
        }
      },
      showContent: function(m) {
        var self = this;
        var find = _.findLast(self.positions, function(pos) {
          return !!pos.fragments[m.id];
        });

        find &&
          $(find.elem.ownerDocument.body).animate(
            {
              scrollTop: find.elem.offsetTop
            },
            200
          );
      },
      chooseComponent: function(pos, type) {
        var self = this;
        self.curPosition = pos;
        Kooboo.Component.TagObjects({
          tag: type.tagName
        }).then(function(res) {
          if (res.success) {
            if (type.tagName.toLowerCase() == "layout") {
              var idx = _.findIndex(res.model, function(l) {
                return l.id == Kooboo.getQueryString("LayoutId");
              });

              if (idx > -1) {
                res.model.splice(idx, 1);
              }

              idx = _.findIndex(res.model, function(l) {
                return l.name == self.curPosition.layoutName;
              });

              if (idx > -1) {
                res.model.splice(idx, 1);
              }
            }
            Kooboo.EventBus.publish("kb/page/layout/component/select", {
              data: res.model,
              type: type.tagName,
              displayName: type.displayName,
              engine: type.requireEngine ? type.engineName : false
            });
          }
        });
      },
      findPosition: function(name) {
        var self = this;
        return _.find(self.positions, function(pos) {
          return pos.name == name;
        });
      },
      reload: function(pos) {
        var self = this;
        function reload(pos) {
          pos.fragments.hasInit = false;
          _.forEach(Object.keys(pos.holders), function(key) {
            pos.remove(key);
          });
          _.forEach(pos.contents, function(content) {
            if (content.html) {
              pos.setContent(content);

              if (content.type.toLowerCase() == "layout") {
                var layoutPosition = _.filter(self.positions, function(p) {
                  var ids = _.chain(pos.contents)
                    .filter(function(l) {
                      return l.type.toLowerCase() == "layout";
                    })
                    .map(function(l) {
                      return l.id;
                    })
                    .value();

                  return p.fromLayout && ids.indexOf(p.layoutId) > -1;
                });

                _.forEach(layoutPosition, function(p) {
                  var elem = $(pos.elem).find(
                    "[k-placeholder='" + p.name + "']"
                  );
                  if (elem.length) {
                    p.elem = elem[0];
                  }
                  reload(p);
                });
              }
            } else {
              if (ComponentStore.hasComponent(content.type, content.name)) {
                var component = ComponentStore.getComponent(
                  content.type,
                  content.name
                );
                content.id = Math.ceil(Math.random() * Math.pow(2, 53));
                content.html = component.html;
                pos.setContent(content);

                if (content.type.toLowerCase() == "layout") {
                  self.addNewLayoutPosition(pos, pos.elem, content);
                }
              } else {
                Kooboo.Component.getSource({
                  tag: content.type,
                  id: Kooboo.Guid.isValid(content.id)
                    ? content.id
                    : "" || content.name
                }).then(function(res) {
                  if (res.success) {
                    ComponentStore.addComponent({
                      id: content.id,
                      type: content.type,
                      name: content.name,
                      html: res.model.body || "",
                      metaBindings: res.model.metaBindings || [],
                      urlParamsBindings: res.model.urlParamsBindings || []
                    });

                    // content.id = Math.ceil(Math.random() * Math.pow(2, 53));
                    content.html = res.model.body || "";

                    pos.setContent(content);

                    if (content.type.toLowerCase() == "layout") {
                      setTimeout(function() {
                        self.addNewLayoutPosition(pos, pos.elem, content);
                      }, 100);
                    }
                  }
                });
              }
            }
          });
        }

        if (pos) {
          reload(pos);
        } else {
          _.forEach(self.positions, function(position) {
            reload(position);
          });
        }
        $(window).trigger("resize");
      },
      onSorted: function(sortedList) {
        var self = this;
        Kooboo.EventBus.publish("kb/page/layout/component/sort", {
          context: self,
          list: sortedList
        });
        Kooboo.EventBus.publish("kb/page/field/change", {
          type: "resource"
        });
      },
      addNewLayoutPosition: function(pos, node, component) {
        var self = this;
        var startIndex = _.findIndex(self.positions, pos);
        var positions = scaner.scan(node, self.container),
          list = [];
        _.forEach(positions, function(p) {
          p.contents = [];
          p.fromLayout = true;
          p.layoutName = component.name;
          p.layoutId = component.id;

          var _find = _.findLast(self.context.positions, function(pos) {
            if (pos.fromLayout) {
              return (
                pos.fromLayout == p.fromLayout &&
                pos.layoutName == p.layoutName &&
                pos.name == p.name
              );
            } else {
              return !p.fromLayout && pos.name == p.name;
            }
          });
          _find && (p.contents = _find.contents);
          list.push(p);
        });
        var _pos = _.cloneDeep(self.positions);
        _pos.splice(startIndex + 1, 0, list);
        self.positions = _.flatten(_pos);
        self.reload();
      }
    }
  });
})();
