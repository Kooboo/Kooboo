(function() {
    var template = Kooboo.getTemplate("/_Admin/Witkey/Scripts/components/DiscussionModal.html");

    ko.components.register('discussion-modal', {
        viewModel: function(params) {
            var self = this;

            this.showError = ko.observable(false);

            this.isShow = params.isShow;

            this.onHide = function() {
                self.showError(false);
                self.isShow(false);
            }

            this.title = ko.validateField({
                required: ''
            })

            this.content = ko.validateField({
                required: ''
            })

            this.isValid = function() {
                return self.title.isValid() && self.content.isValid();
            }

            this.onSave = function() {
                if (self.isValid()) {
                    Kooboo.Discussion.add({
                        title: self.title(),
                        content: self.content()
                    }).then(function(res) {
                        if (res.success) {
                            window.info.done('successful');
                            self.onHide();
                        }
                    })
                } else {
                    self.showError(true);
                }
            }
        },
        template: template
    })
})()