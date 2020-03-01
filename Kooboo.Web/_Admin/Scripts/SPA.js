(function(kb) {
  kb.SPA = {
    CONTAINER_KEY: "SPA_Container_key",
    currentURL: "",
    getView: function(path, options, replaceHistory) {
      var result = "default";
      if (this.beforeUnload) {
        result = this.beforeUnload();
      }

      switch (result) {
        case "default":
          var self = this,
            container = options ? options.container || "#main" : "#main";

          var requestURL = "";
          if (path.toLowerCase().indexOf("?siteid=") > -1) {
            requestURL = path
              .toLowerCase()
              .split("?siteid=")[0]
              .split("_admin/")
              .join("_spa/");
          } else {
            requestURL = path
              .toLowerCase()
              .split("_admin/")
              .join("_spa/");
          }

          var viewCode = localStorage.getItem(
            "SPA_" + (localStorage.getItem("lang") || "en") + "_" + requestURL
          );

          if (!viewCode) {
            $.get(requestURL)
              .then(function(res) {
                setView(path, res, replaceHistory);
                localStorage.setItem(
                  "SPA_" +
                    (localStorage.getItem("lang") || "en") +
                    "_" +
                    requestURL,
                  res
                );
              })
              .fail(function(err) {
                window.info.fail(
                  Kooboo.text.info.networkError +
                    ", " +
                    Kooboo.text.info.checkServer
                );
              });
          } else {
            setView(path, viewCode, replaceHistory);
          }
          break;
        case "refresh":
          window.location.href = path;
          break;
        case "abort":
          break;
      }

      function setView(path, code, replaceHistory) {
        if (
          !(path == "/_Admin/Domains" && location.href.indexOf("Market") > -1)
        ) {
          !replaceHistory &&
            history.pushState({ path: path }, { container: container }, path);
        }
        self.currentURL = location.href;
        sessionStorage.setItem(self.CONTAINER_KEY, container);
        $(container).empty();
        kb.EventBus.keepEvents(["kb/sidebar/refresh"]);
        kb.EventBus.reset();
        $(".modal-backdrop").remove();
        $(container).html(code);
        kb.EventBus.publish("kb/sidebar/refresh");
      }
    },
    beforeUnload: undefined
  };

  window.onpopstate = function(event) {
    var oldURL = Kooboo.SPA.currentURL;
    var newURL = location.href;

    if (newURL) {
      if (
        newURL.toLowerCase().indexOf("/_admin/contents/images") == -1 ||
        oldURL.toLowerCase().indexOf("/_admin/contents/images") == -1
      ) {
        Kooboo.SPA.getView(
          location.pathname + location.search,
          { container: sessionStorage.getItem(Kooboo.SPA.CONTAINER_KEY) },
          true
        );
      } else {
        Kooboo.EventBus.publish("window/popstate");
      }
    } else {
      Kooboo.SPA.getView(
        location.pathname + location.search,
        { container: sessionStorage.getItem(Kooboo.SPA.CONTAINER_KEY) },
        true
      );
    }
  };
})(Kooboo);
