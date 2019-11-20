(function() {
  var self;
  Vue.component("kb-condition-dialog", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/events/conditionsDialog.html"
    ),
    props: {
      modalShow: Boolean,
      conditionData: Object,
      conditionRule: Object,
      parameters: Array
    },
    data: function() {
      self = this;
      return {
        conditions: []
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
      removeCondition = function(condition) {
        self.conditions= _.without(self.conditions , condition);
      },
      J_Submit: function() {
        Kooboo.EventBus.publish("conditionUpdata", {
          conditions: self.conditions,
          rule: self.conditionRule
        });
        self.J_Cancel();
      },
      J_Cancel: function() {
        self.$emit('update:modalShow', false);
      }
    },
    watch: {
      conditionData: function(val) {
        var conditions = [];
        val.forEach(function(condition) {
          conditions.push(new Condition(condition));
        });
        self.conditions = conditions;
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
    // _this.selectionValues.subscribe(function(selectionValues) {
    //   if (selectionValues) {
    //     var arr = [];
    //     for (var key in selectionValues) {
    //       _this.availableValues.push({
    //         key: key,
    //         value: selectionValues[key]
    //       });
    //     }
    //   }
    // });
    if (this.left) {
      var p = self.parameters.filter(function(pa) {
        return pa.name === _this.left;
      })[0];
      if (p.controlType === "Selection") {
        _this.controlType = p.controlType;
        _this.selectionValues = p.selectionValues;
      }
      _this.operators(p.operator);
    }
    // this.left.subscribe(function(paramName) {
    //   var p = self.parameters().filter(function(pa) {
    //     return pa.name === paramName;
    //   })[0];
    //   _this.controlType(p.controlType);
    //   if (p.controlType === "Selection") {
    //     _this.selectionValues(p.selectionValues);
    //   } else {
    //     _this.selectionValues("");
    //   }
    //   _this.operators(p.operator);
    // });
  }
})();
