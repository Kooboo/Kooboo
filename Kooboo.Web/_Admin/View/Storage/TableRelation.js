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
