(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/htmlViewer/kbHtmlPath.html");

    ko.components.register("kb-html-path", {
        viewModel: function(params) {

            /*
             *  IN:
             *  elem: ELEMENT
             * 
             *  OUT:
             *  events:[{
             *      event: "kb/html/elem/change",
             *      params: elem
             *  },{
             *      event: "kb/html/elem/hover",
             *      params: elem
             *  }]
             * 
             */

            var self = this;

            this.rootElem = ko.observable(params.rootElem);

            this.elem = ko.observable(params.elem);

            this.path = ko.observableArray();

            this.changeElem = function(m) {
                Kooboo.EventBus.publish("kb/lighter/holder", m.elem);
            }

            this.hoverElem = function(m) {
                Kooboo.EventBus.publish("kb/html/elem/hover", m.elem);
            }

            var _pathList = [];

            if (self.elem()) {

                if (self.rootElem()) {
                    var parent = null;

                    if ($.contains(self.rootElem(), self.elem()) ||
                        $(self.rootElem()).is($(self.elem()))) {
                        parent = self.elem();
                    } else {
                        parent = self.rootElem();
                    }

                    while (($.contains(self.rootElem(), parent) || $(self.rootElem()).is($(parent)))) {

                        var displayText = parent.tagName.toLowerCase();

                        if ($(parent).attr("id")) {
                            displayText += ("#" + $(parent).attr("id"));
                        } else if ($(parent).attr("class")) {
                            if ($(parent).attr("class").indexOf(" ") == -1) {
                                displayText += ("." + $(parent).attr("class"));
                            } else {
                                displayText += ("." + $(parent).attr("class").split(" ").join("."));
                            }
                        }

                        _pathList.push({
                            text: displayText,
                            elem: parent
                        })

                        parent = $(parent).parent()[0];
                    }

                } else {
                    var parent = self.elem();

                    // TODO: to be reviewed
                    while ((window.viewEditor && $.contains(window.viewEditor.position.elem, parent))) {

                        var displayText = parent.tagName.toLowerCase();

                        if ($(parent).attr("id")) {
                            displayText += ("#" + $(parent).attr("id"));
                        } else if ($(parent).attr("class")) {
                            displayText += ("." + $(parent).attr("class"));
                        }

                        _pathList.push({
                            text: displayText,
                            elem: parent
                        })

                        parent = $(parent).parent()[0];
                    }
                }
            }

            self.path(_pathList.reverse());
        },
        template: template
    })
})()