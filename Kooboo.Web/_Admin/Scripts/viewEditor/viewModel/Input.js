(function() {
    function Input(elem, data) {
        this.type = 'input';
        this.elem = elem;
        this.text = ko.observable(data.text);
        this.selected = ko.observable(false);
    }
    Kooboo.viewEditor.viewModel.Input = Input;
})();