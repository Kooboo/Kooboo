$(function() {
  var self;
  new Vue({
    el: "#main",
    data: function() {
      return {
        urlLang: Kooboo.getQueryString("lang"),
        blockId: Kooboo.getQueryString("id"),
        name: undefined,
        contents: [],
        multiLangs: undefined,
        mediaDialogData: undefined
      };
    },
    created: function() {
      self = this;
      self.getData();
    },
    mounted: function() {
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
    watch: {
      lang: function() {
        self.breads = [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: self.lang,
            url: "#"
          },
          {
            name: Kooboo.text.common.HTMLblocks
          }
        ];
      }
    },
    methods: {
      richeditor: function(data) {
        var self = this;
        var item = {
          value: data,
          mediaDialogData: null,
          editorConfig: {
            min_height: 300,
            max_height: 600
          }
        };
        item = Vue.observable(item);
        this.$watch(
          function() {
            return item.mediaDialogData;
          },
          function(value) {
            self.mediaDialogData = value;
          }
        );

        return item;
      },
      getData: function() {
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
            self.name = r1.model.name;

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
                show: c === self.urlLang,
                value: values[c] || "",
                abbr: c
              });
            });
          }
        });
      },
      cancelUrl: function() {
        return Kooboo.Route.Get(Kooboo.Route.HtmlBlock.MultiLangListPage, {
          lang: self.urlLang
        });
      },
      onSubmit: function() {
        Kooboo.HtmlBlock.post({
          id: self.blockId,
          name: self.name,
          values: JSON.stringify(self.getMultiConents())
        }).then(function(res) {
          if (res.success) {
            location.href = Kooboo.Route.Get(
              Kooboo.Route.HtmlBlock.MultiLangListPage,
              {
                lang: self.urlLang
              }
            );
          }
        });
      },
      getMultiConents: function() {
        var _values = {};
        _.forEach(self.contents, function(c) {
          _values[c.abbr] = c.value;
        });
        return _values;
      }
    }
  });
});
