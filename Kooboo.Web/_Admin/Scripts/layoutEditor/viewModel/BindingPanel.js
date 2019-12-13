(function() {
  var Label =
      Kooboo[Kooboo.layoutEditor ? "layoutEditor" : "pageEditor"].viewModel
        .Label,
    Script =
      Kooboo[Kooboo.layoutEditor ? "layoutEditor" : "pageEditor"].viewModel
        .Script,
    Style =
      Kooboo[Kooboo.layoutEditor ? "layoutEditor" : "pageEditor"].viewModel
        .Style,
    Position =
      Kooboo[Kooboo.layoutEditor ? "layoutEditor" : "pageEditor"].viewModel
        .Position,
    PositionStore =
      Kooboo[Kooboo.layoutEditor ? "layoutEditor" : "pageEditor"].store
        .PositionStore,
    BindingStore =
      Kooboo[Kooboo.layoutEditor ? "layoutEditor" : "pageEditor"].store
        .BindingStore;

  var positionKey = "k-placeholder";

  function newId() {
    return Math.ceil(Math.random() * Math.pow(2, 53));
  }

  function getAllPositions() {
    return PositionStore.getAll().map(function(it) {
      return new Position(it);
    });
  }

  function getAllLabels() {
    var list = BindingStore.getAll(),
      ret = [];
    _.forEach(list, function(it) {
      if (it.type == "label") {
        ret.push(new Label(it));
      }
    });
    return ret;
  }

  function getAllStyles() {
    return _.filter(BindingStore.getAll(), function(style) {
      return style.type === "style" && style.text !== "";
    });
  }

  function publishSortEventToKbFrame(type, context) {
    Kooboo.EventBus.publish("kb/frame/resource/sort", {
      type: type,
      targetIdx: context.targetIdx,
      elem: context.elem,
      list: context.list
    });
  }

  var self;
  var BindingPanel = Vue.component("kb-layout-panel", {
    props: {
      elem: HTMLElement,
      styleResource: {},
      scriptResource: {}
    },
    data: function() {
      self = this;
      return {
        positionList: getAllPositions(),
        labelList: [],
        styleList: [],
        headScriptList: [],
        bodyScriptList: [],
        existList: []
      };
    },
    created: function() {
      Kooboo.EventBus.subscribe("PositionStore/change", function() {
        self.positionList = getAllPositions();
        self.refreshExistList();
      });

      Kooboo.EventBus.subscribe("BindingStore/change", function() {
        self.labelList = getAllLabels();
        self.headScriptList = _.filter(BindingStore.getAll(), {
          type: "script",
          head: true
        });
        self.bodyScriptList = _.filter(BindingStore.getAll(), {
          type: "script",
          head: false
        });
        self.styleList = getAllStyles();
        self.refreshExistList();
      });
    },
    watch: {
      elem: function() {
        self.refreshExistList();
      }
    },
    methods: {
      refreshExistList: function() {
        var elem = self.elem;
        self.existList = [];
        function cb(item) {
          return item.elem == elem;
        }
        if (elem) {
          var list = [
            _.find(self.positionList, cb),
            _.find(self.labelList, cb)
          ];
          _.forEach(list, function(it) {
            it && self.existList.push(it);
          });
        }
      },
      createLabel: function() {
        Kooboo.EventBus.publish("binding/edit", {
          type: "label",
          elem: self.elem
        });
      },
      editLabel: function(item) {
        self.elem = item.elem;
        Kooboo.EventBus.publish("binding/edit", {
          id: item.id,
          elem: item.elem,
          type: item.type,
          text: item.text
        });
      },
      removeLabel: function(item) {
        Kooboo.EventBus.publish("binding/remove", {
          id: item.id
        });

        Kooboo.EventBus.publish("kb/frame/dom/update");
      },
      createScript: function(isHead) {
        var _choosenScriptList = _.concat(
            self.headScriptList,
            self.bodyScriptList
          ),
          _resources = _.cloneDeep(self.scriptResource);

        ["scripts", "scriptGroup"].map(function(key) {
          var _choosenScripts = [];

          _choosenScriptList.map(function(script) {
            _choosenScripts.push(script.name);
          });
          _choosenScripts = _.compact(_choosenScripts);

          var _filterScripts = _.remove(_resources[key], function(script) {
            return _choosenScripts.indexOf(script.text) == -1;
          });

          _resources[key] = _filterScripts;
        });

        Kooboo.EventBus.publish("binding/edit", {
          type: "script",
          elem: self.elem,
          resources: _resources,
          isHead: isHead
        });
      },
      createStyle: function() {
        var self = this;

        var _resources = _.cloneDeep(self.styleResource);

        ["styles", "styleGroup"].map(function(key) {
          var _choosenStyleList = [];

          self.styleList.map(function(style) {
            _choosenStyleList.push(style.name);
          });
          _choosenStyleList = _.compact(_choosenStyleList);

          var _filterStyles = _.remove(_resources[key], function(style) {
            return _choosenStyleList.indexOf(style.text) == -1;
          });

          _filterStyles = _.remove(_filterStyles, function(style) {
            return !_.isEmpty(style.text);
          });

          _resources[key] = _filterStyles;
        });

        Kooboo.EventBus.publish("binding/edit", {
          type: "style",
          elem: self.elem,
          resources: _resources
        });
      },
      editJsCss: function(item) {
        self.elem = item.elem;
        Kooboo.EventBus.publish("binding/edit", {
          id: item.id,
          elem: item.elem,
          type: item.type,
          text: item.text
        });
      },
      removeScript: function(item) {
        item &&
          item.elem &&
          item.elem.tagName === "SCRIPT" &&
          $(item.elem).remove();
        Kooboo.EventBus.publish("binding/remove", {
          id: item.id
        });
        Kooboo.EventBus.publish("kb/page/field/change", {
          type: "resource"
        });
        Kooboo.EventBus.publish("kb/frame/resource/remove", {
          type: "script",
          tag: item.elem
        });
      },
      removeStyle: function(item) {
        item &&
          item.elem &&
          ((item.elem.tagName === "LINK" &&
            item.elem.getAttribute("rel") === "stylesheet") ||
            item.elem.tagName === "STYLE") &&
          $(item.elem).remove();
        Kooboo.EventBus.publish("binding/remove", {
          id: item.id
        });
        Kooboo.EventBus.publish("kb/page/field/change", {
          type: "resource"
        });
        Kooboo.EventBus.publish("kb/frame/resource/remove", {
          type: "style",
          tag: item.elem
        });
      },
      convert: function() {
        var elem = self.elem;
        var foundItem = _.find(this.labelList, function(it) {
          if (it.elem != elem && elem.contains(it.elem)) {
            return it;
          }
        });
        if (
          !foundItem ||
          confirm(Kooboo.text.confirm.layoutEditor.labelInside)
        ) {
          foundItem && self.removeLabel(foundItem);
          Kooboo.EventBus.publish("position/edit", {
            elem: elem,
            type: "attr"
          });
        }
      },
      prepend: function() {
        Kooboo.EventBus.publish("position/edit", {
          elem: self.elem,
          type: "prepend"
        });
      },
      append: function() {
        Kooboo.EventBus.publish("position/edit", {
          elem: self.elem,
          type: "append"
        });
      },
      edit: function(item) {
        var result = {
          id: item.id,
          elem: item.elem,
          type: item.type
        };
        if (item.hasOwnProperty("name")) {
          result.name = item.name;
        } else {
          result.text = item.text;
        }
        Kooboo.EventBus.publish("position/edit", result);
      },
      remove: function(item) {
        Kooboo.EventBus.publish("position:remove", {
          id: item.id,
          elem: item.elem
        });
        Kooboo.EventBus.publish("kb/frame/layout/resource/update");
      },
      focusPosition: function(item) {
        Kooboo.EventBus.publish("position:focus", {
          id: item.id,
          elem: item.elem,
          name: item.name,
          type: item.type
        });
      },
      headScriptSorted: function(data) {
        var self = this;
        var index = data.targetIndex;
        publishSortEventToKbFrame("script", {
          targetIdx: index,
          elem: self.headScriptList[index].elem,
          list: self.headScriptList
        });
      },
      bodyScriptSorted: function(data) {
        var self = this;
        var index = data.targetIndex;
        publishSortEventToKbFrame("script", {
          targetIdx: index,
          elem: self.bodyScriptList[index].elem,
          list: self.bodyScriptList
        });
      },
      styleSorted: function(data) {
        var self = this;
        var index = data.targetIndex;
        publishSortEventToKbFrame("style", {
          targetIdx: index,
          elem: self.styleList[index].elem,
          list: self.styleList
        });
      }
    },
    computed: {
      showConvert: function() {
        var elem = self.elem;
        return (
          elem &&
          elem.tagName &&
          elem.tagName.toLowerCase() != positionKey &&
          !_.some(self.positionList, function(it) {
            var el = it.placeholder || it.elem;
            return el.contains(elem) || elem.contains(el);
          }) &&
          !_.some(self.labelList, function(it) {
            return it.elem.contains(elem);
          }) &&
          elem !== elem.ownerDocument.body
        );
      },
      showPrepend: function() {
        var elem = self.elem;
        return (
          elem &&
          elem.tagName &&
          elem.tagName.toLowerCase() != positionKey &&
          !_.find(self.labelList, function(it) {
            return it.elem.contains(elem);
          }) &&
          elem !== elem.ownerDocument.body
        );
      },
      showAppend: function() {
        var elem = self.elem;
        return (
          elem &&
          elem.tagName &&
          elem.tagName.toLowerCase() != positionKey &&
          !_.some(self.labelList, function(it) {
            return it.elem.contains(elem);
          }) &&
          elem !== elem.ownerDocument.body
        );
      },
      showLabel: function() {
        var elem = self.elem;
        return (
          elem &&
          elem.tagName &&
          elem.tagName.toLowerCase() != positionKey &&
          !_.some(self.positionList, function(it) {
            return (it.placeholder || it.elem).contains(elem);
          }) &&
          !_.some(self.labelList, function(it) {
            return elem.contains(it.elem) || it.elem.contains(elem);
          }) &&
          elem !== elem.ownerDocument.body &&
          !$("[" + positionKey + "]", $(elem)).length
        );
      },
      resources: function() {
        var _res = {};
        _.forEach(Object.keys(self.styleResource), function(key) {
          _res[key] = self.styleResource[key];
        });
        _.forEach(Object.keys(self.scriptResource), function(key) {
          _res[key] = self.scriptResource[key];
        });
        return _res;
      },
      existLabels: function() {
        return self.existList.filter(function(item) {
          return item.type == "label";
        });
      },
      existOthers: function() {
        return self.existList.filter(function(item) {
          return (
            item.type == "attr" ||
            item.type == "append" ||
            item.type == "prepend" ||
            item.type == "ap/prepend"
          );
        });
      }
    }
  });

  if (Kooboo.layoutEditor) {
    Kooboo.layoutEditor.viewModel.BindingPanel = BindingPanel;
  }

  if (Kooboo.pageEditor) {
    Kooboo.pageEditor.viewModel.BindingPanel = BindingPanel;
  }
})();
