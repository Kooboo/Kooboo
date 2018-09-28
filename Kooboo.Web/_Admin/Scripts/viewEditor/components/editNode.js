(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/viewEditor/components/editNode.html");
    ko.components.register("kb-view-edit-node", {
        viewModel: function(params) {

            var self = this;

            this.onSave = params.onSave;

            this.isShow = ko.observable(false);

            this.type = ko.observable("normal");

            this.href = ko.observable();

            this.inNewWindow = ko.observable(false);

            this.html = ko.observable();

            this.reset = function() {
                self.isShow(false);
                self.html("");
                self.href("");
                self.inNewWindow(false);
                self.type("normal");
            }

            this.save = function() {
                self.onSave({
                    type: self.type(),
                    html: self.html(),
                    href: self.href(),
                    inNewWindow: self.inNewWindow()
                });
                Kooboo.EventBus.publish("kb/frame/dom/update")
                self.reset();
            }

            $('#edit-node-modal').on('shown.bs.modal', function() {
                $(".autosize").textareaAutoSize().trigger("keyup");
            });

            Kooboo.EventBus.subscribe("kb/view/edit/node", function(data) {
                self.isShow(true);
                self.type(data.type);

                self.html(data.html || "");
                self.href(data.href || "");
                self.inNewWindow(data.inNewWindow || "");
            })

        },
        template: template
    })
})()