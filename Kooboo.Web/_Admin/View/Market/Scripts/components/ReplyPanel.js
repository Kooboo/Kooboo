(function () {
    Kooboo.loadJS([
        "/_Admin/Scripts/kooboo/Guid.js",
        "/_Admin/Scripts/lib/jquery.textarea_autosize.min.js"
    ])

    var template = Kooboo.getTemplate("/_Admin/Market/Scripts/components/ReplyPanel.html");

    ko.components.register('reply-panel', {
        viewModel: function (params) {
            var self = this;

            this.type = ko.observable(params.type);
            this.typeId = params.typeId;
            this.parentId = params.parentId || ko.observable(Kooboo.Guid.Empty);

            this.disabled = params.disabled || ko.observable(false);

            this.showError = ko.observable(false);

            this.uploadRequired = ko.pureComputed(function () {
                return ['supplier', 'delivery'].indexOf(self.type()) > -1;
            })

            this.content = ko.validateField({
                required: ''
            })

            this.ableToReply = ko.pureComputed(function () {
                return self.content.isValid();
            })

            this.onReply = function () {
                if (this.content.isValid()) {
                    switch (self.type()) {
                        case 'discussion':
                            Kooboo.Discussion.reply({
                                ownerId: self.typeId(),
                                parentId: self.parentId(),
                                content: self.content()
                            }).then(function (res) {
                                if (res.success) {
                                    Kooboo.EventBus.publish('kb/witkey/component/reply/refresh', {
                                        id: self.typeId(),
                                        parentId: self.parentId()
                                    })
                                    self.content('');
                                }
                            })
                            break;
                        case 'demand':
                            Kooboo.Demand.reply({
                                ownerId: self.typeId(),
                                parentId: self.parentId(),
                                content: self.content()
                            }).then(function (res) {
                                if (res.success) {
                                    Kooboo.EventBus.publish('kb/witkey/demand/reply/refresh', {
                                        id: self.typeId(),
                                        parentId: self.parentId()
                                    })
                                    self.content('');
                                }
                            })
                            break;
                        case 'delivery':
                            Kooboo.Demand.chat({
                                ownerId: self.typeId(),
                                isPublic: false,
                                content: self.content()
                            }).then(function (res) {
                                if (res.success) {
                                    Kooboo.EventBus.publish('kb/market/chat/sent', function () {
                                        self.showError(false);
                                        self.content('');
                                    })
                                }
                            })
                            break;
                        case 'supplier':
                            Kooboo.Supplier.reply({
                                ownerId: self.typeId(),
                                content: self.content()
                            }).then(function (res) {
                                if (res.success) {
                                    Kooboo.EventBus.publish('kb/market/reply/sent', function () {
                                        self.showError(false);
                                        self.content('');
                                    })
                                }
                            })
                            break;
                    }
                } else {
                    self.showError(true);
                }
            }

            this.uploadFile = function (data, files) {
                var fd = new FormData();
                fd.append('filename', files[0].name);
                fd.append('file', files[0]);
                Kooboo.Attachment.uploadFile(fd).then(function (res) {
                    if (res.success) {
                        switch (self.type()) {
                            case 'delivery':
                                Kooboo.Demand.chat({
                                    ownerId: self.typeId(),
                                    isPublic: false,
                                    content: '',
                                    attachments: [res.model]
                                });
                                break;
                            case 'supplier':
                                Kooboo.Supplier.reply({
                                    ownerId: self.typeId(),
                                    content: '',
                                    attachments: [res.model]
                                })
                                break;
                        }
                    }
                })
            }

            $(".autosize").textareaAutoSize().trigger("keyup");
        },
        template: template
    })
})()