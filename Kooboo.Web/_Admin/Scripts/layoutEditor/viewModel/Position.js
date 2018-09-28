(function() {

    var PositionStore = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].store.PositionStore;

    function Position(data) {
        var self = this;
        this.id = data.id;
        this.elem = data.elem;
        this.placeholder = data.placeholder;
        this.type = data.type;
        this.name = ko.observable(data.name);
        this.selected = ko.observable(false);

        this.editing = ko.observable(false);
        this.editing.subscribe(function(flag) {
            self.editName(flag ? self.name() : '');
        });
        this.editName = ko.observable(null).extend({
            validate: function(name) {
                if (_.isEmpty(_.trim(name))) {
                    return "This field is required";
                }
                var pos = PositionStore.byName(name);
                if (pos && pos.elem != self.elem && pos.type != self.type) {
                    return "The same name position already exists";
                }
            }
        });

        this.showError = ko.observable(false);
    }

    if (Kooboo.layoutEditor) {
        Kooboo.layoutEditor.viewModel.Position = Position;
    }

    if (Kooboo.pageEditor) {
        Kooboo.pageEditor.viewModel.Position = Position;
    }
})();