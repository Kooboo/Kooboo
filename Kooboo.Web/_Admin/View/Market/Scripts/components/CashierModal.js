(function() {
  var PAYMENT_METHODS = [];

  var PAYMENT_SUCCESS = false;
  var infoShowed = false;
  Kooboo.loadJS(["/_Admin/Scripts/lib/jquery.qrcode.min.js"]);
  Vue.component("cashier-modal", {
    template: Kooboo.getTemplate(
      "/_Admin/View/Market/Scripts/components/CashierModal.html"
    ),
    data: function() {
      return {
        isShow: false,
        isPaying: false,
        paymentMethods: [],
        paymentMethod: "",
        couponCode: "",
        currencySymbol: "",
        price: "",
        order: null,
        userCurrencySymbol: "",
        balance: "",
        paymentId: ""
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.EventBus.subscribe("kb/market/component/cashier/show", function(
        data
      ) {
        self.order = data;

        self.currencySymbol = data.symbol;
        self.price = data.totalAmount;

        Kooboo.Balance.getBalance().then(function(res) {
          if (res.success) {
            self.userCurrencySymbol = res.model.currency.symbol;
            self.balance = res.model.balance;
          }
        });

        if (!PAYMENT_METHODS.length) {
          Kooboo.Payment.getMethods().then(function(res) {
            if (res.success) {
              PAYMENT_METHODS = res.model.filter(function(f) {
                return f.type != "balance";
              });
              self.paymentMethods = PAYMENT_METHODS;
              self.isShow = true;
            }
          });
        } else {
          self.isShow = true;
        }
      });
    },
    methods: {
      onHide: function() {
        var self = this;
        PAYMENT_SUCCESS = false;
        self.order = null;
        self.couponCode = "";
        self.isPaying = false;
        self.isShow = false;
      },
      onPayingSuccess: function() {
        var self = this;
        self.getPaymentStatus(self.paymentId, function(res) {
          self.onHide();
          Kooboo.EventBus.publish("kb/market/cashier/done");
        });
      },
      onPayingFailed: function() {
        this.onHide();
      },
      getPaymentStatus: function(id, cb) {
        Kooboo.Payment.getStatus({
          paymentRequestId: id
        }).then(function(res) {
          cb && cb(res);
        });
      },
      onPay: function() {
        var self = this;
        var order = self.order;
        order.paymentMethod = self.paymentMethod;
        order.returnPath = location.pathname;
        if (self.paymentMethod == "coupon") {
          order.code = self.couponCode;
        }

        Kooboo.Order.pay(order).then(function(res) {
          if (res.success) {
            var data = res.model;
            if (data.actionRequired) {
              if (data.qrCode) {
                self.paymentId = data.paymentRequestId;
                $("#qr-code")
                  .empty()
                  .qrcode(data.qrCode);
                self.isPaying = true;
              } else if (data.redirectUrl) {
                self.paymentId = data.paymentRequestId;
                self.isPaying = true;
                window.open(data.redirectUrl);
              }

              self.checkPaymentStatus(data.paymentRequestId);
            } else {
              window.info.done(Kooboo.text.info.payment.success);
              Kooboo.EventBus.publish("kb/market/cashier/done");
              self.onHide();
            }
          }
        });
      },
      checkPaymentStatus: function(paymentId) {
        var self = this;
        if (!PAYMENT_SUCCESS && self.isPaying) {
          self.getPaymentStatus(paymentId, function(res) {
            if (
              res.success &&
              (res.model.success || res.model.message == "canceled")
            ) {
              PAYMENT_SUCCESS = true;
              self.onHide();
              if (res.model.success) {
                if (!infoShowed) {
                  infoShowed = true;
                  window.info.done(Kooboo.text.info.payment.success);
                }
                Kooboo.EventBus.publish("kb/market/cashier/done");
              } else {
                if (!infoShowed) {
                  infoShowed = true;
                  window.info.done(Kooboo.text.info.payment.failed);
                }
              }
            } else {
              self.checkPaymentStatus(paymentId);
            }
          });
        }
      }
    },
    computed: {
      userBalance: function() {
        return this.userCurrencySymbol + this.balance;
      },
      displayAmount: function() {
        return this.currencySymbol + this.price;
      }
    },
    watch: {
      isShow: function(show) {
        var self = this;
        if (show) {
          self.paymentMethod = PAYMENT_METHODS[0].type;
        }
      }
    }
  });
})();
