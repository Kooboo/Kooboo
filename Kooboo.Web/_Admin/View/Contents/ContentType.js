$(function() {
  var self;

  new Vue({
    el: "#main",
    data: function() {
      return {
        isNewContentType: false,
        tableData: [],
        tableDataSelected: [],
        showSystemField: false,
        name: "",
        contentTypeId: "",
        controlType: "",
        notSystemProperties: [],
        systemProperties: [],
        modalVisible: false,
        contentTypesPageUrl: Kooboo.Route.ContentType.ListPage,
        editingItemData: undefined,
        fieldEditorOptions: {
          controlTypes: [
            "textbox",
            "textarea",
            "richeditor",
            "selection",
            "checkbox",
            "radiobox",
            "switch",
            "mediafile",
            "file",
            "datetime",
            "number"
          ],
          modifiedField: "isSummaryField",
          modifiedFieldText: Kooboo.text.component.fieldEditor.summaryField,
          showMultilingualOption: true,
          showPreviewPanel: true
        }
      };
    },
    created: function() {
      self = this;
      self.contentTypeId = Kooboo.getQueryString("id") || Kooboo.Guid.Empty;
      if (self.contentTypeId === Kooboo.Guid.Empty)
        self.isNewContentType = true;
      self.getContentTypeData(self.contentTypeId);
    },
    methods: {
      toggleSystemFields: function() {
        self.showSystemField
          ? (self.showSystemField = false)
          : (self.showSystemField = true);
      },
      getContentTypeData: function(id) {
        Kooboo.ContentType.Get({
          id: id
        }).then(function(res) {
          if (res.success) {
            self.name = res.model.name;
            var properties = res.model.properties.map(function(o) {
              if (o.controlType.toLowerCase() === "tinymce") {
                o.controlType = "RichEditor";
              }
              return o;
            });
            self.notSystemProperties = properties.filter(function(item) {
              return !item.isSystemField;
            });
            self.systemProperties = properties.filter(function(item) {
              return item.isSystemField;
            });
          }
        });
      },
      getConfirmMessage: function(doc) {
        if (doc.relations) {
          doc.relationsTypes = _.sortBy(Object.keys(doc.relations));
        }
        var find = _.find(doc, function(item) {
          return item.relations && Object.keys(item.relations).length;
        });

        if (!!find) {
          return Kooboo.text.confirm.deleteItemsWithRef;
        } else {
          return Kooboo.text.confirm.deleteItems;
        }
      },
      removeItem: function(event, index, item) {
        this.notSystemProperties.splice(index, 1);
      },
      onFieldEditorSave: function(event) {
        if (event.isNewField) {
          self.notSystemProperties.splice(0, 0, event.data);
        } else {
          if (event.editingIndex > -1) {
            self.notSystemProperties[event.editingIndex] = event.data;
          }
        }
      },
      onSave: function(data) {
        var properties = _.concat(
          self.notSystemProperties,
          self.systemProperties
        );
        var data = {
          id: self.contentTypeId,
          name: self.name,
          properties: properties
        };
        Kooboo.ContentType.save(data).then(function(res) {
          if (res.success) {
            location.href = self.contentTypesPageUrl;
          }
        });
      },
      onAdd: function() {
        this.editingItemData = undefined;
        this.modalVisible = true;
        this.editingItemIndex = self.notSystemProperties.length;
      },
      onCancel: function() {
        location.href = self.contentTypesPageUrl;
      },
      onEdit: function(event, item, index) {
        this.editingItemData = item;
        this.editingItemIndex = index;
        this.modalVisible = true;
      },
      onModalClose: function() {
        if (self.modalVisible) {
          self.modalVisible = false;
          self.editingItemIndex = undefined;
        } else {
          self.modalVisible = true;
        }
      }
    }
  });
});
