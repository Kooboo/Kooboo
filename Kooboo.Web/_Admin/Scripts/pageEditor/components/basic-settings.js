(function() {
  var ComponentStore = Kooboo.pageEditor.store.ComponentStore;
  Vue.component("kb-page-basic-settings", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/pageEditor/components/basic-settings.html"
    ),
    props: {
      settings: Object,
      multiLangs: Object
    },
    data: function() {
      return {
        multiContentTitle: [],
        metaBindingHelpers: [],
        routeHelpers: [],
        urlPath: "",
        urlPathError: "",
        posted: false,
        init: false
      };
    },
    mounted: function() {
      var self = this;
      var cultures = Object.keys(self.multiLangs.cultures),
        defaultCultureIdx = cultures.indexOf(self.multiLangs.default);

      if (defaultCultureIdx > -1) {
        var defaultCulture = cultures[defaultCultureIdx],
          title = {
            name: defaultCulture,
            value:
              self.settings.contentTitle[""] ||
              self.settings.contentTitle[defaultCulture],
            isDefault: true,
            show: true
          };

        if (window.__pageEditor.kbFrame) {
          window.__pageEditor.kbFrame.setTitle(title.value || "");
        }

        title = Vue.observable(title);
        self.$watch(
          function() {
            return title.value;
          },
          function(t) {
            if (window.__pageEditor.kbFrame) {
              window.__pageEditor.kbFrame.setTitle(t);
            }
            Kooboo.EventBus.publish("kb/page/field/change", {
              type: "title"
            });
          }
        );
        self.multiContentTitle.push(title);
        cultures.splice(defaultCultureIdx, 1);
      }

      cultures.forEach(function(lang) {
        var title = {
          name: lang,
          value: self.settings.contentTitle[lang] || "",
          isDefault: false,
          show: false
        };

        title = Vue.observable(title);
        self.$watch(
          function() {
            return title.value;
          },
          function() {
            Kooboo.EventBus.publish("kb/page/field/change", {
              type: "title"
            });
          }
        );

        self.multiContentTitle.push(title);
      });

      self.urlPath = self.settings.urlPath;

      Kooboo.EventBus.subscribe("kb/page/title/set", function(title) {
        var _default = _.findLast(self.multiContentTitle, function(t) {
          return t.isDefault;
        });

        _default && (_default.value = title);
      });

      Kooboo.EventBus.subscribe("kb/multilang/change", function(target) {
        var title = _.findLast(self.multiContentTitle, function(title) {
          return target.name === title.name;
        });

        title && (title.show = target.selected);
      });

      Kooboo.EventBus.subscribe("kb/page/ComponentStore/change", function() {
        self.metaBindingHelpers = ComponentStore.getMetaBindings();
        self.routeHelpers = ComponentStore.getUrlParamsBindings();
      });

      Kooboo.EventBus.subscribe("kb/page/url/route/set", function(name) {
        if (!self.urlPath) {
          if (name.indexOf("/") !== 0) {
            name = "/" + name;
          }
          self.urlPath = name;
        }
      });

      Kooboo.EventBus.subscribe("kb/page/save", function(res) {
        res["contentTitle"] = {};
        _.forEach(self.multiContentTitle, function(title) {
          if (title.isDefault && $($("iframe")[0]).is(":visible")) {
            if ($("title", $("iframe")[0].contentWindow.document.head).length) {
              $("title", $("iframe")[0].contentWindow.document.head).innerHTML =
                title.value;
            } else {
              var el = $("<title>");
              $(el).text(title.value);
              $($("iframe")[0].contentWindow.document.head).prepend(el[0]);
            }

            Kooboo.EventBus.publish("kb/page/field/change", {
              type: "resource"
            });
          }

          // if (title.show()) {
          res["contentTitle"][title.name] = title.value;
          // }
        });

        res["urlPath"] = self.urlPath;

        self.posted = true;
        if (!self.valid()) {
          if (!$("#url-route-input").is(":visible")) {
            window.info.fail(Kooboo.text.component.pageEditor.invalidRoute);
          }
          res.errorCount ? res.errorCount++ : (res.errorCount = 1);
        }
      });

      self.metaBindingHelpers = ComponentStore.getMetaBindings();
      self.routeHelpers = ComponentStore.getUrlParamsBindings();
      self.$nextTick(function() {
        self.init = true;
      });
    },
    methods: {
      routeHelp: function(data) {
        var tokens = this.urlPath.split("");
        if (tokens[this.urlPath.length - 1] == "/") {
          this.urlPath += data;
        } else {
          this.urlPath += "/" + data;
        }
      },
      metaHelp: function(title, meta) {
        title.value = (title.value ? title.value : "") + "{" + meta + "}";
      },
      urlInput: function() {
        if (this.posted) {
          this.valid();
        }
      },
      valid: function() {
        var result = Kooboo.validField(this.urlPath, [
          {
            pattern: /^[^\s|\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\||\[|\]|\;|\:|\"|\'|\,|\<|\>|\?]*$/,
            message: Kooboo.text.validation.urlInvalid
          }
        ]);
        this.urlPathError = result.msg;
        return result.valid;
      }
    },
    watch: {
      urlPath: function() {
        if (!this.init) return;
        Kooboo.EventBus.publish("kb/page/field/change", {
          type: "url"
        });
      }
    }
  });
})();
