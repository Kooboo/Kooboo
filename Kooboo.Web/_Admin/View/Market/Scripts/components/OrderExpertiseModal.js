(function() {
    Kooboo.loadJS([
        "/_Admin/Scripts/lib/jquery.textarea_autosize.min.js"
    ])

    var template = Kooboo.getTemplate('/_Admin/Market/Scripts/components/OrderExpertiseModal.html');

    ko.components.register('order-expertise-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    setTimeout(function() {
                        $(".autosize").textareaAutoSize().trigger("keyup");
                    }, 280);
                }
            })

            this.onHide = function() {
                self.isShow(false);
            }

            this.request = ko.observable();

            this.attachments = ko.observableArray();

            this.contact = ko.observable();

            this.uploadFile = function(data, files) {
                var fd = new FormData();
                fd.append('filename', files[0].name);
                fd.append('file', files[0]);
                Kooboo.Attachment.uploadFile(fd).then(function(res) {
                    if (res.success) {
                        self.attachments.push(res.model);
                    }
                })
            }

            this.removeFile = function(data, e) {
                Kooboo.Attachment.deleteFile({
                    id: data.id
                }).then(function(res) {
                    if (res.success) {
                        self.attachments.remove(data);
                    }
                })
            }

            this.onOrder = function() {
                Kooboo.Order.expertise({
                    request: self.request(),
                    attachments: self.attachments(),
                    contact: self.contact()
                }).then(function(res) {
                    if (res.success) {
                        Kooboo.EventBus.publish("kb/market/component/cashier/show", res.model);
                    }
                })
            }
        },
        template: template
    })
})()