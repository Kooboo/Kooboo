$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      return {
        langName: undefined,
        multiLang: Kooboo.getQueryString("lang"),
        mutliLangName: undefined,
        breads: [],
        labels: [],
        showEditModal: false,
        editingLabel: undefined,
        editingLabelIndex: undefined,
        newMultiLangValue: ""
      };
    },
    created: function() {
      self = this;
      self.getData();
    },
    watch: {
      mutliLangName: function() {
        self.breads = [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: self.mutliLangName,
            url: "#"
          },
          {
            name: Kooboo.text.common.HTMLblocks
          }
        ];
      },
      labels: function() {}
    },
    methods: {
      getData: function() {
        $.when(Kooboo.Site.Langs(), Kooboo.Label.getList()).then(function(
          r1,
          r2
        ) {
          var langRes = r1[0],
            labelRes = r2[0];
          if (langRes.success && labelRes.success) {
            self.lang = langRes.model.default;
            self.langName = langRes.model.defaultName;
            self.mutliLangName = langRes.model.cultures[self.multiLang];

            var temp = [];
            labelRes.model.forEach(function(item) {
              item.defaultLang = self.lang;
              item.multiLang = self.multiLang;

              var _d = new Date(item.lastModified);
              item.date = _d.toDefaultLangString();
              item.defaultValue = item.values[item.defaultLang];
              item.multilingual = item.values[item.multiLang];
              temp.push(item);
            });
            self.labels = temp;
          }
        });
      },
      editLabel: function(event, item, index) {
        self.editingLabel = item;
        self.editingLabelIndex = index;
        self.newMultiLangValue = item.multilingual;
        self.showEditModal = true;
        setTimeout(function() {
          $(".autosize")
            .textareaAutoSize()
            .trigger("keyup");
        }, 300);
      },
      onHideEditModal: function() {
        self.editingLabel = undefined;
        self.showEditModal = false;
        self.editingLabelIndex = undefined;
        self.newMultiLangValue = "";
      },
      onSaveEditModal: function() {
        var values = {};

        values[self.multiLang] = self.newMultiLangValue || "";

        Kooboo.Label.Update({
          id: self.editingLabel.id,
          values: values
        }).then(function(res) {
          if (res.success) {
            self.editingLabel.multilingual = self.newMultiLangValue;
            self.editingLabel.date = new Date().toDefaultLangString();
            self.labels[self.editingLabelIndex] = self.editingLabel;
            self.onHideEditModal();
            window.info.done(Kooboo.text.info.update.success);
          }
        });
      }
    }
  });
});
