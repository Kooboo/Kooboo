(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbHtmlViewer.html");
    ko.components.register("kb-html-viewer", {
        viewModel: function() {

            var self = this;

            this.elem = ko.observable();

            this.rootElem = ko.observable();

            Kooboo.EventBus.subscribe("kb/html/previewer/select", function(elem) {
                self.elem(elem);
            });

            Kooboo.EventBus.subscribe("kb/html/previewer/hover", function(elem) {
                Kooboo.EventBus.publish("kb/html/tree/elem/hover", elem);
            });

            Kooboo.EventBus.subscribe("kb/html/previewer/rootElem", function(elem) {
                self.rootElem(elem);
            })
        },
        template: template
    })
})()