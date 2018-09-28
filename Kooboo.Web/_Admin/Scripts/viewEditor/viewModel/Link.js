(function() {
    function Link(elem, data) {
        this.type = 'link';
        this.elem = elem;
        this.href = ko.observable(data.href);
        this.params = data.params || {};
        this.page = data.page;
        this.selected = ko.observable(false);
    }
    Kooboo.viewEditor.viewModel.Link = Link;
})();