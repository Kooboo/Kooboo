$(function() {
  var self;
  new Vue({
    el: "#main",
    data: function() {
      return {
        isNewCode: false,
        codeId: Kooboo.getQueryString("Id") || Kooboo.Guid.Empty,
        name: undefined,
        codeContent: "",
        compareTarget: "",
        sourceChange: false,
        supportExtensions: undefined,
        extension: undefined,
        codeChange: false,
        nameValidateModel: { valid: true, msg: "" },
        lang: "javascript"
      };
    },
    watch: {
      codeContent: function(value) {
        if (value !== self.compareTarget) {
          self.codeChange = true;
        } else {
          self.codeChange = false;
        }
      },
      name: function() {
        self.nameValidateModel = { valid: true, msg: "" };
      }
    },
    created: function() {
      self = this;
      if (self.codeId === Kooboo.Guid.Empty) {
        self.isNewCode = true;
      }
      self.init();
    },
    methods: {
      init: function() {
        $.when(
          Kooboo.Script.Get({
            id: self.codeId
          }),
          Kooboo.Script.getExtensions()
        ).then(function(r1, r2) {
          var styleRes = $.isArray(r1) ? r1[0] : r1,
            extensionRes = $.isArray(r2) ? r2[0] : r2;

          if (styleRes.success && extensionRes.success) {
            self.codeId = Kooboo.getQueryString("Id") || Kooboo.Guid.Empty;
            self.name = styleRes.model.name;
            self.codeContent = styleRes.model.body || "";
            self.compareTarget = self.codeContent;
            self.sourceChange = styleRes.model.sourceChange;

            self.supportExtensions = extensionRes.model.map(function(ext) {
              return {
                displayName: "." + ext,
                value: ext
              };
            });
            if(styleRes.model.extension && styleRes.model.extension[0] ==='.') {
              styleRes.model.extension = styleRes.model.extension.slice(1);
            }
            self.extension = styleRes.model.extension || extensionRes.model[0];
          }
        });
      },
      formatCode: function() {
        this.$refs.editor.formatCode();
      },
      changeExtensions: function(event, item) {
        this.extension = item.value;
      },
      onSaveAndReturn: function() {
        self.onSubmitCode(function(model) {
          self.goBack();
        });
      },
      onSave: function() {
        self.onSubmitCode(function(id) {
          if (self.isNewCode) {
            location.href = Kooboo.Route.Get(Kooboo.Route.Script.DetailPage, {
              Id: id
            });
          } else {
            window.info.show(Kooboo.text.info.save.success, true);
            self.compareTarget = self.codeContent;
          }
        });
      },
      onCancel: function() {
        if (self.codeChange) {
          if (confirm(Kooboo.text.confirm.beforeReturn)) {
            self.goBack();
          }
        } else {
          self.goBack();
        }
      },
      goBack: function() {
        location.href = Kooboo.Route.Get(Kooboo.Route.Script.ListPage);
      },
      validate: function() {
        var nameRule = [
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
            url: Kooboo.Script.isUniqueName(),
            data: {
              name: function() {
                return self.name;
              }
            },
            message: Kooboo.text.validation.taken
          }
        ];
        self.nameValidateModel = Kooboo.validField(self.name, nameRule);

        return self.nameValidateModel.valid;
      },
      onSubmitCode: function(callback) {
        if ((self.isNewCode && self.validate()) || !self.isNewCode) {
          Kooboo.Script.Update({
            id: self.isNewCode ? Kooboo.Guid.Empty : self.codeId,
            name: self.name,
            body: self.codeContent,
            extension: self.extension
          }).then(function(res) {
            if (res.success) {
              callback && typeof callback == "function" && callback(res.model);
            } else {
              window.info.show(Kooboo.text.info.save.fail, false);
            }
          });
        }
      }
    }
  });
});
