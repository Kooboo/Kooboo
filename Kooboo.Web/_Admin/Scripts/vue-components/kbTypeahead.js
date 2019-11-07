(function() {
  var event = new Event("input", { bubbles: true });
  Vue.directive("kb-typeahead", {
    bind: main,
    update: main,
    unbind: function() {
      event = null;
    }
  });
  function main(element, binding) {
    var $element = $(element);
    var source = binding.value.source;
    var items = binding.value.items || 4;
    var showHintOnFocus = binding.value.defaultShow && true;

    //   var highlighter = function(item) {
    //     var matchSpan = '<span style="color: blue;font-weight:bold">';
    //     var query = this.query.replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, "\\$&");
    //     return item.replace(new RegExp("(" + query + ")", "ig"), function(
    //       $1,
    //       match
    //     ) {
    //       return matchSpan + match + "</span>";
    //     });
    //   };
    var options = {
      source: source,
      items: items,
      updater: function(item) {
        element.value = item;
        element.dispatchEvent(event);
        return item;
      },
      minLength: 0,
      showHintOnFocus: showHintOnFocus,
      afterSelect: function() {
        this.blur();
        this.hide();
        setTimeout(function() {
          $element.blur();
        });
      }
    };

    if (source && source.length) {
      $element.attr("autocomplete", "off").typeahead(options);
    }
  }
})();
