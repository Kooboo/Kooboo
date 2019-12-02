(function() {
  function Data(elem, data) {
    this.type = "data";
    this.elem = elem;
    this.text = data.text;
    this.condition = data.condition || null;
    this.selected = false;
    this.dataSourceId = data.dataSourceId;
  }
  Kooboo.viewEditor.viewModel.Data = Data;
})();
