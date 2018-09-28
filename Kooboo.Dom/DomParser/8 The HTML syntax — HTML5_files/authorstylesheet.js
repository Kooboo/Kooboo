/*global escape, unescape*/
/*
    Author view
    Creates button, sets cookie, toggles authors-only stylesheet.
*/

(function () {
    var link = document.getElementById("author-view")
    ,   authorView = false
    ,   button
    ;
    if (!link) return;
    
    function getCookie (name) {
        var cookieName = name + '='
        ,   cookieArray = document.cookie.split(';')
        ;
        for (var i = 0, n = cookieArray.length; i < n; i++) {
            var c = cookieArray[i].replace(/^\s+/, "");
            if (c.indexOf(cookieName) === 0)
                return unescape(c.substring(cookieName.length, c.length));
        }
        return null;
    }

    function setCookie (name, value) {
        var expDate = new Date(Date.now() + 365 * 24 * 60 * 60 * 1000);
        document.cookie = name + "=" + escape(value) +
                          ";expires=" + expDate.toGMTString() +
                          "; path=/";
    }

    function setState () {
        if (authorView) {
            button.textContent = "Remove developer-view styles";
            link.disabled = false;
            setCookie("authorstyle", "yes");
        }
        else {
            button.textContent = "Add developer-view styles";
            link.disabled = true;
            setCookie("authorstyle", "no");
        }
    }

    function toggle () {
        authorView = !authorView;
        setState();
    }
    
    function init () {
        var cookie = getCookie("authorstyle");
        button = document.createElement("button");
        button.id = "authorButton";
        if (cookie === "yes") authorView = true;
        setState();
        button.onclick = toggle;
        document.getElementById("styleSwitch").appendChild(button);
        checkHash();
    }
    
    function checkHash () {
        if (!authorView) return;
        var hash = document.location.hash.replace(/^#/, "");
        if (!hash) return;
        var target = document.getElementById(hash);
        var node = target;
        while (node) {
            if (window.getComputedStyle(node).getPropertyValue("display") === "none") {
                toggle();
                target.scrollIntoView(true);
                return;
            }
            node = node.parentNode;
        }
    }
    
    window.addEventListener ?
        window.addEventListener("load", init, false) :
        window.attachEvent("onload", init);
    
    window.onhashchange = checkHash;
}());
