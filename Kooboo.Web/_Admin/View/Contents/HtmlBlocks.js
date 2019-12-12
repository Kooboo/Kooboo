$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.HTMLblocks
          }
        ],
        onCreateUrl: Kooboo.Route.Get(Kooboo.Route.HtmlBlock.Create),
        tableData: [],
        selected: []
      };
    },
    mounted: function() {
      Kooboo.HtmlBlock.getList().then(function(res) {
        if (res.success) {
          var blockList = [];
          _.forEach(res.model, function(block) {
            var date = new Date(block.lastModified);

            var model = {
              id: block.id,
              name: {
                text: block.name,
                url: Kooboo.Route.Get(Kooboo.Route.HtmlBlock.DetailPage, {
                  id: block.id
                })
              },
              date: date.toDefaultLangString(),
              relationsComm: "kb/relation/modal/show",
              relationsTypes: Object.keys(block.relations),
              relations: block.relations,
              versions: {
                iconClass: "fa-clock-o",
                title: Kooboo.text.common.viewAllVersions,
                url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
                  KeyHash: block.keyHash,
                  storeNameHash: block.storeNameHash
                }),
                newWindow: true
              }
            };

            blockList.push(model);
          });

          self.tableData = blockList;
        }
      });
    },
    methods: {
      onDelete: function() {
        var hasRel = _.some(self.selected, function(doc) {
          return doc.relations && Object.keys(doc.relations).length;
        });
        var confirmStr = hasRel
          ? Kooboo.text.confirm.deleteItemsWithRef
          : Kooboo.text.confirm.deleteItems;
        if (confirm(confirmStr)) {
          var ids = self.selected.map(function(row) {
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
