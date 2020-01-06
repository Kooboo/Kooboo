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
            name: Kooboo.text.common.Orders
          }
        ],
        tableData: [],
        selected: [],
        newOrder: Kooboo.Route.Get(Kooboo.Route.Order.DetailPage),
        pager: {},
        searchKey: "",
        cacheData: null,
        isSearching: false
      };
    },
    mounted: function() {
      self.getList();
    },
    methods: {
      getList: function(page) {
        Kooboo.Order.getList({
          pageNr: page
        }).then(function(res) {
          if (res.success) {
            self.cacheData = res.model;
            self.handleData(res.model);
          }
        });
      },
      searchStart: function() {
        if (this.searchKey) {
          Kooboo.Order.search({
            keyword: self.searchKey
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
        this.isSearching = false;
        self.handleData(this.cacheData);
      },
      dataMapping: function(data) {
        return data.map(function(item) {
          var ob = {};
          ob.id = {
            text: item.id,
            url: Kooboo.Route.Get(Kooboo.Route.Order.DetailPage, {
              id: item.id
            })
          };
          ob.createDate = new Date(item.createDate).toDefaultLangString();
          ob.isPaid = {
            text: item.isPaid ? Kooboo.text.paid.yes : Kooboo.text.paid.no,
            class: item.isPaid
              ? "label-sm label-success"
              : "label-sm label-default"
          };
          ob.Edit = {
            text: Kooboo.text.common.edit,
            url: Kooboo.Route.Get(Kooboo.Route.Order.DetailPage, {
              id: item.id
            })
          };
          return ob;
        });
      },
      handleData: function(data) {
        self.pager = data;
        self.tableData = self.dataMapping(data.list);
      },
      onDelete: function() {
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var ids = self.selected.map(function(row) {
            return row.id;
          });
          Kooboo.Order.Deletes({
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
      changePage: function(page) {
        self.getList(page);
      }
    }
  });
});
