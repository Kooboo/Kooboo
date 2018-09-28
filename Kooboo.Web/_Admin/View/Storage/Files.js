$(function() {

    var filesModel = function() {
        var self = this;

        this.onCreateFolder = function() {
            self.newFolderModal(true);
        }

        this.hasDocs = ko.pureComputed(function() {
            return (self.folders().length + self.files().length) > 0;
        })

        this.crumbPath = ko.observableArray();

        this.folders = ko.observableArray();

        this.files = ko.observableArray();

        this.selectDoc = function(doc) {
            doc.selected(!doc.selected());

            if (doc.selected()) {
                self.selectedFiles.push(doc);
            } else {
                self.selectedFiles.remove(doc);
            }

            return true;
        }

        this.selectedFiles = ko.observableArray();
        this.selectedFiles.subscribe(function(files) {
            self.showDeleteBtn(files.length);
        })

        this.localDate = function(date) {
            var d = new Date(date());
            return d.toDefaultLangString();
        };

        this.onChoosingFolder = function(path) {

            Kooboo.File.getList({
                path: path
            }).then(function(res) {

                if (res.success) {
                    self.selectAll(false);
                    self.selectedFiles.removeAll();
                    self.crumbPath(res.model.crumbPath);

                    var _folders = [];
                    _.forEach(res.model.folders, function(folder) {
                        _folders.push(new folderModel(folder));
                    })
                    self.folders(_folders);

                    var _files = [];
                    _.forEach(res.model.files, function(file) {
                        _files.push(new fileModel(file));
                    });
                    self.files(_files);

                    location.hash = path
                }
            })
        }

        // Delete button
        this.showDeleteBtn = ko.observable(false);
        this.onDelete = function() {

            if (confirm(Kooboo.text.confirm.deleteItems)) {
                var folders = [],
                    files = [];

                _.forEach(self.selectedFiles(), function(selected) {

                    if (selected.type() == "folder") {
                        folders.push(selected.fullPath());
                    } else if (selected.type() == "file") {
                        files.push(selected.id());
                    }
                })

                Kooboo.File.deleteFolders(
                    JSON.stringify(folders)
                ).then(function(res) {

                    if (res.success) {
                        _.forEach(folders, function(fullPath) {
                            var _find = _.find(self.folders(), function(folder) {
                                return folder.fullPath() == fullPath;
                            })

                            _find && self.folders.remove(_find);
                        });
                    }
                });

                Kooboo.File.deleteFiles(
                    JSON.stringify(files)
                ).then(function(res) {

                    if (res.success) {
                        _.forEach(files, function(id) {
                            var _find = _.find(self.files(), function(files) {
                                return files.id() == id;
                            })

                            _find && self.files.remove(_find);
                        });
                    }
                })

                self.selectedFiles.removeAll();
            }
        }

        this.selectAll = ko.pureComputed({
            read: function() {
                var allLength = self.folders().length + self.files().length
                if (allLength === 0) {
                    return false;
                }
                return self.selectedFiles().length == allLength;
            },
            write: function(checked) {

                self.selectedFiles.removeAll();

                _.forEach(self.folders(), function(folder) {
                    folder.selected(checked);
                    checked && self.selectedFiles.push(folder);
                })
                _.forEach(self.files(), function(file) {
                    file.selected(checked);
                    checked && self.selectedFiles.push(file);
                })
            },
            owner: this
        })

        this.uploadFile = function(data, files) {
            function upload() {
                var folders = _.cloneDeep(self.crumbPath());
                data.append("folder", folders.reverse()[0].fullPath);

                Kooboo.Upload.File(data).then(function(res) {

                    if (res.success) {
                        self.onChoosingFolder(folders[0].fullPath);
                    }
                })
            }

            if (!Kooboo.isFileNameExist(files, self.files())) {
                upload();
            } else {

                if (confirm(Kooboo.text.confirm.overrideFile)) {
                    upload();
                }
            }
        }

        // new folder
        this.newFolderModal = ko.observable(false);

        this.showError = ko.observable(false);

        this.folderName = ko.validateField({
            required: Kooboo.text.validation.required,
            localUnique: {
                compare: function() {
                    var list = [];
                    _.forEach(self.folders(), function(folder) {
                        list.push(folder.name());
                    })
                    list.push(self.folderName());
                    return list;
                },
                message: Kooboo.text.validation.taken
            },
            stringlength: {
                min: 1,
                max: 64,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
            },
            regex: {
                pattern: /^([a-zA-Z\w\-\.])*$/,
                message: Kooboo.text.validation.folderNameInvalid
            }
        });

        this.onCreateFolder = function() {
            self.folderName("");
            self.newFolderModal(true);
        }

        this.onNewFolderModalReset = function() {
            self.newFolderModal(false);
            self.folderName("");
            self.showError(false);
        }

        this.onNewFolderModalSubmit = function() {

            if (self.folderName.isValid()) {
                Kooboo.File.createFolder({
                    path: self.crumbPath()[self.crumbPath().length - 1].fullPath,
                    name: self.folderName()
                }).then(function(res) {

                    if (res.success) {
                        self.onChoosingFolder(location.hash ? location.hash.split("#")[1] : "");
                        self.onNewFolderModalReset()
                    }
                })
            } else {
                self.showError(true);
            }
        }

        // Relation Modal
        this.getRelation = function(file, type) {
            Kooboo.EventBus.publish("kb/relation/modal/show", {
                id: file.id(),
                by: type,
                type: "CmsFile"
            });
        }
    }
    var vm = new filesModel();


    var fileModel = function(file) {
        var self = this;

        ko.mapping.fromJS(file, {}, self);

        self.type = ko.observable("file");

        self.selected = ko.observable(false);

        self.onSelect = function(file) {
            file.selected(!file.selected());
            return true;
        }
    }

    var folderModel = function(folder) {
        var self = this;

        ko.mapping.fromJS(folder, {}, self);

        self.type = ko.observable("folder");

        self.selected = ko.observable(false);

        self.onSelect = function(folder) {
            folder.selected(!folder.selected());
            return true;
        }
    }

    ko.applyBindings(vm, document.getElementById("main"));

    vm.onChoosingFolder(location.hash ? location.hash.split("#")[1] : "");
})