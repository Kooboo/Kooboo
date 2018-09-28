(function() {
    function Repeat(elem, data) {
        this.type = 'repeat';
        this.elem = elem;
        this.text = ko.observable(data.text);
        this.repeatSelf = ko.observable(data.repeatSelf);
        this.selected = ko.observable(false);
        this.dataSourceId = data.dataSourceId;
    }
    Kooboo.viewEditor.viewModel.Repeat = Repeat;
})();