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
                id: item.id,
                name: item.name,
                tablea: item.tableA,
                fielda: item.fieldA,
                tableb: item.tableB,
                fieldb: item.fieldB,
                relation: {
                    text: item.relationName,
                    class: 'label-sm green'
                }
              };
            }),
            columns: [
              {
                displayName: Kooboo.text.common.name,
                fieldName: "name",
                type: "text"
              },
              {
                displayName: Kooboo.text.component.tableRelation.tableA,
                fieldName: "tablea",
                type: "text"
              },
              {
                  displayName: Kooboo.text.component.tableRelation.fieldA,
                fieldName: "fielda",
                type: "text"
              },
              {
                  displayName: Kooboo.text.common.Relation,
                fieldName: "relation",
                type: "label"
              },
              {
                  displayName: Kooboo.text.component.tableRelation.tableB,
                fieldName: "tableb",
                type: "text"
              },
              {
                  displayName: Kooboo.text.component.tableRelation.fieldB,
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
