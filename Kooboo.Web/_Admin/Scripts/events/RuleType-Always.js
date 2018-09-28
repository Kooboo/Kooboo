(function() {
    window.RuleTypes = window.RuleTypes || {};
    window.RuleTypes['Do'] = {
        name: 'Always',
        displayName: 'DO', // 添加顶级规则时显示 Always,
        displayNameWhenNotInTopLevel: 'DO', // 添加为 IF-ELSE 或其它规则的内嵌规则时显示为 DO

        createModel: function(data) {
            var model = null;
            if (!data) {
                ob = {
                    activity: [],
                    expanded: false,
                    ruleType: "Do",
                    id: Kooboo.Guid.Empty
                }
                model = ko.mapping.fromJS(ob)
            } else {
                model = ko.mapping.fromJS(data, {}, model);
            }
            return model;
        },

        activitiesSummary: function(rule) {
            var html = '<span class="label blue">';

            if (rule.activity().length > 0) {
                if (rule.activity()[0].name instanceof Function) {
                    html += rule.activity()[0].name();
                } else {
                    html += rule.activity()[0].name;
                }
                if (rule.activity().length > 1) {
                    html += '...';
                }

            } else {
                html += '[' + Kooboo.text.component.event.noActivity + ']';
            }

            html += '</span>'

            return html;
        },

        activitiesTooltip: function(rule) {
            if (rule.activity().length <= 1) {
                return '';
            }

            return window.RuleTypes['Do'].renderTooltip(rule, 0);
        },

        renderTooltip: function(rule, level) {
            var html = '<div class="rule-summary">';

            $.each(rule.activity(), function(i) {
                if (this.name instanceof Function) {
                    html += '<div><span class="label blue">' + this.name() + '</span></div>';
                } else {
                    html += '<div><span class="label blue">' + this.name + '</span></div>';
                }
            });

            html += '</div>';

            return html;
        },

        onPageDataBinding: function(root) {
            $.extend(root, {
                doRule: {
                    addRule: function() {}
                }
            })
        }
    };

})()