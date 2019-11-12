(function() {
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

          var result = Kooboo.validField(this.model[item.prop], rules);

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
      "<div class='form-group' :class=\"{'has-error':!valid}\" v-kb-tooltip:right.manual.error='msg' :data-container='errorContainer'><slot></slot></div>",
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
