(function(kb) {
    kb.BrowserInfo = {
        KEY: "KB_BROWSER_INFO_TYPE",
        init: function() {
            var self = this;
            if (!self.getBrowser()) {
                /* https://stackoverflow.com/questions/9847580/how-to-detect-safari-chrome-ie-firefox-and-opera-browser */
                var isOpera = (!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0,
                    isFirefox = typeof InstallTrigger !== 'undefined',
                    isSafari = /constructor/i.test(window.HTMLElement) || (function(p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || safari.pushNotification),
                    isIE = false || !!document.documentMode,
                    isEdge = !isIE && !!window.StyleMedia,
                    isChrome = !!window.chrome && !!window.chrome.webstore;
                self.setBrowser(isOpera ? "opera" : isFirefox ? "firefox" : isSafari ? "safari" : isIE ? "ie" : isEdge ? "edge" : isChrome ? "chrome" : "");
            }
        },
        getBrowser: function() {
            return localStorage.getItem(this.KEY);
        },
        setBrowser: function(type) {
            localStorage.setItem(this.KEY, type);
        }
    }
})(Kooboo)