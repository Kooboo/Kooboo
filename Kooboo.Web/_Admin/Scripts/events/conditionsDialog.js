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
        self.conditions.push(self.mapCondition());
      },
      removeCondition: function(index) {
        self.conditions.splice(index, 1);
      },
      J_Submit: function() {
        Kooboo.EventBus.publish("conditionUpdata", {
          conditions: self.conditions,
          rule: self.conditionRule
        });
        // self.$emit(self.conditions);
        self.J_Cancel();
      },
      J_Cancel: function() {
        self.$emit("update:modalShow", false);
      },
      mapCondition(opt) {
        var condition = {
          left: (opt && opt.left) || "",
          operators: [],
          operator: (opt && opt.operator) || "",
          controlType: (opt && opt.controlType) || "",
          right: (opt && opt.right) || "",
          selectionValues: (opt && opt.right) || ""
        };
        self.changeLeft(condition);
        return condition;
      },
      changeLeft(condition) {
        if (!condition.left) {
          condition.left = self.parameters[0].name;
        }
        var p = _.find(self.parameters, function(pa) {
          return pa.name === condition.left;
        });
        if (p) {
          condition.controlType = p.controlType;
          if (p.controlType === "Selection") {
            condition.selectionValues = p.selectionValues;
            if (!condition.right) {
              condition.right = condition.selectionValues[0].key;
            }
          } else {
            condition.selectionValues = "";
          }
          condition.operators = p.operator;
          if (!condition.operator) {
            condition.operator = condition.operators[0];
          }
        }
      }
    },
    computed: {
      visibleConditionData: function() {
        return self.modalShow ? self.conditionData : null;
      }
    },
    watch: {
      visibleConditionData: function(val) {
        if (!val) {
          return;
        }
        var conditions = [];
        val.forEach(function(condition) {
          conditions.push(self.mapCondition(condition));
        });
        self.conditions = conditions;
      }
    }
  });
})();
