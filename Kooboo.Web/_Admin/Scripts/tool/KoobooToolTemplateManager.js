(function() {
    var KoobooToolTemplateManager = function(config) {
        var self = this;
        var defaults = {
            templateName: null,
            init: function() {
                self.getTemplateHtml();
            }

        }
        $.extend(self, defaults, config);
        self.init();
        return self;
    }
    KoobooToolTemplateManager.prototype = {
        basePath: "/_Admin/Scripts/tool/toolHtml/",
        getTemplateHtml: function() {
            if (this.templateName) {
                var html = this.getHtml(this.basePath, this.templateName);
                this.templateString = html;
            }
        },
        getHtml: function(basePath, templateName) {
            var htmlUrl = basePath + templateName;
            //use ajax cache
            var html = Kooboo.getTemplate(htmlUrl);
            return html;
        },
        renderHtmlWithData: function(data) {
            var tmpl = "<div></div>";
            if (this.templateString) {
                var _templateFn = _.template(this.templateString);
                tmpl = _templateFn(data);
            }
            return tmpl;
        }
    }
    Kooboo.ToolTemplateManager = KoobooToolTemplateManager;
})();