(function() {
    Kooboo.loadJS([
        "/_Admin/Scripts/kobindings.textError.js"
    ]);


    var template = Kooboo.getTemplate('/_Admin/View/Market/Scripts/components/ExpertiseModal.html');

    ko.components.register('expertise-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = ko.observable(false);
            this.isShow.subscribe(function(show) {
                if (show) {
                    if (!self.symbol()) {
                        Kooboo.Currency.get().then(function(res) {
                            if (res.success) {
                                self.symbol(res.model.symbol);
                            }
                        })
                    }
                    setTimeout(function() {
                        $(".autosize").textareaAutoSize().trigger("keyup");
                    }, 250);
                }
            })

            this.showError = ko.observable(false);

            this.id = ko.observable();
            this.name = ko.validateField({
                required: ''
            });
            this.price = ko.validateField({
                required: ''
            });
            this.description = ko.observable();

            this.symbol = ko.observable();

            this.isValid = function() {
                return self.name.isValid() &&
                    self.price.isValid();
            }

            this.onSave = function() {
                if (self.isValid()) {
                    var exp = {
                        name: self.name(),
                        price: self.price(),
                        description: self.description()
                    };

                    self.id() && (exp.id = self.id());

                    Kooboo.Supplier.addOrUpdate(exp).then(function(res) {
                        if (res.success) {
                            window.info.done(Kooboo.text.info[self.id() ? 'update' : 'save'].success);
                            Kooboo.EventBus.publish('kb/market/component/expertise-modal/updated');
                            self.onHide();
                        }
                    })
                } else {
                    self.showError(true);
                }
            }
            this.onHide = function() {
                self.showError(false);
                self.id('');
                self.name('');
                self.price('');
                self.description('');
                self.isShow(false);
            }

            Kooboo.EventBus.subscribe('kb/market/component/expertise-modal/show', function(id) {
                if (id) {
                    Kooboo.Supplier.get({
                        id: id
                    }).then(function(res) {
                        if (res.success) {
                            var data = res.model;
                            self.id(data.id);
                            self.name(data.name);
                            self.price(data.price);
                            self.symbol(data.symbol);
                            self.description(data.description);
                            self.isShow(true);
                        }
                    })
                } else {
                    self.isShow(true);
                }
            })

        },
        template: template
    })
})()