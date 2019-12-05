(function() {
  Vue.component("sidebar-list", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/sidebarList.html"
    ),
    props: {
      listData: Array
    },
    methods: {
      SPAClick: function(m, e) {
        var path =
            m.url.toLowerCase().indexOf("?siteid=") > -1
              ? m.url.toLowerCase().split("?siteid=")[0]
              : m.url;
          if (
            location.pathname.toLowerCase() !== path ||
            !Kooboo.isSameURLParams(
              Kooboo.getURLParams(m.url),
              Kooboo.getURLParams(location.search)
            )
          ) {
            Kooboo.SPA.getView(m.url);
          }
      }
    }
  });
})();
