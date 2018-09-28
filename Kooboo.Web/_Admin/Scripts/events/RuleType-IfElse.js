(function() {
    window.RuleTypes = window.RuleTypes || {};
    window.RuleTypes['IfElse'] = {
        name: 'IfElse',
        displayName: 'IF-ELSE-THEN',
        displayNameWhenNotInTopLevel: 'IF-ELSE-THEN',

        createModel: function(data) {
            var model = null;
            if (!data) {
                ob = {
                    if: [],
                    else: [],
                    expanded: false,
                    then: [],
                    ruleType: "IfElse",
                    id: Kooboo.Guid.Empty
                }
                model = ko.mapping.fromJS(ob)
            } else {
                model = ko.mapping.fromJS(data);
            }
            return model;
        },

        branchSummary: function(rules) {
            var html = '';

            if (rules.length === 1 && rules[0].ruleType() === 'Do') {
                if (rules[0].activity().length > 0) {
                    html += '<span class="label">';
                    if (rules[0].activity()[0].name instanceof Function) {
                        html += rules[0].activity()[0].name();
                    } else {
                        html += rules[0].activity()[0].name;
                    }
                    if (rules[0].activity().length > 1) {
                        html += '...';
                    }
                    html += '</span>';
                }
            } else {
                html += '<span class="label">...</span>';
            }

            return html;
        },

        branchTooltip: function(rules) {
            // In this case, the detail is shown directly, so no need to have a tooltip
            if (rules.length === 1 && rules[0].ruleType() === 'Do' && rules[0].activity().length <= 1) {
                return '';
            }

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

            html += '<div><span class="tag">IF</span> (' + window.RuleTypes['IfElse'].conditionsSummary(rule) + ')</div>';
            html += '<div><span class="tag">THEN</span></div>';

            if (rule.then().length > 0) {
                $.each(rule.then(), function(i) {
                    if (i > 0) {
                        html += '<hr class="rule-divider"/>';
                    }
                    html += window.RuleTypes[this.ruleType()].renderTooltip(this, level + 1);
                });
            } else {
                html += '<div>[' + Kooboo.text.component.event.noActivity + ']</div>';
            }

            if (rule['else']().length > 0) {
                html += '<div><span class="tag">ELSE</span></div>';
                $.each(rule['else'](), function(i) {
                    if (i > 0) {
                        html += '<hr class="rule-divider"/>';
                    }
                    html += window.RuleTypes[this.ruleType()].renderTooltip(this, level + 1);
                });
            }

            html += '</div>';

            return html;
        },

        conditionsSummary: function(rule, emptyText) {
            //todo remove instanceof Function
            if (rule.if().length === 0) {
                return emptyText || Kooboo.text.component.events.editCondition;
            }

            var desc = '';

            if (rule.if().length === 1) {
                if (rule.if()[0].left instanceof Function) {
                    desc = rule.if()[0].left() + " " + rule.if()[0].operator() + " " + rule.if()[0].right()
                } else {
                    desc = rule.if()[0].left + " " + rule.if()[0].operator + " " + rule.if()[0].right
                }
            } else {
                $.each(rule.if(), function(i) {
                    if (i > 0) {
                        desc += ' AND '
                    }
                    if (this.left instanceof Function) {
                        desc += '(' + this.left() + " " + this.operator() + " " + this.right() + ')';
                    } else {
                        desc += '(' + this.left + " " + this.operator + " " + this.right + ')';
                    }
                });
            }

            return desc;
        },

        onPageDataBinding: function(root) {
            $.extend(root, {
                ifelseRule: {
                    editCondition: function(rule) {
                        root.conditionDialogShow(true);
                        root.conditionData(rule.if());
                        root.conditionRule(rule);
                        Kooboo.EventBus.subscribe("conditionUpdata", function(opt) {
                            var conditions = opt.conditions,
                                rule = opt.rule;
                            rule.if.removeAll();
                            $.each(conditions, function() {
                                rule.if.push(this);
                            });
                        });
                    }
                }
            });
        }
    };
})()