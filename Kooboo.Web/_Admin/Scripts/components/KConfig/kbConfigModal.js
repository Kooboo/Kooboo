(function() {
    var template = Kooboo.getTemplate('/_Admin/Scripts/components/KConfig/kbConfigModal.html');

    ko.components.register('kb-config-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    self.id(self.rawData().id);
                    self.items(Kooboo.objToArr(self.rawData().binding));

                    setTimeout(function() {
                        $(".autosize").textareaAutoSize().trigger("keyup");
                    }, 300);
                }
            })

            this.rawData = params.rawData;

            this.id = ko.observable();
            this.items = ko.observableArray();

            this.onSave = function() {
                Kooboo.KConfig.update({
                    id: self.id(),
                    binding: Kooboo.arrToObj(self.items())
                }).then(function(res) {
                    if (res.success) {
                        self.isShow(false);
                        Kooboo.EventBus.publish("kb/config/attribute/update")
                    }
                })
            }

            this.onHide = function() {
                self.items([]);
                self.rawData(null);
                self.isShow(false);
            }
        },
        template: template
    })
})()