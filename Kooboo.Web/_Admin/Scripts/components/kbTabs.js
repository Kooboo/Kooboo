(function() {
  Vue.component("kb-tabs", {
    template: Kooboo.getTemplate("/_Admin/Scripts/components/kbTabs.html"),
    data: function() {
      return {
        containers: [],
        d_active: this.active
      };
    },
    props: {
      active: {
        type: Number,
        default: 0
      },
      displayNames: {
        type: Array
      },

      showHeader: {
        type: Boolean,
        default: true
      }
    },
    watch: {
      d_active: function(value) {
        this.$emit("update:active", value);
      },
      active: function(value) {
        this.d_active = value;
      }
    },
    mounted: function() {
      var self = this;
      self.containers = this.getComponents("kb-container");
    },
    methods: {
      getComponents: function(tagName, role) {
        var list = [];
        this.$children.map(function(item) {
          if (tagName && item.$options._componentTag === tagName) {
            if (role) {
              if (item.$attrs.role && item.$attrs.role === role) {
                list.push(item);
              }
            } else {
              list.push(item);
            }
          }
        });
        return list;
      },
      changeIndex: function(event, index) {
        this.d_active = index;
        this.$emit("change", index);
      }
    }
  });
})();
