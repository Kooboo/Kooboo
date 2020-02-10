(function(KB, $) {
  KB.VersionManager = {
    init: function() {
      this.checkAdminVersion();
    },
    checkAdminVersion() {
      var system_version_key = "_system_version_";
      var system_version = Kooboo.GetCookie(system_version_key);
      if (system_version) {
        var localSystemVersion = localStorage.getItem(system_version_key);
        if (localSystemVersion != system_version) {
          localStorage.clear();
          localStorage.setItem(system_version_key, system_version);
          location.reload();
        }
      }
    }
  };
})(Kooboo, jQuery);
