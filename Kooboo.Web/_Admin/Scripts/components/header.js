(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/header.html");
    ko.components.register("header", {
        viewModel: function(params) {

            var self = this;

            var menus = params.menu.map(function(menu) {
                return {
                    name: menu.name,
                    icon: menu.icon,
                    url: menu.url
                }
            })

            self.menus = ko.observableArray(menus);
            self.menuClick = function(url, m, e) {
                if (location.pathname.toLowerCase() == url.toLowerCase()) {
                    e.preventDefault();
                }
                return true;
            }

            self.user = ko.observable(params.user);

            self.logout = function() {
                Kooboo.User.logout().then(function(res) {
                    res.success && (location.href = Kooboo.Route.Get(Kooboo.Route.User.LoginPage));
                });
            }

            this.removeLocalStorage = function() {
                // under debug mode
                localStorage.clear();
                location.reload();
            }
        },
        template: template
    });

    Kooboo.Bar.getHeader().then(function(res) {
        if (res.success) {
            ko.applyBindings(res.model, document.getElementById("component_container_header"));

            var lang = Kooboo.LanguageManager.getLang();

            if (!lang || (lang && lang !== res.model.user.language)) {
                Kooboo.LanguageManager.setLang(res.model.user.language);
            }
        }
    });
})();