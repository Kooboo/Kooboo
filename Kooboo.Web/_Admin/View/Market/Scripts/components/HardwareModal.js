(function() {
  var template = Kooboo.getTemplate(
    "/_Admin/View/Market/Scripts/components/HardwareModal.html"
  );

  Vue.component("hardware-modal", {
    template: template,
    props: {
      isShow: Boolean,
      data: Object
    },
    data: function() {
      return {
        id: "",
        title: "",
        variants: [],
        currentVar: "",
        symbol: "",
        quantity: 1,
        startMonth: "",
        availableMonths: [],
        totalPrice: 0,
        error: "",
        posted: false
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.EventBus.subscribe("kb/market/cashier/done", function() {
        self.onHide();
      });
      document.getElementById("_quantity").onkeydown = function(e) {
        return Kooboo.event.input.positiveIntegerOnly(null, e);
      };
    },
    computed: {
      isAbleToBuy: function() {
        var self = this;
        var flag = true;

        if (!self.currentVar) {
          flag = false;
        }

        if (!self.isValid()) {
          flag = false;
        }

        return flag;
      }
    },
    methods: {
      isValid: function() {
        var self = this;
        if (!this.posted) return true;
        var result = Kooboo.validField(Number(self.quantity), [
          { required: Kooboo.text.validation.required },
          { min: 1, message: Kooboo.text.validate.minimalValue + " " + 1 }
        ]);
        this.error = result.msg;
        return result.valid;
      },
      onHide: function() {
        var self = this;
        self.error = "";
        self.posted = false;
        self.$emit("update:isShow", false);
      },
      onSelectType: function(m) {
        var self = this;
        if (!m.selected) {
          self.currentVar = m;
          self.variants.forEach(function(va) {
            va.selected = false;
          });

          m.selected = true;
          self.quantity = self.quantity || m.quantity;
          self.totalPrice = self.quantity * m.price;
        }
      },
      onBuy: function() {
        var self = this;
        self.posted = true;
        if (this.isAbleToBuy) {
          var obj = {
            id: self.id,
            startMonth: self.startMonth,
            variant: {
              id: self.currentVar.id,
              quantity: self.quantity
            }
          };
          Kooboo.Order.infra(obj).then(function(res) {
            if (res.success) {
              Kooboo.EventBus.publish(
                "kb/market/component/cashier/show",
                res.model
              );
            }
          });
        }
      },
      handleData: function() {
        var self = this;
        var data = self.data;
        self.id = data.id;
        self.title = data.name;
        self.symbol = data.symbol;
        self.availableMonths = data.startMonth.map(function(mon) {
          return {
            displayName: mon,
            value: mon
          };
        });
        if (self.availableMonths.length) {
          self.startMonth = self.availableMonths[0].value;
        }
        self.variants = data.variants.map(function(va) {
          return {
            id: va.id,
            displayText: va.name + " / " + va.period,
            price: va.price,
            period: va.period,
            quantity: va.quantity,
            selected: false
          };
        });
      }
    },
    watch: {
      isShow: function(show) {
        var self = this;
        self.$nextTick(function() {
          if (show) {
            self.handleData();
            var defaultVar = self.variants[0];
            defaultVar.selected = true;
            self.currentVar = defaultVar;
            self.quantity = defaultVar.quantity;
            self.totalPrice = self.quantity * defaultVar.price;
          }
        });
      },
      quantity: function(quantity) {
        var self = this;
        if (self.currentVar) {
          self.totalPrice = quantity * self.currentVar.price;
        } else {
          self.totalPrice = 0;
        }
      }
    }
  });
})();
