(function() {
  function Repeat(elem, data) {
    this.type = "repeat";
    this.elem = elem;
    this.text = data.text;
    this.repeatSelf = data.repeatSelf;
    this.selected = false;
    this.dataSourceId = data.dataSourceId;
  }
  Kooboo.viewEditor.viewModel.Repeat = Repeat;
})();
