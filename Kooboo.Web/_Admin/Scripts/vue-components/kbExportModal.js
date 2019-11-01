(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbExportModal.html");
    ko.components.register("kb-export-modal", {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;

            this.onHideExportModal = function() {
                self.exportType('complete');
                self.isShow(false);
                self.exportContents().forEach(function(cnt) {
                    cnt.selected(false);
                })
                self.selectedContent([]);
                params.siteId && params.siteId(null);
            }

            this.onExport = function() {
                if (self.exportType() == 'complete') {
                    window.open(Kooboo.Route.Get(Kooboo.Site.ExportUrl(), params.siteId ? {
                        siteId: params.siteId()
                    } : null));
                    self.onHideExportModal();
                } else {
                    var contents = [];
                    self.selectedContent().forEach(function(cnt) {
                        contents.push(cnt.name());
                        contents = _.concat(contents, getRelatedContent(cnt.related()))
                    })

                    if (contents.length) {
                        var hasSiteId = (Kooboo.Site.ExportStoreUrl().indexOf("?") > -1);
                        window.open(Kooboo.Route.Get(Kooboo.Site.ExportStoreUrl(), hasSiteId ? {
                            Stores: contents.join(",")
                        } : {
                            SiteId: params.siteId(),
                            Stores: contents.join(",")
                        }));
                        self.onHideExportModal();
                    } else {
                        window.info.fail(Kooboo.text.info.seleteExportStoreName)
                    }
                }

                function getRelatedContent(list) {
                    var _con = [];
                    list && list.forEach(function(c) {
                        _con.push(c);

                        var find = _.find(contentsData, function(cnt) {
                            return cnt.name == c;
                        })

                        if (find) {
                            _con = _.concat(_con, getRelatedContent(find.related));
                        }
                    })
                    return _con;
                }
            }

            this.exportType = ko.observable('complete');

            this.exportContents = ko.observableArray();

            this.selectedContent = ko.observableArray();

            this.onContentSelected = function(m) {
                m.selected(!m.selected());
                if (m.selected()) {
                    self.selectedContent.push(m);
                } else {
                    self.selectedContent.remove(m);
                }
            }

            Kooboo.Site.getExportStoreNames().then(function(res) {
                if (res.success) {
                    res.model.forEach(function(cnt) {
                        self.exportContents.push(new ExpContent(cnt));
                    })
                }
            })
        },
        template: template
    });

    function ExpContent(info) {
        var self = this;

        ko.mapping.fromJS(info, {}, self);

        self.selected = ko.observable(false);
    }
})()