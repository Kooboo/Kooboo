(function() {
  var self;
  Vue.component("kb-relation-modal", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/kbRelationModal.html"
    ),
    beforeCreate: function() {
      self = this;
    },
    data: function() {
      return {
        isShow: false,
        by: "",
        relations: [],
        loading: true
      };
    },
    methods: {
      reset: function() {
        self.isShow = false;
        self.relations = [];
        self.loading = true;
      },
      getRelationEditUrl: function(rel) {
        var url = "";
        switch (self.by.toLowerCase()) {
          case "layout":
            url = Kooboo.Route.Get(Kooboo.Route.Layout.DetailPage, {
              Id: rel.objectId
            });
            break;
          case "view":
            url = Kooboo.Route.Get(Kooboo.Route.View.DetailPage, {
              Id: rel.objectId
            });
            break;
          case "page":
            url = Kooboo.Route.Get(Kooboo.Route.Page.EditRedirector, {
              Id: rel.objectId
            });
            break;
          case "textcontent":
            url = Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
              Id: rel.objectId
            });
            break;
          case "style":
          case "cssdeclaration":
            url = Kooboo.Route.Get(Kooboo.Route.Style.DetailPage, {
              Id: rel.objectId
            });
            break;
          case "menu":
            url = Kooboo.Route.Get(Kooboo.Route.Menu.DetailPage, {
              Id: rel.objectId
            });
            break;
          case "htmlblock":
            url = Kooboo.Route.Get(Kooboo.Route.HtmlBlock.DetailPage, {
              Id: rel.objectId
            });
            break;
          case "form":
            url = Kooboo.Route.Get(Kooboo.Route.Form.Redirector, {
              Id: rel.objectId
            });
            break;
          case "datamethodsetting":
            url = Kooboo.Route.Get(Kooboo.Route.DataSource.DataMethodSetting, {
              Id: rel.objectId
            });
            break;
          default:
            url = "";
            /*  window.info.show("Unhandle relation type: " + self.by(), false);
                        console.warn("unhandle relation type:" + self.by());*/
            break;
        }
        return url;
      }
    },
    mounted: function() {
      Kooboo.EventBus.subscribe("kb/relation/modal/show", function(comm) {
        self.by = comm.by;
        self.loading = true;
        self.isShow = true;
        Kooboo.Relation.showBy({
          id: comm.id,
          by: comm.by,
          type: comm.type
        }).then(function(res) {
          if (res.success) {
            self.relations = res.model;
            self.loading = false;
          } else {
            self.loading = false;
            self.isShow = false;
          }
        });
      });
    }
  });
})();
