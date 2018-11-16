(function() {
    var template = Kooboo.getTemplate("/_Admin/Witkey/Scripts/components/ChatModal.html");

    var CURRENT_USER_ID = '';
    var INTERVAL = null;

    ko.components.register('chat-modal', {
        viewModel: function(params) {
            var self = this;

            this.showError = ko.observable(false);

            this._isShow = params.isShow;
            this._isShow.subscribe(function(show) {
                if (show) {
                    self.getMessages(function() {
                        self.isShow(true);
                    })


                    INTERVAL = setInterval(function() {
                        self.getMessages();
                    }, 5000);

                } else {
                    self.isShow(false);
                }
            })

            this.getMessages = function(cb) {
                Kooboo.Demand.getPrivateCommentList({
                    proposalId: self.proposalId()
                }).then(function(res) {
                    if (res.success) {
                        self.messages(res.model.list.map(function(item) {
                            return new Message(item);
                        }));
                        cb && cb();
                    }
                })
            }

            this.demandId = params.demandId;
            this.proposalId = params.proposalId;

            this.isShow = ko.observable(false);
            this.messages = ko.observableArray();

            this.newMsg = ko.validateField({
                required: ''
            })

            this.onSendMsg = function() {
                if (self.newMsg.isValid()) {
                    Kooboo.Demand.reply({
                        demandId: self.demandId(),
                        proposalId: self.proposalId(),
                        isPublic: false,
                        content: self.newMsg()
                    }).then(function(res) {
                        if (res.success) {
                            self.getMessages(function() {
                                self.showError(false);
                                self.newMsg('');
                            });
                        }
                    })
                } else {
                    self.showError(true);
                }
            }

            this.onHide = function() {
                self.showError(false);
                self._isShow(false);
                clearInterval(INTERVAL);
            }

            this.rendered = function() {
                setTimeout(function() {
                    Holder.run()
                }, 300);
            }

            this.uploadFile = function(data, files) {
                var fd = new FormData();
                fd.append('filename', files[0].name);
                fd.append('file', files[0]);
                debugger
                Kooboo.Demand.uploadFile(fd).then(function(res) {
                    if (res.success) {
                        debugger
                    }
                })
            }
        },
        template: template
    })

    function Message(data) {
        if (!CURRENT_USER_ID) {
            CURRENT_USER_ID = localStorage.getItem('_kooboo_api_user');
        }

        var date = new Date(data.createTime);

        this.firstLetter = ko.observable(data.userName.split('')[0].toUpperCase());
        this.content = ko.observable(data.content);
        this.isCurrentUser = ko.observable(data.userId == CURRENT_USER_ID);
        this.date = ko.observable(date.toDefaultLangString());
    }
})()