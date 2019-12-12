(function() {
  Kooboo.loadJS(["/_Admin/Scripts/components/kbTable.js"]);

  var modal = Kooboo.viewEditor && Kooboo.viewEditor.component.modal;
  Vue.component("kb-embedded-dialog", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/textContent/embeddedDialog.html"
    ),
    props: {
      choosedEmbedded: Object
    },
    data: function() {
      return {
        modal: null,
        columns: [],
        tableData: [],
        selected: [],
        contents: []
      };
    },
    mounted: function() {
      $("#embeddedDialog").on("shown.bs.modal", function() {
        $(".modal-backdrop:last").css("z-index", 200001);
      });
    },
    methods: {
      newTextContent: function() {
        var self = this;
        self.modal = self.showContentDialog();
      },
      showContentDialog: function(contentId) {
        var self = this;
        var model = null;
        var iframeUrl = "";

        iframeUrl = Kooboo.Route.Get(Kooboo.Route.TextContent.DialogPage, {
          folder: self.choosedEmbedded.embeddedFolder.id || "",
          id: contentId || Kooboo.Guid.Empty
        });
        model = modal.open({
          title: Kooboo.text.component.modal.embeddedFolder,
          width: 1000,
          url: iframeUrl,
          zIndex: 200003,
          buttons: [
            {
              id: "Save",
              text: Kooboo.text.common.save,
              cssClass: "green",
              click: function(context) {
                self.saveIframe();
              }
            },
            {
              id: "cancel",
              text: Kooboo.text.common.cancel,
              cssClass: "gray",
              click: function(context) {
                context.modal.close();
              }
            }
          ]
        });
        return model;
      },
      close: function() {
        $("#embeddedDialog").modal("hide");
      },
      saveIframe: function() {
        var self = this;
        if (window.__gl) {
          if (!window.__gl.saveContentFinish) {
            window.__gl.saveContentFinish = self.saveContentFinishEvent;
          }
          if (window.__gl.saveContent) {
            window.__gl.saveContent();
          }
        }
      },
      saveContentFinishEvent: function(fieldData, textContentId, folderId) {
        var self = this;
        var existed = self.choosedEmbedded.contents.some(function(o) {
          return o.id === textContentId;
        });
        if (existed) {
          var index = _.findIndex(self.choosedEmbedded.contents, function(o) {
            return o.id === textContentId;
          });
          self.choosedEmbedded.contents.splice(
            index,
            1,
            $.extend(
              { values: savedContentMapping(fieldData) },
              { id: textContentId }
            )
          );
        } else {
          self.choosedEmbedded.contents.push(
            $.extend(
              { values: savedContentMapping(fieldData) },
              { id: textContentId }
            )
          );
        }

        self.modal.close();
        window.__gl = {};
      },
      onEdit: function(contentId) {
        var self = this;
        self.modal = self.showContentDialog(contentId);
      },
      onDelete: function() {
        var self = this;
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var ids = self.selected.map(function(row) {
            return row.id;
          });
          Kooboo.TextContent.Deletes({
            ids: JSON.stringify(ids)
          }).then(function(res) {
            if (res.success) {
              self.choosedEmbedded.contents = _.filter(
                self.choosedEmbedded.contents,
                function(row) {
                  return ids.indexOf(row.id) === -1;
                }
              );
              self.selected = [];
            }
          });
        }
      }
    },
    watch: {
      "choosedEmbedded.contents": function(contents) {
        var self = this;
        if (contents) {
          self.columns = [];
          var columnName = getDefaultColumnName(contents);
          self.columns.unshift({
            displayName: columnName,
            fieldName: columnName,
            type: "communication-link"
          });
          self.tableData = dataMapping(contents);
          // Kooboo.EventBus.publish(
          //   "kb/textContent/embedded/edit",
          //   self.choosedEmbedded
          // );
        }
      }
    }
  });
  function savedContentMapping(data) {
    var outputObj = {};
    for (var key in data) {
      outputObj[key.toCamelCase()] = data[key];
    }
    return outputObj;
  }
  function getDefaultColumnName(records) {
    if (!!records && records instanceof Array && records.length > 0) {
      return Object.keys(records[0].values)[0];
    }
  }
  function dataMapping(data) {
    var tempArr = [];
    var columnName = getDefaultColumnName(data);
    data.forEach(function(item) {
      var ob = {};
      ob[columnName] = {
        text: item.values[columnName],
        url: "kb/textContent/edit"
      };
      ob.id = item.id;
      tempArr.push(ob);
    });
    return tempArr;
  }
})();
