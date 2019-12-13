(function() {
  Vue.component("kb-multilang-selector", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/kbMultilangSelector.html"
    ),
    /*
     *  cultures: {
     *      aaet: "Afar (Ethiopia)",
     *      enus: "English"
     *  },
     *  defaultCulture: "enus"，
     *  selected: []
     */
    props: {
      cultures: Object,
      defaultCulture: String
      // ,selected: Array
    },
    data: function() {
      return {
        innerCultures: []
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.EventBus.subscribe("kb/multilang/change", function(target) {
        var lang = _.findLast(self.innerCultures, function(lang) {
          return lang.key == target.name;
        });
        lang && (lang.selected = target.selected);
      });

      Kooboo.EventBus.subscribe("kb/multilang/get", function() {
        _.forEach(self.innerCultures, function(cul) {
          cul.selected &&
            Kooboo.EventBus.publish("kb/multilang/change", {
              name: cul.key,
              fullName: cul.value,
              selected: true
            });
        });
      });
    },
    methods: {
      changeSelected(culture) {
        var self = this;
        /* publish language-change event */
        var _cultrue = {
          name: culture.key,
          fullName: culture.value,
          selected: culture.selected
        };
        Kooboo.EventBus.publish("kb/multilang/change", _cultrue);
        self.$emit("change", _cultrue);
        // if (self.selected) {
        //   var selectedCultures = _.clone(self.selected);
        //   if (selectedCultures.length == 0) {
        //     selectedCultures = [self.defaultCulture];
        //   }
        //   if (selectedCultures.indexOf(culture.key) !== -1) {
        //     if (!culture.selected) {
        //       selectedCultures = _.without(selectedCultures, culture.key);
        //     }
        //   } else {
        //     if (culture.selected) {
        //       selectedCultures.push(culture.key);
        //     }
        //   }
        //   self.$emit("update:selected", selectedCultures);
        // }
      },
      formatCultures() {
        var self = this;
        var _culturesArr = Kooboo.objToArr(self.cultures) || [],
          _cultures = [];
        var defaultCultureIdx = _.findIndex(_culturesArr, function(c) {
          return c.key == self.defaultCulture;
        });
        if (defaultCultureIdx > -1) {
          var culture = _culturesArr[defaultCultureIdx];
          culture.selected = true;
          _cultures.push(culture);
          _culturesArr.splice(defaultCultureIdx, 1);
        }
        _.forEach(_culturesArr, function(culture) {
          culture.selected = false; // !!(self.selected && self.selected.indexOf(culture.key) !== -1);
          _cultures.push(culture);
        });
        return _cultures;
      }
    },
    watch: {
      cultures: {
        handler: function() {
          this.innerCultures = this.formatCultures();
        },
        immediate: true
      }
    }
  });
})();
