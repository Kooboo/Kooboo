$(function () {
  var self;
  new Vue({
    el: "#main",
    data: function () {
      self = this;
      return {
        dataType: "",
        title1: "",
        title2: "",
        id1: "",
        id2: "",
        source1: "",
        source2: ""
      };
    },
    methods: {
      compare: function () {
        switch (self.dataType) {
          case 0:
            var monacoService = new MonacoEditorService();
            monacoService.loader(function (monaco) {
              // https://microsoft.github.io/monaco-editor/playground.html#creating-the-diffeditor-hello-diff-world
              var diffEditor = monaco.editor.createDiffEditor(
                document.getElementById("compare"),
                {
                  enableSplitViewResizing: true,
                  readOnly: true
                }
              );
              var lhsModel = monaco.editor.createModel(
                self.source1 || "",
                "text/plain"
              );
              var rhsModel = monaco.editor.createModel(
                self.source2 || "",
                "text/plain"
              );
              diffEditor.setModel({
                original: lhsModel,
                modified: rhsModel
              });
            }, true);
            break;
          case 1:

          default:
        }
      }
    },
    mounted: function () {
      Kooboo.SiteLog.Compare({
        id1: Kooboo.getQueryString("id1"),
        id2: Kooboo.getQueryString("id2")
      }).then(function (res) {
        if (res.success) {
          self.dataType = res.model.dataType;
          self.title1 = res.model.title1;
          self.title2 = res.model.title2;
          self.id1 = res.model.id1;
          self.id2 = res.model.id2;
          self.source1 = res.model.source1;
          self.source2 = res.model.source2;
          self.compare();
        }
      });
    },
    computed: {
      backUrl() {
        return Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
          KeyHash: Kooboo.getQueryString("KeyHash"),
          StoreNameHash: Kooboo.getQueryString("StoreNameHash"),
          tableNameHash: Kooboo.getQueryString("tableNameHash")
        });
      }
    },
    beforeDestory: function () {
      self = null;
    }
  });
});
