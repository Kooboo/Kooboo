(function() {
  var controlBase = Vue.component("controls-base", {
    props: ["name", "value", "fields"],
    methods: {
      change: function(value) {
        // console.log(this.name, value);
        this.$emit("change", {
          name: this.name,
          value: value
        });
      }
    }
  });

  Vue.component("control-string", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/string.html"
    ),
    extends: controlBase
  });

  Vue.component("control-textarea", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/textarea.html"
    ),
    extends: controlBase
  });

  Vue.component("control-checkbox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/checkbox.html"
    ),
    extends: controlBase,
    computed: {
      checked: function() {
        return this.value === "True" || this.value === "true";
      }
    }
  });

  Vue.component("control-order", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/orderBy.html"
    ),
    extends: controlBase,
    computed: {
      selectedValue: function() {
        var self = this;
        var value = this.value;
        if (this.fields && this.fields.length) {
          if (
            !this.value ||
            !_.some(this.fields, function(field) {
              return self.value === field.name;
            })
          ) {
            value = this.fields[0].name;
            this.change(value);
          }
        }
        return value;
      }
    }
  });

  Vue.component("control-filter", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/filter.html"
    ),
    extends: controlBase,
    data: function() {
      return {
        values: []
      };
    },
    created: function() {
      var self = this;
      this.debounceChange = _.debounce(function() {
        if (self.fields && self.fields.length) {
          self.change(self.values);
        } else {
          self.change([]);
        }
      }, 400);
    },
    methods: {
      selectField: function(filter) {
        if (this.fields) {
          var choosedOperator = this.fields.filter(function(item) {
            return item.name === filter.key;
          })[0];
          if (choosedOperator) {
            filter.operators = choosedOperator.operators;
            if (
              !filter.comparison ||
              filter.operators.indexOf(filter.comparison) === -1
            ) {
              filter.comparison = filter.operators[0];
            }
          } else {
            filter.operators = [];
            filter.comparison = "";
          }
        }
      },
      add: function() {
        var filter = {
          key: this.fields ? this.fields[0].name : "Id",
          value: "",
          comparison: ""
        };
        this.selectField(filter);
        this.values.push(filter);
      },
      remove: function(i) {
        this.values.splice(i, 1);
      }
    },
    watch: {
      fields: function(val) {
        var self = this;
        this.values = [];
        if (val && val.length) {
          if (
            this.value !== undefined &&
            this.value !== "" &&
            this.value !== "[{}]"
          ) {
            var values = JSON.parse(this.value);
            if (Array.isArray(values) && values.length) {
              values.forEach(function(item) {
                var filter = {
                  key: item.FieldName,
                  value: item.FieldValue,
                  comparison: item.Comparer
                };
                self.selectField(filter);
                self.values.push(filter);
              });
            }
          }
        }
      },
      values: {
        handler: function() {
          this.debounceChange();
        },
        deep: true
      }
    }
  });

  var controlCollection = Vue.component("control-collection", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/collection.html"
    ),
    extends: controlBase,
    data: function() {
      return {
        values: []
      };
    },
    created: function() {
      var self = this;
      this.debounceChange = _.debounce(function() {
        self.change(self.values);
      }, 400);
    },
    mounted: function() {
      if (!this.value) {
        var values = JSON.parse(this.value);
        if (Array.isArray(values) && values.length) {
          this.values = values;
        } else {
          this.values = [];
        }
      } else {
        this.values = [];
      }
    },
    methods: {
      add: function() {
        this.values.push({
          value: ""
        });
      },
      remove: function(i) {
        this.values.splice(i, 1);
      }
    },
    watch: {
      values: {
        handler: function() {
          this.debounceChange();
        },
        deep: true
      }
    }
  });

  Vue.component("control-dictionary", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/dictionary.html"
    ),
    extends: controlCollection,
    methods: {
      add: function() {
        this.values.push({
          key: "",
          value: ""
        });
      }
    }
  });
})();
