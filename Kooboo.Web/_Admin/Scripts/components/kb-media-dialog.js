(function() {
    var url = "/_Admin/Scripts/components/kb-media-dialog.html";
    var template = Kooboo.getTemplate(url);
    var mapping = ko.mapping;
    ko.components.register("kb-media-dialog", {
        viewModel: function(params) {
            var self = this;

            this.multiple = ko.observable(!!params.multiple);
            this.mediaDialog = ko.observable(false);
            this.folders = ko.observableArray();
            this.crumbPath = ko.observableArray();
            this.files = ko.observableArray();

            this.loading = ko.observable(true);

            this.onAdd = null;

            this.context = ko.observable();

            this.mediaData = params.data;
            this.mediaData.subscribe(function(data) {
                self.mediaDialog(true);
                if (data && data.show) {
                    self.loading(false);
                    self.crumbPath(data.crumbPath);
                    self.folders(data.folders);
                    self.context(data.context);
                    self.files([]);
                    data.files.forEach(function(f) {
                        self.files.push(new fileModel(f));
                    })

                    self.onAdd = data.onAdd;
                }
            })

            this.onHideMediaDialog = function() {
                self.folders([]);
                self.files([]);
                self.crumbPath([]);
                self.context(null);
                self.mediaData(null);
                self.mediaDialog(false);
                self.selectedFiles([]);
                self.loading(true);
            }

            this.curType = ko.observable("list");

            this.changeType = function(type) {
                self.curType(type);
            }

            this.onChoosingFolder = function(path) {
                self.loading(true);
                Kooboo.Media.getDialogList({ path: path }).then(function(res) {
                    if (res.success) {
                        if (!$("body").hasClass("modal-open")) {
                            $("body").addClass("modal-open");
                        }
                        self.crumbPath(res.model.crumbPath);
                        self.folders(res.model.folders);
                        self.files([]);
                        _.forEach(res.model.files, function(file) {
                            self.files.push(new fileModel(file));
                        });
                        self.loading(false);
                    }
                })
            };

            this.onChoosingFile = function(file) {

                var currentSelectedStatus = file.selected();

                if (!self.multiple()) {
                    _.forEach(self.files(), function(_f) {
                        _f.selected(false);
                    });
                    self.selectedFiles.removeAll();
                }

                file.selected(!currentSelectedStatus);

                self.selectedFiles[file.selected() ? "push" : "remove"](file);
            }

            this.localDate = function(date) {
                var d = new Date(date);
                return d.toDefaultLangString();
            };

            this.save = function() {
                if (self.onAdd && typeof self.onAdd == "function") {
                    var data = mapping.toJS(self.selectedFiles());
                    self.onAdd(self.multiple() ? data : data[0]);
                } else {
                    console.warn("Uninitialize function: MediaDialog.onAdd");
                }
                self.onHideMediaDialog();
            }

            this.selectedFiles = ko.observableArray();
            this.selectedFiles.subscribe(function(selected) {

            });

            this.uploadMedia = function(data, files) {
                function upload() {
                    var folders = _.cloneDeep(self.crumbPath());
                    data.append("folder", folders.reverse()[0].fullPath);

                    Kooboo.Upload.Images(data).then(function(res) {

                        if (res.success) {
                            self.onChoosingFolder(folders[0].fullPath);
                        }
                    })
                }

                if (files.length) {
                    if (!Kooboo.isFileNameExist(files, self.files())) {
                        upload();
                    } else {
                        if (confirm(Kooboo.text.confirm.overrideFile)) {
                            upload();
                        }
                    }
                }
            }
        },
        template: template
    })

    var fileModel = function(file) {
        var self = this;

        file.thumbnail = file.thumbnail + "&timestamp=" + (+new Date());

        mapping.fromJS(file, {}, self);

        self.selected = ko.observable(false);

        self.onSelect = function() {
            self.selected(!self.selected());
            return true;
        }
    }
})();