(function() {
  var DataStore = Kooboo.viewEditor.store.DataStore,
    ActionStore = Kooboo.viewEditor.store.ActionStore,
    DataContext = Kooboo.viewEditor.DataContext,
    Filter = Kooboo.viewEditor.util.fieldFilter;

  var bindingType = "repeat",
    repeatKey = "k-repeat",
    repeatAttr = "[" + repeatKey + "]",
    repeatSelfKey = repeatKey + "-self";

  var self;
  Vue.component("kb-view-repeat", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/viewEditor/components/repeat.html"
    ),
    data: function() {
      self = this;
      return {
        elem: null,
        isShow: false,
        fields: [],
        repeatSelf: false,
        fieldId: "",
        model: {
          text: ""
        },
        rules: {
          text: [
            {
              required: true,
              message: Kooboo.text.validation.required
            }
          ]
        }
      };
    },
    mounted: function() {
      Kooboo.EventBus.subscribe("binding/edit", function(data) {
        if (data.bindingType == bindingType) {
          self.elem = data.elem;
          self.isShow = true;
          self.refreshFields();
          self.model.text = data.text;
          self.repeatSelf = data.repeatSelf;
        }
      });

      Kooboo.EventBus.subscribe("DataStore/change", function() {
        self.refreshFields();
      });
    },
    methods: {
      refreshFields: function() {
        if (self.elem) {
          var fields = DataContext.get(self.elem).getDataSource(),
            _fields = [];

          var repeatElements = self.getRepeatElements(self.elem);

          if (repeatElements.length) {
            repeatElements.forEach(function(el) {
              var itemName = el.key.split(" ")[0],
                listName = el.key.split(" ")[1],
                find = null;

              if (self.elem == el.elem) {
                find = _.findLast(fields, function(field) {
                  if (listName.indexOf(".") > -1) {
                    return field.name == listName.split(".")[0];
                  } else {
                    return field.name == listName;
                  }
                });

                if (find) {
                  _fields.push({
                    name: find.name,
                    list: Filter.getEnumerableList(find)
                  });
                }
              } else {
                find = _.find(fields, function(field) {
                  return field.name == itemName;
                });

                if (find && find.name.match(/(\w*)_Item$/)) {
                  _fields.push({
                    name: find.name,
                    list: Filter.getEnumerableList(find)
                  });
                }
              }
            });

            var usedName = [];
            _fields.forEach(function(field) {
              if (field.name.indexOf("_Item") > -1) {
                usedName.push(field.name.split("_Item")[0]);
              } else {
                usedName.push(field.name);
              }
            });

            var filtered = _.filter(fields, function(field) {
              if (field.name.indexOf("_Item") > -1) {
                return usedName.indexOf(field.name.split("_Item")[0]) == -1;
              } else {
                return usedName.indexOf(field.name) == -1;
              }
            });

            filtered.forEach(function(field) {
              _fields.push({
                name: field.name,
                list: Filter.getEnumerableList(field)
              });
            });
          } else {
            fields.forEach(function(field) {
              _fields.push({
                name: field.name,
                list: Filter.getEnumerableList(field)
              });
            });
          }
        }

        self.fields = _.uniqBy(_fields, function(o) {
          return o.name;
        });
      },
      add: function() {
        Kooboo.EventBus.publish("action/edit", {
          parentId: null,
          context: {
            actions: DataStore.getAll()
          }
        });
      },
      valid: function() {
        return self.$refs.repeatForm.validate();
      },
      save: function() {
        if (self.valid()) {
          self.$emit("on-save", {
            bindingType: bindingType,
            elem: self.elem,
            text: self.model.text,
            repeatSelf: self.repeatSelf,
            dataSourceId: self.getDataSourceId(self.model.text) || null
          });
          self.reset();
        }
      },
      reset: function() {
        self.elem = null;
        self.model.text = "";
        self.isShow = false;
        self.$refs.repeatForm.clearValid();
      },
      getRepeatElements: function(elem) {
        var repeatElements = [],
          _parent = elem || self.elem;

        while (
          $(_parent).closest(repeatAttr).length &&
          !repeatElements.length
        ) {
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
      getDataSourceId: function(text) {
        var id = null;
        self.fields.forEach(function(field) {
          var find = _.find(field.list, function(li) {
            return li.name == text;
          });
          find && (id = find.id);
        });
        return id;
      }
    }
  });
})();
