(function() {
    Kooboo.loadJS([
        "/_Admin/Scripts/lib/jquery.textarea_autosize.min.js"
    ])

    var template = Kooboo.getTemplate("/_Admin/View/Witkey/Scripts/components/ReplyPanel.html");

    ko.components.register('reply-panel', {
        viewModel: function(params) {
            var self = this;

            this.discussionId = params.discussionId;
            this.parentCommentId = params.parentCommentId || ko.observable(Kooboo.Guid.Empty);

            this.showError = ko.observable(false);

            this.content = ko.validateField({
                required: ''
            })

            this.ableToReply = ko.pureComputed(function() {
                return self.content.isValid();
            })

            this.onReply = function() {
                if (this.content.isValid()) {

                    var content = self.content();

                    if (content.indexOf('\n') > -1) {
                        content = content.split('\n').join('<br>')
                    }
                    Kooboo.Discussion.reply({
                        discussionId: self.discussionId(),
                        parentCommentId: self.parentCommentId(),
                        content: content
                    }).then(function(res) {
                        if (res.success) {
                            Kooboo.EventBus.publish('kb/witkey/component/reply/refresh', {
                                id: self.discussionId(),
                                parentCommentId: self.parentCommentId()
                            })
                            self.content('');
                        }
                    })
                } else {
                    self.showError(true);
                }
            }

            $(".autosize").textareaAutoSize().trigger("keyup");
        },
        template: template
    })
})()