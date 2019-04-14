Kooboo.UrlHelper={
    Get: function(url, params) {
        if(url.indexOf("/_Admin")==-1){
            url="/_Admin"+url;
        }
        var paramStr = "";
        var siteid = Kooboo.getQueryString("SiteId");
        if (siteid) {
            url += "?SiteId=" + siteid;
        }

        if (params) {
            if (url.indexOf("?") > -1) {
                paramStr += "&";
            } else {
                paramStr += "?";
            }

            if (typeof params === "object") {
                var keys = Object.keys(params);
                for (var i = 0, len = keys.length; i < len; i++) {
                    paramStr += keys[i] + "=" + params[keys[i]];

                    if (i !== len - 1) {
                        paramStr += "&";
                    }
                }
            }
            return url + paramStr;
        } else {
            return url;
        }
    },
}