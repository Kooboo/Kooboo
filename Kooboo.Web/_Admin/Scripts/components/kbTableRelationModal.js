(function() {
  var template = Kooboo.getTemplate(
    "/_Admin/Scripts/components/kbTableRelationModal.html"
  );

  ko.components.register("kb-table-relation-modal", {
    viewModel: function(params) {
      var self = this;
      this.showError = ko.observable(false);

      this.isShow = params.isShow;
      this.isShow.subscribe(function(show) {
        if (show) {
          $.when(
            Kooboo.TableRelation.getTablesAndFields(),
            Kooboo.TableRelation.getRelationTypes()
          ).then(function(r1, r2) {
            self.tables(
              r1[0].model.map(function(item) {
                item.fields = item.fields.map(function(f) {
                  return { name: f };
                });
                return item;
              })
            );

            self.relationTypes(r2[0].model);
          });
        }
      });

      this.tables = ko.observableArray();
      this.relationTypes = ko.observableArray();
      this.relationType = ko.validateField({
        required: ""
      });

      this.name = ko.validateField({
        required: ""
      });

      this.mainTable = ko.validateField({
        required: ""
      });
      this.mainTableField = ko.validateField({
        required: ""
      });
      this.mainTableFields = ko.pureComputed(function() {
        var table = self.tables().find(function(t) {
          return t.name == self.mainTable();
        });
        return table ? table.fields : [];
      });

      this.subTable = ko.validateField({
        required: ""
      });
      this.subTableField = ko.validateField({
        required: ""
      });
      this.subTableFields = ko.pureComputed(function() {
        var table = self.tables().find(function(t) {
          return t.name == self.subTable();
        });
        return table ? table.fields : [];
      });

      this.isValid = function() {
        return (
          this.name.isValid() &&
          this.mainTable.isValid() &&
          this.mainTableField.isValid() &&
          this.subTable.isValid() &&
          this.subTableField.isValid() &&
          this.relationType.isValid()
        );
      };

      this.onSave = function() {
        if (this.isValid()) {
          Kooboo.TableRelation.post({
            name: this.name(),
            tableA: this.mainTable(),
            fieldA: this.mainTableField(),
            tableB: this.subTable(),
            fieldB: this.subTableField(),
            relation: this.relationType()
          }).then(function(res) {
            if (res.success) {
              window.info.done(Kooboo.text.info.save.success);
              self.reset();
            }
          });
        } else {
          this.showError(true);
        }
      };
      this.reset = function() {
        this.relationType();
        this.mainTable("");
        this.mainTableField("");
        this.subTable("");
        this.subTableField("");
        this.isShow(false);
      };
    },
    template: template
  });
})();
