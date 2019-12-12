$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "MARKET"
          },
          {
            name: Kooboo.text.common.Hardwares,
            url: Kooboo.Route.Hardware.ListPage
          },
          {
            name: Kooboo.text.market.supplier.myOrders
          }
        ],
        pager: {},
        showHardwareModal: false,
        hardwareData: null,
        tableData: []
      };
    },
    mounted: function() {
      var self = this;
      this.getList();
      Kooboo.EventBus.subscribe("kb/market/cashier/done", function() {
        setTimeout(function() {
          self.getList();
        }, 200);
      });
    },
    methods: {
      changePage: function(page) {
        self.getList(page);
      },
      getList: function() {
        Kooboo.Infrastructure.getMyOrders().then(function(res) {
          if (res.success) {
            self.handleData(res.model);
          }
        });
      },
      handleData: function(data) {
        self.pager = data;
        self.tableData = data.list.map(function(doc) {
          return {
            id: doc.salesItemId,
            name: doc.displayName,

            type: {
              text: doc.type,
              class: "label-sm label-info"
            },
            amount: {
              text: doc.amount,
              class: "label-sm label-info"
            },
            startTime: {
              text: new Date(doc.startTime).toDefaultLangString(),
              class: "label-sm green"
            },
            endMonth: {
              text: doc.endMonth,
              class: "label-sm green"
            },
            renew: {
              text: Kooboo.text.common.renew,
              url: "kb/market/hardware/orders/renew"
            }
          };
        });
      },
      renew: function(id) {
        Kooboo.Infrastructure.getSalesItem({
          id: id
        }).then(function(res) {
          if (res.success) {
            self.hardwareData = res.model;
            self.showHardwareModal = true;
          }
        });
      }
    }
  });
});
