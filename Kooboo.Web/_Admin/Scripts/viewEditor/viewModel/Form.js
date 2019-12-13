(function() {
  var ActionStore = Kooboo.viewEditor.store.ActionStore;

  function Form(elem, data) {
    var self = this;
    this.type = "form";
    this.elem = elem;
    this.method = data.method;
    this.redirect = data.redirect;
    this.callback = data.callback;
    this.selected = false;
    this.dataSourceMethodId = data.dataSourceMethodId;
    this.dataSourceMethodDisplay = null;
    if (this.dataSourceMethodId) {
      var method = ActionStore.byId(this.dataSourceMethodId);
      this.dataSourceMethodDisplay = method
        ? method.group.replace(/DataSource$/, "") + "." + method.methodName
        : "";
    }
  }
  Kooboo.viewEditor.viewModel.Form = Form;
})();
