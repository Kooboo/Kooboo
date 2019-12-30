$(function() {
  var self;
  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        id: Kooboo.getQueryString("id") || Kooboo.Guid.Empty,
        model: {
          typename: ""
        },
        rules: {
          typename: [
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
                url: Kooboo.ProductType.isUniqueName(),
                data: function() {
                  return {
                    name: self.model.typename
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ]
        },
        fields: [],
        isNewProductType: true,
        isNewField: false,
        onFieldModalShow: false,
        fieldData: {},
        productTypesUrl: Kooboo.Route.Product.Type.ListPage,
        editingItemIndex: -1,
        fieldEditorOptions: {
          controlTypes: [
            "textbox",
            "textarea",
            "richeditor",
            "radiobox",
            "switch",
            "mediafile",
            "number"
          ],
          specControlTypes: ["dynamicspec", "fixedspec"],
          modifiedField: "isSpecification",
          modifiedFieldText:
            Kooboo.text.component.fieldEditor.specificationField,
          modifiedFieldSubscriber: self.specSelect,
          showMultilingualOption: true,
          showPreviewPanel: true
        }
      };
    },
    mounted: function() {
      Kooboo.ProductType.get({
        id: self.id
      }).then(function(res) {
        if (res.success) {
          self.isNewProductType = !res.model.name;
          self.model.typename = res.model.name;
          self.fields = res.model.properties;
        }
      });
    },
    methods: {
      addNewField: function() {
        self.isNewField = true;
        self.fieldData = {};
        self.onFieldModalShow = true;
        self.editingItemIndex = -1;
      },
      editField: function(m, index) {
        self.isNewField = false;
        self.fieldData = m;
        self.editingIndex = index;
        self.onFieldModalShow = true;
      },
      removeField: function(m, index) {
        if (confirm(Kooboo.text.confirm.deleteItem)) {
          this.fields.splice(index, 1);
        }
      },
      onFieldSave: function(event) {
        if (self.isNewField) {
          self.fields.push(event.data);
        } else {
          if (self.editingIndex > -1) {
            self.fields[self.editingIndex] = event.data;
          }
        }
      },
      onSave: function() {
        if (self.isNewProductType && !self.$refs.form.validate()) {
          return;
        }
        self.fields.forEach(function(prop) {
          if (
            ["fixedspec", "dynamicspec"].indexOf(
              prop.controlType.toLowerCase()
            ) > -1
          ) {
            prop.dataType = "Array";
          }
        });
        Kooboo.ProductType.post({
          id: self.id,
          name: self.model.typename,
          properties: self.fields
        }).then(function(res) {
          if (res.success) {
            location.href = self.productTypesUrl;
          }
        });
      },
      onModalClose: function() {
        if (self.onFieldModalShow) {
          self.onFieldModalShow = false;
          self.editingItemIndex = -1;
        } else {
          self.onFieldModalShow = true;
        }
      }
    }
  });
});
