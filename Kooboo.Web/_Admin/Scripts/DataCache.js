(function(win) {
  var DataCache = {
    siteId: "",

    isCacheObject: function(objectType) {
      // TODO: add cache type to the array
      // 2018.08.10 TODO: menu.getLang should not be cached.
      return (
        objectType &&
        [
          "page",
          "layout",
          "view",
          "bar" /*"menu"*/,
          ,
          "style",
          "script",
          "resourcegroup"
        ].indexOf(objectType.toLowerCase()) > -1
      );
    },

    isMultilingualObject: function(objectType) {
      return (
        objectType &&
        ["textcontent", "htmlblock", "label"].indexOf(
          objectType.toLowerCase()
        ) > -1
      );
    },

    getSiteId: function() {
      if (!this.siteId) {
        this.siteId = Kooboo.getQueryString("SiteId");
      }

      return this.siteId;
    },

    getData: function(objectName, method, paradata, useSync) {
      if (this.isCacheObject(objectName)) {
        var key = this.getKey(objectName, method, paradata);
        var item = localStorage.getItem(key);
        if (item && key) {
          var result = {
            model: JSON.parse(item),
            success: true
          };
          return $.Deferred().resolve(result);
        } else {
          return this.requestData(objectName, method, paradata).done(function(
            res
          ) {
            if (res && res.success && key) {
              localStorage.setItem(key, JSON.stringify(res.model));
            }
          });
        }
      } else {
        return this.requestData(objectName, method, paradata, useSync);
      }
    },

    postData: function(objectName, method, data, extendParams, useSync) {
      var url = this._getUrl(objectName, method, extendParams);
      // TODO: remove all data for now. should be:
      // when delete= remove all data, when update = search and update one item in cache.
      this.removeRelatedData(objectName, method, data);
      return this.requestData(objectName, method, data, useSync, "POST");
      //return $.post(url, data);
    },

    uploadData: function(objectName, method, data, progressor) {
      this.removeRelatedData(objectName, method);
      return $.ajax({
        url: this._getUrl(objectName, method),
        type: "POST",
        data: data,
        cache: false,
        contentType: "multipart/form-data",
        processData: false,
        xhr: function() {
          var xhr = new XMLHttpRequest();
          xhr.upload.addEventListener(
            "progress",
            function(evt) {
              if (evt.lengthComputable) {
                progressor && progressor(evt.loaded / evt.total);
              }
            },
            false
          );
          return xhr;
        }
      });
    },

    requestData: function(objectName, method, paradata, useSync, type) {
      var self = this;
      if (!type) type = "GET";
      return $.ajax({
        url: this._getUrl(objectName, method),
        type: type,
        data: paradata,
        async: !useSync
      }).done(function(res) {
        if (res && res.success) {
          var site_version_key = "_site_version_";
          var site_id_key = "_site_id_";

          Kooboo.VersionManager.checkAdminVersion();

          var siteId = Kooboo.GetCookie(site_id_key);
          var site_version = Kooboo.GetCookie(site_version_key);
          var localSiteVersion = localStorage.getItem(
            site_version_key + siteId
          );
          if (localSiteVersion != site_version) {
            var removed = [];
            for (var i = 0, len = localStorage.length; i < len; i++) {
              var cachedKey = localStorage.key(i);
              if (cachedKey.indexOf(siteId) > -1) {
                removed.push(cachedKey);
              }
            }

            removed.forEach(function(rm) {
              localStorage.removeItem(rm);
            });

            localStorage.setItem(site_version_key + siteId, site_version);
          }
        }
      });
    },

    removeData: function(objectName) {
      var keyPrefix =
        (objectName !== "bar" ? this.getSiteId() : "") +
        "_cache_" +
        objectName.toLowerCase();
      var allitems = searchLocalStorage(keyPrefix);
      for (var key in allitems) {
        localStorage.removeItem(key);
      }
    },

    removeSiteCacheById: function(id) {
      var allitems = searchLocalStorage(id + "_cache");
      for (var key in allitems) {
        localStorage.removeItem(key);
      }
    },

    removeRelatedData: function(name, method, data) {
      var self = this,
        list = [];
      switch (name.toLowerCase()) {
        case "user":
        case "site":
        case "contenttype":
          list.push("bar");
          break;
        case "upload":
          var type = method.toLowerCase();
          type == "image" && (type = "media");
          list.push(type);
          break;
        case "page":
          list = list.concat([
            "layout",
            "view",
            "menu",
            "style",
            "script",
            "htmlblock",
            "media"
          ]);
          break;
        case "view":
          list = list.concat(["page", "media"]);
          break;
        case "layout":
          list = list.concat([
            "page",
            "media",
            "style",
            "script",
            "resourcegroup"
          ]);
          break;
        case "url":
        case "form":
          list = list.concat([
            "page",
            "view",
            "layout",
            "media",
            "style",
            "script",
            "file",
            "menu"
          ]);
          break;
        case "menu":
          list = list.concat(["page"]);
          break;
        case "pull":
        case "sitelog":
        case "inlineeditor":
        case "binding":
          list = list.concat([
            "page",
            "view",
            "layout",
            "media",
            "style",
            "script",
            "file",
            "menu",
            "bar",
            "resourcegroup"
          ]);
          break;
        case "style":
        case "cssrule":
        case "script":
          list = list.concat(["page", "media", "layout", "resourcegroup"]);
          break;
        case "htmlblock":
          list = list.concat(["page", "media"]);
          break;
        case "textcontent":
          list = list.concat(["media"]);
          break;
        case "contentfolder":
          list = list.concat(["bar", "media"]);
          break;
        case "transfer":
          list = list.concat(["page", "style", "script", "media"]);
          break;
        case "resourcegroup":
          list = list.concat(["style", "script"]);
          break;
        case "datamethodsetting":
          list = list.concat(["view"]);
          break;
      }
      list.push(name.toLowerCase());

      list.forEach(function(objName) {
        if (objName == "site") {
          if (method.toLowerCase() == "delete") {
            self.removeSiteCacheById(
              JSON.parse(decodeURIComponent(data))["Id"]
            );
          } else if (method.toLowerCase() == "deletes") {
            var ids = JSON.parse(decodeURIComponent(data))["ids"];
            ids.forEach(function(id) {
              self.removeSiteCacheById(id);
            });
          }
        } else {
          self.removeData(objName);
        }
      });
    },

    getKey: function(objectName, method, paradata) {
      var siteId = this.getSiteId();
      if (!siteId) return null;
      var key = siteId + "_cache_" + objectName.toLowerCase();
      if (this.isMultilingualObject(objectName)) {
        // TODO: add the user language here for multilingual objects.
      }
      key += "_" + method;

      if (paradata) {
        key += "_" + GetPropertyHash(paradata).toString();
      }
      return key;
    },

    _getUrl: function(objectName, method, otherParams) {
      var url =
        "/_api/" +
        objectName +
        "/" +
        method +
        (this.getSiteId() ? "?SiteId=" + this.getSiteId() : "");
      if (otherParams) {
        Object.keys(otherParams).forEach(function(key) {
          url += "&" + key + "=" + otherParams[key];
        });
      }
      return url;
    }
  };

  win.DataCache = DataCache;

  function GetPropertyHash(paradata) {
    if (!paradata) {
      return 0;
    }
    var value = "";
    if (typeof paradata === "string") {
      value = paradata;
    } else {
      for (var key in paradata) {
        value += paradata[key];
      }
    }
    return value.hashCode();
  }

  // should move to move generfic extension function file.
  String.prototype.hashCode = function() {
    var hash = 0,
      i,
      chr;
    if (this.length === 0) return hash;
    for (i = 0; i < this.length; i++) {
      chr = this.charCodeAt(i);
      hash = (hash << 5) - hash + chr;
      hash |= 0;
    }
    return hash;
  };

  function searchLocalStorage(key) {
    var result = {};

    for (var i = 0; i < localStorage.length; i++) {
      var localkey = localStorage.key(i);
      if (localkey.indexOf(key) != -1) {
        result[localkey] = localStorage[localkey];
      }
    }
    return result;
  }
})(window);
