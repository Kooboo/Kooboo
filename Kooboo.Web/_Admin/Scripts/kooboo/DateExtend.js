(function() {
    Date.prototype.toDefaultLangString = function() {
        return this.toLocaleString(Kooboo.LanguageManager.getDateLanguage());
    }
})();