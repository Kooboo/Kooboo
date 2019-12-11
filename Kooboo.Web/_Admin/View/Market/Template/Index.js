$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "MARKET"
          },
          {
            name: Kooboo.text.common.Templates
          }
        ],
        templates: [],
        pager: undefined,
        showTemplateModal: false,
        templateData: undefined
      };
    },
    created: function() {
      self.getData();
    },
    methods: {
      getData: function() {
        Kooboo.Template.getList().then(function(res) {
          if (res.success) {
            self.pager = res.model;
            self.templates = res.model.list;
            self.$nextTick(function() {
              self.templatesRendered();
            });
          }
        });
      },
      templatesRendered: function() {
        $("img.lazy").lazyload({
          event: "scroll",
          effect: "fadeIn"
        });
      },
      onSelectTemplate: function(event, item) {
        Kooboo.Template.Get({
          id: item.id
        }).then(function(res) {
          if (res.success) {
            self.showTemplateModal = true;
            self.templateData = res.model;
          }
        });
      },
      changePager: function(pageNr) {
        window.scrollTo(0, 0);
        Kooboo.Template.getList({
          pageNr: pageNr
        }).then(function(res) {
          if (res.success) {
            self.pager = res.model;
            self.templates = res.model.list;
          }
        });
      }
    }
  });
});
