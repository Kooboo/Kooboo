$(function() {
  var self;

  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        isNewContentType: false,
        tableData: [],
        tableDataSelected: [],
        showSystemField: false,
        model: {
          name: ""
        },
        rules: {
          name: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              min: 1,
              max: 64,
              message:
                Kooboo.text.validation.minLength +
                1 +
                ", " +
                Kooboo.text.validation.maxLength +
                64
            },
            {
              remote: {
                url: Kooboo.ContentType.isUniqueName(),
                data: function() {
                  return {
                    name: self.model.name
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ]
        },
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
            self.model.name = res.model.name;
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
        if (event.data.isSystemField) {
          self.systemProperties[event.editingIndex] = event.data;
        } else {
          if (event.isNewField) {
            self.notSystemProperties.push(event.data);
          } else {
            if (event.editingIndex > -1) {
              self.notSystemProperties[event.editingIndex] = event.data;
            }
          }
        }
      },
      onSave: function(data) {
        if (self.isNewContentType && !self.$refs.form.validate()) {
          return;
        }
        var properties = _.concat(
          self.notSystemProperties,
          self.systemProperties
        );
        var data = {
          id: self.contentTypeId,
          name: self.model.name,
          properties: properties
        };
        Kooboo.ContentType.save(data).then(function(res) {
          if (res.success) {
            location.href = self.contentTypesPageUrl;
          }
        });
      },
      onAdd: function() {
        this.fieldEditorOptions.isSystemField = false;
        this.editingItemData = undefined;
        this.modalVisible = true;
        this.editingItemIndex = self.notSystemProperties.length;
      },
      onCancel: function() {
        location.href = self.contentTypesPageUrl;
      },
      onEdit: function(event, item, index, isSystemField) {
        this.editingItemData = item;
        if (isSystemField) {
          this.fieldEditorOptions.isSystemField = true;
        } else {
          this.fieldEditorOptions.isSystemField = false;
        }
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
