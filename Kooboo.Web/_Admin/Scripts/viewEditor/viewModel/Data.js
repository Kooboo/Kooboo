(function() {
    function Data(elem, data) {
        this.type = 'data';
        this.elem = elem;
        this.text = ko.observable(data.text);
        this.condition = ko.observable(data.condition || null);
        this.selected = ko.observable(false);
        this.dataSourceId = data.dataSourceId;
    }
    Kooboo.viewEditor.viewModel.Data = Data;
})();