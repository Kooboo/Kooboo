(function() {
  var template = Kooboo.getTemplate(
    "/_Admin/Scripts/components/kbExportModal.html"
  );
  var self;
  Vue.component("kb-export-modal", {
    template: template,
    props: {
      isShow: Boolean,
      siteId: String
    },
    data: function() {
      self = this;
      return {
        exportType: "complete",
        exportContents: [],
        selectedContent: []
      };
    },
    mounted: function() {
      Kooboo.Site.getExportStoreNames().then(function(res) {
        if (res.success) {
          res.model.forEach(function(cnt) {
            cnt.selected = false;
            self.exportContents.push(cnt);
          });
        }
      });
    },
    methods: {
      onHideExportModal: function() {
        self.exportType = "complete";
        // self.isShow = false;
        this.$emit("update:isShow", false);
        self.exportContents.forEach(function(cnt) {
          cnt.selected = false;
        });
        self.selectedContent = [];
        self.siteId && this.$emit("update:siteId", null);
      },
      onExport: function() {
        if (self.exportType == "complete") {
          window.open(
            Kooboo.Route.Get(
              Kooboo.Site.ExportUrl(),
              self.siteId
                ? {
                    siteId: self.siteId
                  }
                : null
            )
          );
          self.onHideExportModal();
        } else {
          var contents = [];
          self.selectedContent.forEach(function(cnt) {
            contents.push(cnt.name);
            contents = _.concat(contents, getRelatedContent(cnt.related));
          });

          if (contents.length) {
            var hasSiteId = Kooboo.Site.ExportStoreUrl().indexOf("?") > -1;
            window.open(
              Kooboo.Route.Get(
                Kooboo.Site.ExportStoreUrl(),
                hasSiteId
                  ? {
                      Stores: contents.join(",")
                    }
                  : {
                      SiteId: self.siteId,
                      Stores: contents.join(",")
                    }
              )
            );
            self.onHideExportModal();
          } else {
            window.info.fail(Kooboo.text.info.seleteExportStoreName);
          }
        }

        function getRelatedContent(list) {
          var _con = [];
          list &&
            list.forEach(function(c) {
              _con.push(c);

              var find = _.find(contentsData, function(cnt) {
                return cnt.name == c;
              });

              if (find) {
                _con = _.concat(_con, getRelatedContent(find.related));
              }
            });
          return _con;
        }
      },
      onContentSelected: function(m) {
        m.selected = !m.selected;
        if (m.selected) {
          self.selectedContent.push(m);
        } else {
          self.selectedContent = _.without(self.selectedContent, m);
        }
      }
    }
  });
})();
