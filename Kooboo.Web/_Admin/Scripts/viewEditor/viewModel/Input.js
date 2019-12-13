(function() {
  function Input(elem, data) {
    this.type = "input";
    this.elem = elem;
    this.text = data.text;
    this.selected = false;
  }
  Kooboo.viewEditor.viewModel.Input = Input;
})();
