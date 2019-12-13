$(function() {
  var self;
  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        formId: Kooboo.getQueryString("Id") || Kooboo.Guid.Empty,
        compareTarget: "",
        isEmbedded: false,
        formContent: "",
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
              pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
              message: Kooboo.text.validation.objectNameRegex
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
                url: Kooboo.Form.isUniqueName(),
                data: function() {
                  return {
                    name: self.model.name
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ]
        }
      };
    },
    mounted: function() {
      Kooboo.Form.GetEdit({
        Id: self.formId
      }).then(function(res) {
        if (res.success) {
          self.model.name = res.model.name;
          self.formContent = res.model.body;
          self.compareTarget = self.formContent;
          self.isEmbedded = res.model.isEmbedded;
        }
      });
    },
    methods: {
      formatCode: function() {
        this.$refs.editor.formatCode();
      },
      onSubmitStyle: function(callback) {
        if (
          (self.isNewForm && self.$refs.mainForm.validate()) ||
          !self.isNewForm
        ) {
          Kooboo.Form.post({
            id: self.formId,
            body: self.formContent,
            name: self.isEmbedded ? "" : self.model.name,
            isEmbedded: self.isEmbedded
          }).then(function(res) {
            if (res.success) {
              if (typeof callback == "function") {
                callback(res.model);
              }
            } else {
              window.info.show(Kooboo.text.info.save.fail, false);
            }
          });
        }
      },
      onSaveAndReturn: function() {
        self.onSubmitStyle(function() {
          self.goBack();
        });
      },
      onSave: function() {
        self.onSubmitStyle(function(id) {
          if (self.isNewForm) {
            location.href = Kooboo.Route.Get(Kooboo.Route.Form.DetailPage, {
              Id: id
            });
          } else {
            window.info.show(Kooboo.text.info.save.success, true);
            self.compareTarget = self.formContent;
          }
        });
      },
      userCancel: function() {
        if (self.isContentChanged) {
          if (confirm(Kooboo.text.confirm.beforeReturn)) {
            self.goBack();
          }
        } else {
          self.goBack();
        }
      },
      goBack: function() {
        location.href =
          Kooboo.Route.Get(Kooboo.Route.Form.ListPage) +
          (self.isEmbedded ? "#Embedded" : "");
      }
    },
    computed: {
      isNewForm: function() {
        return Kooboo.Guid.Empty == self.formId;
      },
      isContentChanged: function() {
        return self.formContent !== self.compareTarget;
      }
    }
  });
});
