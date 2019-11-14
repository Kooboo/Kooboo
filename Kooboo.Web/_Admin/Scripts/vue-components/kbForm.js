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
      simple: Boolean
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
          var model = this.model[item.prop];
          var rules = this.rules[item.prop];

          if (!item.prop || model == undefined || rules == undefined) {
            continue;
          }

          // object array fields
          if (item.prop.lastIndexOf("]") !== -1) {
            var arrayProp = item.prop.match(/\w+/g);
            // using correctly
            if (arrayProp.length === 3) {
              var objRule = this.rules[arrayProp[0] + "[]"];
              if (objRule) {
                rules = objRule[arrayProp[2]];
                var arrModel = this.model[arrayProp[0]];
                if (arrModel) {
                  var objModel = arrModel[arrayProp[1]];
                  if (objModel) {
                    model = objModel[arrayProp[2]];
                  }
                }
              }
            }
          }

          if (!outsideCall) {
            rules = rules.filter(function(f) {
              return !f.remote;
            });
          }
          var result = Kooboo.validField(model, rules);

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
      "<div v-if=\"kbForm.simple\"><slot :error=\"msg\"></slot></div><div v-else class='form-group' :class=\"{'has-error':!valid}\" v-kb-tooltip:right.manual.error='msg' :data-container='errorContainer'><slot></slot></div>",
    props: {
      prop: String,
      errorContainer: String
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
