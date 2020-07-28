﻿(function() {
  Kooboo.loadJS([
    "/_Admin/Scripts/components/kbForm.js",
    "/_Admin/Scripts/components/controlType/TextBox.js",
    "/_Admin/Scripts/components/controlType/Selection.js",
    "/_Admin/Scripts/components/controlType/CheckBox.js"
  ]);
  var self;
  Vue.component("kb-activity-dialog", {
    template: Kooboo.getTemplate("/_Admin/Scripts/events/activityDialog.html"),
    props: {
      modalShow: Boolean,
      activityData: Object
    },
    data: function() {
      self = this;
      return {
        name: "",
        settings: [],
        container: [],
        cacheData: {},
        mode: "",
        model: {},
        rules: {}
      };
    },
    methods: {
      submit: function() {
        if (self.$refs.form && self.$refs.form.validate()) {
          var data = self.cacheData,
                   values = {};
          self.settings.forEach(function(setting) {
            values[setting.name] = self.model[setting.prop];
            // var orig = _.find(data.settings, function(s) {
            //   return s.name == setting.name;
            // });
            // orig && (orig.defaultValue = self.model[setting.prop]);
          });
          data.values = values;
          self.$emit("on-save", data);
          self.close();

          // console.log(params);
          // console.log(self.container);
          // //transfer setting from [{name:xx, values:xx, controlType: "textBox", defaultValue: "", values:""}] to {key: value, key: value}
          // var o = {}
          // self.setting().forEach(function(setting) {
          //     o[setting.name()] = setting.values;
          // })
          // self.activity.setting = o;
          // if (params.activityData().mode === "edit") {
          //     var index = params.activityData().index;
          //     self.container.splice(index, 1, self.activity);
          // } else {
          //     self.container.push(self.activity);
          // }
          // self.close()
        }else {
          self.close();
        }
      },
      close: function() {
        self.$emit("update:modalShow", false);
      },
      getFieldRules: function(validations) {
        var rules = [];
        validations.forEach(function(rule) {
          var type = rule.type || rule.validateType,
            msg = rule.msg || rule.errorMessage;

          switch (type) {
            case "required":
              rules.push({
                required: true,
                message: msg || Kooboo.text.validation.required
              });
              break;
            case "regex":
              rules.push({
                pattern: new RegExp(rule.pattern),
                message: msg || Kooboo.text.validation.inputError
              });
              break;
            case "range":
            case "stringLength":
              rules.push({
                min: Number(rule.min),
                max: Number(rule.max),
                message: msg || Kooboo.text.validation.inputError
              });
              break;
            case "min":
            case "minLength":
            case "minChecked":
              rules.push({
                min: Number(rule.value),
                message: msg || Kooboo.text.validation.inputError
              });
              break;
            case "max":
            case "maxLength":
            case "maxChecked":
              rules.push({
                max: Number(rule.value),
                message: msg || Kooboo.text.validation.inputError
              });
              break;
          }
        });
        return rules;
      }
    },
    computed: {
      visibleActivityData: function() {
        if (self.modalShow) {
          return self.activityData ? self.activityData : null;
        }
      }
    },
    watch: {
      visibleActivityData: function(data) {
        if (!data) {
          return;
        }
        self.name = data.name;
        self.mode = data.mode;
        self.cacheData = data;
        var fields = [];
        var model = {};
        var rules = {};
        data.settings.forEach(function(setting) {
          setting.displayName = setting.displayName || setting.name;
          setting.fieldValue = setting.defaultValue;
          if (data.values) {
            setting.fieldValue = data.values[setting.name];
          } else if (data.setting) {
            setting.fieldValue = getValue(data.setting, setting.name);
          }
          if (setting.controlType == "Selection") {
            setting.selectionOptions = JSON.stringify(
              Kooboo.objToArr(setting.selectionValues).map(function(s) {
                return {
                  key: s.value,
                  value: s.key
                };
              })
            );
          }
          var field = new Field(setting);

          // fields
          fields.push(field);
          // model
          model[field.prop] = field.fieldValue;
          // rules
          var validations = JSON.parse(data.validations || "[]") || [];
          var fieldRules = self.getFieldRules(validations);
          if (fieldRules.length) {
            rules[field.prop] = fieldRules;
          }
        });
        self.settings = fields;
        self.model = model;
        self.rules = rules;
      }
    }
  });
  function Field(data) {
    this._id = Kooboo.getRandomId();
    this.showError = false;
    this.isShow = data.isShow;
    this.name = data.name;
    this.prop = this.name;
    this.fieldName = data.displayName;
    this.fieldValue = data.fieldValue;
    this.lang = data.lang;
    this.controlType = data.controlType;
    this.controlName = "kb-control-" + data.controlType.toLowerCase();
    this.disabled = data.disabled;
    this.tooltip = data.tooltip;
    this.options = JSON.parse(data.options || data.selectionOptions || "[]");
    this.isMultilingual = data.isMultilingual && data.isMultilingualSite;
    this.isMultipleValue = data.multipleValue;
  }

  function getValue(setting, name) {
    var keys = Object.keys(setting).map(function(key) {
        return key.toLowerCase();
      }),
      idx = keys.indexOf(name.toLowerCase());
    return idx > -1 ? setting[keys[idx]] : "";
  }
})();
