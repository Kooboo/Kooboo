$(function() {
  var TableRelationModEL = function() {
    var self = this;

    this.showCreateModal = ko.observable(false);

    this.onCreate = function() {
      this.showCreateModal(true);
    };

    getList();

    function getList() {
      Kooboo.TableRelation.getList().then(function(res) {
        if (res.success) {
          self.tableData({
            docs: res.model.map(function(item) {
              return {
                name: item.name,
                tablea: item.tablea,
                fielda: item.fielda,
                tableb: item.tableb,
                fieldb: item.fieldb,
                relation: item.relation.name
              };
            }),
            columns: [
              {
                displayName: "Name",
                fieldName: "name",
                type: "text"
              },
              {
                displayName: "Main table",
                fieldName: "tablea",
                type: "text"
              },
              {
                displayName: "Main table field",
                fieldName: "fielda",
                type: "text"
              },
              {
                displayName: "Relation",
                fieldName: "relation",
                type: "text"
              },
              {
                displayName: "Sub table",
                fieldName: "tableb",
                type: "text"
              },
              {
                displayName: "Sub table field",
                fieldName: "fieldb",
                type: "text"
              }
            ],
            kbType: Kooboo.TableRelation.name
          });
        }
      });
    }
  };

  TableRelationModEL.prototype = new Kooboo.tableModel(
    Kooboo.TableRelation.name
  );

  var vm = new TableRelationModEL();
  ko.applyBindings(vm, document.getElementById("main"));
});
