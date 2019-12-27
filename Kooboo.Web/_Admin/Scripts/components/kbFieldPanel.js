(function() {
  Kooboo.loadJS([
    "/_Admin/Scripts/kooboo-web-editor/richEditor.min.js",
    "/_Admin/Scripts/components/tinymceMonaco/plugin.min.js",
    "/_Admin/Scripts/lib/jstree.min.js",
    "/_Admin/Scripts/lib/moment.min.js",
    "/_Admin/Scripts/lib/bootstrap-switch/bootstrap-switch.min.js",
    "/_Admin/Scripts/lib/bootstrap-datetimepicker.js",

    "/_Admin/Scripts/tableModel.js",
    "/_Admin/Scripts/viewEditor/components/modal.js",
    "/_Admin/Scripts/textContent/embeddedDialog.js",
    "/_Admin/Scripts/components/kbTable.js",
    "/_Admin/Scripts/components/kbForm.js",
    "/_Admin/Scripts/components/kbDatetimepicker.js",

    "/_Admin/Scripts/components/controlType/TextBox.js",
    "/_Admin/Scripts/components/controlType/TextArea.js",
    "/_Admin/Scripts/components/controlType/RichEditor.js",
    "/_Admin/Scripts/components/controlType/Number.js",
    "/_Admin/Scripts/components/controlType/Selection.js",
    "/_Admin/Scripts/components/controlType/CheckBox.js",
    "/_Admin/Scripts/components/controlType/RadioBox.js",
    "/_Admin/Scripts/components/kbSwitch.js",
    "/_Admin/Scripts/components/controlType/Switch.js",
    "/_Admin/Scripts/components/kb-media-dialog.js",
    "/_Admin/Scripts/components/controlType/MediaFile.js",
    "/_Admin/Scripts/components/kb-file-dialog.js",
    "/_Admin/Scripts/components/controlType/File.js",
    "/_Admin/Scripts/components/controlType/DateTime.js"
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
      "/_Admin/Scripts/components/kbFieldPanel.html"
    ),
    props: {
      siteLangs: Object,
      fields: Array,
      values: Object,
      categories: {
        type: Array,
        default: function() {
          return [];
        }
      },
      embedded: {
        type: Array,
        default: function() {
          return [];
        }
      }
    },
    data: function() {
      self = this;
      return {
        isMultiContent: !!LANG,
        mappedFields: [],
        currentLangs: [],
        model: {},
        rules: {},
        // category
        currentCategory: null,
        choosedCategory: [],
        showCategoryDialog: false,
        // embedded
        currentEmbedded: null
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
      if (!this.siteLangs || !Object.keys(this.siteLangs).length) {
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

      // Kooboo.EventBus.subscribe("kb/textContent/embedded/edit", function(
      //   choosedEmbedded
      // ) {
      //   var embeddedFolderId = choosedEmbedded.embeddedFolder.id;
      //   var index = _.findIndex(self.embedded, function(o) {
      //     return o.embeddedFolder.id === embeddedFolderId;
      //   });
      //   self.embedded.splice(index, 1);
      //   self.embedded.splice(index, 0, choosedEmbedded);
      // });
    },
    computed: {
      multilingualSite: function() {
        return Object.keys(self.siteLangs.cultures).length > 1;
      }
    },
    methods: {
      validate: function() {
        var valid = this.$refs.panelForm.validate();
        if (valid) {
          var values = {},
            groups = _.groupBy(self.mappedFields, function(f) {
              return f.lang;
            });
          var langs = Object.keys(groups);
          langs.forEach(function(key) {
            values[key] = {};
            groups[key].forEach(function(v) {
              var fieldValue = self.model[v.prop];
              if (v.isMultipleValue || $.isArray(fieldValue)) {
                fieldValue = JSON.stringify(fieldValue);
              }
              values[key][v.name] = fieldValue;
            });
          });

          var categories = {};
          self.categories.forEach(function(cate) {
            categories[cate.categoryFolder.id] = cate.contents.map(function(c) {
              return c.id;
            });
          });
          var embedded = {};
          self.embedded.forEach(function(emb) {
            embedded[emb.embeddedFolder.id] = emb.contents.map(function(e) {
              return e.id;
            });
          });

          this.$emit("update:values", {
            fieldsValue: values,
            categories: categories,
            embedded: embedded
          });
        }
        return valid;
      },
      isShow: function(field) {
        return self.currentLangs.indexOf(field.lang) > -1;
      },
      getControl: function(controlType) {
        if (controlType) {
          var type = controlType.toLowerCase();
          switch (type) {
            case "tinymce":
              return "kb-control-richeditor";
            case "boolean":
              return "kb-control-switch";
            default:
              return "kb-control-" + controlType.toLowerCase();
          }
        }
        return "kb-control-textbox";
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
      },
      // category
      onShowCategoryDialog: function(category) {
        Kooboo.TextContent.getByFolder({
          folderId: category.categoryFolder.id,
          pageSize: 99999
        }).then(function(res) {
          self.currentCategory = category;
          self.showCategoryDialog = true;
          var list = res.model["list"];
          list.forEach(function(item) {
            item.text = item.values[Object.keys(list[0].values)[0]];
          });
          var choosed = _.cloneDeep(category.contents);
          var jsTreeData = list.map(function(o) {
            var selected = choosed.some(function(choosed) {
              return choosed.id === o.id;
            });
            o.state = {};
            o.state.selected = selected;
            return o;
          });
          $("#categoryTree")
            .jstree({
              plugins: ["types", "checkbox"],
              types: {
                default: {
                  icon: "fa fa-file icon-state-warning"
                }
              },
              core: {
                strings: { "Loading ...": Kooboo.text.common.loading + " ..." },
                data: jsTreeData,
                multiple: category.multipleChoice
              }
            })
            .on("changed.jstree", function(e, data) {
              var selectedCategoryId = data.selected;
              var choosedCate = list.filter(function(o) {
                return selectedCategoryId.indexOf(o.id) > -1;
              });
              self.choosedCategory = choosedCate;
            });
        });
      },
      removeCategory: function(category, content) {
        category.contents = _.without(category.contents, content);
      },
      onSaveCategory: function() {
        self.currentCategory.contents = self.choosedCategory;
        self.onHideCategoryDialog();
      },
      onHideCategoryDialog: function() {
        $.jstree.reference("#categoryTree").destroy();
        self.showCategoryDialog = false;
        self.currentCategory = null;
      },
      // embedded
      addEmbedded: function(choosedEmbedded) {
        self.currentEmbedded = choosedEmbedded;
        $("#embeddedDialog").modal("show");
      }
    },
    watch: {
      fields: function(val) {
        if (!val || val.length === 0) {
          return [];
        }
        this.$refs.panelForm && this.$refs.panelForm.clearValid();
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
            // fields
            fields.push(field);
            // model
            model[field.prop] = item.multipleValue
              ? !v.value
                ? []
                : JSON.parse(v.value)
              : v.value;
            // rules
            var validations = JSON.parse(item.validations || "[]") || [];
            var fieldRules = self.getFieldRules(validations);
            if (fieldRules.length) {
              rules[field.prop] = fieldRules;
            }
          });
        });
        self.mappedFields = fields;
        self.model = model;
        self.rules = rules;
      }
    }
  });
})();
