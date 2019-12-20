(function() {
  var Label = Kooboo.viewEditor.viewModel.Label,
    Data = Kooboo.viewEditor.viewModel.Data,
    Attribute = Kooboo.viewEditor.viewModel.Attribute,
    Repeat = Kooboo.viewEditor.viewModel.Repeat,
    Link = Kooboo.viewEditor.viewModel.Link,
    Form = Kooboo.viewEditor.viewModel.Form,
    Input = Kooboo.viewEditor.viewModel.Input,
    Condition = Kooboo.viewEditor.viewModel.Condition,
    ActionStore = Kooboo.viewEditor.store.ActionStore,
    BindingStore = Kooboo.viewEditor.store.BindingStore,
    DataStore = Kooboo.viewEditor.store.DataStore,
    kTag = Kooboo.viewEditor.util.k2attrTag,
    DataContext = Kooboo.viewEditor.DataContext;
  var ctors = {
    label: Label,
    data: Data,
    attribute: Attribute,
    link: Link,
    repeat: Repeat,
    form: Form,
    input: Input,
    condition: Condition
  };
  var _createProxy = {
    label: function(elem) {
      Kooboo.EventBus.publish("binding/edit", {
        bindingType: "label",
        elem: elem,
        text: ""
      });
    },
    data: function(elem) {
      Kooboo.EventBus.publish("binding/edit", {
        bindingType: "data",
        elem: elem,
        text: ""
      });
    },
    attribute: function(elem) {
      var attributes = {};
      if (!_.isEmpty($(elem).attr("k-attributes"))) {
        var rawAttributes = $(elem).attr("k-attributes");
        var _pairs = _.filter(rawAttributes.split(";"), function(p) {
          return !_.isEmpty(p);
        });
        _.forEach(_pairs, function(pair) {
          var _key = pair.split(" ")[0];
          var _val = pair.split(" ")[1];
          attributes[_key] = _val;
        });
      }
      Kooboo.EventBus.publish("binding/edit", {
        bindingType: "attribute",
        elem: elem,
        attributes: _.isEmpty(attributes) ? null : attributes
      });
    },
    repeat: function(elem) {
      Kooboo.EventBus.publish("binding/edit", {
        bindingType: "repeat",
        elem: elem,
        text: "",
        repeatSelf: false
      });
    },
    link: function(elem) {
      Kooboo.EventBus.publish("binding/edit", {
        bindingType: "link",
        elem: elem,
        href: elem.getAttribute("href"),
        params: {}
      });
    },
    form: function(elem) {
      Kooboo.EventBus.publish("binding/edit", {
        bindingType: "form",
        elem: elem
      });
    },
    input: function(elem) {
      Kooboo.EventBus.publish("binding/edit", {
        bindingType: "input",
        elem: elem,
        text: ""
      });
    },
    condition: function(elem) {
      Kooboo.EventBus.publish("binding/edit", {
        bindingType: "condition",
        elem: elem,
        text: ""
      });
    }
  };
  var _editProxy = {
    label: function(item) {
      Kooboo.EventBus.publish("binding/edit", {
        elem: item.elem,
        bindingType: "label",
        text: item.text
      });
    },
    data: function(item) {
      Kooboo.EventBus.publish("binding/edit", {
        elem: item.elem,
        bindingType: "data",
        text: item.text
      });
    },
    attribute: function(item) {
      var elem = item.elem,
        attributes = {};
      if (!_.isEmpty($(elem).attr("k-attributes"))) {
        var rawAttributes = $(elem).attr("k-attributes");
        var _pairs = _.filter(rawAttributes.split(";"), function(p) {
          return !_.isEmpty(p);
        });
        _.forEach(_pairs, function(pair) {
          var _tempArr = pair.split(" ");
          var _key = _tempArr.splice(0, 1)[0];
          var _val = _tempArr.join(" ");
          attributes[_key] = _val;
        });
      }
      Kooboo.EventBus.publish("binding/edit", {
        bindingType: "attribute",
        elem: elem,
        attributes: _.isEmpty(attributes) ? null : attributes
      });
    },
    repeat: function(item) {
      Kooboo.EventBus.publish("binding/edit", {
        elem: item.elem,
        bindingType: "repeat",
        text: item.text,
        repeatSelf: item.repeatSelf
      });
    },
    link: function(item) {
      Kooboo.EventBus.publish("binding/edit", {
        elem: item.elem,
        bindingType: "link",
        href: item.href,
        params: item.params,
        page: item.page
      });
    },
    form: function(item) {
      Kooboo.EventBus.publish("binding/edit", {
        elem: item.elem,
        bindingType: "form",
        dataSourceMethodId: item.dataSourceMethodId,
        dataSourceMethodDisplay: item.dataSourceMethodDisplay,
        method: item.method,
        redirect: item.redirect,
        callback: item.callback
      });
    },
    input: function(item) {
      Kooboo.EventBus.publish("binding/edit", {
        elem: item.elem,
        bindingType: "input",
        text: item.text
      });
    },
    condition: function(item) {
      Kooboo.EventBus.publish("binding/edit", {
        elem: item.elem,
        bindingType: "condition",
        text: item.text
      });
    }
  };
  var _updateProxy = {
    label: function(item, data) {
      item.text = data.text;
    },
    data: function(item, data) {
      item.text = data.text;
    },
    attribute: function(item, data) {
      item.text = data.text;
    },
    repeat: function(item, data) {
      item.text = data.text;
      item.repeatSelf = data.repeatSelf;
    },
    link: function(item, data) {
      item.href = data.href;
      item.params = data.params;
      item.page = data.page;
    },
    form: function(item, data) {
      item.dataSourceMethodId = data.dataSourceMethodId;
      item.method = data.method;
      item.redirect = data.redirect;
      item.callback = data.callback;
    },
    input: function(item, data) {
      item.text = data.text;
    },
    condition: function(item, data) {
      item.text = data.text;
    }
  };
  function isInput(elem) {
    return $(elem).is(":text,textarea,select,:radio,:checkbox");
  }
  function isElemInsideDynamicContent(elem) {
    if (
      $(elem).parents(kTag["label"]).length ||
      $(elem).parents(kTag["data"]).length
    ) {
      return true;
    }
  }
  function isSameNodeFn(elem) {
    return function(it) {
      return it.elem.isSameNode(elem);
    };
  }

  var self;
  var BindingPanel = Vue.component("kb-view-panel", {
    props: {
      elem: HTMLElement
    },
    data: function() {
      self = this;
      return {
        const_external_link: "__external_link",
        labelList: [],
        dataList: [],
        attributeList: [],
        repeatList: [],
        linkList: [],
        formList: [],
        inputList: [],
        existList: [],
        conditionList: []
      };
    },
    created: function() {
      Kooboo.EventBus.subscribe("DataStore/removed", function(removed) {
        if (confirm(Kooboo.text.confirm.removeBinding)) {
          _.forEach(removed, function(r) {
            self.removeBindings(
              _.cloneDeep(BindingStore.getRemoveBindingInfos(r.id))
            );
            $(window).trigger("resize");
          });
        }
      });
    },
    methods: {
      refreshExistList: function() {
        var elem = self.elem;
        self.existList = [];
        if (elem) {
          var list = [
            _.find(self.labelList, isSameNodeFn(elem)),
            _.find(self.dataList, isSameNodeFn(elem)),
            _.find(self.attributeList, isSameNodeFn(elem)),
            _.find(self.repeatList, isSameNodeFn(elem)),
            _.find(self.linkList, isSameNodeFn(elem)),
            _.find(self.formList, isSameNodeFn(elem)),
            _.find(self.inputList, isSameNodeFn(elem)),
            _.find(self.existList, isSameNodeFn(elem)),
            _.find(self.conditionList, isSameNodeFn(elem))
          ];
          _.compact(list).forEach(function(li) {
            self.existList.push(li);
          });
        }
      },
      create: function(bindingType, ctx) {
        var elem = self.elem;
        elem && _createProxy[bindingType](elem);
      },
      add: function(data) {
        var item = new ctors[data.bindingType](data.elem, data);
        self[data.bindingType + "List"].push(item);
        BindingStore.addOrUpdateBindingInfo(data);
        self.refreshExistList();
      },
      get: function(elem, bindingType) {
        return _.find(self[bindingType + "List"], isSameNodeFn(elem));
      },
      getAll: function() {
        return _.concat(
          self.labelList(),
          self.dataList(),
          self.attributeList(),
          self.linkList(),
          self.repeatList(),
          self.formList(),
          self.inputList(),
          self.conditionList()
        );
      },
      remove: function(item) {
        var _find = _.findLastIndex(self[item.type + "List"], function(i) {
          return i.elem == item.elem;
        });

        if (_find > -1) {
          self[item.type + "List"].splice(_find, 1);
          self.onRemove({
            elem: item.elem,
            bindingType: item.type
          });
          BindingStore.removeBindingInfo(item);
          self.refreshExistList();
        }
      },
      onRemove: function(binding) {
        // override by outside
      },
      removeBindings: function(bindings) {
        if (bindings && bindings.length) {
          bindings.forEach(function(binding) {
            !binding.type && (binding.type = binding.bindingType);
            var _find = _.findLastIndex(self[binding.type + "List"], function(
              i
            ) {
              return i.elem == binding.elem;
            });

            if (_find > -1) {
              self[binding.type + "List"].splice(_find, 1);
              self.onRemove({
                elem: binding.elem,
                bindingType: binding.type
              });
              BindingStore.removeBindingInfo(binding);
            }
          });
        }
        self.refreshExistList();
      },
      edit: function(item) {
        _editProxy[item.type](item);
      },
      update: function(data) {
        var list = self[data.bindingType + "List"],
          inst = _.find(list, isSameNodeFn(data.elem));
        inst && _updateProxy[data.bindingType](inst, data);
        BindingStore.addOrUpdateBindingInfo(data);
      },
      reset: function() {
        self.labelList = [];
        self.dataList = [];
        self.attributeList = [];
        self.repeatList = [];
        self.linkList = [];
        self.formList = [];
        self.inputList = [];
        self.conditionList = [];
        self.elem = null;
      }
    },
    watch: {
      elem: function(elem) {
        self.refreshExistList();
      }
    },
    computed: {
      showLabel: function() {
        var elem = self.elem,
          unableToLabelTags = ["input", "img", "br", "hr"];
        return (
          elem &&
          !isInput(elem) &&
          !_.some(self.labelList, isSameNodeFn(elem)) &&
          !_.some(self.dataList, isSameNodeFn(elem)) &&
          !_.some(self.repeatList, isSameNodeFn(elem)) &&
          unableToLabelTags.indexOf(elem.tagName.toLowerCase()) == -1 &&
          !isElemInsideDynamicContent(elem) &&
          !elem.hasAttribute("k-placeholder")
        );
      },
      showLink: function() {
        var elem = self.elem;
        return (
          elem &&
          elem.tagName.toLowerCase() == "a" &&
          !_.some(self.linkList, isSameNodeFn(elem)) &&
          !isElemInsideDynamicContent(elem) &&
          !elem.hasAttribute("k-placeholder")
        );
      },
      showData: function() {
        var elem = self.elem,
          unableToSetDataTag = ["input", "img", "br", "hr"];
        return (
          elem &&
          !_.some(self.labelList, isSameNodeFn(elem)) &&
          !_.some(self.dataList, isSameNodeFn(elem)) &&
          unableToSetDataTag.indexOf(elem.tagName.toLowerCase()) == -1 &&
          !isElemInsideDynamicContent(elem) &&
          !elem.hasAttribute("k-placeholder")
        );
      },
      showAttribute: function() {
        var elem = self.elem;
        return (
          elem &&
          !_.some(self.labelList, isSameNodeFn(elem)) &&
          !_.some(self.attributeList, isSameNodeFn(elem)) &&
          !isElemInsideDynamicContent(elem) &&
          !elem.hasAttribute("k-placeholder")
        );
      },
      showRepeat: function() {
        var elem = self.elem,
          unableToSetRepeatTag = ["input", "img"];
        return (
          elem &&
          !_.some(self.labelList, isSameNodeFn(elem)) &&
          !_.some(self.dataList, isSameNodeFn(elem)) &&
          !_.some(self.repeatList, isSameNodeFn(elem)) &&
          unableToSetRepeatTag.indexOf(elem.tagName.toLowerCase()) == -1 &&
          !isElemInsideDynamicContent(elem) &&
          !elem.hasAttribute("k-placeholder")
        );
      },
      showForm: function() {
        var elem = self.elem;
        return (
          elem &&
          elem.tagName.toLowerCase() == "form" &&
          !_.some(self.formList, isSameNodeFn(elem)) &&
          !elem.hasAttribute("k-placeholder")
        );
      },
      showInput: function() {
        var elem = self.elem;
        return (
          elem &&
          isInput(elem) &&
          !_.some(self.inputList, isSameNodeFn(elem)) &&
          !elem.hasAttribute("k-placeholder")
        );
      },
      showCondition: function() {
        var elem = self.elem;
        return (
          elem &&
          !_.some(self.labelList, isSameNodeFn(elem)) &&
          !_.some(self.conditionList, isSameNodeFn(elem)) &&
          !isElemInsideDynamicContent(elem) &&
          !elem.hasAttribute("k-placeholder")
        );
      },
      showBindingBtns: function() {
        return (
          self.showAttribute ||
          self.showData ||
          self.showForm ||
          self.showInput ||
          self.showLink ||
          self.showRepeat ||
          self.showCondition
        );
      }
    }
  });
  Kooboo.viewEditor.viewModel.BindingPanel = BindingPanel;
})();
