(function() {
    var LayoutStore = Kooboo.viewEditor.store.LayoutStore;

    function LayoutPosition() {
        var self = this;

        this.layoutList = ko.observableArray();
        this.positionList = ko.observableArray();

        this.layout = ko.observable();
        this.layout.subscribe(function(val) {
            Kooboo.EventBus.publish("layout/change", val);
        });

        this.position = ko.observable();
        this.position.subscribe(function(val) {
            Kooboo.EventBus.publish("position/change", val);
        });

        this.changeLayout = function(name) {
            var layout = _.find(this.layoutList(), function(it) {
                return it.name === name;
            });
            if (layout) {
                this.layout(layout);
            }
        }

        Kooboo.EventBus.subscribe("LayoutStore/change", function() {
            self.layoutList(LayoutStore.getAll());
        })
    }

    Kooboo.viewEditor.viewModel.LayoutPosition = LayoutPosition;
})();