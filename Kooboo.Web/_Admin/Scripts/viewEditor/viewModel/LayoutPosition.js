(function() {
  var LayoutStore = Kooboo.viewEditor.store.LayoutStore;
  var LayoutPosition = Vue.component("kb-view-layout", {
    data: function() {
      return {
        layoutList: [],
        positionList: [],
        layout: null,
        position: null
      };
    },
    created: function() {
      var self = this;
      Kooboo.EventBus.subscribe("LayoutStore/change", function() {
        self.layoutList = LayoutStore.getAll();
        if (
          !self.layout ||
          !_.some(self.layoutList, function(val) {
            self.layout.name === val.name;
          })
        ) {
          self.layout = self.layoutList[0];
        }
      });
    },
    methods: {
      changeLayout: function(name) {
        var layout = _.find(this.layoutList, function(it) {
          return it.name === name;
        });
        if (layout) {
          this.layout = layout;
        }
      }
    },
    watch: {
      layout: function(val) {
        Kooboo.EventBus.publish("layout/change", val);
      },
      position: function(val) {
        Kooboo.EventBus.publish("position/change", val);
      }
    }
  });

  Kooboo.viewEditor.viewModel.LayoutPosition = LayoutPosition;
})();
