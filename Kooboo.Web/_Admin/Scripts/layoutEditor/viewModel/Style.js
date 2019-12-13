(function() {
  function Style(data) {
    this.id = data.id;
    this.type = "style";
    this.elem = data.elem;
    this.url = data.url;
    this.displayName = data.displayName;
    this.text = data.text;
    this.selected = false;
    this.name = _.trim(this.text || this.displayName).substr(0, 200);
  }

  if (Kooboo.layoutEditor) {
    Kooboo.layoutEditor.viewModel.Style = Style;
  }

  if (Kooboo.pageEditor) {
    Kooboo.pageEditor.viewModel.Style = Style;
  }
})();
