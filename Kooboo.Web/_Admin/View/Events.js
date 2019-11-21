$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.Events
          }
        ],
        tableData: []
      };
    },
    mounted: function() {
      Kooboo.BusinessRule.getList().then(function(res) {
        if (res.success) {
          var docs = res.model.map(function(item) {
            return {
              type: item.name,
              count: {
                text: item.count,
                class: "badge-sm blue"
              },
              edit: {
                text: Kooboo.text.common.edit,
                url: Kooboo.Route.Get(Kooboo.Route.Event.DetailPage, {
                  name: item.name
                })
              }
            };
          });
          self.tableData = docs;
        }
      });
    }
  });
});
