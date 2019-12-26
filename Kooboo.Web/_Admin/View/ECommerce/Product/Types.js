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
            name: Kooboo.text.common.ProductTypes
          }
        ],
        newTypeUrl: Kooboo.Route.Product.Type.DetailPage,
        tableData: [],
        selected: []
      };
    },
    mounted: function() {
      Kooboo.ProductType.getList().then(function(res) {
        if (res.success) {
          self.tableData = res.model.map(function(item) {
            return {
              id: item.id,
              name: item.name,
              edit: {
                text: Kooboo.text.common.edit,
                url: Kooboo.Route.Get(Kooboo.Route.Product.Type.DetailPage, {
                  id: item.id
                })
              }
            };
          });
        }
      });
    },
    methods: {
      onDelete: function() {
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var ids = self.selected.map(function(row) {
            return row.id;
          });
          Kooboo.ProductType.Deletes({
            ids: JSON.stringify(ids)
          }).then(function(res) {
            if (res.success) {
              self.tableData = _.filter(self.tableData, function(row) {
                return ids.indexOf(row.id) === -1;
              });
              self.selected = [];
              window.info.show(Kooboo.text.info.delete.success, true);
            }
          });
        }
      }
    }
  });
});
