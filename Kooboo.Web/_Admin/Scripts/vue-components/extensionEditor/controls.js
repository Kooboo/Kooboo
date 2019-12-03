(function() {
  Vue.component("control-string", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/string.html"
    ),
    inheritAttrs: false,
    props: ["name", "value"],
    methods: {
      change: function(value) {
        this.$emit("change", {
          name: this.name,
          value: value
        });
      }
    }
  });

  Vue.component("control-textarea", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/textarea.html"
    ),
    inheritAttrs: false,
    props: ["name", "value"],
    methods: {
      change: function(value) {
        this.$emit("change", {
          name: this.name,
          value: value
        });
      }
    }
  });

  Vue.component("control-dictionary", {
    inheritAttrs: false,
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/dictionary.html"
    ),
    props: ["name", "value"],
    inheritAttrs: false,
    data: function() {
      return {
        values: []
      };
    },
    mounted: function() {
      if (!this.value) {
        var dictionaries = JSON.parse(this.value);
        if (dictionaries instanceof Array && dictionaries.length > 0) {
          this.values = dictionaries;
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
          key: "",
          value: ""
        });
      },
      remove: function(i) {
        this.values.splice(i, 1);
      }
    },
    watch: {
      values: {
        handler: function(val) {
          this.$emit("change", {
            name: this.name,
            value: this.values
          });
        },
        deep: true
      }
    }
  });

  Vue.component("control-collection", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/collection.html"
    ),
    inheritAttrs: false,
    props: ["name", "value"],
    data: function() {
      return {
        values: []
      };
    },
    mounted: function() {
      if (!this.value) {
        var collections = JSON.parse(this.value);
        if (collections instanceof Array && collections.length > 0) {
          this.values = collections;
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
        this.$emit("change", {
          name: this.name,
          value: this.values
        });
      }
    },
    watch: {
      values: {
        handler: function(val) {
          this.$emit("change", {
            name: this.name,
            value: this.values
          });
        },
        deep: true
      }
    }
  });

  Vue.component("control-checkbox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/checkbox.html"
    ),
    inheritAttrs: false,
    props: ["name", "value"],
    data: function() {
      return {
        valueBool: false
      };
    },
    mounted: function() {
      this.valueBool = this.value === "True" || this.value === "true";
    },
    watch: {
      valueBool: function(val) {
        this.$emit("change", {
          name: this.name,
          value: val
        });
      }
    }
  });

  Vue.component("control-order", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/orderBy.html"
    ),
    props: ["name", "value", "fields"],
    methods: {
      change: function(value) {
        this.$emit("change", {
          name: this.name,
          value: value
        });
      }
    }
  });

  function Filter(ob, fields) {
    var choosedOperator;
    if (ob.key) {
      choosedOperator = fields.filter(function(item) {
        return item.name === ob.key;
      })[0];
    }
    if (choosedOperator !== undefined) {
      ob.operators = choosedOperator.operators;
    } else {
      ob.operators = [];
    }
    return ob;
  }

  Vue.component("control-filter", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/filter.html"
    ),
    props: ["name", "value", "fields"],
    data: function() {
      return {
        values: []
      };
    },
    methods: {
      chooseField: function(filter) {
        if (this.fields) {
          var choosedOperator = this.fields.filter(function(item) {
            return item.name === filter.key;
          })[0];
          filter.operators = choosedOperator ? choosedOperator.operators : [];
          if (
            !filter.comparison ||
            filter.operators.indexOf(filter.comparison) === -1
          ) {
            filter.comparison = filter.operators[0];
          }
        }
      },
      add: function() {
        var newFilter = Filter(
          {
            key: this.fields ? this.fields[0].name : "Id",
            value: "",
            comparison: ""
          },
          this.fields
        );
        this.chooseField(newFilter);
        this.values.push(newFilter);
      },
      remove: function(i) {
        this.values.splice(i, 1);
      }
    },
    watch: {
      fields: {
        handler: function(val) {
          if (val) {
            var self = this;
            this.values = [];
            if (
              this.value !== undefined &&
              this.value !== "" &&
              this.value !== "[{}]"
            ) {
              if (
                JSON.parse(this.value) instanceof Array &&
                JSON.parse(this.value).length > 0
              ) {
                JSON.parse(self.value).forEach(function(item) {
                  console.log(item);
                  var filter = Filter(
                    {
                      key: item.FieldName,
                      value: item.FieldValue,
                      comparison: item.Comparer
                    },
                    self.fields
                  );
                  self.values.push(filter);
                });
              }
            }
          }
        }
      },
      values: {
        handler: function(val) {
          this.$emit("change", {
            name: this.name,
            value: this.values
          });
        },
        deep: true
      }
    }
  });
})();
