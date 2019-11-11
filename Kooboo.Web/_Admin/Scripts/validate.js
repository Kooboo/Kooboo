var Validate = (function() {
  function Validate() {
    this.config = { noMessage: false, rawResult: false };
  }
  Validate.prototype.register = function(name, ruleResolve) {
    this[name] = this.registerFatory(name, ruleResolve);
  };
  Validate.prototype.registerAll = function(ruleResolves) {
    var self = this;
    var keys = Object.keys(ruleResolves);
    keys.forEach(function(key) {
      self.register(key, ruleResolves[key]);
    });
  };
  Validate.prototype.get = function(name) {
    return this[name];
  };
  Validate.prototype.hasError = function(result) {
    var keys = Object.keys(result);
    var hasError;
    for (var i = 0; i < keys.length; i++) {
      var key = keys[i];
      var value = result[key];
      if (key === "hasError") {
        if (value === true) {
          return true;
        }
      } else {
        if (
          value instanceof Object &&
          !(key === "rule" || key === "ruleResolve")
        ) {
          hasError = this.hasError(value);
          if (hasError) {
            return hasError;
          }
        }
      }
    }
    return hasError;
  };
  Validate.prototype.setRules = function(rules) {
    var self = this;
    return {
      validate: function(values, isMulti) {
        return self.dispatcher("setRules", values, {
          rules: rules,
          isMulti: isMulti
        });
      }
    };
  };
  Validate.prototype.setKeyRules = function(keyRules) {
    var self = this;
    return {
      validate: function(values) {
        return self.dispatcher("setKeyRules", values, { keyRules: keyRules });
      }
    };
  };
  Validate.prototype.validateRulesHandle = function(ruleOrRules, value) {
    var result;
    if (ruleOrRules instanceof Array) {
      if (ruleOrRules.length > 0) {
        result = {};
      }
      for (var i = 0; i < ruleOrRules.length; i++) {
        var rule = ruleOrRules[i];
        var validateName = rule.name;
        var temp = this[validateName](rule).validate(value);
        if (!this.config.rawResult) {
          if (temp.hasError) {
            result = temp;
            break;
          }
        } else {
          result[validateName] = temp;
        }
      }
    } else {
      result = this[ruleOrRules.name](ruleOrRules).validate(value);
    }
    return result;
  };
  Validate.prototype.validateValuesHandle = function(
    ruleOrRules,
    values,
    isMulti
  ) {
    var _this = this;
    var result;
    if (isMulti) {
      var keys = Object.keys(values);
      if (keys.length > 0) {
        result = {};
      }
      keys.forEach(function(key) {
        var value = values[key];
        var temp = _this.validateRulesHandle(ruleOrRules, value);
        if (temp) {
          result[key] = temp;
        } else {
          result[key] = undefined;
        }
      });
    } else {
      var temp = this.validateRulesHandle(ruleOrRules, values);
      if (temp === {} || temp === null) {
        result = undefined;
      } else {
        result = temp;
      }
    }
    return result;
  };
  Validate.prototype.dispatcher = function(type, values, options) {
    var _this = this;
    var result;
    if (type === "setKeyRules") {
      var keyRules_1 = options.keyRules;
      var keys = Object.keys(keyRules_1);
      if (keys.length > 0) {
        result = {};
      }
      var hasError = false;
      keys.forEach(function(key) {
        var rules = keyRules_1[key];
        var value = values[key];
        /*        if (!value) {
                  console.error(`not found option.key: ${key}  at validate(values)`);
                }*/
        result[key] = _this.validateRulesHandle(rules, value);
      });
    } else if (type === "setRules") {
      var rules = options.rules;
      var isMulti = options.isMulti;
      result = this.validateValuesHandle(rules, values, isMulti);
    }
    return result;
  };
  Validate.prototype.validateFatory = function(
    validateName,
    rule,
    ruleResolve
  ) {
    var self = this;
    return function(values, isMulti) {
      if (!self.config.noMessage) {
        if (!rule.message) {
          console.error(
            "message undefined,you can defined by message() or options:{\n                        message: 'i am message'\n                        } "
          );
        }
      }
      var result;
      if (isMulti) {
        var keys = Object.keys(values);
        if (keys.length > 0) {
          result = {};
        }
        keys.forEach(function(key) {
          var value;
          if (values[key]) {
            value = values[key];
          }
          result[key] = {
            name: validateName,
            hasError: !ruleResolve(value, rule),
            rule: rule,
            value: value,
            ruleResolve: ruleResolve
          };
          if (!self.config.noMessage) result[key].message = rule.message;
        });
      } else {
        result = {
          name: validateName,
          hasError: !ruleResolve(values, rule),
          rule: rule,
          value: values,
          ruleResolve: ruleResolve
        };
        if (!self.config.noMessage) result.message = rule.message;
      }
      return result;
    };
  };
  Validate.prototype.registerFatory = function(validateName, ruleResolve) {
    var self = this;
    if (ruleResolve instanceof Function) {
      return function(rules) {
        //定义validate()
        var validate = self.validateFatory(validateName, rules, ruleResolve);
        //定义message()
        var messageFunc = function(message) {
          rules.message = message;
          return { validate: validate };
        };
        return {
          rules: rules,
          ruleResolve: ruleResolve,
          validate: validate,
          message: messageFunc
        };
      };
    }
  };
  return Validate;
})();

var ValidateRuleResolves = {
  required: function(value, params) {
    if (
      value == undefined ||
      value == null ||
      (typeof value == "number" && Number.isNaN(value)) ||
      (typeof value == "string" && value.trim() == "")
    ) {
      return false;
    } else return true;
  },
  pattern: function(value, params) {
    return params.pattern.test(value);
  },
  min: function(value, params) {
    if (typeof value == "number") {
      return value >= params.min;
    } else if (value.length != undefined) {
      return value.length >= params.min;
    }
  },
  max: function(value, params) {
    if (typeof value == "number") {
      return value <= params.max;
    } else if (value.length != undefined) {
      return value.length <= params.max;
    }
  },
  validate: function(value, params) {
    return params.validate(value);
  },
  remote: function(value, params) {
    var res = $.ajax(params.url, {
      type: params.type || "get",
      data: params.data(),
      async: false
    });
    return res.responseJSON.success;
  }
};
