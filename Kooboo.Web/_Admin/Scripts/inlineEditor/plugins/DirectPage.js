function DirectPage(){
    var aRegExp= /^a$/i,
        urlRegExp=/^(http:\/\/||https:\/\/)/i;
    var param={
        context:null
    }
    function getATag() {
        var el = param.context.el;
        if (el && el.tagName) {
            return el.tagName.toLocaleLowerCase() === "a" ? el : $(el).closest("a").get(0);
        }
        return null;
    }
    function isExternalUrl(url) {
        var editUrlHost = window.location.host;
        return url.indexOf(editUrlHost) == -1;
    }
    function  getRedirectUrl(currentUrl) {
        var url =  window.location.href;

        var origin = window.location["origin"];
        var pageUrl = currentUrl.replace(origin, "");

        url = setUrlValueParam(url, "PageId", "");
        url = setUrlValueParam(url, "pageId", "");
        url = setUrlValueParam(url, "PageUrl", encodeURIComponent(pageUrl));
        url = setUrlValueParam(url, "pageUrl", encodeURIComponent(pageUrl));
        url = setUrlValueParam(url, "koobooinline", "design");
        return url;
    }
    function setUrlValueParam(destiny, par, par_value) {
        var pattern = par + "=([^&]*)";
        var replaceText = par + "=" + par_value;
        if (destiny.match(pattern)) {
            var tmp = "/\\" + par + "=[^&]*/";
            tmp = destiny.replace(eval(tmp), replaceText);
            return (tmp);
        } else {
            if (destiny.match("[\?]")) {
                return destiny + "&" + replaceText;
            } else {
                return destiny + "?" + replaceText;
            }
        }
    }
    return {
        menuName: Kooboo.text.inlineEditor.directToPage,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            var aTag = getATag();
            if (aTag) {
                var url = aTag.href;
                return aRegExp.test(aTag.tagName) && 
                urlRegExp.test(url) && 
                !isExternalUrl(url);
            }
            return false;
        },
        getHtml:function(){
            "";
        },
        init:function(){

        },
        click: function() {
            var el = param.context.el;
            var curEl = el.tagName.toLocaleLowerCase() === "a" ? el : $(el).closest("a").get(0);
            if (curEl && curEl.href && window.confirm(Kooboo.text.inlineEditor.directToLink)) {
                var redictUrl = getRedirectUrl(curEl.href);

                window.__gl.actionMenu.save(false);

                parent.window.location.href = redictUrl;
                window.location.href = redictUrl;
            }
        },

    }
}