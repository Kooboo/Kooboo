(function() {
  var ComparerStore = Kooboo.viewEditor.store.ComparerStore,
    DataContext = Kooboo.viewEditor.DataContext,
    Filter = Kooboo.viewEditor.util.fieldFilter;

  var bindingType = "condition",
    repeatKey = "k-repeat",
    repeatAttr = "[" + repeatKey + "]",
    repeatSelfKey = repeatKey + "-self";
  var validateModel = {
    normalConditionField: { msg: "", valid: true },
    conditionOperator: { msg: "", valid: true },
    conditionValue: { msg: "", valid: true },
    repeatConditionNumberParam: { msg: "", valid: true }
  };
  var self;
  Vue.component("kb-view-condition", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/viewEditor/components/condition.html"
    ),
    data: function() {
      return {
        elem: undefined,
        isShow: false,
        isRepeatConditionEnabled: false,
        conditionType: "normal",
        repeatConditionField: "repeat/odd",
        operators: [],
        normalConditionField: undefined,
        fields: [],
        conditionOperator: undefined,
        validateModel: validateModel,
        conditionValue: undefined,
        repeatConditionNumberParam: undefined
      };
    },
    watch: {
      normalConditionField: function(fieldName) {
        self.conditionOperator = undefined;
        self.validateModel = validateModel;
        if (!fieldName || self.conditionType !== "normal") {
          self.operators = [];
        } else {
          var field = null;
          var fields = self.fields;
          for (var i = 0; i < fields.length; i++) {
            var group = fields[i];
            for (var j = 0; j < group.list.length; j++) {
              var f = group.list[j];
              if (fieldName === f.name) {
                field = f;
                break;
              }
            }
          }
          if (!field) {
            self.operators = [];
          } else {
            var temp = field.type.split(".");
            temp[0] = temp[0].toLowerCase();
            var type = temp.join(".");
            self.operators = ComparerStore.getByType(type);
            if (self.operators[0] && self.operators[0].symbol) {
              self.conditionOperator = self.operators[0].symbol;
            }
          }
        }
      },
      conditionOperator: function() {
        self.validateModel.conditionOperator = { msg: "", valid: true };
      },
      conditionValue: function() {
        self.validateModel.conditionValue = { msg: "", valid: true };
      },
      conditionType: function() {
        self.validateModel = validateModel;
        this.$forceUpdate();
      },
      repeatConditionNumberParam: function() {
        self.validateModel.repeatConditionNumberParam = {
          msg: "",
          valid: true
        };
      }
    },
    created: function() {
      self = this;
      Kooboo.EventBus.subscribe("binding/edit", function(data) {
        if (data.bindingType === bindingType) {
          var condition = self.parseConditionExpression(data.text);
          self.elem = data.elem;
          self.refreshFields();
          self.$nextTick(function() {
            self.isRepeatConditionEnabled = self.getRepeatElements.length > 0;
            if (condition.type !== "repeat" || self.isRepeatConditionEnabled) {
              self.conditionType = condition.type;
              self.conditionValue = condition.value;
              if (condition.repeatField) {
                self.repeatConditionField = condition.repeatField;
              }
              self.repeatConditionNumberParam = condition.number;
              self.normalConditionField = condition.normalField;
              self.$nextTick(function() {
                self.conditionOperator = condition.operator;
              });
            }
            self.isShow = true;
          });
        }
      });
    },
    methods: {
      parseConditionExpression: function(exp) {
        var result = {
          type: null,
          normalField: null,
          repeatField: null,
          operator: null,
          value: null,
          number: null
        };
        var index = exp.indexOf(" ");
        if (index < 0 && exp !== "") {
          result = {
            type: "repeat",
            repeatField: exp
          };
          if (
            ["odd", "even", "first", "last"].indexOf(
              exp.split("repeat/")[1]
            ) === -1
          ) {
            result.repeatField = "repeat/";
            result.number = exp.substr("repeat/".length);
          }
          return result;
        } else {
          var operatorEndIndex = exp.indexOf(" ", index + 1) - 1;
          result = {
            type: "normal",
            normalField: exp.substr(0, index),
            operator: exp.substring(index + 1, operatorEndIndex + 1),
            value: exp.substr(operatorEndIndex + 2)
          };
          return result;
        }
      },
      refreshFields: function() {
        if (self.elem) {
          var fields = DataContext.get(self.elem).getDataSource(),
            _fields = [];
          fields.forEach(function(field) {
            _fields.push({
              name: field.name,
              list: Filter.getNotEnumerableList(field, [])
            });
          });

          var repeatElements = self.getRepeatElements();
          if (repeatElements instanceof Array) {
            repeatElements.forEach(function(el) {
              var itemName = el.key.split(" ")[0];

              var find = _.find(fields, function(field) {
                return field.name === itemName;
              });

              if (find) {
                _fields.push({
                  name: find.name,
                  list: Filter.getNotEnumerableList(find)
                });
              } else {
                var match = itemName.match(/(\w*)_Item$/);
                if (match && el.repeatSelf) {
                  itemName = match[1];
                  var _find = _.find(fields, function(field) {
                    return field.name === itemName;
                  });

                  if (_find) {
                    _fields.push({
                      name: _find.name + "_Item",
                      list: Filter.getNotEnumerableList(_find)
                    });
                  }
                }
              }
            });
          }

          _fields.forEach(function(field) {
            var list = _.uniqBy(field.list, function(li) {
              return li.name;
            });
            field.list = list;
          });
          self.fields = _.uniqBy(_fields, function(o) {
            return o.name;
          });
        }
      },
      validate: function() {
        var keyValues = {
          normalConditionField: self.normalConditionField,
          conditionOperator: self.conditionOperator,
          conditionValue: self.conditionValue
        };
        var rules = {
          normalConditionField: [
            { required: true, message: Kooboo.text.validation.required }
          ],
          conditionOperator: [
            { required: true, message: Kooboo.text.validation.required }
          ],
          conditionValue: [
            { required: true, message: Kooboo.text.validation.required }
          ]
        };
        if (self.conditionType === "repeat") {
          if (self.repeatConditionField === "repeat/") {
            keyValues = {
              repeatConditionNumberParam: self.repeatConditionNumberParam
            };
            rules = {
              repeatConditionNumberParam: [
                { required: true, message: Kooboo.text.validation.required }
              ]
            };
          } else {
            return true;
          }
        }
        var validate = Kooboo.validate(keyValues, rules);
        self.validateModel = validate.result;
        return !validate.hasError;
      },
      getRepeatElements: function(elem) {
        var repeatElements = [],
          _parent = elem || self.elem;

        while ($(_parent).closest(repeatAttr).length) {
          var __parent = $(_parent).closest(repeatAttr)[0];
          repeatElements.push({
            elem: __parent,
            key: $(__parent).attr(repeatKey),
            repeatSelf: !!$(__parent).attr(repeatSelfKey)
          });
          _parent = $(__parent).parent()[0];
        }

        return repeatElements;
      },
      save: function() {
        if (self.validate()) {
          var context = {
            bindingType: bindingType,
            elem: self.elem,
            text: self.getConditionExpression()
          };
          self.$emit("on-save", context);
          self.reset();
        }
      },
      reset: function() {
        self.isShow = false;
        self.normalConditionField = "";
        self.validateModel = validateModel;
      },
      getConditionExpression: function() {
        if (self.conditionType === "normal") {
          return (
            self.normalConditionField +
            " " +
            self.conditionOperator +
            " " +
            self.conditionValue
          );
        }
        if (self.repeatConditionField === "repeat/") {
          return "repeat/" + self.repeatConditionNumberParam;
        }
        return self.repeatConditionField;
      }
    }
  });
})();
