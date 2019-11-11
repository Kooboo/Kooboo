(function() {
  var _rules = {
    required: function(value) {
      if (
        value == undefined ||
        value == null ||
        (typeof value == "number" && Number.isNaN(value)) ||
        (typeof value == "string" && value.trim() == "")
      ) {
        return false;
      } else return true;
    },
    pattern: function(value, pattern) {
      return pattern.test(value);
    },
    min: function(value, min) {
      if (typeof value == "number") {
        return value >= min;
      } else if (value.length != undefined) {
        return value.length >= min;
      }
    },
    max: function(value, max) {
      if (typeof value == "number") {
        return value <= max;
      } else if (value.length != undefined) {
        return value.length <= max;
      }
    },
    validate: function(value, validate) {
      return validate(value);
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

  Vue.component("kb-form", {
    template:
      "<div :class=\"{'form-horizontal':align=='horizontal'}\"><slot></slot></div>",
    props: {
      align: {
        type: String,
        default: "horizontal"
      },
      model: Object,
      rules: Object,
      autoValidate: {
        type: Boolean,
        default: true
      }
    },
    methods: {
      validate: function() {
        this.outsideCalled = true;
        return this._validate(true);
      },
      clearValid: function() {
        for (var i = 0; i < this.formItems.length; i++) {
          var item = this.formItems[i];
          item.valid = true;
          item.msg = "";
        }
        this.outsideCalled = false;
      },
      _validate: function(outsideCall) {
        var valid = true;
        for (var i = 0; i < this.formItems.length; i++) {
          var item = this.formItems[i];

          if (
            !item.prop ||
            this.model[item.prop] == undefined ||
            this.rules[item.prop] == undefined
          ) {
            continue;
          }

          var rules = this.rules[item.prop];
          if (!outsideCall) {
            rules = rules.filter(function(f) {
              return !f.remote;
            });
          }

          var result = this.validField(this.model[item.prop], rules);

          if (!result.valid) valid = false;
          item.valid = result.valid;
          item.msg = result.msg;
        }
        return valid;
      },
      validField: function(prop, rules) {
        var result = { valid: true, msg: "" };
        for (var i = 0; i < rules.length; i++) {
          var item = rules[i];
          for (var key in _rules) {
            if (item.hasOwnProperty(key)) {
              if (!_rules[key](prop, item[key])) {
                result.valid = false;
                result.msg = item.message || item[key];
                return result;
              }
            }
          }
        }
        return result;
      }
    },
    data: function() {
      return {
        formItems: [],
        outsideCalled: false
      };
    },
    provide: function() {
      return {
        kbForm: this
      };
    },
    watch: {
      model: {
        handler: function() {
          this.autoValidate && this.outsideCalled && this._validate();
        },
        deep: true
      }
    }
  });

  Vue.component("kb-form-item", {
    template:
      "<div class='form-group' :class=\"{'has-error':!valid}\" v-kb-tooltip:right.manual.error='msg'><slot></slot></div>",
    props: {
      prop: String
    },
    inject: ["kbForm"],
    data: function() {
      return {
        valid: true,
        msg: ""
      };
    },
    mounted: function() {
      this.kbForm.formItems.push(this);
    }
  });
})();