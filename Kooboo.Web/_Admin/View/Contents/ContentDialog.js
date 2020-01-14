$(function() {
  var self;
  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        id: Kooboo.getQueryString("id") || Kooboo.Guid.Empty,
        folderId: Kooboo.getQueryString("folder"),
        fields: [],
        contentValues: {},
        siteLangs: null,
        categories: [],
        embedded: [],
        choosedEmbedded: {},
        mediaDialogData: null
      };
    },
    mounted: function() {
      $.when(
        Kooboo.Site.Langs(),
        Kooboo.TextContent.getEdit({
          folderId: self.folderId,
          id: self.id
        })
      ).then(function(r1, r2) {
        var langRes = r1[0],
          contentRes = r2[0];

        if (langRes.success && contentRes.success) {
          self.siteLangs = langRes.model;
          self.fields = contentRes.model.properties;
          self.categories = contentRes.model.categories || [];
          self.embedded = contentRes.model.embedded || [];

          setTimeout(function() {
            self.adjustHeight();
          }, 300);
        }
      });

      if (window.parent.__gl) {
        window.parent.__gl.saveContent = this.save;
      }
      $(window).on("resize.dialog", function() {
        self.adjustHeight();
      });
    },
    methods: {
      isAbleToSave: function() {
        return this.$refs.fieldPanel.validate();
      },
      save: function() {
        if (self.isAbleToSave()) {
          Kooboo.TextContent.update({
            id: self.id,
            folderId: self.folderId,
            values: self.contentValues.fieldsValue || {},
            categories: self.contentValues.categories || {},
            embedded: self.contentValues.embedded || {}
          }).then(function(res) {
            if (res.success) {
              if (window.parent.__gl && window.parent.__gl.saveContentFinish) {
                var fields = {},
                  data = self.contentValues.fieldsValue[self.siteLangs.default],
                  keys = Object.keys(data);

                keys.forEach(function(key) {
                  fields[key] = data[key];
                });

                window.parent.__gl.saveContentFinish(
                  fields,
                  res.model,
                  self.folderId
                );
              }
            }
          });
        }
      },
      adjustHeight: function() {
        var hasTinyMceField = !!_.find(self.fields, function(field) {
          return (
            ["tinymce", "mediafile"].indexOf(field.controlType.toLowerCase()) >
            -1
          );
        });

        var data = {
          height: hasTinyMceField
            ? parent.window.innerHeight - 100
            : window.document.body.scrollHeight,
          hasTinyMceField: hasTinyMceField
        };
        window.parent.Kooboo.EventBus.publish(
          "kb/component/modal/set/height",
          data
        );
      }
    },
    beforeDestroy: function() {
      $(window).off("resize.dialog");
    }
  });
});
