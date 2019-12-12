(function() {
  Vue.component("kb-control-richeditor", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/RichEditor.html"
    ),
    props: {
      field: Object
    },
    data: function() {
      var self = this;
      return {
        richeditor: {
          editorConfig: { readonly: self.field.disabled === true },
          mediaDialogData: null
        }
      };
    },
    inject: ["kbFormItem"],
    watch: {
      "richeditor.mediaDialogData": function(value) {
        this.$emit("media-dialog-data", value);
      }
    }
  });
})();
