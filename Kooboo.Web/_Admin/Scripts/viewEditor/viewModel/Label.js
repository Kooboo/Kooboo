(function() {
  function Label(elem, data) {
    this.type = "label";
    this.elem = elem;
    this.text = data.text;
    this.selected = false;
  }
  Kooboo.viewEditor.viewModel.Label = Label;
})();
