(function() {

    function Style(data) {
        this.id = data.id;
        this.type = 'style';
        this.elem = data.elem;
        this.url = ko.observable(data.url);
        this.displayName = ko.observable(data.displayName);

        this.text = ko.observable(data.text);
        this.selected = ko.observable(false);

        this.name = ko.pureComputed(function() {
            return _.trim(this.text() || this.displayName()).substr(0, 200);
        }, this);
    }

    if (Kooboo.layoutEditor) {
        Kooboo.layoutEditor.viewModel.Style = Style;
    }

    if (Kooboo.pageEditor) {
        Kooboo.pageEditor.viewModel.Style = Style;
    }
})();