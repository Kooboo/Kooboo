(function() {
  Kooboo.loadJS(["/_Admin/Scripts/Kooboo/ControlType.js"]);

  var template = Kooboo.getTemplate(
    "/_Admin/Scripts/components/kbFieldEditor.html"
  );

  var CONTROL_TYPES = Kooboo.controlTypes;

  ko.components.register("kb-field-editor", {
    viewModel: function(params) {
      var self = this;

      this.showError = ko.observable(false);

      this.field = ko.observable();

      this.controlTypes = ko.observableArray();
      this._controlTypes = ko.observableArray();

      this.showPreviewPanel = ko.observable(params.options.showPreviewPanel);
      this.previewImage = ko.observable();
      this.previewType = ko.observable();

      this.showMultilingualOption = ko.observable(
        params.options.showMultilingualOption
      );
      this.multilinguable = ko.observable(false);

      this.showFilePanel = ko.observable(false);

      this.showDBColumnPanel = ko.observable(
        !!params.options.showDBColumnPanel
      );

      this.onShow = params.onShow;
      this.onShow.subscribe(function(show) {
        if (show) {
          var field = new Field(
            _.assignIn(params.data() || { editable: true }, params.options)
          );

          field.on("controlType/change", function(type) {
            var t = CONTROL_TYPES.find(function(o) {
              return o.value.toLowerCase() == type.toLowerCase();
            });

            self.field().dataType(t.dataType || "Undefined");

            self.previewImage(t ? t.preview : "");
            self.previewType(t ? t.value : "");
            self.multilinguable(
              t ? t.dataType.toLowerCase() == "string" : false
            );

            if (
              [
                "boolean",
                "switch",
                "selection",
                "fixedspec",
                "dynamicspec"
              ].indexOf(type.toLowerCase()) > -1
            ) {
              self.showValidationTab(false);
            } else {
              self.showValidationTab(true);
            }

            if (
              ["selection", "checkbox", "radiobox", "fixedspec"].indexOf(
                type.toLowerCase()
              ) > -1
            ) {
              self.showAddOptionPanel(true);
            } else {
              self.showAddOptionPanel(false);
            }

            if (["mediafile", "file"].indexOf(type.toLowerCase()) > -1) {
              self.showFilePanel(true);
            } else {
              self.showFilePanel(false);
            }
          });

          field.on("controlTypes/change", function(types) {
            self.controlTypes(getControlTypes(types));
          });

          self.field(field);

          if (self.field().isNewField()) {
            self.controlTypes(self._controlTypes());
          } else {
            if (!self.field().isSystemField()) {
              if (
                self
                  .field()
                  .controlType()
                  .toLowerCase() == "tinymce"
              ) {
                self.field().controlType("RichEditor");
              }
              var type = self._controlTypes().find(function(t) {
                  return (
                    t.value.toLowerCase() ==
                    self
                      .field()
                      .controlType()
                      .toLowerCase()
                  );
                }),
                types = [];

              var dataType = type.dataType;
              self._controlTypes().forEach(function(t) {
                if (t.dataType == dataType) {
                  types.push(t);
                }
              });
              self.controlTypes(types);
            }
          }

          setTimeout(function() {
            field.emit("controlType/change", field.controlType());
          }, 300);
        }
      });

      this.onSave = params.onSave;

      this.showValidationTab = ko.observable(true);
      this.showAddOptionPanel = ko.observable(false);
      this.showAddOptionPanel.subscribe(function(show) {
        if (!show) {
          self.field()._optionRequired("OPTION");
          self
            .field()
            .options()
            .forEach(function(opt) {
              opt.showError(false);
            });
          self.field().options.removeAll();
        }
      });

      this.onModalSave = function() {
        if (this.showAddOptionPanel()) {
          this.field()._optionRequired(
            this.field().options().length ? "OPTIONS" : ""
          );
        }

        if (this.field().isValid()) {
          this.onSave(this.demappingField(this.field()));
          this.onHide();
        } else {
          this.field().hideError();
          if (!this.field().isBasicValid()) {
            this.curTab("basic");
          } else {
            this.curTab("validation");
          }
          this.showError(true);
          this.field().showAllError();
        }
      };

      this.onHide = function() {
        this.controlTypes([]);
        this.showAddOptionPanel(false);
        this.showFilePanel(false);
        this.field().hideError();
        this.field(null);
        this.showError(false);
        this.onShow(false);
        this.curTab("basic");
      };

      this.curTab = ko.observable("basic");
      this.tabs = ko.observableArray([
        {
          value: "basic",
          displayText: Kooboo.text.component.fieldEditor.basic
        },
        {
          value: "advanced",
          displayText: Kooboo.text.component.fieldEditor.advanced
        }
      ]);
      this.changeTab = function(m, e) {
        if (m.value !== self.curTab()) {
          self.curTab(m.value);
        }
      };
      this.changeTabToValidation = function() {
        if (self.curTab() !== "validation") {
          self.curTab("validation");
        }
      };

      if (params.options.controlTypes && params.options.controlTypes.length) {
        // TODO：若参数变化，controltype 取到的值就是旧的值了
        self._controlTypes(getControlTypes(params.options.controlTypes));
      }

      this.demappingField = function(field) {
        var options = [];
        if (field.options().length) {
          options = field.options().map(function(opt) {
            return opt.getValue();
          });
        }

        var validations = [];
        if (field._validations().length) {
          validations = field._validations().map(function(v) {
            return v.getValue();
          });
        }

        if (!this.multilinguable()) {
          field.multilingual(false);
        }

        var result = {
          name: field.name(),
          displayName: field.displayName() || field.name(),
          controlType: field.controlType(),
          editable: field.isSystemField() ? field.editable() : true,
          options: JSON.stringify(options),
          selectionOptions: JSON.stringify(options),
          tooltip: field.tooltip(),
          multilingual: field.multilingual(),
          multipleLanguage: field.multilingual(),
          isSystemField: field.isSystemField(),
          multipleValue:
            field.controlType().toLowerCase() == "checkbox"
              ? true
              : field.isMultipleValue(),
          validations: JSON.stringify(validations),
          dataType: field.dataType
        };

        if (field.modifiedField()) {
          result[field.modifiedFieldName] = field[field.modifiedFieldName]();
        }

        return result;
      };

      function getControlTypes(types) {
        var _types = [];
        types.forEach(function(t) {
          var _t = CONTROL_TYPES.find(function(c) {
            return c.value.toLowerCase() == t;
          });

          _types.push(_t || { displayName: "NOT_FOUND" });
        });

        return _types;
      }
    },
    template: template
  });

  function Field(data) {
    var self = this;

    this.showError = ko.observable(false);
    this.hasError = ko.observable(false);

    this.isNewField = ko.observable(!data.name);

    this.name = ko.validateField(data.name || "", {
      required: "",
      regex: {
        pattern: /^([A-Za-z][\w\-]*)*[A-Za-z0-9]$/,
        message: Kooboo.text.validation.contentTypeNameRegex
      },
      localUnique: {
        compare: function() {
          if (self.name) {
            return _.concat(self.name(), data.getFieldNames());
          } else {
            return data.getFieldNames();
          }
        }
      }
    });

    this.displayName = ko.observable(data.displayName || "");

    this.editable = ko.observable(data.editable || false);

    this.controlType = ko.observable(data.controlType || "");

    this.controlType.subscribe(function(type) {
      self.emit("controlType/change", type);
    });

    this.dataType = ko.observable();

    var _options = JSON.parse(
      data.options || data.selectionOptions || "[]"
    ).map(function(o) {
      return new Option(o);
    });
    this.options = ko.observableArray(_options);

    this.tooltip = ko.observable(data.tooltip || "");

    this.validations = ko.observableArray(JSON.parse(data.validations || "[]"));
    this._validations = ko.observableArray();

    this.addOption = function() {
      if (self.isOptionsValid()) {
        self.options.push(new Option({}));
        self._optionRequired("OPTION");
      }
    };
    this.removeOption = function(m, e) {
      m.showError(false);
      self.options.remove(m);

      self._optionRequired(self.options().length ? "OPTION" : "");
    };
    this._optionRequired = ko.validateField({
      required: ""
    });

    this.isMultipleValue = ko.observable(
      data.isMultipleValue || data.multipleValue || false
    );
    this.isSystemField = ko.observable(data.isSystemField || false);
    var multilingual =
      data.hasOwnProperty("multilingual") ||
      data.hasOwnProperty("multipleLanguage")
        ? data.multilingual || data.multipleLanguage
        : true;
    this.multilingual = ko.observable(multilingual);

    this.modifiedField = ko.observable(data.modifiedField || false);
    this.modifiedFieldText = ko.observable(data.modifiedFieldText || "");

    if (data.modifiedField) {
      this.modifiedFieldName = data.modifiedField;
      this[data.modifiedField] = ko.observable(
        data[data.modifiedField] || false
      );
      if (data.modifiedFieldSubscriber) {
        this[data.modifiedField].subscribe(function(value) {
          data.modifiedFieldSubscriber(self, value);
        });
      }
    }

    this.isValid = function() {
      return this.isBasicValid() && this.isValidationValid();
    };

    this.isBasicValid = function() {
      var flag = this.name.isValid();
      if (self.options().length) {
        flag = flag && this.isOptionsValid();
      }

      if (
        ["selection", "checkbox", "radiobox", "fixedspec"].indexOf(
          this.controlType().toLowerCase()
        ) > -1
      ) {
        flag = flag && this._optionRequired.isValid();
      }

      return flag;
    };

    this.isValidationValid = function() {
      if (
        ["boolean", "switch", "selection"].indexOf(
          this.controlType().toLowerCase()
        ) > -1
      ) {
        return true;
      } else {
        var flag = true;
        if (this._validations().length) {
          this._validations().forEach(function(v) {
            if (!v.isValid()) {
              flag = false;
            }
          });
        }
        return flag;
      }
    };

    this.isOptionsValid = ko.pureComputed(function() {
      var flag = true;
      self.options().forEach(function(opt) {
        !opt.isValid() && (flag = false);
      });

      return flag;
    });

    this.showAllError = function() {
      this.showError(true);

      if (this.options().length) {
        this.options().forEach(function(opt) {
          opt.showError(true);
        });
      }

      if (this._validations().length) {
        this._validations().forEach(function(v) {
          v.showError(true);
        });
      }
    };
    this.hideError = function() {
      this.showError(false);
      this.options().forEach(function(opt) {
        opt.showError(false);
      });
      if (this._validations().length) {
        this._validations().forEach(function(v) {
          v.showError(false);
        });
      }
    };

    this.events = {};
    this.emit = function(event, data) {
      if (!this.events[event] || this.events[event].length == 0) return;
      this.events[event].forEach(function(fn) {
        fn(data);
      });
    };
    this.on = function(event, fn) {
      if (!this.events[event]) this.events[event] = [];
      this.events[event].push(fn);
    };
  }

  function Option(data) {
    this.showError = ko.observable(false);

    this.key = ko.validateField(data.key || "", { required: "" });

    this.value = ko.validateField(data.value || "", { required: "" });

    this.isValid = function() {
      return this.key.isValid() && this.value.isValid();
    };

    this.getValue = function() {
      return { key: this.key(), value: this.value() };
    };
  }

  Kooboo.loadJS(["/_Admin/Scripts/components/kbFieldValidation.js"]);
})();
