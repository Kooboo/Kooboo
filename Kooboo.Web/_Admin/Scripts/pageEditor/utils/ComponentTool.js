(function() {
  var _tags = [],
    componentStore = Kooboo.pageEditor.store.ComponentStore;
  Kooboo.pageEditor.util.ComponentTool = {
    isNormalComponent: function(el) {
      if (!_tags.length) {
        _tags = getTags();
      }

      return _tags.indexOf(el.tagName.toLowerCase()) > -1;
    },
    isEnginedComponent: function(el) {
      return el.hasAttribute("engine");
    },
    getComponentTags: function() {
      if (!_tags.length) {
        _tags = getTags();
      }
      return _tags;
    }
  };

  function getTags() {
    if (!_tags.length) {
      var tags = [],
        types = componentStore.getTypes();

      types.forEach(function(type) {
        var selectorStr = type.tagName.toLowerCase();
        if (type.requireEngine) {
          selectorStr += "[engine=" + type.engineName + "]";
        }
        tags.push(selectorStr);
      });
      return tags;
    }
  }
})();
