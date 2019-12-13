$(function() {
  var self;
  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        blockId: Kooboo.getQueryString("id") || Kooboo.Guid.Empty,
        isMultilingual: false,
        richeditor: {
          mediaDialogData: null,
          editorConfig: {
            min_height: 300,
            max_height: 600
          }
        },
        contents: [],
        multiLangs: {},
        htmlBlock: {
          name: ""
        },
        htmlBlockRules: {
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
                url: Kooboo.HtmlBlock.isUniqueName(),
                data: function() {
                  return {
                    name: self.htmlBlock.name
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ]
        },
        cancelUrl: Kooboo.Route.Get(Kooboo.Route.HtmlBlock.ListPage)
      };
    },
    mounted: function() {
      $.when(
        Kooboo.HtmlBlock.Get({
          Id: self.blockId
        }),
        Kooboo.Site.Langs()
      ).then(function(hbRes, langRes) {
        var r1 = hbRes[0],
          r2 = langRes[0];

        if (r1.success && r2.success) {
          self.multiLangs = r2.model;
          self.htmlBlock.name = r1.model.name;

          self.isMultilingual = _.keys(r2.model.cultures).length > 1;

          var values = r1.model.values || {};

          var cultures = Object.keys(r2.model.cultures),
            defaultCultureIdx = cultures.indexOf(r2.model.default);

          if (defaultCultureIdx > -1) {
            var defaultCulture = cultures[defaultCultureIdx];
            self.contents.push({
              show: true,
              value: values[defaultCulture] || "",
              abbr: defaultCulture
            });
            cultures.splice(defaultCultureIdx, 1);
          }

          cultures.forEach(function(c) {
            self.contents.push({
              show: false,
              value: values[c] || "",
              abbr: c
            });
          });
        }
      });

      Kooboo.EventBus.subscribe("kb/multilang/change", function(target) {
        var content = _.findLast(self.contents, function(c) {
          return c.abbr == target.name;
        });

        if (content) {
          content.show = target.selected;
        }
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
      onSubmit: function() {
        var isValid = true;
        if (self.isNewBlock) {
          isValid = this.$refs.htmlBlockForm.validate();
        }
        if (isValid) {
          Kooboo.HtmlBlock.post({
            id: self.blockId,
            name: self.htmlBlock.name,
            values: JSON.stringify(self.getMultiConents())
          }).then(function(res) {
            if (res.success) {
              location.href = Kooboo.Route.Get(Kooboo.Route.HtmlBlock.ListPage);
            }
          });
        }
      },
      getMultiConents: function() {
        var _values = {};
        self.contents.forEach(function(c) {
          _values[c.abbr] = c.value;
        });

        return _values;
      }
    },
    computed: {
      isNewBlock: function() {
        return this.blockId == Kooboo.Guid.Empty;
      }
    }
  });
});
