(function() {
  function Condition(elem, data) {
    this.type = "condition";
    this.elem = elem;
    this.text = data.text;
    this.selected = false;
  }
  Kooboo.viewEditor.viewModel.Condition = Condition;
})();
