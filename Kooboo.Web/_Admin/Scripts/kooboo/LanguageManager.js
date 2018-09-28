(function() {
    var LanguageManager = function() {};

    LanguageManager.prototype.setLang = function(languange) {
        localStorage.setItem("lang", languange);
    }
    LanguageManager.prototype.getLang = function() {
        return localStorage.getItem("lang");
    }
    LanguageManager.prototype.getTinyMceLanguage = function() {
        var lang = "",
            storedLang = this.getLang();
        if (storedLang) {
            switch (storedLang) {
                case "zh":
                    lang = "zh_CN";
                    break;
                default:
                    lang = "en_US";
                    break;
            }
        } else {
            lang = "en_US";
        }
        return lang;
    };
    LanguageManager.prototype.getDateLanguage = function() {
        return this.getTinyMceLanguage().split("_").join("-");
    };
    Kooboo.LanguageManager = new LanguageManager();
})();