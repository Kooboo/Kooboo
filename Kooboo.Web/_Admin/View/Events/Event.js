$(function() {

    if ($.fn.bootstrapSwitch) {
        $.fn.bootstrapSwitch.defaults.onText = Kooboo.text.common.yes;
        $.fn.bootstrapSwitch.defaults.offText = Kooboo.text.common.no;
    }

    function RulesViewModel() {
        var self = this;

        self.pageName = ko.observable();

        self.ruleTypes = [];

        self.conditionDialogShow = ko.observable(false);

        self.conditionData = ko.observableArray([]);

        self.conditionRule = ko.observable();

        self.activityDialogShow = ko.observable(false);

        self.activityData = ko.observable();

        self.availableActivities = [];

        self.availableCodes = [];

        self.shouldShowSaveButton = ko.observable(false);

        self.generateSortableConnectClass = function() {
            return 'ko-container-' + new Date().getTime() + Math.round(Math.random() * 10000);
        }

        self.createCode = function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.Code.EditPage, {
                eventType: Kooboo.getQueryString('name')
            })
        }

        self.rules = ko.observableArray();

        self.addRule = function(rules, type, data) {
            if (typeof type == 'string') {
                type = window.RuleTypes[type];
            }
            var rule = type.createModel(data.hasOwnProperty('which') ? null : data);
            rules.push(rule);
            self.toggleRule(rule);
            return rule;
        }

        self.removeRule = function(rule, e) {
            var container = $(e.target).parents('[data-rules-prop]')[0];
            var rules = ko.dataFor(container)[$(container).data('rules-prop')];

            if (rule.id() !== Kooboo.Guid.Empty) {
                Kooboo.BusinessRule.deleteRule({
                    id: rule.id()
                }).then(function(res) {
                    if (res.success) { rules.remove(rule); }
                })
            } else {
                rules.remove(rule);
            }
        }

        self.ruleSummary = function(rule) {
            return window.RuleTypes[rule.type()].ruleSummary(rule);
        }

        self.toggleRule = function(rule) {
            rule.expanded(!rule.expanded());
        }

        self.currentRule = ko.observable();

        self.addActivity = function(rule, m, cb) {
            self.currentRule(rule);

            Kooboo.BusinessRule.getSetting({
                id: m.id
            }).then(function(res) {
                if (res.success) {
                    model.activityData({
                        id: m.id,
                        name: m.displayName,
                        mode: 'add',
                        settings: res.model
                    })

                    if (cb && (typeof cb == 'function')) {
                        cb(model.activityData());
                    } else {
                        model.activityDialogShow(true);
                    }
                }
            })
        }

        self.saveActivity = function(values, parentRule) {
            parentRule = parentRule && (parentRule.hasOwnProperty('which') ? null : parentRule);
            var currentRule = parentRule || self.currentRule();

            if (!currentRule.hasOwnProperty('activity')) {
                currentRule.activity = [];
            }

            if (values.mode == 'add') {
                currentRule.activity.push(values);
            } else {
                var idx = values.index;
                currentRule.activity.splice(idx, 1, values);
            }
        }

        String.prototype.toCamelCase = function() {
            return this[0].toLocaleLowerCase() + this.slice(1);
        }

        self.editActivity = function(rule, m, e) {
            m.mode = 'edit';
            m.index = rule.activity.indexOf(m);
            model.activityDialogShow(true);
            model.activityData(m);
        }

        self.removeActivity = function(container, activity) {
            if (!confirm(Kooboo.text.confirm.deleteItem)) {
                return false;
            }

            container.remove(activity);
        }

        function dataDemapping(rules) {

            rules.forEach(function(rule) {

                if (rule.hasOwnProperty('expanded')) {
                    delete rule.expanded;
                }

                if (rule.ruleType == 'IfElse') {
                    delete rule.ruleType;
                    rule.do = [];
                    rule.if.forEach(function(cond, idx) {
                        Object.keys(cond).forEach(function(key) {
                            if (['left', 'operator', 'right'].indexOf(key) == -1) {
                                delete rule.if[idx][key];
                            }
                        })
                    })

                    dataDemapping(rule.then);
                    dataDemapping(rule.else);
                } else {
                    rule.do = rule.activity.map(function(act) {
                        return {
                            codeId: act.id,
                            setting: act.values
                        }
                    })
                    rule.if = [];
                    rule.then = [];
                    rule.else = [];

                    delete rule.ruleType;
                    delete rule.activity;
                }
            })
            return rules;
        }

        self.save = function() {
            var rules = ko.mapping.toJS(self.rules());
            dataDemapping(rules);
            Kooboo.BusinessRule.post({
                eventName: Kooboo.getQueryString('name'),
                rules: rules
            }).then(function(res) {
                if (res.success) {
                    location.reload();
                }
            });
        }

        self.loadRules = function(rules, parentRule) {

            if (rules && rules.length) {

                rules.forEach(function(rule) {
                    var type = '',
                        currentRule = null;

                    var elseCondition = rule.else,
                        thenCondition = rule.then,
                        activities = rule.do;

                    if (rule.do && !rule.do.length) {
                        type = 'IfElse';
                        currentRule = self.addRule(parentRule || self.rules, type, _.extend(rule, {
                            else: [],
                            then: [],
                            do: [],
                            expanded: false,
                            ruleType: type
                        }));

                        self.loadRules(thenCondition, currentRule.then);
                        self.loadRules(elseCondition, currentRule.else);
                    } else {
                        self.loadActivities(rule.id, activities, parentRule || self.rules);
                    }

                })
            }
        }

        self.loadActivities = function(id, activities, parentRule) {
            if (activities && activities.length) {
                var currentRule = self.addRule(parentRule, 'Do', {
                    activity: [],
                    expanded: false,
                    ruleType: "Do",
                    id: id
                });
                activities.forEach(function(activity) {

                    var findActivity = self.availableCodes.find(function(code) {
                        return code.id == activity.codeId;
                    })

                    if (findActivity) {
                        self.addActivity(parentRule, {
                            id: findActivity.id,
                            displayName: findActivity.displayName
                        }, function(data) {
                            self.saveActivity(_.extend(data, { setting: activity.setting }), currentRule);
                            activity.setting && Object.keys(activity.setting).forEach(function(key) {
                                data.settings.forEach(function(setting) {
                                    if (key == setting.name) {
                                        setting.defaultValue = setting.fieldValue = activity.setting[key]
                                    }
                                })
                            })
                        })
                    } else {
                        debugger;
                    }

                })
            }
        }
    }

    var model = new RulesViewModel();

    if (model.rules().length > 0) {
        model.shouldShowSaveButton(true);
    } else {
        model.rules.subscribe(function(rule) {
            if (rule.length > 0) {
                model.shouldShowSaveButton(true);
            } else {
                model.shouldShowSaveButton(false);
            }
        });
    }

    window.RootViewModel = model;

    // Initialize
    $.each(window.RuleTypes, function() {
        model.ruleTypes.push(this);
        if (this.onPageDataBinding) {
            this.onPageDataBinding(model);
        }
    });

    $.when(Kooboo.BusinessRule.getAvailableCodes({
        eventName: Kooboo.getQueryString("name")
    }), Kooboo.BusinessRule.getListByEvent({
        eventName: Kooboo.getQueryString("name")
    })).then(function(r1, r2) {
        if (r1[0].success && r2[0].success) {

            var codes = (_.isEmpty(r1[0].model) ? [] : Kooboo.objToArr(r1[0].model, 'id', 'displayName'));
            model.availableCodes = codes.map(function(code) {
                code.setting = {};
                return code;
            })

            model.loadRules(r2[0].model);
        }
        ko.applyBindings(model, document.getElementById('main'));
    })

    // Button Actions
    $('[data-toggle="save-rules"]').on('click', function(e) {
        model.save();
        e.preventDefault();
    });

    $(window).on('beforeunload', function() {
        //if (JSON.stringify(ko.mapping.toJS(model.rules)) !== JSON.stringify(originalRules)) {
        //    return 'Rules are modified. Are you sure to leave without saving?';
        //}
    });
})