(function(window) {
  var Validate = {
    register: function(name, validate) {
      self[name] = self.registerFatory(name, validate);
    },

    validateRulesHandle: function(rules, value) {
      var temp;
      if (rules instanceof Array) {
        temp = {};
        rules.forEach(function(rule) {
          var validateName = rule.name;
          temp[validateName] = this[validateName](rule.rule).validate(value);
        });
      } else {
        var rule = rules;
        var validateName = rule.name;
        temp = this[validateName](rule.rule).validate(value);
      }
      return temp;
    },

    validateValuesHandle: function(rules, values, isMulti) {
      var result;
      if (isMulti) {
        result = {};
        var keys = Object.keys(values);
        keys.forEach(function(key) {
          var value = values[key];
          result[key] = Validate.validateRulesHandle(rules, value);
        });
      } else {
        result = Validate.validateRulesHandle(rules, values);
      }
      return result;
    },

    setRules: function(rules) {
      var self = Validate;
      return {
        validate: function(values, isMulti) {
          return self.validateValuesHandle(rules, values, isMulti);
        }
      };
    },
    setKeyRules: function(KeyRules) {
      var keys = Object.keys(KeyRules);
      return {
        validate: function(values) {
          var result = {};
          keys.forEach(function(key) {
            var rules = KeyRules[key];
            var value = values[key];
            if (!value) {
              console.error(
                "not found option.key: " + key + "at validate(values)"
              );
            }
            result[key] = self.validateRulesHandle(rules, values);
          });
          return result;
        }
      };
    },
    validateFatory: function(validateName, rule, ruleResolve) {
      return function(values, isMulti) {
        if (!rule.message) {
          console.error(
            "message undefined,you can defined by message() or options:{" +
              "message:" +
              "'i am message'" +
              "} "
          );
        }
        var result;
        if (isMulti) {
          result = {};
          var keys = Object.keys(values);
          keys.forEach(function(key) {
            var value = values[key];
            result[key] = {
              name: validateName,
              value: value,
              status: ruleResolve(value, rule),
              message: rule.message
            };
          });
        } else {
          result = {
            name: validateName,
            value: values,
            status: ruleResolve(values, rule),
            message: rule.message
          };
        }
        return result;
      };
    },

    registerFatory: function(validateName, ruleResolve) {
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
    }
  };
  var self = Validate;
  window.Validate = Validate;
})(window);
