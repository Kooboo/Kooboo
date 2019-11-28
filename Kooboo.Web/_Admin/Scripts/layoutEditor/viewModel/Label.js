(function() {
  function Label(data) {
    this.id = data.id;
    this.type = "label";
    this.elem = data.elem;
    this.text = data.text;
    this.selected = false;
  }

  if (Kooboo.layoutEditor) {
    Kooboo.layoutEditor.viewModel.Label = Label;
  }

  if (Kooboo.pageEditor) {
    Kooboo.pageEditor.viewModel.Label = Label;
  }
})();
