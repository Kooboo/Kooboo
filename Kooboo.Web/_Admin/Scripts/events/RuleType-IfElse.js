(function() {
  var self;
  var baseComponent = Vue.component("events-ruletype-ifelse", {
    template: "#RuleTemplate-IfElse",
    props: {
      rule: Object,
      rules: Array,
      ruleIndex: Number
    },
    beforeCreate: function() {
      self = this;
    },
    methods: {
      editCondition: function(rule) {
        self.$root.conditionDialogShow = true;
        self.$root.conditionData = rule.if;
        self.$root.conditionRule = rule;

        Kooboo.EventBus.subscribe("conditionUpdata", function(opt) {
          var conditions = opt.conditions,
            rule = opt.rule;
          rule.if = [];
          $.each(conditions, function() {
            rule.if.push(this);
          });
        });
      },
      branchSummary: function(rules) {
        var html = "";
        if (rules.length === 1 && rules[0].ruleType === "Do") {
          if (rules[0].activity.length > 0) {
            html += '<span class="label">';
            html += rules[0].activity[0].name;
            if (rules[0].activity.length > 1) {
              html += "...";
            }
            html += "</span>";
          }
        } else {
          html += '<span class="label">...</span>';
        }
        return html;
      },
      branchTooltip: function(rules) {
        // In this case, the detail is shown directly, so no need to have a tooltip
        if (
          rules.length === 1 &&
          rules[0].ruleType === "Do" &&
          rules[0].activity.length <= 1
        ) {
          return "";
        }

        var html = "";
        $.each(rules, function(i, rule) {
          if (i > 0) {
            html += '<hr class="rule-divider"/>';
          }
          html += self.renderTooltip(rule, 0);
        });

        return html;
      },
      renderTooltip: function(rule, level) {
        var html = '<div class="rule-summary">';

        html +=
          '<div><span class="tag">IF</span> (' +
          self.conditionsSummary(rule) +
          ")</div>";
        html += '<div><span class="tag">THEN</span></div>';

        if(rule.then && rule.then.length > 0) {
          $.each(rule.then, function(i) {
            if (i > 0) {
              html += '<hr class="rule-divider"/>';
            }
            html += self.renderTooltip(this, level + 1);
          });
        } else {
          html += "<div>[" + Kooboo.text.component.event.noActivity + "]</div>";
        }

        if (rule.else && rule.else.length > 0) {
          html += '<div><span class="tag">ELSE</span></div>';
          $.each(rule["else"], function(i) {
            if (i > 0) {
              html += '<hr class="rule-divider"/>';
            }
            html += self.renderTooltip(this, level + 1);
          });
        }

        html += "</div>";

        return html;
      },
      conditionsSummary: function(rule, emptyText) {
        if (!rule.if || rule.if.length === 0) {
          return emptyText || Kooboo.text.component.events.editCondition;
        }
        var desc = "";
        if (rule.if.length === 1) {
          desc =
            rule.if[0].left +
            " " +
            rule.if[0].operator +
            " " +
            rule.if[0].right;
        } else {
          $.each(rule.if, function(i) {
            if (i > 0) {
              desc += " AND ";
            }
            desc +=
              "(" + this.left + " " + this.operator + " " + this.right + ")";
          });
        }
        return desc;
      }
    }
  });
  Vue.component("events-ruletype-ifelse-summary", {
    extends: baseComponent,
    template: "#RuleSummaryTemplate-IfElse"
  });
})();
