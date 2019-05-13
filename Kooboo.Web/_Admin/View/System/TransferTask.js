(function() {
  var transferTaskModel = function() {
    var self = this;

    this.getList = function() {
      Kooboo.TransferTask.getList().then(function(res) {
        if (res.success) {
          var docs = res.model.map(function(item) {
            var date = new Date(item.lastModified);
            return {
              id: item.id,
              url: item.fullStartUrl,
              date: date.toDefaultLangString()
            };
          });

          self.tableData({
            docs: docs,
            columns: [
              {
                displayName: Kooboo.text.common.URL,
                fieldName: "url",
                type: "text"
              },
              {
                displayName: Kooboo.text.common.lastModified,
                fieldName: "date",
                type: "text"
              }
            ],
            actions: [],
            kbType: Kooboo.TransferTask.name
          });
        }
      });
    };
    this.getList();
  };

  transferTaskModel.prototype = new Kooboo.tableModel(Kooboo.TransferTask.name);
  var vm = new transferTaskModel();

  ko.applyBindings(vm, document.getElementById("main"));
})();
