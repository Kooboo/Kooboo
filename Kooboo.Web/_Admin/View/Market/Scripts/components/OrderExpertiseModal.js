(function() {
    var template = Kooboo.getTemplate('/_Admin/Market/Scripts/components/OrderExpertiseModal.html');

    ko.components.register('order-expertise-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;

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

            this.onOrder = function() {
                Kooboo.Order.expertise({

                })
            }
        },
        template: template
    })
})()