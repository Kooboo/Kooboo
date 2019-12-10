(function() {
  Vue.component("kb-relation-modal", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/kbRelationModal.html"
    ),
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
        this.isShow = false;
        this.relations = [];
        this.loading = true;
      },
      getRelationEditUrl: function(rel) {
        var url = "";
        switch (this.by.toLowerCase()) {
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
            /*  window.info.show("Unhandle relation type: " + this.by(), false);
                        console.warn("unhandle relation type:" + this.by());*/
            break;
        }
        return url;
      }
    },
    mounted: function() {
      var self = this;
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

        // a hack for image loading twice
        if (comm.type === "Image") {
          self.$nextTick(function() {
            $(".modal-backdrop:eq(1)").remove();
          });
        }
      });
    }
  });
})();
