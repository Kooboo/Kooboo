$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      return {
        urlLang: Kooboo.getQueryString("lang"),
        lang: undefined,
        breads: [],
        multiBlocks: undefined,
        tableData: [],
        tableSelected: []
      };
    },
    created: function() {
      self = this;
      self.getTableData();
      self.getLangs();
    },
    watch: {
      lang: function() {
        self.breads = [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: self.lang,
            url: "#"
          },
          {
            name: Kooboo.text.common.HTMLblocks
          }
        ];
      }
    },
    methods: {
      getTableData: function() {},
      getLangs: function() {
        Kooboo.Site.Langs().then(function(res) {
          if (res.success) {
            self.multiLangs = res.model;
            self.lang = self.multiLangs.cultures[self.urlLang];
            Kooboo.HtmlBlock.getList().then(function(res) {
              if (res.success) {
                self.tableData = res.model;
              }
            });
          }
        });
      },
      getTranslateStatus: function(item) {
        return (
          item.values.hasOwnProperty(self.urlLang) && item.values[self.urlLang]
        );
      },
      getUrl: function(row) {
        return Kooboo.Route.Get(Kooboo.Route.HtmlBlock.MultiLangDetailPage, {
          id: row.id,
          lang: self.urlLang
        });
      },
      onDelete: function() {
        var hasRel = _.some(self.tableSelected, function(doc) {
          return doc.relations && Object.keys(doc.relations).length;
        });
        var confirmStr = hasRel
          ? Kooboo.text.confirm.deleteItemsWithRef
          : Kooboo.text.confirm.deleteItems;
        if (confirm(confirmStr)) {
          var ids = self.tableSelected.map(function(row) {
            return row.id;
          });
          Kooboo.HtmlBlock.Deletes({
            ids: JSON.stringify(ids)
          }).then(function(res) {
            if (res.success) {
              self.tableData = _.filter(self.tableData, function(row) {
                return ids.indexOf(row.id) === -1;
              });
              self.selected = [];
              window.info.show(Kooboo.text.info.delete.success, true);
            }
          });
        }
      }
    }
  });
});
