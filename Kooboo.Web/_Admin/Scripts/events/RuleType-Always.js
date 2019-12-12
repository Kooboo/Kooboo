(function() {
  var self;
  var baseComponent = Vue.component("events-ruletype-always", {
    template: "#RuleTemplate-Do",
    props: {
      rule: Object,
      rules: Array,
      ruleIndex: Number
    },
    beforeCreate: function() {
      self = this;
    },
    methods: {
      activitiesSummary: function(rule) {
        var html = '<span class="label blue">';
        if (rule.activity.length > 0) {
          html += rule.activity[0].name;
          if (rule.activity.length > 1) {
            html += "...";
          }
        } else {
          html += "[" + Kooboo.text.component.event.noActivity + "]";
        }
        html += "</span>";
        return html;
      },
      activitiesTooltip: function(rule) {
        if (rule.activity.length <= 1) {
          return "";
        }
        return self.renderTooltip(rule, 0);
      },
      renderTooltip: function(rule, level) {
        var html = '<div class="rule-summary">';
        $.each(rule.activity, function(i) {
          html += "<div>" + this.name + "</div>";
        });
        html += "</div>";
        return html;
      }
    }
  });
  Vue.component("events-ruletype-always-summary", {
    extends: baseComponent,
    template: "#RuleSummaryTemplate-Do"
  });
})();
