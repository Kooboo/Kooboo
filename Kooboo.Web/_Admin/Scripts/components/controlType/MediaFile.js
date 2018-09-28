(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/controlType/MediaFile.html");

    ko.components.register("media-file", {
        viewModel: function(params) {
            var self = this;

            _.assign(this, params);

            this.mediaDialogData = ko.observable();
            this.enableMultiple = this.isMultipleValue();

            if (this.fieldValue()) {
                if (this.isMultipleValue()) {
                    var pics = this.fieldValue().map(function(p) {
                        return {
                            thumbnail: p + "?SiteId=" + Kooboo.getQueryString("SiteId"),
                            url: p
                        }
                    })
                } else {
                    var pics = [{
                        thumbnail: this.fieldValue() + "?SiteId=" + Kooboo.getQueryString("SiteId"),
                        url: self.fieldValue()
                    }]
                }
                this.pics = ko.observableArray(pics);
            } else {
                this.pics = ko.observableArray([]);
                this.fieldValue(this.isMultipleValue() ? [] : '');
            }

            this.pics.subscribe(function(value) {
                if (self.isMultipleValue()) {
                    self.fieldValue(value.map(function(v) { return v.url; }))
                } else {
                    self.fieldValue(value.length ? value[0].url : null)
                }
            })

            this.selectFile = function() {
                Kooboo.Media.getList().then(function(res) {
                    if (res.success) {
                        res.model["show"] = true;
                        res.model["context"] = self;
                        res.model["onAdd"] = function(selected) {
                            if (self.enableMultiple) {
                                _.forEach(selected, function(sel) {
                                    if (!_.find(self.pics(), { 'url': sel.url })) {
                                        self.pics.push({
                                            url: sel.url,
                                            thumbnail: sel.thumbnail
                                        })
                                    }
                                })
                            } else {
                                self.pics([{
                                    url: selected.url,
                                    thumbnail: selected.thumbnail
                                }])
                            }
                        };
                        self.mediaDialogData(res.model);
                    }
                });
            }

            this.removePic = function(pic) {
                self.pics.remove(pic)
            }
        },
        template: template
    })
})()