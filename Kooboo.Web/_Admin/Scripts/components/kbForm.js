(function() {
  Vue.component("kb-form", {
    template:
      "<div v-if=\"simple\"><slot></slot></div><div v-else :class=\"{'form-horizontal':align=='horizontal'}\"><slot></slot></div>",
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
      },
      simple: Boolean,
      errorVisible: {
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
          if (!item.prop) {
            continue;
          }
          var value = this.model[item.prop];
          var rules = this.rules[item.prop];
          // if it's object array fields
          if (
            value === undefined &&
            rules === undefined &&
            item.prop.indexOf("].") !== -1
          ) {
            var arrayProp = item.prop.match(/\w+/g);
            var objRule = this.rules[arrayProp[0] + "[]"];
            if (objRule) {
              rules = this.rules[arrayProp[0] + "[]"][arrayProp[2]];
              var objValue = this.model[arrayProp[0]][arrayProp[1]];
              if (objValue) {
                value = objValue[arrayProp[2]];
              }
            }
          }

          if (value === undefined || rules == undefined) {
            continue;
          }
          if (!outsideCall) {
            rules = rules.filter(function(f) {
              return !f.remote;
            });
          }
          var result = Kooboo.validField(value, rules);

          if (!result.valid) valid = false;
          item.valid = result.valid;
          item.msg = result.msg;
        }
        return valid;
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
      "<div v-if=\"kbForm.simple\"><slot :error=\"errorMessage\"></slot></div><div :data-valid-id='validId' v-else class='form-group' :class=\"{'has-error':!valid}\" v-kb-tooltip:right.manual.error='errorMessage' :data-container='errorContainer'><slot></slot></div>",
    props: {
      prop: String,
      errorContainer: {
        type: String,
        default: function() {
          this._validId = "_form" + new Date().getTime();
          return "[data-valid-id='" + this._validId + "']";
        }
      }
    },
    inject: ["kbForm"],
    provide: function() {
      return {
        kbFormItem: this
      };
    },
    computed: {
      errorMessage: function() {
        if (this.kbForm.errorVisible) {
          return this.msg;
        }
        return "";
      }
    },
    data: function() {
      var me = this;
      return {
        valid: true,
        msg: "",
        validId: me._validId
      };
    },
    mounted: function() {
      this.kbForm.formItems.push(this);
    }
  });
})();
