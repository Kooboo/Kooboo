(function() {
  Kooboo.loadJS([
    "/_Admin/Scripts/lib/tinymce/tinymceInitPath.js",
    "/_Admin/Scripts/lib/tinymce/tinymce.min.js",
    "/_Admin/Scripts/lib/jstree.min.js",
    "/_Admin/Scripts/lib/moment.min.js",
    "/_Admin/Scripts/lib/bootstrap-switch/bootstrap-switch.min.js",
    "/_Admin/Scripts/lib/bootstrap-datetimepicker.js",

    "/_Admin/Scripts/tableModel.js",
    "/_Admin/Scripts/viewEditor/components/modal.js",
    "/_Admin/Scripts/textContent/embeddedDialog.js",
    "/_Admin/Scripts/vue-components/kbTable.js",
    "/_Admin/Scripts/vue-components/kbForm.js",
    "/_Admin/Scripts/vue-components/kbDatetimepicker.js",

    "/_Admin/Scripts/vue-components/controlType/TextBox.js",
    "/_Admin/Scripts/vue-components/controlType/TextArea.js",
    "/_Admin/Scripts/vue-components/controlType/RichEditor.js",
    "/_Admin/Scripts/vue-components/controlType/Number.js",
    "/_Admin/Scripts/vue-components/controlType/Selection.js",
    "/_Admin/Scripts/vue-components/controlType/CheckBox.js",
    "/_Admin/Scripts/vue-components/controlType/RadioBox.js",
    "/_Admin/Scripts/vue-components/kbSwitch.js",
    "/_Admin/Scripts/vue-components/controlType/Switch.js",
    "/_Admin/Scripts/vue-components/kb-media-dialog.js",
    "/_Admin/Scripts/vue-components/controlType/MediaFile.js",
    "/_Admin/Scripts/vue-components/kb-file-dialog.js",
    "/_Admin/Scripts/vue-components/controlType/File.js",
    "/_Admin/Scripts/vue-components/controlType/DateTime.js"
  ]);
  Kooboo.loadCSS([
    "/_Admin/Styles/jstree/style.min.css",
    "/_Admin/Styles/bootstrap-datetimepicker.min.css",
    "/_Admin/Scripts/lib/bootstrap-switch/bootstrap-switch.min.css"
  ]);

  var LANG = Kooboo.getQueryString("lang");
  var self;
  Vue.component("kb-field-panel", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/kbFieldPanel.html"
    ),
    props: {
      siteLangs: Object,
      fields: Array
    },
    data: function() {
      self = this;
      return {
        isMultiContent: !!LANG,
        mappedFields: [],
        categories: [],
        currentLangs: [],
        model: {},
        rules: {}
      };
    },
    mounted: function() {
      Kooboo.EventBus.subscribe("kb/multilang/change", function(lang) {
        if (lang.selected) {
          self.currentLangs.push(lang.name);
        } else {
          self.currentLangs = _.without(self.currentLangs, lang.name);
        }
      });
      if (!this.siteLangs) {
        self.$watch("siteLangs", function(langs) {
          if (langs) {
            if (self.currentLangs.indexOf(langs.default) == -1) {
              self.currentLangs.push(langs.default);
            }
          }
        });
      } else {
        self.currentLangs.push(self.siteLangs.default);
      }
      LANG && self.currentLangs.push(LANG);
    },
    computed: {
      multilingualSite: function() {
        return Object.keys(self.siteLangs.cultures).length > 1;
      }
    },
    methods: {
      isShow: function(field) {
        return self.currentLangs.indexOf(field.lang) > -1;
      },
      getControl: function(controlType) {
        if (controlType == "Tinymce") {
          return "kb-control-richeditor";
        } else if (controlType == "Boolean") {
          return "kb-control-switch";
        } else if (controlType) {
          return "kb-control-" + controlType.toLowerCase();
        }
        return "kb-control-textbox";
      }
    },
    watch: {
      fields: function(val) {
        if (!val || val.length === 0) {
          return [];
        }
        var defaultLang = self.siteLangs.default;
        var d = _.cloneDeep(val);
        var fields = [];
        var model = {};
        var rules = {};

        d.forEach(function(item) {
          item.fieldValue = item.values || "";
          var valueArr = [];

          var langKeys = Object.keys(item.fieldValue),
            defaultLangIdx = langKeys.indexOf(defaultLang);

          langKeys.splice(defaultLangIdx, 1);

          valueArr.push({
            lang: defaultLang,
            value: item.fieldValue[defaultLang]
          });

          langKeys.forEach(function(key) {
            valueArr.push({
              lang: key,
              value: item.fieldValue[key]
            });
          });

          valueArr.forEach(function(v) {
            if (
              ["boolean", "switch"].indexOf(item.controlType.toLowerCase()) > -1
            ) {
              if (v.value && typeof v.value == "string") {
                v.value = JSON.parse(v.value.toLowerCase());
              } else {
                v.value = true;
              }
            }
            var field = {
              _id: Kooboo.getRandomId(),
              lang: v.lang,
              name: item.name,
              fieldName:
                item.isMultilingual && self.multilingualSite
                  ? item.displayName + " (" + v.lang + ")"
                  : item.displayName,
              prop: item.name + "_" + v.lang,
              controlName: self.getControl(item.controlType),
              disabled: self.isMultiContent ? v.lang !== LANG : false,
              tooltip: item.toolTip,
              options: JSON.parse(
                item.options || item.selectionOptions || "[]"
              ),
              isMultipleValue: item.multipleValue
            };
            fields.push(field);
            model[field.prop] = item.multipleValue
              ? !v.value
                ? []
                : JSON.parse(v.value)
              : v.value;
          });
        });
        self.mappedFields = fields;
        self.model = model;
        self.rules = rules;
      }
    },
    beforeDestroy: function() {
      self = null;
    }
  });

  function Field(data) {
    this._id = Kooboo.getRandomId();
    this.name = data.name;
    this.fieldName = data.displayName;
    this.fieldValue = data.fieldValue;
    this.prop = data.name + "_" + data.lang;
    this.lang = data.lang;
    this.controlType = data.controlType;

    if (data.controlType == "Tinymce") {
      this.controlName = "kb-control-richeditor";
    } else if (data.controlType == "Boolean") {
      this.controlName = "kb-control-switch";
    } else if (data.controlType) {
      this.controlName = "kb-control-" + data.controlType.toLowerCase();
    } else {
      this.controlName = "kb-control-textbox";
    }

    this.disabled = data.disabled;
    this.tooltip = data.tooltip;
    this.options = JSON.parse(data.options || data.selectionOptions || "[]");
    this.isMultilingual = data.isMultilingual && data.isMultilingualSite;
    this.isMultipleValue = data.multipleValue;

    // var _validations = JSON.parse(data.validations || "[]") || [],
    //   validateRules = {};

    // this.validations = ko.observableArray(_validations);
    // for (var i = 0, len = _validations.length; i < len; i++) {
    //   var rule = _validations[i],
    //     type = rule.type || rule.validateType,
    //     msg = rule.msg || rule.errorMessage;

    //   switch (type) {
    //     case "required":
    //       validateRules[type] = msg || Kooboo.text.validation.required;
    //       break;
    //     case "regex":
    //       validateRules[type] = {
    //         pattern: new RegExp(rule.pattern),
    //         message: msg || Kooboo.text.validation.inputError
    //       };
    //       break;
    //     case "range":
    //       validateRules[type] = {
    //         from: Number(rule.min),
    //         to: Number(rule.max),
    //         message: msg || Kooboo.text.validation.inputError
    //       };
    //       break;
    //     case "stringLength":
    //       validateRules["stringlength"] = {
    //         min: parseInt(rule.min),
    //         max: parseInt(rule.max),
    //         message: msg || Kooboo.text.validation.inputError
    //       };
    //       break;
    //     case "min":
    //     case "max":
    //     case "minLength":
    //     case "maxLength":
    //     case "minChecked":
    //     case "maxChecked":
    //       validateRules[type] = {
    //         value: Number(rule.value),
    //         message: msg || Kooboo.text.validation.inputError
    //       };
    //       break;
    //   }
    // }
    // this.fieldValue.extend({ validate: validateRules });
    // this.validateRules = validateRules;

    // this.isValid = function() {
    //   return this.fieldValue.isValid();
    // };
  }
})();
