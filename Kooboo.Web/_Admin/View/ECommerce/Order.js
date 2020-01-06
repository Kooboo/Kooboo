$(function() {
  var CONTENT_ID = Kooboo.getQueryString("id");
  var self;
  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        id: CONTENT_ID || Kooboo.Guid.Empty,
        isNewContent: !CONTENT_ID,
        mediaDialogData: {},
        fields: [],
        siteLangs: null,
        contentValues: {}
      };
    },
    mounted: function() {
      Kooboo.Site.Langs().then(function(langRes) {
        if (langRes.success) {
          self.siteLangs = langRes.model;
        }
        self.getContentFields();
      });
      Kooboo.EventBus.subscribe("ko/style/list/pickimage/show", function(ctx) {
        Kooboo.Media.getList().then(function(res) {
          if (res.success) {
            res.model["show"] = true;
            res.model["context"] = ctx;
            res.model["onAdd"] = function(selected) {
              ctx.settings.file_browser_callback(
                ctx.field_name,
                selected.url + "?SiteId=" + Kooboo.getQueryString("SiteId"),
                ctx.type,
                ctx.win,
                true
              );
            };
            self.mediaDialogData = res.model;
          }
        });
      });
    },
    methods: {
      getContentFields: function() {
        var params = {};
        if (CONTENT_ID) {
          params.id = self.id;
        }
        Kooboo.Order.getEdit(params).then(function(res) {
          if (res.success) {
            self.fields = res.model.properties;
          }
        });
      },
      getSaveOrder: function() {
        return {
          id: self.contentId,
          values: self.contentValues.fieldsValue || {}
        };
      },
      isAbleToSaveOrder: function() {
        return this.$refs.fieldPanel.validate();
      },
      onSubmit: function(cb) {
        if (self.isAbleToSaveOrder()) {
          Kooboo.Order.post(self.getSaveOrder()).then(function(res) {
            if (res.success) {
              if (cb && typeof cb == "function") {
                cb(res.model);
              }
            }
          });
        }
      },
      onContentSave: function() {
        self.onSubmit(function(id) {
          location.href = Kooboo.Route.Get(Kooboo.Route.Order.DetailPage, {
            id: id
          });
        });
      },
      onContentSaveAndCreate: function() {
        self.onSubmit(function() {
          window.info.done(Kooboo.text.info.save.success);
          setTimeout(function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.Order.DetailPage);
          }, 300);
        });
      },
      onContentSaveAndReturn: function() {
        self.onSubmit(function() {
          location.href = Kooboo.Route.Get(Kooboo.Route.Order.ListPage);
        });
      },
      userCancel: function() {
        location.href = Kooboo.Route.Get(Kooboo.Route.Order.ListPage);
      }
    }
  });
});
