Vue.use(window.vuelidate.default);
var helpers = validators.helpers;

Vue.resetValid = function (vueData) {
    if (vueData.validations &&
        Object.keys(vueData.validations).length > 0) {
        vueData.validations = window.validators.Extend.extendValidations(vueData.validations);
    }
    return vueData;
}
validators.Extend = {
    validateRule: function (rules, value) {
        if (!rules || rules.length == 0) {
            return {
                isValid: true,
                errors: []
            }
        };
        var isError = false;
        var errors = [];
        rules.forEach(function (rule) {
            var isValid = validators.Extend.isValid(rule, value);
            if (!isValid) {
                isError = true;
                errors.push(rule.message);
            }
        })

        return {
            isValid: !isError,
            errors: errors
        }
    },
    isValid: function (rule, value) {
        var isValid = true;

        var validator = validators[rule.type];;
        if (validator) {
            var params = validators.Extend.getValidatorParams(rule);
            if (params.length > 0) {
                isValid = validator.apply(this, params)(value);
            } else {
                isValid = validator(value);
            }

        } else {
            console.log("valid" + rule.type + "doesn't exist");
            return false;
        }
        return isValid;
    },
    getValidatorParams: function (rule) {
        var validatorParamsKeys = {
            regex: ["regex"],
            minLength: ["minLength"],
            maxLength: ["maxLength"],
            between: ["from", "to"],
            sameAs: ["field"]
        };
        var keys = [];
        if (validatorParamsKeys[rule.type]) {
            keys = validatorParamsKeys[rule.type];
        }
        var params = [];
        keys.forEach(function (para) {
            params.push(rule[para]);
        })
        return params;
    },
    extendValidations: function (validations) {
        function getValidation(validation){
            var keys=Object.keys(validation);
            keys.forEach(function(key){
                var value=validation[key];
                if(value instanceof Array){
                    var rules=value;
                    validation[key]={
                        rules:validators.multiRule(rules)
                    }
                }else if(value instanceof Object){
                    validation[key]=getValidation(value);
                } 
            });
            return validation;
        }
        return getValidation(validations);
    }
}

validators.multiRule = function (rules) {
    var param = {
        type: "rules",
        rules: rules,
        errors:[]
    };
    return helpers.withParams(param, function (value) {
        var result = validators.Extend.validateRule(rules, value);
        //param.errors=param.errors.concat(result.errors);
        //todo check,param.errors=param.errors.concat(result.errors); 
        //can't refresh the errors
        for(var i=0;i<result.errors.length;i++){
            param.errors.push(result.errors[i]);
        }

        return result.isValid;
    });
}
validators.regex = function (regex) {
    return function (value) {
        return validators.helpers.regex("regex", new RegExp(regex))(value);
    }
}
validators.unique = function (urlvalue) {
    return function (value) {
        //url=//parameterbinder.bind(url);
        //todo need wrap ajaxpost
        //api post
        //return 
    }
}
