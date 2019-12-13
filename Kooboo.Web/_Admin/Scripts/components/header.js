(function() {
  Vue.component("kb-header", {
    template: Kooboo.getTemplate("/_Admin/Scripts/components/header.html"),
    data: function() {
      return {
        menus: [],
        user: {}
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.Bar.getHeader().then(function(res) {
        if (res.success) {
          self.menus = res.model.menu;
          self.user = res.model.user;

          var lang = Kooboo.LanguageManager.getLang();

          if (!lang || (lang && lang !== res.model.user.language)) {
            Kooboo.LanguageManager.setLang(res.model.user.language);
          }
        }
      });
    },
    methods: {
      removeLocalStorage: function() {
        // under debug mode
        localStorage.clear();
        location.reload();
      },
      logout: function() {
        Kooboo.User.logout().then(function(res) {
          res.success &&
            (location.href = Kooboo.Route.Get(Kooboo.Route.User.LoginPage));
        });
      }
    }
  });
})();
