(function() {
    var ActionStore = Kooboo.viewEditor.store.ActionStore;

    function Form(elem, data) {
        var self = this;
        this.type = 'form';
        this.elem = elem;
        this.dataSourceMethodId = ko.observable();
        this.dataSourceMethodDisplay = ko.observable();
        this.dataSourceMethodId.subscribe(function(val) {
            if (!val) {
                return null;
            }
            var method = ActionStore.byId(val);
            self.dataSourceMethodDisplay(method ? method.group.replace(/DataSource$/, '') + '.' + method.methodName : '');
        });
        this.method = ko.observable(data.method);
        this.redirect = ko.observable(data.redirect);
        this.callback = ko.observable(data.callback);
        this.selected = ko.observable(false);
        this.dataSourceMethodId(data.dataSourceMethodId);
    }
    Kooboo.viewEditor.viewModel.Form = Form;
})();