$(function() {

    var mediaViewModel = function() {

        var self = this;

        this.curType = ko.observable("list");

        this.pager = ko.observable();
        this.currentPath = ko.observable();

        this.changeType = _.debounce(function(type) {
            self.curType(type);
        }, 300, {
            leading: true
        });

        this.imgTypes = ko.observableArray([{
            displayName: Kooboo.text.site.images.all,
            value: "all"
        }, {
            displayName: Kooboo.text.site.images.page,
            value: "page"
        }, {
            displayName: Kooboo.text.site.images.style,
            value: "style"
        }, {
            displayName: Kooboo.text.site.images.view,
            value: "view"
        }, {
            displayName: Kooboo.text.site.images.layout,
            value: "layout"
        }, {
            displayName: Kooboo.text.common.HTMLblock,
            value: "HTMLBlock"
        }, {
            displayName: Kooboo.text.site.images.content,
            value: "TextContent"
        }]);
        this.curImgType = ko.observable("all")
        this.changeImgType = function(type) {

            if (type !== self.curImgType()) {

                if (type !== "all") {
                    self.currentPath(undefined);
                    Kooboo.Media.getPagedListBy({
                        by: type
                    }).then(function(res) {
                        if (res.success) {
                            var _folders = [];
                            _.forEach(res.model.folders, function(folder) {
                                _folders.push(new folderModel(folder));
                            })
                            self.folders(_folders);
                            self._folders(_folders);

                            var _files = [];
                            _.forEach(res.model.files.list, function(file) {
                                _files.push(new fileModel(file));
                            })
                            self.files(_files);
                            self._files(_files);

                            self.pager(res.model.files);

                            self.crumbPath(res.model.crumbPath);
                            self.curImgType(type);

                            self.reorder();
                            location.hash = "";
                        }
                    })
                } else {
                    self.onChoosingFolder("/");
                    self.curImgType(type);
                }
            }
        }
        this.resetCurImgType = function() {
            self.changeImgType("all")
        }

        this.currentSort = ko.observable("url");
        this.isAsc = ko.observable(false);
        this.currentOrderCSS = function(type) {
            if (self.currentSort() == type) {
                return this.isAsc() ? "asc" : "desc";
            } else {
                return ""
            }
        }
        this.changeSort = function(sort) {
            self.isAsc(self.currentSort() == sort ? !self.isAsc() : false);
            self.currentSort(sort);
            self.reorder();
        }

        this.reorder = function() {

            var _folders = _.cloneDeep(self.folders()),
                _files = _.cloneDeep(self.files());

            switch (self.currentSort()) {

                case "url":
                    _folders = _.sortBy(self.folders(), [function(f) {
                        return f.name()
                    }]);
                    _files = _.sortBy(self.files(), [function(f) {
                        return f.name()
                    }]);
                    break;
                case "size":
                    _files = _.sortBy(self.files(), [function(f) {
                        return f.size();
                    }]);
                    break;
                case "date":
                    _folders = _.sortBy(self.folders(), [function(f) {
                        return f.lastModified();
                    }]);
                    _files = _.sortBy(self.files(), [function(f) {
                        return f.lastModified();
                    }]);
                    break;
            }

            self.folders(_folders);
            self.files(_files);

            if (!self.isAsc()) {
                self.folders.reverse();
                self.files.reverse();
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

        this.selectedFiles = ko.observableArray();
        this.selectedFiles.subscribe(function(files) {
            self.showDeleteBtn(files.length);
        })

        this.crumbPath = ko.observableArray();

        this.folders = ko.observableArray();
        this._folders = ko.observableArray();
        this.files = ko.observableArray();
        this._files = ko.observableArray();

        this.selectDoc = function(doc) {
            doc.selected(!doc.selected());

            if (doc.selected()) {
                self.selectedFiles.push(doc);
            } else {
                self.selectedFiles.remove(doc);
            }

            return true;
        }

        this.editImage = function(m) {
            var id = "";

            if (self.curType() == "grid") {
                id = self.selectedFiles()[0].id();
            } else {
                id = m.id();
            }

            var crumbPath = _.cloneDeep(self.crumbPath());

            location.href = Kooboo.Route.Get(Kooboo.Route.Image.Edit, {
                Id: id
            }) + "#" + crumbPath.reverse()[0].fullPath;
        }

        this.onChoosingFolder = function(path, page) {

            if (!self.currentPath() || self.currentPath() !== path) {
                Kooboo.Media.getPagedList({
                    path: path,
                    pageNr: page ? (typeof page == "number" ? page : 1) : 1
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
                        self._folders(_folders);

                        var _files = [];
                        _.forEach(res.model.files.list, function(file) {
                            _files.push(new fileModel(file));
                        });
                        self.files(_files);
                        self._files(_files);

                        self.pager(res.model.files);

                        self.reorder();

                        self.currentPath(path);
                        if (path !== '/') {
                            location.hash = path
                        } else {
                            location.hash = "";
                        }
                    }
                })
            }
        };

        this.getRelation = function(file, type) {
            Kooboo.EventBus.publish("kb/relation/modal/show", {
                id: file.id(),
                by: type,
                type: "Image"
            });
        }

        this.localDate = function(date) {
            var d = new Date(date());
            return d.toDefaultLangString();
        };

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

                Kooboo.Media.deleteFolders(
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

                Kooboo.Media.deleteImages(
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

        this.uploadImage = function(data, files) {

            function upload() {
                var folders = _.cloneDeep(self.crumbPath());
                data.append("folder", folders.reverse()[0].fullPath);
                Kooboo.Upload.Images(data).then(function(res) {

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

        // Create Folder
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
                var isExistFolder = _.find(self.folders(), function(folder) {
                    return self.folderName() == folder.name();
                })

                if (isExistFolder) {
                    self.onNewFolderModalReset();
                } else {
                    Kooboo.Media.createFolder({
                        path: self.crumbPath()[self.crumbPath().length - 1].fullPath,
                        name: self.folderName()
                    }).then(function(res) {

                        if (res.success) {
                            self.folders.push(new folderModel(res.model));
                            self.onNewFolderModalReset()
                        }
                    })
                }
            } else {
                self.showError(true);
            }
        }

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            if (self.curImgType() == "all") {
                self.onChoosingFolder(self.currentPath(), page);
            } else {
                Kooboo.Media.getPagedListBy({
                    by: type,
                    pageNr: page
                }).then(function(res) {
                    if (res.success) {
                        var _folders = [];
                        _.forEach(res.model.folders, function(folder) {
                            _folders.push(new folderModel(folder));
                        })
                        self.folders(_folders);
                        self._folders(_folders);

                        var _files = [];
                        _.forEach(res.model.files.list, function(file) {
                            _files.push(new fileModel(file));
                        })
                        self.files(_files);
                        self._files(_files);

                        self.pager(res.model.files);

                        self.crumbPath(res.model.crumbPath);
                        self.curImgType(type);

                        self.reorder();
                        location.hash = "";
                    }
                })
            }
        })

    }
    var vm = new mediaViewModel();

    var fileModel = function(file) {
        var self = this;

        file.thumbnail = file.thumbnail + "&timestamp=" + (+new Date());

        ko.mapping.fromJS(file, {}, self);

        self.type = ko.observable("file");

        self.selected = ko.observable(false);

        self.editUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.Image.Edit, {
                Id: self.id()
            })
        })

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

    Kooboo.EventBus.subscribe('window/popstate', function() {
        $('.modal').modal('hide')
        if (location.hash) {
            var path = location.hash.split('#')[1];
            vm.onChoosingFolder(path);
        } else {
            vm.curImgType() == "all" && vm.onChoosingFolder('/')
        }
    })
})