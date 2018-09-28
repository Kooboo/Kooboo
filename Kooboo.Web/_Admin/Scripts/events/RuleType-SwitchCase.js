(function() {
    window.RuleTypes = window.RuleTypes || {};
    window.RuleTypes['SwitchCase'] = {
        name: 'SwitchCase',
        displayName: 'SWITCH-CASE-DEFAULT',
        displayNameWhenNotInTopLevel: 'SWITCH-CASE-DEFAULT',

        createModel: function(data) {
            if (!data) {
                ob = {
                    cases: [],
                    default: [],
                    expanded: false,
                    parameter: null,
                    ruleType: "SwitchCase"
                }
                var model = ko.mapping.fromJS(ob)
                window.RootViewModel.switchCaseRule.addCase(model);
            } else {
                ko.mapping.fromJS(data, {}, model);
            }
            //var model = komapping.fromJS(DefaultModels['switchCaseRule']);
            //if (data) {
            //    komapping.fromJS(data, {}, model);
            //} else {
            //    // Add a default case
            //    window.RootViewModel.switchCaseRule.addCase(model);
            //}

            return model;
        },

        casesTooltip: function(cases, start) {
            var html = '';
            for (var i = start, len = cases.length; i < len; i++) {
                var caze = cases[i];
                html += '<div>CASE (' + caze.value() + ')</div>';
                $.each(caze.rules(), function(i, rule) {
                    if (i > 0) {
                        html += '<hr class="rule-divider"/>';
                    }
                    html += window.RuleTypes[rule.ruleType()].renderTooltip(rule, 1);
                });
            }

            return html;
        },

        rulesSummary: function(rules) {
            var html = '<span class="label blue">';

            if (rules.length === 1 && rules[0].ruleType() === 'Do' && rules[0].activitiy().length > 0) {
                var rule = rules[0];
                html += rule.activitiy()[0].name();
                if (rule.activitiy().length > 1) {
                    html += '...';
                }
            } else {
                html += '...';
            }

            html += '</span>';

            return html;
        },

        rulesTooltip: function(rules) {
            var html = '';
            $.each(rules, function(i, rule) {
                if (i > 0) {
                    html += '<hr class="rule-divider"/>';
                }
                html += window.RuleTypes[rule.ruleType()].renderTooltip(rule, 0);
            });

            return html;
        },

        renderTooltip: function(rule, level) {
            var html = '<div class="rule-summary">';

            html += '<div><span>SWITCH</span> (' + rule.parameter() + ')</div>';

            $.each(rule.cases(), function(i, caze) {
                html += '<div><span>CASE</span> ' + caze.value() + '</div>';
                $.each(caze.rules(), function() {
                    html += window.RuleTypes[this.ruleType()].renderTooltip(this, level + 1);
                });
            });

            if (rule.Default().length > 0) {
                html += '<div><span>DEFAULT</span></div>';
                $.each(rule.default(), function() {
                    html += window.RuleTypes[this.ruleType()].renderTooltip(this, level + 1);
                });
            }

            html += '</div>';

            return html;
        },

        onPageDataBinding: function(root) {
            $.extend(root, {
                switchCaseRule: {
                    addCase: function(rule) {
                        var caze = {
                            value: ko.observable(),
                            rules: ko.observableArray()
                        };
                        rule.cases.push(caze);
                    },

                    removeCase: function(rule, caze) {
                        rule.cases.remove(caze);
                    },

                    availableParameters: ko.observableArray()
                }
            });

            if (window.DataContextType) {
                $.get('/api/events/rules/params?dataContextType=' + encodeURIComponent(window.DataContextType))
                    .then(function(data) {
                        $.each(data, function() {
                            var supportsEquals = function(x) {
                                return _.any(x.supportedOperators, function(it) {
                                    return it.name === 'equals'
                                });
                            }

                            if (supportsEquals(this)) {
                                root.switchCaseRule.availableParameters.push(this);
                            }
                        });
                    });
            }
        }
    };
})()