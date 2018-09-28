(function() {

    function Label(elem, data) {
        this.type = 'label';
        this.elem = elem;
        this.text = ko.observable(data.text);
        this.selected = ko.observable(false);
    }
    Kooboo.viewEditor.viewModel.Label = Label;
})();