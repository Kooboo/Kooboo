(function() {
    function Attribute(elem, attr) {
        this.type = 'attribute';
        this.elem = elem;
        this.text = ko.observable(attr.text);
    }
    Kooboo.viewEditor.viewModel.Attribute = Attribute;
})();