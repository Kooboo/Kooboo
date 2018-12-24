(function() {
    var template = Kooboo.getTemplate('/_Admin/View/Market/Scripts/components/DataCenterModal.html');

    ko.components.register('data-center-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;

            this.dataCenter = ko.observable();
            this.dataCenters = params.available;

            this.onHide = function() {
                self.dataCenter('');
                self.isShow(false);
            }
            this.onSave = function() {
                if (confirm(Kooboo.text.confirm.changeDataCenter)) {
                    Kooboo.Organization.updateDataCenter({
                        datacenter: self.dataCenter()
                    }).then(function(res) {
                        if (res.success) {
                            window.info.done(Kooboo.text.info.update.success);
                            var loc = self.dataCenters().find(function(dc) {
                                return dc.value == self.dataCenter()
                            })
                            Kooboo.EventBus.publish('kb/market/datacenter/updated', {
                                loc: loc.displayName
                            });
                            self.onHide();
                            setTimeout(function() {
                                window.location.href = res.model.redirectUrl || Kooboo.Route.User.LoginPage;
                            }, 300);
                        }
                    })
                }
            }
        },
        template: template
    })
})()