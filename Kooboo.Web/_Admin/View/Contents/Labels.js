$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.Labels
          }
        ],
        labels: [],
        defaultLang: {},
        langs: [],
        editingLabel: {},
        editingContent: [],
        showEditModal: false,
        isSearching: false,
        keyword: ""
      };
    },
    created: function() {
      self.debouncedSearch = _.debounce(self.onSearch, 300);
    },
    mounted: function() {
      $.when(Kooboo.Site.Langs(), Kooboo.Label.getList()).then(function(
        r1,
        r2
      ) {
        var langRes = r1[0],
          labelRes = r2[0];

        if (langRes.success && labelRes.success) {
          self.defaultLang = langRes.model.default;
          self.langs = langRes.model.cultures;
          var labels = _.sortBy(labelRes.model, [
            function(o) {
              return o.lastModified;
            }
          ]);
          labels.reverse().forEach(function(label) {
            self.labels.push(labelModel(label));
            self._labels = self.labels;
          });
        }
      });

      $(window).on("resize.label", function() {
        try {
          waterfall(".label-list");
        } catch (e) {}
      });
    },
    methods: {
      editLabel: function(m) {
        self.showEditModal = true;
        self.editingLabel = m;

        var langs = Object.keys(self.langs),
          defaultLangIdx = langs.indexOf(self.defaultLang);

        if (defaultLangIdx > -1) {
          var value = m.values.hasOwnProperty(self.defaultLang)
            ? m.values[self.defaultLang]
            : "";

          self.editingContent.push({
            lang: self.defaultLang,
            label: self.defaultLang + " - " + self.langs[self.defaultLang],
            value: value
          });
          langs.splice(defaultLangIdx, 1);
        }

        langs.forEach(function(lang) {
          self.editingContent.push({
            lang: lang,
            label: lang + " - " + self.langs[lang],
            value: m.values[lang] ? m.values[lang] : ""
          });
        });

        setTimeout(function() {
          $(".autosize")
            .textareaAutoSize()
            .trigger("keyup");
        }, 300);
      },
      removeLabel: function(m) {
        if (confirm(Kooboo.text.confirm.deleteItem)) {
          Kooboo.Label.Deletes([m.id]).then(function(res) {
            if (res.success) {
              self.labels = _.without(self.labels, m);
              self._labels = self.labels;
              window.info.done(Kooboo.text.info.delete.success);
            } else {
              window.info.done(Kooboo.text.info.delete.fail);
            }
          });
        }
      },
      rendered: function() {
        try {
          setTimeout(function() {
            waterfall(".label-list");
            waterfall(".label-list"); // re-do to sure
          }, 100);
        } catch (e) {}
      },
      showRelationModal: function(m) {
        Kooboo.EventBus.publish("kb/relation/modal/show", {
          id: m.id,
          by: m.by,
          type: "label"
        });
      },
      onHideEditModal: function() {
        self.showEditModal = false;
        self.editingContent = [];
      },
      onSaveEditModal: function() {
        var values = {};
        self.editingContent.forEach(function(content) {
          values[content.lang] = content.value;
        });

        Kooboo.Label.Update({
          id: self.editingLabel.id,
          values: values
        }).then(function(res) {
          if (res.success) {
            self.editingContent.forEach(function(content) {
              self.editingLabel.values[content.lang] = content.value;
            });
            self.onHideEditModal();
            window.info.done(Kooboo.text.info.update.success);
          }
        });
      },
      onSearch: function() {
        var keyword = self.keyword;
        if (!keyword) {
          self.isSearching = false;
          self.labels = self._labels;
        } else {
          var _keyword = keyword.toLowerCase();
          self.isSearching = true;
          var result = _.filter(self._labels, function(label) {
            var flag = false;
            Object.keys(self.langs).forEach(function(key) {
              if (
                label.values[key] &&
                label.values[key].toLowerCase().indexOf(_keyword) > -1
              ) {
                flag = true;
              }
            });
            return label.name.toLowerCase().indexOf(_keyword) > -1 || flag;
          });
          self.labels = result;
        }
      }
    },
    watch: {
      keyword: function() {
        self.debouncedSearch();
      },
      labels: function(val) {
        if (val && val.length) {
          self.rendered();
        }
      }
    },
    beforeDestroy: function() {
      $(window).off("resize.label");
    }
  });

  function labelModel(data) {
    var _d = new Date(data.lastModified);
    data.date = _d.toDefaultLangString();
    data.versionUrl = Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
      KeyHash: data.keyHash,
      storeNameHash: data.storeNameHash
    });
    var refers = [];
    Object.keys(data.relations).forEach(function(key) {
      refers.push({
        id: data.id,
        by: key,
        text:
          data.relations[key] +
          " " +
          Kooboo.text.component.table[key.toLowerCase()],
        bgColor: Kooboo.getLabelColor(key)
      });
    });
    data.refers = refers;
    return data;
  }
});
