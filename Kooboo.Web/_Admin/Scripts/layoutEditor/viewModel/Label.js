(function() {

    function Label(data) {
        this.id = data.id;
        this.type = 'label';
        this.elem = data.elem;
        this.text = ko.observable(data.text);
        this.selected = ko.observable(false);
    }

    if (Kooboo.layoutEditor) {
        Kooboo.layoutEditor.viewModel.Label = Label;
    }

    if (Kooboo.pageEditor) {
        Kooboo.pageEditor.viewModel.Label = Label;
    }
})();