$(function() {
  var EXTERNAL_LINK = "__external_link";
  var DEFAULT_STYLE = Kooboo.getTemplate(
    "/_Admin/View/Development/kooFormTemplate.html"
  );
  var NORMAL_TABS = [
    {
      displayName: Kooboo.text.component.fieldEditor.basic,
      value: "basic"
    },
    /*{
      displayName: Kooboo.text.common.Style,
      value: "style"
    }, */
    {
      displayName: Kooboo.text.component.fieldEditor.validation,
      value: "validation"
    }
  ];
  Vue.component("slot-bridge", {
    props: ["errors", "index", "prop", "error"],
    render: function() {
      return null;
    },
    watch: {
      error: function(val) {
        this.errors[this.index] = this.errors[this.index] || {};
        this.errors[this.index][this.prop] = val;
        this.errors.splice();
      }
    }
  });
  var self;
  var nameRule = {
    name: [
      {
        required: true,
        message: Kooboo.text.validation.required
      },
      {
        validate: function(val) {
          return (
            _.filter(self.fields, function(field) {
              return val && val.toLowerCase() == field.model.name.toLowerCase();
            }).length < 2
          );
        },
        message: Kooboo.text.validation.taken
      }
    ]
  };
  var optionRule = {
    options: [
      {
        required: true,
        message: Kooboo.text.validation.required
      }
    ],
    "options[]": {
      key: [
        {
          required: true,
          message: Kooboo.text.validation.required
        }
      ],
      value: [
        {
          required: true,
          message: Kooboo.text.validation.required
        }
      ]
    }
  };
  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        id: "",
        name: "",
        model: {
          name: ""
        },
        rules: {
          name: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
              message: Kooboo.text.validation.objectNameRegex
            },
            {
              min: 1,
              max: 64,
              message:
                Kooboo.text.validation.minLength +
                1 +
                ", " +
                Kooboo.text.validation.maxLength +
                64
            },
            {
              remote: {
                url: Kooboo.Form.isUniqueName(),
                data: function() {
                  return {
                    name: self.model.name
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ]
        },
        steps: [
          {
            displayName: Kooboo.text.component.fieldEditor.field,
            value: "field"
          },
          {
            displayName: Kooboo.text.site.form.globalStyle,
            value: "style"
          },
          {
            displayName: Kooboo.text.common.setting,
            value: "setting"
          }
        ],
        curStep: "field",
        isNewForm: true,
        fields: [],
        showContentFolderSelector: false,
        contentFolders: [],
        choosedFolder: "",
        choosedFolderName: "",
        _cacheField: {},
        fieldTypes: [
          {
            displayName: Kooboo.text.component.controlType.textBox,
            value: "TextBox"
          },
          {
            displayName: Kooboo.text.component.controlType.textArea,
            value: "TextArea"
          },
          {
            displayName: Kooboo.text.component.controlType.checkBox,
            value: "CheckBox"
          },
          {
            displayName: Kooboo.text.component.controlType.radioBox,
            value: "RadioBox"
          },
          {
            displayName: Kooboo.text.component.controlType.selection,
            value: "Selection"
          },
          {
            displayName: Kooboo.text.component.controlType.plainText,
            value: "PlainText"
          },
          {
            displayName: Kooboo.text.component.controlType.divider,
            value: "Divider"
          },
          {
            displayName: Kooboo.text.component.controlType.password,
            value: "Password"
          },
          {
            displayName: Kooboo.text.component.controlType.email,
            value: "Email"
          },
          {
            displayName: Kooboo.text.component.controlType.number,
            value: "Number"
          },
          {
            displayName: Kooboo.text.component.controlType.submitBtn,
            value: "Submit"
          },
          {
            displayName: Kooboo.text.component.controlType.resetBtn,
            value: "Reset"
          },
          {
            displayName: Kooboo.text.component.controlType.submitAndResetBtn,
            value: "SubmitAndReset"
          }
        ],
        method: "post",
        allowAjax: false,
        successCallback: "",
        failedCallback: "",
        linkList: [],
        redirect: "",
        showStyleContent: false,
        styleContent: "",
        externalLink: "",
        availableSubmitters: "",
        formSubmitter: "",
        settings: []
      };
    },
    computed: {
      showExternalLinkInput: function() {
        return this.redirect === EXTERNAL_LINK;
      }
    },
    mounted: function() {
      $.when(
        Kooboo.Form.getKoobooForm({
          id: Kooboo.getQueryString("id") || Kooboo.Guid.Empty
        }),
        Kooboo.Page.getAll()
      ).then(function(r1, r2) {
        var formRes = r1[0],
          pageRes = r2[0] || r2;

        if (formRes.success && pageRes.success) {
          var pageList = pageRes.model.pages;
          pageList.push({
            name: Kooboo.text.common.externalLink,
            path: EXTERNAL_LINK
          });

          pageList.reverse();

          pageList.push({
            name: Kooboo.text.site.form.refreshSelf,
            path: "RefreshSelf()"
          });

          self.linkList = pageList.reverse();

          self.id = formRes.model.id;
          self.model.name = formRes.model.name;
          self.isNewForm = formRes.model.id == Kooboo.Guid.Empty;
          self.styleContent = formRes.model.style || DEFAULT_STYLE;
          $(".wrapper").readOnly(true);

          if (formRes.model.fields) {
            var fields = JSON.parse(formRes.model.fields);
            fields.forEach(function(field) {
              field.nameUnchangable = true;
              field.configuring = false;
              self.fields.push(fieldModel(field));
            });
          } else {
            self.fields = [];
          }

          self.method = formRes.model.method || "post";
          self.allowAjax = formRes.model.allowAjax;
          self.successCallback = formRes.model.successCallBack;
          self.failedCallback = formRes.model.failedCallBack;

          if (formRes.model.redirectUrl) {
            var _find = _.findLast(self.linkList, function(link) {
              return link.path == formRes.model.redirectUrl;
            });

            if (_find) {
              self.redirect = formRes.model.redirectUrl;
            } else {
              self.redirect = EXTERNAL_LINK;
              self.externalLink = formRes.model.redirectUrl;
            }
          } else {
            self.redirect = self.linkList[0].path;
          }

          _.forEach(formRes.model.availableSubmitters, function(submitter) {
            _.forEach(submitter.settings, function(setting) {
              setting.selectionValues = Kooboo.objToArr(
                setting.selectionValues
              );
            });
          });

          self.availableSubmitters = formRes.model.availableSubmitters;
          self.formSubmitter = formRes.model.formSubmitter || "FormValue";
        }
      });
    },
    methods: {
      changeStep: function(m) {
        if (m.value !== self.curStep) {
          self.curStep = m.value;
        }
        if (m.value.toLowerCase() == "style") {
          self.$nextTick(function() {
            self.renderPreview();
          });
        }
      },
      onCreateField: function() {
        self.unConfiguringAll();
        var newField = {
          type: self.fieldTypes[0].value,
          label: "Label",
          name: "",
          placeholder: "Placeholder",
          validations: [],
          options: [],
          advanced: [],
          configuring: false,
          isNew: true
        };
        var field = fieldModel(newField);
        self.fields.push(field);
        self._cacheField = field;
        // make validate immediately
        self.$nextTick(function() {
          self.isFieldValid(field, self.fields.length - 1);
          field.configuring = true;
        });
      },
      onCreateContentForm: function() {
        Kooboo.ContentFolder.getList().then(function(res) {
          if (res.success) {
            self.contentFolders = res.model;
            if (
              !self.choosedFolder &&
              self.contentFolders &&
              self.contentFolders.length
            ) {
              self.choosedFolder = self.contentFolders[0].id;
            }
            self.showContentFolderSelector = true;
          }
        });
      },
      onHideContentFolderSelector: function() {
        self.showContentFolderSelector = false;
      },
      onChooseContentFolder: function() {
        if (self.choosedFolder) {
          var find = _.find(self.contentFolders, function(folder) {
            return folder.id == self.choosedFolder;
          });
          find && (self.choosedFolderName = find.name);

          Kooboo.TextContent.getEdit({
            folderId: self.choosedFolder
          }).then(function(res) {
            if (res.success) {
              var fields = [];
              res.model.properties.forEach(function(prop) {
                var model = {
                  name: prop.name || "",
                  label: prop.displayName || "",
                  placeholder: prop.tooltip || "",
                  options: [],
                  validations: [],
                  advanced: [],
                  nameUnchangable: true,
                  configuring: false,
                  isNew: false
                };

                switch (prop.controlType.toLowerCase()) {
                  case "textbox":
                  case "datetime":
                    model.type = "TextBox";
                    break;
                  case "email":
                    model.type = "Email";
                    break;
                  case "textarea":
                  case "tinymce":
                    model.type = "TextArea";
                    break;
                  case "selection":
                    model.type = "Selection";
                    var _options = JSON.parse(prop.selectionOptions);
                    _options.forEach(function(opt) {
                      model.options.push(opt);
                    });
                    break;
                  case "checkbox":
                    model.type = "CheckBox";
                    var _options = JSON.parse(prop.selectionOptions);
                    _options.forEach(function(opt) {
                      model.options.push(opt);
                    });
                    break;
                  case "radiobox":
                    model.type = "RadioBox";
                    var _options = JSON.parse(prop.selectionOptions);
                    _options.forEach(function(opt) {
                      model.options.push(opt);
                    });
                    break;
                  case "number":
                    model.type = "Number";
                    break;
                }

                model.type && fields.push(fieldModel(model));
              });

              fields.push(
                fieldModel({
                  type: "SubmitAndReset",
                  label: Kooboo.text.site.form.submit,
                  placeholder: Kooboo.text.site.form.reset,
                  options: [],
                  validations: [],
                  advanced: [],
                  nameUnchangable: true,
                  configuring: false,
                  isNew: false
                })
              );

              self.fields = fields;
              self.onHideContentFolderSelector();
            }
          });
        }
      },
      switchTab: function(field, tab) {
        field.curTab = tab;
        if (field.showError) {
          self.$nextTick(function() {
            field.showError = false;
            setTimeout(function() {
              field.showError = true;
            }, 100);
          });
        }
      },
      focusErrorTab: function(errorTab, field, checkTab) {
        if (checkTab && field.curTab !== errorTab) {
          self.switchTab(field, errorTab);
        }
      },
      isFieldValid: function(field, index, checkTab) {
        var isValid = true;
        var $basicForm = self.$refs.basicForm;
        if ($basicForm && $basicForm[index]) {
          isValid = $basicForm[index].validate();
          if (!isValid) {
            self.focusErrorTab("basic", field, checkTab);
          } else if (field.validations.length > 0) {
            var $validationForm = self.$refs.formValidator;
            if ($validationForm && self.$refs.formValidator[index]) {
              isValid = $validationForm[index].validate();
              if (!isValid) {
                self.focusErrorTab("validation", field, checkTab);
              }
            }
          }
        }
        return isValid;
      },
      isAbleToAddNewField: function() {
        return self.isAbleToUnconfigureAllField();
      },
      changeFieldType: function(field) {
        var type = field.type.toLowerCase();
        if (["divider"].indexOf(type) > -1) {
          field.isPlaceholderRequired = false;
        } else if (["radiobox", "checkbox", "selection"].indexOf(type) > -1) {
          field.needOptions = true;
          field.isPlaceholderRequired = false;
        } else {
          field.needOptions = false;
          field.isPlaceholderRequired = true;
          field.model.options = [];
          field.optionRowErrors = [];
        }
        field.showError = false;
        field.rules = {};
        if (
          ["divider", "plaintext", "submit", "reset", "submitandreset"].indexOf(
            type
          ) > -1
        ) {
          field.rules = {};
        } else if (["selection", "checkbox", "radiobox"].indexOf(type) > -1) {
          field.rules = _.extend(
            {},
            field.rules,
            field.nameUnchangable ? {} : nameRule,
            optionRule
          );
        } else {
          field.rules = _.extend(
            {},
            field.rules,
            field.nameUnchangable ? {} : nameRule
          );
        }

        // validation tab
        self.$nextTick(function() {
          self.setValidationRules(field, type);
        });
      },
      isAbleToAddOption: function(field) {
        var flag = true;
        field.model.options.forEach(function(opt) {
          if (!opt.key || !opt.value) {
            flag = false;
            return flag;
          }
        });
        return flag;
      },
      addOption: function(field) {
        if (!self.isAbleToAddOption(field)) {
          return;
        }
        field.model.options.push({
          key: "",
          value: ""
        });
      },
      removeOption: function(field, index) {
        field.model.options.splice(index, 1);
        field.optionRowErrors[index] && field.optionRowErrors.splice(index, 1);
      },
      onRemoveField: function(index) {
        if (confirm(Kooboo.text.confirm.deleteItem)) {
          self.fields.splice(index, 1);
        }
      },
      onSaveField: function(m, index) {
        if (self.isFieldValid(m, index, true)) {
          m.configuring = false;
        }
        m.showError = true;
      },
      onFieldCancel: function(m, index) {
        m.configuring = false;
        m.showError = false;
        if (!self._cacheField.isNew) {
          self._cacheField.configuring = false;
          setTimeout(function() {
            var idx = _.findIndex(self.fields, function(field) {
              return field.id == m.id;
            });
            self.fields.splice(idx, 1, self._cacheField);
          }, 310);
        } else {
          setTimeout(function() {
            self.fields.splice(index, 1);
          }, 320);
        }
      },
      onEditField: function(m, e) {
        var configuring = !!m.configuring;
        if (self.isAbleToUnconfigureAllField(true)) {
          self.unConfiguringAll();
          m.configuring = !configuring;
          m.isNew = false;
          self._cacheField = m.configuring ? _.cloneDeep(m) : null;
        }
      },
      isAbleToUnconfigureAllField: function(showError) {
        var flag = true;
        self.fields.forEach(function(field, index) {
          if (field.configuring) {
            if (!self.isFieldValid(field, index)) {
              if (showError) {
                field.showError = true;
              }
              flag = false;
              return false;
            }
          }
        });
        return flag;
      },
      unConfiguringAll: function() {
        self.fields.forEach(function(field) {
          field.configuring = false;
        });
      },
      setValidationRules: function(field, type) {
        type = type.toLowerCase();
        var validationRules = [];
        switch (type) {
          case "textbox":
            validationRules = validationRules.concat([
              {
                displayName: Kooboo.text.validationRule.required,
                value: "required"
              },
              {
                displayName: Kooboo.text.validationRule.minLength,
                value: "minLength"
              },
              {
                displayName: Kooboo.text.validationRule.maxLength,
                value: "maxLength"
              },
              {
                displayName: Kooboo.text.validationRule.regex,
                value: "regex"
              }
            ]);
            break;
          case "textarea":
            validationRules = validationRules.concat([
              {
                displayName: Kooboo.text.validationRule.required,
                value: "required"
              },
              {
                displayName: Kooboo.text.validationRule.minLength,
                value: "minLength"
              },
              {
                displayName: Kooboo.text.validationRule.maxLength,
                value: "maxLength"
              }
            ]);
            break;
          case "number":
            validationRules = validationRules.concat([
              {
                displayName: Kooboo.text.validationRule.required,
                value: "required"
              },
              {
                displayName: Kooboo.text.validationRule.min,
                value: "min"
              },
              {
                displayName: Kooboo.text.validationRule.max,
                value: "max"
              }
            ]);
            break;
          case "checkbox":
            validationRules = validationRules.concat([
              {
                displayName: Kooboo.text.validationRule.required,
                value: "required"
              },
              {
                displayName: Kooboo.text.validationRule.minChecked,
                value: "minChecked"
              },
              {
                displayName: Kooboo.text.validationRule.maxChecked,
                value: "maxChecked"
              }
            ]);
            break;
          case "radiobox":
            validationRules = validationRules.concat([
              {
                displayName: Kooboo.text.validationRule.required,
                value: "required"
              }
            ]);
            break;
          case "password":
            validationRules = validationRules.concat([
              {
                displayName: Kooboo.text.validationRule.required,
                value: "required"
              },
              {
                displayName: Kooboo.text.validationRule.minLength,
                value: "minLength"
              },
              {
                displayName: Kooboo.text.validationRule.maxLength,
                value: "maxLength"
              }
            ]);
            break;
          case "email":
            validationRules = validationRules.concat([
              {
                displayName: Kooboo.text.validationRule.required,
                value: "required"
              },
              {
                displayName: Kooboo.text.validationRule.email,
                value: "email"
              }
            ]);
            break;
        }
        var result = [];
        validationRules.forEach(function(vr) {
          var find = _.some(field.validations, function(va) {
            return va.type.toLowerCase() == vr.value.toLowerCase();
          });

          !find && result.push(vr);
        });

        field.avaliableRules = result;
        if (field.avaliableRules.length) {
          field.validateType = field.avaliableRules[0].value;
        } else {
          field.validateType = "";
        }

        var newValidations = [];
        field.validations.forEach(function(valid) {
          var find = _.some(validationRules, function(rule) {
            return rule.value == valid.type;
          });
          find && newValidations.push(valid);
        });
        field.validations = newValidations;
      },
      addValidation: function(field) {
        var validate = {
          type: field.validateType,
          message: ""
        };
        if (["required", "email"].indexOf(field.validateType) === -1) {
          validate[field.validateType] = "";
        }
        field.validations.push(validate);
        self.setValidationRules(field, field.type);
        // make validate immediately
        this.$nextTick(function() {
          var error = field.showError;
          field.showError = !error;
          field.showError = error;
        });
      },
      removeValidation: function(field, index) {
        field.validations.splice(index, 1);
        self.setValidationRules(field, field.type);
      },
      onSubmit: function(cb) {
        if (self.isFormValid()) {
          var data = {};
          data.id = self.id;
          data.name = self.model.name;
          data.style = self.styleContent;
          data.body = self.getFormBody();
          data.isEmbedded = false;
          var fields = self.fields.map(function(field) {
            return _.pick(field, [
              "type",
              "label",
              "name",
              "placeholder",
              "validations",
              "options",
              "advanced",
              "isNew"
            ]);
          });
          data.fields = JSON.stringify(fields);
          data.enable = true;
          data.method = self.method;
          data.allowAjax = self.allowAjax;
          if (data.allowAjax) {
            data.successCallback = self.successCallback;
            data.failedCallback = self.failedCallback;
          }
          data.redirectUrl = self.showExternalLinkInput
            ? self.externalLink
            : self.redirect;

          if (self.choosedFolder) {
            data.formSubmitter = "ContentFolder";
            data.setting = {
              ContentFolder: self.choosedFolder
            };
          } else {
            data.formSubmitter = self.formSubmitter;
            var settings = {};
            self.settings &&
              self.settings.forEach(function(setting) {
                settings[setting.name] = setting.defaultValue;
              });
            data.setting = settings;
          }

          Kooboo.Form.updateKoobooForm(data).then(function(res) {
            if (res.success) {
              if (cb && typeof cb == "function") {
                cb(res.model);
              }
            }
          });
        }
      },
      onSave: function() {
        self.onSubmit(function(id) {
          if (self.id == Kooboo.Guid.Empty) {
            location.href = Kooboo.Route.Get(Kooboo.Route.Form.KooFormPage, {
              id: id
            });
          } else {
            window.info.done(Kooboo.text.info.save.success);
          }
        });
      },
      onSaveAndReturn: function() {
        self.onSubmit(function() {
          location.href = Kooboo.Route.Form.ListPage;
        });
      },
      isFormValid: function() {
        var valid = self.isAbleToUnconfigureAllField(true);
        if (self.isNewForm) {
          var validName = self.$refs.nameForm.validate();
          valid = valid && validName;
        }
        return valid;
      },
      onBack: function() {
        location.href = Kooboo.Route.Form.ListPage + "#External";
      },
      getFormBody: function() {
        var html = "";

        html += "<style>" + self.styleContent + "</style>";

        var containerDOM = $("<div>");
        self.fields.forEach(function(field) {
          field.name = field.model.name;
          field.options = field.model.options;

          var groupDOM = $("<div>");
          $(groupDOM).addClass("form-group control-group");

          if (field.label && field.type.toLowerCase() !== "submitandreset") {
            var labelDOM = $("<label>");
            $(labelDOM)
              .text(field.label)
              .addClass("control-label");
            $(groupDOM).append(labelDOM);
          }

          var tempDOM = null;

          switch (field.type.toLowerCase()) {
            case "textbox":
              var inputDOM = $("<input>");
              $(inputDOM)
                .attr("type", "text")
                .attr("name", field.name)
                .attr("placeholder", field.placeholder)
                .addClass("form-control");
              tempDOM = inputDOM;
              break;
            case "email":
              var inputDOM = $("<input>");
              $(inputDOM)
                .attr("type", "email")
                .attr("name", field.name)
                .attr("placeholder", field.placeholder)
                .addClass("form-control");
              tempDOM = inputDOM;
              break;
            case "number":
              var inputDOM = $("<input>");
              $(inputDOM)
                .attr("type", "number")
                .attr("name", field.name)
                .attr("placeholder", field.placeholder)
                .addClass("form-control");
              tempDOM = inputDOM;
              break;
            case "checkbox":
              var divDOM = $("<div>");
              field.options.forEach(function(option) {
                var checkboxLabelDOM = $("<label>");
                var inputDOM = $("<input>");
                $(inputDOM)
                  .attr("type", "checkbox")
                  .attr("name", field.name)
                  .attr("value", option.value);

                var textDOM = $("<span>");
                $(textDOM).text(option.key);

                $(checkboxLabelDOM)
                  .addClass("checkbox-inline")
                  .append(inputDOM)
                  .append(textDOM);

                $(divDOM).append(checkboxLabelDOM);
              });
              tempDOM = divDOM;
              break;
            case "plaintext":
              var plaintextDOM = $("<p>");
              $(plaintextDOM)
                .text(field.placeholder)
                .addClass("form-control-static");
              tempDOM = plaintextDOM;
              break;
            case "divider":
              var hrDOM = $("<hr>");
              $(hrDOM).addClass("control-hr");
              tempDOM = hrDOM;
              break;
            case "radiobox":
              var inputDOM = $("<input>");
              var divDOM = $("<div>");
              field.options.forEach(function(option) {
                var radioboxLabelDOM = $("<label>");

                var inputDOM = $("<input>");
                $(inputDOM)
                  .attr("type", "radio")
                  .attr("name", field.name)
                  .attr("value", option.value);

                var textDOM = $("<span>");
                $(textDOM).text(option.key);

                $(radioboxLabelDOM)
                  .addClass("radio-inline")
                  .append(inputDOM)
                  .append(textDOM);

                $(divDOM).append(radioboxLabelDOM);
              });
              tempDOM = divDOM;
              break;
            case "selection":
              var selectDOM = $("<select>");
              $(selectDOM)
                .attr("name", field.name)
                .addClass("form-control");
              field.options.forEach(function(option) {
                var optionDOM = $("<option>");
                $(optionDOM)
                  .attr("value", option.value)
                  .text(option.key);
                $(selectDOM).append(optionDOM);
              });
              tempDOM = selectDOM;
              break;
            case "textarea":
              var textAreaDOM = $("<textarea>");
              $(textAreaDOM)
                .attr("name", field.name)
                .attr("placeholder", field.placeholder)
                .addClass("form-control");
              tempDOM = textAreaDOM;
              break;
            case "password":
              var passwordDOM = $("<input>");
              $(passwordDOM)
                .attr("type", "password")
                .attr("name", field.name)
                .attr("placeholder", field.placeholder)
                .addClass("form-control");
              tempDOM = passwordDOM;
              break;
            case "submit":
              var submitDOM = $("<button>");
              $(submitDOM)
                .attr("type", "submit")
                .text(field.placeholder)
                .addClass("btn submit-btn btn-block");
              tempDOM = submitDOM;
              break;
            case "reset":
              var resetDOM = $("<button>");
              $(resetDOM)
                .attr("type", "reset")
                .text(field.placeholder)
                .addClass("btn reset-btn btn-block");
              tempDOM = resetDOM;
              break;
            case "submitandreset":
              var wrapperDOM = $("<div>");
              var submitDOM = $("<button>"),
                resetDOM = $("<button>");
              $(submitDOM)
                .attr("type", "submit")
                .text(field.label)
                .addClass("btn submit-btn");
              $(resetDOM)
                .attr("type", "reset")
                .text(field.placeholder)
                .addClass("btn reset-btn");
              $(wrapperDOM)
                .append(submitDOM)
                .append(resetDOM);
              tempDOM = wrapperDOM;
              break;
          }

          $(groupDOM).append(tempDOM);

          var errorContainerDOM = $("<div>");
          $(errorContainerDOM).addClass("help-block");
          $(groupDOM).append(errorContainerDOM);

          field.validations.forEach(function(validation) {
            var _dom = $(tempDOM).find("input, textarea").length
              ? $(tempDOM).find("input, textarea")
              : $(tempDOM);
            switch (validation.type) {
              case "required":
                _dom
                  .first()
                  .attr("required", true)
                  .attr("data-validation-required-message", validation.message);
                break;
              case "min":
                _dom
                  .attr("min", validation.min)
                  .attr("data-validation-min-message", validation.message);
                break;
              case "max":
                _dom
                  .attr("max", validation.max)
                  .attr("data-validation-max-message", validation.message);
                break;
              case "minLength":
                _dom
                  .attr("minLength", validation.minLength)
                  .attr(
                    "data-validation-minLength-message",
                    validation.message
                  );
                break;
              case "maxLength":
                _dom
                  .attr("maxLength", validation.maxLength)
                  .attr(
                    "data-validation-maxLength-message",
                    validation.message
                  );
                break;
              case "minChecked":
                _dom
                  .first()
                  .attr(
                    "data-validation-minchecked-minchecked",
                    validation.minChecked
                  )
                  .attr(
                    "data-validation-minchecked-message",
                    validation.message
                  );
                break;
              case "maxChecked":
                _dom
                  .first()
                  .attr(
                    "data-validation-maxchecked-maxchecked",
                    validation.maxChecked
                  )
                  .attr(
                    "data-validation-maxchecked-message",
                    validation.message
                  );
                break;
              case "email":
                _dom
                  .find("input")
                  .attr("type", "email")
                  .attr("data-validation-email-message", validation.message);
                break;
              case "regex":
                _dom
                  .attr("pattern", validation.regex)
                  .attr("data-validation-pattern-message", validation.message);
                break;
            }
          });
          $(containerDOM).append(groupDOM);
        });

        return html + $(containerDOM).html();
      },
      renderPreview: function() {
        var $iframe = $("#preview");

        var rawHTML =
          '<!DOCTYPE html><html><head><style>*{margin:0;padding:0}form{width: 96%;}</style></head><body><form method="get" onsubmit="return false">' +
          self.getFormBody() +
          "</form></body></html>";

        var _doc = $iframe[0].contentWindow.document;
        _doc.open();
        _doc.write(rawHTML);
        _doc.close();

        $iframe.css("height", _doc.body.scrollHeight + 50);
      }
    },
    watch: {
      fields: function(fields) {
        self.$nextTick(function() {
          $(".wrapper").readOnly(!fields.length);
        });
      },
      styleContent: function(content) {
        self.$nextTick(function() {
          self.renderPreview();
        });
      },
      formSubmitter: function(name) {
        if (name) {
          var _submitter = _.findLast(self.availableSubmitters, function(as) {
            return as.name == name;
          });

          if (_submitter.settings && _submitter.settings.length) {
            _submitter.settings.forEach(function(item) {
              if (
                !item.defaultValue &&
                item.selectionValues &&
                item.selectionValues[0]
              ) {
                item.defaultValue = item.selectionValues[0].key;
              }
            });
            self.settings = _submitter.settings;
          } else {
            self.settings = null;
          }
        }
      }
    }
  });
  function fieldModel(field) {
    field.id = Kooboo.getRandomId();
    field.model = {
      name: field.name || "",
      options: field.options || []
    };
    field.needOptions = false;
    field.optionRowErrors = [];
    field.isPlaceholderRequired = true;
    field.avaliableRules = [];
    field.validateType = "";
    field.curTab = "basic";
    field.tabs = NORMAL_TABS;
    self.changeFieldType(field);
    return field;
  }
});
