(function() {
    function Condition(elem, data) {
        this.type = 'condition';
        this.elem = elem;
        this.text = ko.observable(data.text);
        this.selected = ko.observable(false);
    }
    Kooboo.viewEditor.viewModel.Condition = Condition;
})();