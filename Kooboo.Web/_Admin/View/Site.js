$(function() {
  new Vue({
    el: "#app",
    data: function() {
      return {
        widgets: []
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.Dashboard.getItems().then(function(res) {
        if (res.success) {
          res.model.forEach(function(item ,index ) {
            self.widgets.push(item);
          });
          self.$nextTick(function() {
            waterfall(".block-dashboard-stat");
            waterfall(".block-dashboard-stat"); // twice to fix bugs
          });
        }
      });
      $(window).on("resize.dashboard", function() {
        try {
          waterfall(".block-dashboard-stat");
        } catch (e) {
          // console.error(e);
        }
      });
    },
    beforeDestory: function() {
      $(window).off("resize.dashboard");
    }
  });
});
