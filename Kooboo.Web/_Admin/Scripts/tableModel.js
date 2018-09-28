(function() {
    var tableModel = function(type) {

        var self = this;

        this.type = type;

        self.tableData = ko.observable({
            columns: [],
            docs: [],
            kbType: this.type
        });

        self.showDeleteBtn = ko.observable(false);

        self.onDelete = function() {
            Kooboo.EventBus.publish("ko/table/on/delete");
        }

        // handle inner operations
        Kooboo.EventBus.subscribe("ko/table/delete/show", function(hasSelected) {
            self.showDeleteBtn(hasSelected);
        });
    }

    Kooboo.tableModel = tableModel;
})();