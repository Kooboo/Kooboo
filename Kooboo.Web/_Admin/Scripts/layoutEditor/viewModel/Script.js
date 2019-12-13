(function() {
  function Script(data) {
    this.id = data.id;
    this.type = "script";
    this.elem = data.elem;
    this.url = data.url;
    this.displayName = data.displayName;

    this.head = data.head;
    this.pullToHead = data.pullToHead;
    this.text = data.text;
    this.selected = false;
    this.name = _.trim(this.text || this.displayName).substr(0, 200);
  }

  if (Kooboo.layoutEditor) {
    Kooboo.layoutEditor.viewModel.Script = Script;
  }

  if (Kooboo.pageEditor) {
    Kooboo.pageEditor.viewModel.Script = Script;
  }
})();
