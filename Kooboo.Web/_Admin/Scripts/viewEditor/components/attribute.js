(function() {
  var template = Kooboo.getTemplate(
      "/_Admin/Scripts/viewEditor/components/attribute.html"
    ),
    BindingStore = Kooboo.viewEditor.store.BindingStore,
    DataContext = Kooboo.viewEditor.DataContext,
    Filter = Kooboo.viewEditor.util.fieldFilter;

  var bindingType = "attribute",
    repeatKey = "k-repeat",
    repeatAttr = "[" + repeatKey + "]",
    repeatSelfKey = repeatKey + "-self";

  Vue.component("kb-view-attribute", {
    template: template,
    data: function() {
      return {
        isShow: false,
        elem: null,
        attributes: [],
        origIds: [],
        keys: ["alt", "class", "href", "id", "src", "style", "title", "value"],
        key: "",
        value: ""
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.EventBus.subscribe("binding/edit", function(data) {
        if (data.bindingType === bindingType) {
          self.elem = data.elem;
          self.isShow = true;
          self.attributes = _.map(Kooboo.objToArr(data.attributes), function(
            attr
          ) {
            self.handleDataStoreChanged(attr);
            return attr;
          });
          Kooboo.EventBus.publish("tal-attribute:DataStoreChanged", {
            type: "edit"
          });
          self.origIds = self.getDataSourceIds();
        }
      });

      Kooboo.EventBus.subscribe("DataStore/change", function() {
        Kooboo.EventBus.publish("tal-attribute:DataStoreChanged", {
          type: "change"
        });
      });
    },
    computed: {
      valid: function() {
        if (!this.attributes.length) return false;
        return this.attributes.every(function(e) {
          return e.key && e.value;
        });
      }
    },
    methods: {
      removeAttribute: function(m) {
        this.attributes = this.attributes.filter(function(f) {
          return f != m;
        });
      },
      insertDataField: function(attr, li) {
        attr.value = attr.value + "{" + li.name + "}";
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
      refreshFields: function(type, _self) {
        var self = this;
        if (self.elem) {
          var fields = DataContext.get(self.elem).getDataSource(),
            _fields = [];

          _.forEach(fields, function(field) {
            _fields.push({
              name: field.name,
              list: Filter.getNotEnumerableList(field)
            });
          });

          var repeatElements = self.getRepeatElements();

          if (repeatElements.length) {
            _.forEach(repeatElements, function(el) {
              var itemName = el.key.split(" ")[0];

              var find = _.find(fields, function(field) {
                return field.name == itemName;
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
                    return field.name == itemName;
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
        }

        switch (type) {
          case "change":
          case "edit":
            _self.fields = _.uniqBy(_fields, function(o) {
              return o.name;
            });
            break;
          case "add":
            if (_.isEmpty(_self.value)) {
              _self.fields = _.uniqBy(_fields, function(o) {
                return o.name;
              });
            }
            break;
        }
      },
      handleDataStoreChanged: function(item) {
        var self = this;
        Kooboo.EventBus.subscribe("tal-attribute:DataStoreChanged", function(
          data
        ) {
          self.refreshFields(data.type, item);
        });
      },
      addAttribute: function() {
        var item = { key: "", value: "" };
        this.handleDataStoreChanged(item);
        this.attributes.push(item);
        Kooboo.EventBus.publish("tal-attribute:DataStoreChanged", {
          type: "add"
        });
      },
      getRawAttributes: function() {
        var self = this;
        var rawAttribute = "";
        _.forEach(self.attributes, function(attr) {
          var value = attr.value.split('"').join("'");
          rawAttribute += attr.key;
          rawAttribute += " ";
          rawAttribute += value;
          rawAttribute += ";";
        });
        return rawAttribute;
      },
      getDataSourceIds: function() {
        var self = this;
        var ids = [];
        _.forEach(self.attributes, function(attr) {
          var list = [];
          attr.fields.forEach(function(field) {
            list = _.concat(field.list, list);
          });

          list.forEach(function(li) {
            if (attr.value.indexOf("{" + li.name + "}") > -1) {
              ids.push(li.id);
            }
          });
        });
        return _.uniq(ids);
      },
      getRemovedDataSourceId: function() {
        var self = this;
        var newIds = self.getDataSourceIds(),
          oldIds = self.origIds;
        var ids = [];
        oldIds.forEach(function(id) {
          newIds.indexOf(id) == -1 && ids.push(id);
        });

        return ids;
      },
      save: function() {
        var self = this;
        var context = {
          bindingType: bindingType,
          elem: self.elem,
          text: self.getRawAttributes(),
          ids: self.getDataSourceIds()
        };
        self.$emit("on-save", context);
        BindingStore.removeAttributeBindings(self.getRemovedDataSourceId());
        self.isShow = false;
      }
    },
    watch: {
      isShow: function(value) {
        var self = this;
        if (value) return;
        self.elem = null;
        self.attributes = [];
        self.origIds = [];
      }
    }
  });
})();
