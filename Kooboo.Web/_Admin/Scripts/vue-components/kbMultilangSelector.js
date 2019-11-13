(function() {
  var self;
  Vue.component("kb-multilang-selector", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/kbMultilangSelector.html"
    ),
    /*
     *  cultures: {
     *      aaet: "Afar (Ethiopia)",
     *      enus: "English"
     *  },
     *  default: "enus",
     */
    props: {
      cultures: Object,
      default: String,
      selected: Array
    },
    data: function() {
      self = this;
      return {};
    },
    mounted: function() {
      Kooboo.EventBus.subscribe("kb/multilang/change", function(target) {
        var lang = _.findLast(self.cultures(), function(lang) {
          return lang.key() == target.name;
        });

        lang && lang.selected(target.selected);
      });

      Kooboo.EventBus.subscribe("kb/multilang/get", function() {
        _.forEach(self.cultures(), function(cul) {
          cul.selected() &&
            Kooboo.EventBus.publish("kb/multilang/change", {
              name: cul.key(),
              fullName: cul.value(),
              selected: true
            });
        });
      });
    },
    computed: {
      formatCultures() {
        var _culturesArr = Kooboo.objToArr(self.cultures) || [],
          _cultures = [];
        var defaultCultureIdx = _.findIndex(_culturesArr, function(c) {
          return c.key == self.default;
        });
        if (defaultCultureIdx > -1) {
          _cultures.push(_culturesArr[defaultCultureIdx]);
          _culturesArr.splice(defaultCultureIdx, 1);
        }
        _.forEach(_culturesArr, function(culture) {
          culture.selected = culture.key === self.default;
          _cultures.push(culture);
        });
        return _cultures;
      }
    }
  });

  var x = {
    viewModel: function(params) {
      /*
       * params:{
       *  cultures: {
       *      aaet: "Afar (Ethiopia)",
       *      enus: "English"
       *  },
       *  default: "enus",
       *  defaultName: "English"
       * }
       */

      var self = this;

      this.cultures = ko.observableArray(_cultures);
    },
    template: template
  };

  var cultureModel = function(cul, culList) {
    var self = this;

    ko.mapping.fromJS(cul, {}, self);

    self.selected = ko.observable(self.key() == culList.defaultCulture());
    self.selected.subscribe(function(selected) {
      /* publish language-change event */
      Kooboo.EventBus.publish("kb/multilang/change", {
        name: self.key(),
        fullName: self.value(),
        selected: selected
      });
    });
  };
})();
