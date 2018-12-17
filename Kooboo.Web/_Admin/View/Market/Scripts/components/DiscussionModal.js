(function() {
    Kooboo.loadJS([
        "/_Admin/Scripts/kobindings.textError.js"
    ]);


    var template = Kooboo.getTemplate("/_Admin/Market/Scripts/components/DiscussionModal.html");

    ko.components.register('discussion-modal', {
        viewModel: function(params) {
            var self = this;

            this.showError = ko.observable(false);

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    if (self.id()) {
                        Kooboo.Discussion.get({
                            id: self.id()
                        }).then(function(res) {
                            if (res.success) {
                                self.title(res.model.title);
                                self.content(res.model.content);
                                setTimeout(function() {
                                    self.contentLoaded(true);
                                }, 250);
                            }
                        })
                    } else {
                        setTimeout(function() {
                            self.contentLoaded(true);
                        }, 250);
                    }
                }
            })
            this.id = params.id || ko.observable();

            this.contentLoaded = ko.observable(false);

            this.onHide = function() {
                self.title('');
                self.content('');
                self.id(null);
                self.contentLoaded(false);
                self.showError(false);
                self.isShow(false);
            }

            this.title = ko.validateField({
                required: '',
                maxLength: {
                    value: 140,
                    message: Kooboo.text.validation.maxLength + 140
                }
            })

            this.content = ko.validateField({
                required: ''
            })

            this.isValid = function() {
                return self.title.isValid() && self.content.isValid();
            }

            this.onSave = function() {
                if (self.isValid()) {
                    var data = {
                        title: self.title(),
                        content: self.content()
                    }

                    self.id() && (data.id = self.id());

                    Kooboo.Discussion.addOrUpdate(data).then(function(res) {
                        if (res.success) {
                            Kooboo.EventBus.publish('kb/component/discussion-modal/saved')
                            window.info.done(Kooboo.text.info[data.id ? 'update' : 'save'].success);
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