(function() {
  // var PositionStore =
  //   Kooboo[Kooboo.layoutEditor ? "layoutEditor" : "pageEditor"].store
  //     .PositionStore;

  function Position(data) {
    this.id = data.id;
    this.elem = data.elem;
    this.placeholder = data.placeholder;
    this.type = data.type;
    this.name = data.name;
    this.selected = false;
    // var self = this;
    // this.editing = false;
    // this.editing.subscribe(function(flag) {
    //     self.editName(flag ? self.name() : '');
    // });
    // this.editName = this.editing ? self.name : "";
    // this.editName = this.editName.extend({
    //     validate: function(name) {
    //         if (_.isEmpty(_.trim(name))) {
    //             return "This field is required";
    //         }
    //         var pos = PositionStore.byName(name);
    //         if (pos && pos.elem != self.elem && pos.type != self.type) {
    //             return "The same name position already exists";
    //         }
    //     }
    // });
    // this.showError = false;
  }

  if (Kooboo.layoutEditor) {
    Kooboo.layoutEditor.viewModel.Position = Position;
  }

  if (Kooboo.pageEditor) {
    Kooboo.pageEditor.viewModel.Position = Position;
  }
})();
