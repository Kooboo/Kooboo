$(function() {
  var self;
  var IFELSETHEN = "IF-ELSE-THEN";
  var DO = "DO";
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
            name: Kooboo.text.common.Event
          }
        ],
        pageName: "",
        ruleTypes: [IFELSETHEN, DO],
        conditionDialogShow: false,
        conditionData: [],
        conditionRule: {},
        activityDialogShow: false,
        activityData: {},
        availableActivities: [],
        availableCodes: [],
        rules: [],
        currentRule: {}
      };
    },
    mounted: function() {
      // $.each(window.RuleTypes, function() {
      //   console.log(this);
      //   self.ruleTypes.push(this);
      //   if (this.onPageDataBinding) {
      //     this.onPageDataBinding(self);
      //   }
      // });

      $.when(
        Kooboo.BusinessRule.getAvailableCodes({
          eventName: Kooboo.getQueryString("name")
        }),
        Kooboo.BusinessRule.getListByEvent({
          eventName: Kooboo.getQueryString("name")
        })
      ).then(function(r1, r2) {
        if (r1[0].success && r2[0].success) {
          var codes = _.isEmpty(r1[0].model)
            ? []
            : Kooboo.objToArr(r1[0].model, "id", "displayName");
          self.availableCodes = codes.map(function(code) {
            code.setting = {};
            return code;
          });

          self.loadRules(r2[0].model);
        }
      });

      // Button Actions
      $('[data-toggle="save-rules"]').on("click", function(e) {
        self.save();
        e.preventDefault();
      });

      $(window).on("beforeunload", function() {
        //if (JSON.stringify(ko.mapping.toJS(model.rules)) !== JSON.stringify(originalRules)) {
        //    return 'Rules are modified. Are you sure to leave without saving?';
        //}
      });
    },
    methods: {
      createCode: function() {
        location.href = Kooboo.Route.Get(Kooboo.Route.Code.EditPage, {
          eventType: Kooboo.getQueryString("name")
        });
      },
      addRule: function(rules, type, rule) {
        if (!rule) {
          if (type === DO) {
            rule = {
              activity: [],
              expanded: false,
              id: Kooboo.Guid.Empty
            };
          } else {
            rule = {
              if: [],
              else: [],
              expanded: false,
              then: [],
              id: Kooboo.Guid.Empty
            };
          }
        }
        rule.ruleType = type;
        if (rule.ruleType === DO) {
          rule.component = "events-ruletype-always";
        } else {
          rule.component = "events-ruletype-ifelse";
        }
        rules.push(rule);
        self.toggleRule(rule);
        return rule;
      },
      removeRule: function(rule, rules, index) {
        if (rule.id !== Kooboo.Guid.Empty) {
          Kooboo.BusinessRule.deleteRule({
            id: rule.id
          }).then(function(res) {
            if (res.success) {
              rules.splice(index, 1);
            }
          });
        } else {
          rules.splice(index, 1);
        }
      },
      ruleSummary: function(rule) {
        return window.RuleTypes[rule.type].ruleSummary(rule);
      },
      toggleRule: function(rule) {
        rule.expanded = !rule.expanded;
      },
      addActivity: function(rule, m, cb) {
        self.currentRule = rule;
        Kooboo.BusinessRule.getSetting({
          id: m.id
        }).then(function(res) {
          if (res.success) {
            self.activityData = {
              id: m.id,
              name: m.displayName,
              mode: "add",
              settings: res.model
            };

            if (cb && typeof cb == "function") {
              cb(self.activityData);
            } else {
              self.activityDialogShow = true;
            }
          }
        });
      },
      saveActivity: function(values, parentRule) {
        parentRule =
          parentRule && (parentRule.which !== undefined ? null : parentRule);
        var currentRule = parentRule || self.currentRule;

        if (!currentRule.activity) {
          currentRule.activity = [];
        }

        if (values.mode == "add") {
          currentRule.activity.push(values);
        } else {
          var idx = values.index;
          currentRule.activity.splice(idx, 1, values);
        }
      },
      editActivity: function(m, index) {
        m.mode = "edit";
        m.index = index;
        self.activityDialogShow = true;
        self.activityData = m;
      },
      removeActivity: function(activities, index) {
        if (!confirm(Kooboo.text.confirm.deleteItem)) {
          return false;
        }
        activities.splice(index, 1);
      },
      save: function() {
        var rules = self.rules;
        dataDemapping(rules);
        Kooboo.BusinessRule.post({
          eventName: Kooboo.getQueryString("name"),
          rules: rules
        }).then(function(res) {
          if (res.success) {
            location.reload();
          }
        });
      },
      loadRules: function(rules, parentRule) {
        if (rules && rules.length) {
          rules.forEach(function(rule) {
            var type = "",
              currentRule = null;

            var elseCondition = rule.else,
              thenCondition = rule.then,
              activities = rule.do;

            if (rule.do && !rule.do.length) {
              type = IFELSETHEN;
              currentRule = self.addRule(
                parentRule || self.rules,
                type,
                _.extend(rule, {
                  else: [],
                  then: [],
                  do: [],
                  expanded: false,
                  ruleType: type
                })
              );

              self.loadRules(thenCondition, currentRule.then);
              self.loadRules(elseCondition, currentRule.else);
            } else {
              self.loadActivities(
                rule.id,
                activities,
                parentRule || self.rules
              );
            }
          });
        }
      },
      loadActivities: function(id, activities, parentRule) {
        if (activities && activities.length) {
          var currentRule = self.addRule(parentRule, DO, {
            activity: [],
            expanded: false,
            ruleType: DO,
            id: id
          });
          activities.forEach(function(activity) {
            var findActivity = self.availableCodes.find(function(code) {
              return code.id == activity.codeId;
            });

            if (findActivity) {
              self.addActivity(
                parentRule,
                {
                  id: findActivity.id,
                  displayName: findActivity.displayName
                },
                function(data) {
                  self.saveActivity(
                    _.extend(data, { setting: activity.setting }),
                    currentRule
                  );
                  activity.setting &&
                    Object.keys(activity.setting).forEach(function(key) {
                      data.settings.forEach(function(setting) {
                        if (key == setting.name) {
                          setting.defaultValue = setting.fieldValue =
                            activity.setting[key];
                        }
                      });
                    });
                }
              );
            } else {
              debugger;
            }
          });
        }
      }
    },
    computed: {
      shouldShowSaveButton: function() {
        return self.rules.length > 0;
      }
    }
  });

  function dataDemapping(rules) {
    rules.forEach(function(rule) {
      if (rule.hasOwnProperty("expanded")) {
        delete rule.expanded;
      }
      if (rule.ruleType == IFELSETHEN) {
        delete rule.ruleType;
        rule.do = [];
        rule.if.forEach(function(cond, idx) {
          Object.keys(cond).forEach(function(key) {
            if (["left", "operator", "right"].indexOf(key) == -1) {
              delete rule.if[idx][key];
            }
          });
        });

        dataDemapping(rule.then);
        dataDemapping(rule.else);
      } else {
        rule.do = rule.activity.map(function(act) {
          return {
            codeId: act.id,
            setting: act.values
          };
        });
        rule.if = [];
        rule.then = [];
        rule.else = [];

        delete rule.ruleType;
        delete rule.activity;
      }
    });
    return rules;
  }
});
