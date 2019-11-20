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
      return {};
    },
    methods: {

    },
    computed: {
      conditions: function() {
        var conditions = [];
        self.conditionData.forEach(function(condition) {
          var condition = ko.mapping.toJS(condition);
          conditions.push(new Condition(condition));
        });
        return conditions;
      }
    }
  });

  ko.components.register("condition-dialog", {
    viewModel: function(params) {


      self.addCondition = function() {
        self.conditions.push(new Condition());
      };

      self.J_Submit = function() {
        Kooboo.EventBus.publish("conditionUpdata", {
          conditions: self.conditions(),
          rule: self.conditionRule()
        });
        self.modalShow(false);
      };

      self.J_Cancel = function() {
        self.modalShow(false);
      };

      function Condition(opt) {
        var _this = this;
        this.left = ko.observable((opt && opt.left) || "");
        this.operators = ko.observableArray();
        this.operator = ko.observable((opt && opt.operator) || "");
        this.controlType = ko.observable((opt && opt.controlType) || "");
        this.right = ko.observable((opt && opt.right) || "");
        this.selectionValues = ko.observable((opt && opt.right) || "");
        this.availableValues = ko.observableArray([]);
        _this.selectionValues.subscribe(function(selectionValues) {
          if (selectionValues) {
            var arr = [];
            for (var key in selectionValues) {
              _this.availableValues.push({
                key: key,
                value: selectionValues[key]
              });
            }
          }
        });
        if (this.left()) {
          var p = self.parameters().filter(function(pa) {
            return pa.name === _this.left();
          })[0];
          if (p.controlType === "Selection") {
            _this.controlType(p.controlType);
            _this.selectionValues(p.selectionValues);
          }
          _this.operators(p.operator);
        }
        this.left.subscribe(function(paramName) {
          var p = self.parameters().filter(function(pa) {
            return pa.name === paramName;
          })[0];
          _this.controlType(p.controlType);
          if (p.controlType === "Selection") {
            _this.selectionValues(p.selectionValues);
          } else {
            _this.selectionValues("");
          }
          _this.operators(p.operator);
        });
      }

      Kooboo.BusinessRule.getConditionOption({
        eventName: Kooboo.getQueryString("name")
      }).then(function(data) {
        debugger;
        if (data.success) {
          $.each(data.model, function() {
            self.parameters(data.model);
          });
        }
      });

      self.removeCondition = function(condition) {
        self.conditions.remove(condition);
      };
    },
    template: template
  });
})();
