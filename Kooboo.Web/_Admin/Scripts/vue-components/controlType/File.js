(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/controlType/File.html");

    ko.components.register("file", {
        viewModel: function(params) {
            var self = this;

            _.assign(this, params);

            this.fileDialogData = ko.observable();
            this.enableMultiple = this.isMultipleValue();

            if (this.fieldValue()) {
                if (this.isMultipleValue()) {
                    var files = this.fieldValue().map(function(p) {
                        return {
                            thumbnail: p + "?SiteId=" + Kooboo.getQueryString("SiteId"),
                            url: p
                        }
                    })
                } else {
                    var files = [{
                        thumbnail: this.fieldValue() + "?SiteId=" + Kooboo.getQueryString("SiteId"),
                        url: self.fieldValue()
                    }]
                }
                this.files = ko.observableArray(files);
            } else {
                this.files = ko.observableArray([]);
                this.fieldValue(this.isMultipleValue() ? [] : '');
            }

            this.files.subscribe(function(value) {
                if (self.isMultipleValue()) {
                    self.fieldValue(value.map(function(v) { return v.url; }))
                } else {
                    self.fieldValue(value.length ? value[0].url : null)
                }
            })

            this.selectFile = function() {
                Kooboo.File.getList().then(function(res) {
                    if (res.success) {
                        res.model["show"] = true;
                        res.model["context"] = self;
                        res.model["onAdd"] = function(selected) {
                            if (self.enableMultiple) {
                                _.forEach(selected, function(sel) {
                                    if (!_.find(self.files(), { 'url': sel.url })) {
                                        self.files.push({
                                            url: sel.url,
                                            thumbnail: sel.thumbnail
                                        })
                                    }
                                })
                            } else {
                                self.files([{
                                    url: selected.url,
                                    thumbnail: selected.thumbnail
                                }])
                            }
                        };
                        self.fileDialogData(res.model);
                    }
                });
            }

            this.removeFile = function(File) {
                self.files.remove(File)
            }
        },
        template: template
    })
})()