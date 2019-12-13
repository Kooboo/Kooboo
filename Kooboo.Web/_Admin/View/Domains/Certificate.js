$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      return {
        tableData: [],
        tableDataSelected: [],
        modalShow: false,
        domainList: [],
        subDomain: undefined,
        selectedDomain: undefined
      };
    },
    created: function() {
      self = this;
    },
    methods: {
      getConfirmMessage: function(doc) {
        if (doc.relations) {
          var reorderedKeys = _.sortBy(Object.keys(doc.relations));
          doc.relationsTypes = reorderedKeys;
        }
        var find = _.find(doc, function(item) {
          return item.relations && Object.keys(item.relations).length;
        });

        if (!!find) {
          return Kooboo.text.confirm.deleteItemsWithRef;
        } else {
          return Kooboo.text.confirm.deleteItems;
        }
      },
      getTableData: function() {
        Kooboo.Certificate.getList().then(function(res) {
          if (res.success) {
            self.tableData = res.model;
          }
        });
      },
      showDialog: function() {
        self.modalShow = true;
        Kooboo.Domain.getList().then(function(res) {
          if (res.success) {
            self.domainList = res.model.map(function(d) {
              return {
                id: d.id,
                name: "." + d.domainName
              };
            });
            if (self.domainList.length > 0) {
              self.selectedDomain = self.domainList[0].name;
            }
          }
        });
      },
      onDelete: function() {
        if (confirm(this.getConfirmMessage(this.tableDataSelected))) {
          var ids = this.tableDataSelected.map(function(m) {
            return m.id;
          });

          Kooboo[Kooboo.Certificate.name]
            .Deletes({
              ids: ids
            })
            .then(function(res) {
              if (res.success) {
                window.info.done(Kooboo.text.info.delete.success);
                self.getTableData();
              } else {
                window.info.fail(Kooboo.text.info.delete.failed);
              }
            });
        }
      },
      cancelDialog: function() {
        self.modalShow = false;
        self.subDomain = undefined;
        self.selectedDomain = undefined;
      },
      save: function() {
        Kooboo.Certificate.post({
          fullDomain: self.subDomain + self.selectedDomain
        }).then(function(res) {
          if (res.success) {
            window.info.done(res.model);
            self.cancelDialog();
          }
        });
      }
    }
  });
});
