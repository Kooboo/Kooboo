(function() {
  function Link(elem, data) {
    this.type = "link";
    this.elem = elem;
    this.href = data.href;
    this.params = data.params || {};
    this.page = data.page;
    this.selected = false;
  }
  Kooboo.viewEditor.viewModel.Link = Link;
})();
