(function() {
  Vue.component("kb-table-column", {
    props: {
      label: {
        default: ""
      },
      prop: String,
      width: String,
      customSlot: Array,
      align: String,
      headClass: String | Array,
      bodyClass: String | Array,
      if: {
        type: Boolean,
        default: true
      }
    },
    render: function(h) {
      return h("div", this.customSlot);
    }
  });
  Vue.component("kb-table", {
    template: Kooboo.getTemplate("/_Admin/Scripts/components/kbTable.html"),
    props: {
      showSelect: Boolean,
      data: {
        type: Array,
        default: function() {
          return [];
        }
      },
      selected: {
        type: Array,
        default: function() {
          return [];
        }
      },
      hideEmpty: {
        type: Boolean,
        default: false
      }
    },
    data: function() {
      return {
        slots: [],
        progressorRows: []
      };
    },
    mounted: function() {
      if (this.$slots.default) this.slots = this.$slots.default;
    },
    methods: {
      selectedAll: function(e) {
        var arr = e.target.checked ? this.data : [];
        this.$emit("update:selected", arr);
      },
      selectRow: function(item) {
        var arr = [];
        if (this.itemIsSelected(item)) {
          arr = this.selected.filter(function(i) {
            return i !== item;
          });
        } else {
          arr = this.selected.concat([item]);
        }

        this.$emit("update:selected", arr);
      },
      itemIsSelected: function(item) {
        return this.selected.some(function(i) {
          return i === item;
        });
      }
    },
    watch: {
      data: function(value) {
        this.$nextTick(function() {
          if (this.$slots.default) {
            this.slots = this.$slots.default;
          }
        });
        this.$emit("update:selected", []);

        this.$emit("change", value);
      }
    }
  });
})();
