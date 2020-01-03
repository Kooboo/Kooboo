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
        selected: [],
        defaultColumns: [],
        searchKey: "",
        selectedCategories: [],
        showCategoriesModal: false,
        categories: [],
        cacheData: null,
        isSearching: false,
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
      Kooboo.ProductCategory.getList().then(function(res) {
        if (res.success) {
          self.categories = res.model;
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
            self.cacheData = res.model;
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
          var type = _.find(self.productTypes, function(t) {
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

        if (data.list.length) {
          self.defaultColumns = Object.keys(data.list[0].values);
        }
      },
      onDelete: function() {
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var ids = self.selected.map(function(row) {
            return row.id;
          });
          Kooboo.Product.Deletes({
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
      },
      searchStart: function() {
        if (this.searchKey || this.selectedCategories.length) {
          self.isSearching = true;
          Kooboo.Product.search({
            categories: self.selectedCategories.map(function(item) {
              return item.id;
            }),
            keyword: self.searchKey || " "
          }).then(function(res) {
            if (res.success) {
              self.handleData(res.model);
              self.isSearching = true;
            }
          });
        } else {
          this.isSearching = false;
          self.handleData(this.cacheData);
        }
      },
      clearSearching: function() {
        this.searchKey = "";
        this.selectedCategories = [];
        this.isSearching = false;
        self.handleData(this.cacheData);
      },
      onShowCategoriesModal: function() {
        self.showCategoriesModal = true;
      },
      onHideCategoriesModal: function() {
        self.showCategoriesModal = false;
      },
      onSaveCategoriesModal: function() {
        self.selectedCategories = getSelected(self.categories);
        self.onHideCategoriesModal();
      }
    }
  });
  function getSelected(cates) {
    var temp = [];
    cates.forEach(function(c) {
      if (c.selected) {
        temp.push(c);
      }

      if (c.subCats && c.subCats.length) {
        temp = _.concat(temp, getSelected(c.subCats));
      }
    });
    return temp;
  }
  Vue.component("product-category", {
    template: "#category-template",
    props: {
      category: Object
    }
  });
});
