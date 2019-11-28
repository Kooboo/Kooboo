Vue.component("kb-page-parameters", {
  template: Kooboo.getTemplate(
    "/_Admin/Scripts/pageEditor/components/parameters.html"
  ),
  props: {
    parameters: Object
  },
  data: function() {
    return {
      defaultRouteValues: []
    };
  },
  mounted: function() {
    var self = this;
    if (self.parameters) {
      for (var name in self.parameters) {
        self.defaultRouteValues.push({
          name: name,
          value: self.parameters[name]
        });
      }
    }

    Kooboo.EventBus.subscribe("kb/page/save", function(res) {
      res["parameters"] = {};

      _.forEach(self.defaultRouteValues, function(drv) {
        res["parameters"][drv.name] = drv.value;
      });

      Kooboo.EventBus.publish("kb/page/final/save", res);
    });
  },
  methods: {
    removeDefaultRouteValue: function(routeValue) {
      this.defaultRouteValues = this.defaultRouteValues.filter(function(f) {
        return f != routeValue;
      });
      Kooboo.EventBus.publish("kb/page/field/change", {
        type: "url"
      });
    },
    addDefaultRouteValue: function() {
      this.defaultRouteValues.push({
        name: "",
        value: ""
      });
      Kooboo.EventBus.publish("kb/page/field/change", {
        type: "url"
      });
    }
  }
});
