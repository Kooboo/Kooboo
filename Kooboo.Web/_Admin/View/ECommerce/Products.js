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
            name: Kooboo.text.common.ProductManagement
          }
        ],
        productTypes: [],
        pager: {},
        tableData: [],
        selected: []
      };
    },
    mounted: function() {
      Kooboo.ProductType.getList().then(function(res) {
        if (res.success) {
          self.productTypes = res.model.map(function(t) {
            return {
              id: t.id,
              name: t.name
            };
          });
          self.getListByPage();
        }
      });
    },
    methods: {
      getListByPage: function(page) {
        Kooboo.Product.getList({
          page: page || 1
        }).then(function(res) {
          if (res.success) {
            self.handleData(res.model);
          }
        });
      },
      changePage: function(page) {
        self.getListByPage(page);
      },
      handleData: function(data) {
        self.pager = data;
        self.tableData = data.list.map(function(d) {
          var date = new Date(d.lastModified);
          var type = _.find(self.productTypes(), function(t) {
            return t.id == d.productTypeId;
          });

          return _.assign(
            {
              id: d.id,
              lastModified: date.toDefaultLangString(),
              online: d.online,
              productType: {
                text: type ? type.name : "",
                class: "label-sm " + (type ? "blue" : "label-default")
              },
              edit: {
                text: Kooboo.text.common.edit,
                url: Kooboo.Route.Get(Kooboo.Route.Product.DetailPage, {
                  id: d.id,
                  type: d.productTypeId
                })
              }
            },
            d.values
          );
        });
        // TODO
        // var columns = [];
        // if (data.list.length) {
        //   var keys = Object.keys(data.list[0].values);
        //   keys.forEach(function(key) {
        //     columns.push({
        //       displayName: key,
        //       fieldName: key,
        //       type: "text"
        //     });
        //   });

        //   columns.push({
        //     displayName: Kooboo.text.common.ProductType,
        //     fieldName: "productType",
        //     type: "label"
        //   });
        // }

        // self.tableData({
        //   docs: docs,
        //   columns: columns,
        //   tableActions: [
        //     {
        //       fieldName: "edit",
        //       type: "link-btn"
        //     }
        //   ],
        //   kbType: Kooboo.Product.name,
        //   class: "table-bordered"
        // });
      },
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
