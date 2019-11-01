$(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/sidebarList.html");
    ko.components.register("sidebar-list", {
        viewModel: function(params) {
            var self = this;
            this.listData = params.listData;

            this.SPAClick = _.debounce(function(m, e) {
                e.preventDefault();
                var path = (m.url.toLowerCase().indexOf("?siteid=") > -1) ? m.url.toLowerCase().split("?siteid=")[0] : m.url;
                if (location.pathname.toLowerCase() !== path ||
                    !Kooboo.isSameURLParams(Kooboo.getURLParams(m.url), Kooboo.getURLParams(location.search))) {
                    Kooboo.SPA.getView(m.url);
                }
            }, 100);
        },
        template: template
    });
})