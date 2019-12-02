(function() {
  Vue.component("kb-page-widget-component-selector", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/pageEditor/widgets/component-selector.html"
    ),
    data: function() {
      return {
        isShow: false,
        title: "",
        selectContext: null,
        emptyMode: false,
        nameList: []
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.EventBus.subscribe("kb/page/layout/component/select", function(
        context
      ) {
        self.title = context.displayName;
        self.selectContext = context;
        $("#component_tree").jstree("destroy");
        if (context.data && context.data.length) {
          self.emptyMode = false;
          $("#component_tree")
            .jstree({
              plugins: ["types", "conditionalselect", "checkbox"],
              types: {
                default: {
                  icon: "fa fa-file icon-state-warning"
                }
              },
              core: {
                strings: {
                  "Loading ...": Kooboo.text.common.loading + " ..."
                },
                data: function(obj, cb) {
                  var treeData = [];
                  _.forEach(context.data, function(data) {
                    treeData.push({
                      text: data.name,
                      data: {
                        id: data.id,
                        name: data.name,
                        settings: data.settings
                      }
                    });
                  });
                  cb.call(this, treeData);
                }
              }
            })
            .on("select_node.jstree", function(e, selected) {
              self.nameList.push(selected.node.data);
            })
            .on("deselect_node.jstree", function(e, selected) {
              self.nameList = self.nameList.filter(function(f) {
                return f != selected.node.data;
              });
            });
        } else {
          self.emptyMode = true;
        }

        self.isShow = true;
      });
    },
    methods: {
      reset: function() {
        var self = this;
        self.nameError = "";
        self.showParameters = false;
        self.isShow = false;
        self.nameList = [];
      },
      save: function() {
        var self = this;
        var list = [];
        if (!self.nameList.length) return alert(Kooboo.text.validation.required);
        _.forEach(self.nameList, function(data) {
          data.type = self.selectContext.type;
          data.engine = self.selectContext.engine;
          list.push(data);
        });
        Kooboo.EventBus.publish("kb/page/layout/components/save", list);
        Kooboo.EventBus.publish("kb/page/field/change", { type: "resource" });
        self.reset();
      }
    }
  });
})();
