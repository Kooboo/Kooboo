(function() {
  var self;
  Vue.component("kb-condition-dialog", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/events/conditionsDialog.html"
    ),
    props: {
      modalShow: Boolean,
      conditionData: Array,
      conditionRule: Object
    },
    data: function() {
      self = this;
      return {
        conditions: [],
        parameters: []
      };
    },
    mounted: function() {
      Kooboo.BusinessRule.getConditionOption({
        eventName: Kooboo.getQueryString("name")
      }).then(function(data) {
        if (data.success) {
          $.each(data.model, function() {
            self.parameters = data.model;
          });
        }
      });
    },
    methods: {
      addCondition: function() {
        self.conditions.push(new Condition());
      },
      removeCondition: function(condition) {
        self.conditions = _.without(self.conditions, condition);
      },
      J_Submit: function() {
        Kooboo.EventBus.publish("conditionUpdata", {
          conditions: self.conditions,
          rule: self.conditionRule
        });
        self.J_Cancel();
      },
      J_Cancel: function() {
        self.$emit("update:modalShow", false);
      }
    },
    watch: {
      conditionData: function(val) {
        var conditions = [];
        val.forEach(function(condition) {
          conditions.push(new Condition(condition));
        });
        self.conditions = conditions;
      },
      conditions: {
        handler: function(val, oldVal) {
          if (!val.length) {
            return;
          }
          val.forEach(function(conditon, index) {
            var selectionValues = conditon.selectionValues;
            if (selectionValues) {
              conditon.availableValues = [];
              for (var key in selectionValues) {
                conditon.availableValues.push({
                  key: key,
                  value: selectionValues[key]
                });
              }
            }

            var p = self.parameters.filter(function(pa) {
              return pa.name === conditon.left;
            })[0];
            conditon.controlType = p.controlType;
            if (p.controlType === "Selection") {
              conditon.selectionValues = p.selectionValues;
            } else {
              conditon.selectionValues = "";
            }
            conditon.operators = p.operator;
            console.log(conditon);
          });
        },
        deep: true
      }
    }
  });
  function Condition(opt) {
    var _this = this;
    this.left = (opt && opt.left) || "";
    this.operators = [];
    this.operator = (opt && opt.operator) || "";
    this.controlType = (opt && opt.controlType) || "";
    this.right = (opt && opt.right) || "";
    this.selectionValues = (opt && opt.right) || "";
    this.availableValues = [];
    if (this.left) {
      var p = self.parameters.filter(function(pa) {
        return pa.name === _this.left;
      })[0];
      if (p.controlType === "Selection") {
        _this.controlType = p.controlType;
        _this.selectionValues = p.selectionValues;
      }
      _this.operators = p.operator;
    }
  }
})();
