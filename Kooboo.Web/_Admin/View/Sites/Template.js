$(function() {

    var publicTabNeedRefresh = false;

    var templateModel = function() {
        var self = this;

        this.types = ko.observableArray([{
                displayText: Kooboo.text.site.template.public,
                value: "list"
            }, /*{displayText: Kooboo.text.site.template.private,value: "private"}, */
            {
                displayText: Kooboo.text.site.template.personal,
                value: "personal"
            }
        ]);
        this.type = ko.observable();

        this._cachedList = ko.observable();
        this._cachePersonal = ko.observable();

        this.changeType = function(type) {

            if (['list', 'personal'].indexOf(type.toLowerCase()) == -1) {
                type = 'list';
            }

            if (self.type() !== type) {
                self.type(type);

                if (type == 'list') {
                    if (!self.packages().length || publicTabNeedRefresh) {
                        Kooboo.Template.list().then(function(res) {
                            if (res.success) {
                                publicTabNeedRefresh = false;
                                self.packages(res.model.list || []);
                                self.pager(res.model);
                                self._cachedList(res.model);
                            }
                        })
                    } else {
                        self.pager(self._cachedList());
                    }
                } else {
                    if (!self.personalPackages().length) {
                        Kooboo.Template.personal().then(function(res) {
                            if (res.success) {
                                self.personalPackages(res.model.list || []);
                                self.pager(res.model);
                                self._cachePersonal(res.model);
                            }
                        })
                    } else {
                        self.pager(self._cachePersonal());
                    }
                }
            }
        }

        this.searched = ko.observable(false);
        this.searchCount = ko.observable();
        this.keywordFocus = ko.observable(false);
        this.searchKey = ko.observable();
        this.searchStart = function() {

            if (self.searchKey()) {
                self.searched(true);
                Kooboo.Template.Search({
                    keyword: self.searchKey()
                }).then(function(res) {

                    if (res.success) {
                        self.pager(res.model);
                        if (res.model.list && res.model.list.length) {
                            self.packages(res.model.list);
                            self.searchCount(res.model.totalCount);
                        } else {
                            self.packages([]);
                        }
                    }
                })
            } else {
                Kooboo.Template[self.type()]().then(function(res) {

                    if (res.success) {
                        self.searched(false);
                        if (self.type() == 'personal') {
                            self.personalPackages(res.model.list || [])
                        } else {
                            self.packages(res.model.list || []);
                        }
                        self.pager(res.model);
                    }
                })
            }
        }

        this.handleEnter = function(m, e) {

            if (e.keyCode == 13) {
                self.searchStart();
            }
        }

        this.changeKeyword = function() {
            self.keywordFocus(true);
        }

        this.clearSearchResult = function() {
            Kooboo.Template.getList().then(function(res) {

                if (res.success) {
                    self.searched(false);
                    self.searchKey("");
                    self.searchCount(0);
                    self.packages(res.model.list || []);
                    self.pager(res.model);
                }
            })
        }

        this.pager = ko.observable();

        this.packages = ko.observableArray();

        this.personalPackages = ko.observableArray();

        this.afterRender = function() {
            $("img.lazy").lazyload({
                event: "scroll",
                effect: "fadeIn"
            });
        }

        this.package = ko.observable();

        this.removePackage = function(p) {

            if (confirm(Kooboo.text.confirm.deleteItem)) {
                Kooboo.Template.Delete({
                    Id: p.id
                }).then(function(res) {

                    if (res.success) {
                        publicTabNeedRefresh = true;
                        self.personalPackages.remove(p);
                        Kooboo.Template.personal().then(function(res) {
                            if (res.success) {
                                self.personalPackages(res.model.list || []);
                                self.pager(res.model);
                                self._cachePersonal(res.model);
                            }
                        })
                        window.info.show(Kooboo.text.info.delete.success, true);
                    } else {
                        window.info.show(Kooboo.text.info.delete.fail, false);
                    }
                })
            }
        }

        this.editing = ko.observable();

        this.editPackage = function(package) {

            Kooboo.Template.Get({
                siteId: package.siteId,
                id: package.id
            }).then(function(res) {

                if (res.success) {
                    self.editTemplateModal(true);
                    self.editing(new packageModel(res.model));

                    var defaultImgIdx = _.findIndex(res.model.images, function(img) {
                        return img.indexOf(res.model.thumbNail) > -1;
                    });
                    var index = defaultImgIdx > -1 ? defaultImgIdx : 0;
                    if (self.editing().imgList().length > index)
                        self.editing().imgList()[index].selected(true);
                }
            })
        }
        this.packageExist = ko.observable();
        this.packageName = ko.observable();

        this.updatePackage = ko.observable();
        this.updatePackage.subscribe(function(file) {
            $("#update-package").val("");

            if (file) {
                self.packageExist(true);
                self.packageName(file.name);
            } else {
                self.packageExist(false);
            }
        })

        this.clearFile = function() {
            self.updatePackage("");
        }

        this.chooseTemplate = function(package) {
            Kooboo.Template.Get({
                id: package.id
            }).then(function(res) {

                if (res.success) {
                    self.templateData(res.model);
                    self.showTemplateModal(true);
                }
            })
        }

        this.showTemplateModal = ko.observable(false);
        this.templateData = ko.observable();

        this.templateModal = ko.observable(false);

        this.searchTag = function(tag) {
            console.log("TODO\t search tag: " + tag);
        }

        this.editTemplateModal = ko.observable(false);

        this.selectedTags = ko.observableArray();

        this.resetEdit = function() {
            self.editTemplateModal(false);
            self.editing(null);
            self.package(null);
            $("#select2_element").select2("destroy");
        }

        this.chooseAsDefault = function(m, v) {
            self.editing().imgList().forEach(function(f) {
                f.selected(false);
            })
            m.selected(true);
        }

        this.removeFile = function(type, data) {
            var find = _.find(self.editing().imgList(), function(f) {
                return f.type == type && f == data;
            })

            if (find) {
                self.editing().imgList.remove(find);
                if (find.selected() && self.editing().imgList().length) {
                    self.editing().imgList()[0].selected(true);
                }

                if (type == "img") {
                    var idx = _.findIndex(self.editing().images(), function(img) {
                        return img == data.value();
                    })

                    if (idx > -1) {
                        var orig = self.editing().images();
                        orig.splice(idx, 1);
                        self.editing().images(orig);
                    }
                } else {
                    self.editing().files.remove(data.target);
                }
            }
        }

        this.saveEdit = function() {
            var package = ko.mapping.toJS(self.editing());
            package.files = self.editing().files();

            var data = new FormData();

            data.append("id", package.id);
            data.append("isPrivate", package.isPrivate);
            data.append("category", package.category);
            data.append("tags", package.selectedTags);
            data.append("description", package.description ? package.description : "");
            data.append("link", package.link ? package.link : "");
            data.append("images", JSON.stringify(package.images));

            var fileIds = [];
            package.imgList.forEach(function(img) {
                img.type == "file" && fileIds.push(img.target.lastModified + img.target.name);
            })

            var idx = -1;
            package.files.forEach(function(file) {
                if (fileIds.indexOf(file.lastModified + file.name) > -1) {
                    data.append("file_" + (++idx), file);
                }
            })

            if (self.packageExist()) {
                data.append("binary", self.updatePackage());
            }

            var coverImg = _.find(self.editing().imgList(), function(f) {
                return f.selected();
            })

            if (coverImg) {
                if (coverImg.type == "file") {
                    var idx = _.findIndex(_.filter(self.editing().imgList(), function(f) {
                        return f.type == "file";
                    }), function(f) {
                        return f.selected();
                    })
                    data.append("defaultFile", Math.max(0, idx));
                } else {
                    var idx = _.findIndex(_.filter(self.editing().imgList(), function(f) {
                        return f.type == "img";
                    }), function(f) {
                        return f.selected();
                    })
                    data.append("defaultImg", Math.max(0, idx));
                }
            }

            Kooboo.Template.Update(data).then(function(res) {

                if (res.success) {
                    publicTabNeedRefresh = true;
                    self.updatePackage(null);
                    Kooboo.Template.personal().then(function(r) {
                        if (r.success) {
                            self.searched(false);
                            self.personalPackages(r.model.list);
                            self.pager(r.model);
                            self.resetEdit();
                            window.info.show(Kooboo.text.info.update.success, true);
                        }
                    })
                } else {
                    window.info.show(Kooboo.text.info.update.fail, false);
                }
            })
        }

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {

            if (self.searched()) {
                Kooboo.Template.Search({
                    keyword: self.searchKey(),
                    pageNr: page
                }).then(function(res) {

                    if (res.success) {
                        self.packages(res.model.list || []);
                        self.pager(res.model);
                    }
                })
            } else {
                Kooboo.Template[self.type()]({
                    pageNr: page
                }).then(function(res) {

                    if (res.success) {
                        if (self.type() == 'personal') {
                            self.personalPackages(res.model.list || []);
                            self._cachePersonal(res.model);
                        } else {
                            self.packages(res.model.list || []);
                            self._cachedList(res.model);
                        }
                        self.pager(res.model);
                    }
                })
            }
        })

        self.changeType(Kooboo.getQueryString("type") || "list");

        if (self.type() == 'personal') {
            $('.nav.nav-tabs').children().last().find('a').tab('show');
        } else {
            $('.nav.nav-tabs').children().first().find('a').tab('show');
        }
    }

    var vm = new templateModel();

    ko.applyBindings(vm, document.getElementById("main"));

    $("#editTemplateModal").on("shown.bs.modal", function() {
        var test = $("#select2_element").select2({
            tags: true,
            tokenSeparators: [',', ' ', ';']
        });
        $(".autosize").textareaAutoSize().trigger("keyup");

        $("#edit_file").change(function() {
            var files = this.files,
                filesLength = files.length,
                uploadLength = 0;

            if (filesLength) {
                _.forEach(files, function(file) {
                    let suffix = file.name.split('.').reverse()[0].toLowerCase();
                    if (['bmp', 'png', 'jpg', 'jpeg'].indexOf(suffix) > -1) {
                        vm.editing().files.push(file);
                        uploadLength++;
                    }
                })
            }

            if (uploadLength !== filesLength) {
                alert(Kooboo.text.alert.imageFileUploaded);
            }

            $(this).val("");
        })
    })
    $('.nav.nav-tabs a').click(function(e) {
        $(this).tab('show');
        setTimeout(function() {
            $(window).resize();
        }, 300);
    })

    function packageModel(package) {
        var self = this;
        package.files = [];
        package.fileList = [];

        package.imgList = [];

        package.defaultImg = "";

        package.selectedTags = [];

        if (!package.tags) {
            package.tags = [];
        } else if (typeof package.tags == "string") {
            package.tags = package.tags.split(",");
        }
        package.selectedTags = _.cloneDeep(package.tags);

        ko.mapping.fromJS(package, {}, self);

        self.images().forEach(function(img) {
            self.imgList.push(new imgModel(img));
        })

        self.files.subscribe(_.debounce(function(files) {
            self.imgList().forEach(function(f) {
                f.type == "file" && self.imgList.remove(f);
            })
            _.forEach(files, function(f) {
                self.imgList.push(new fileModel(f));
            })
        }, 300));
    }

    var imgModel = function(url) {
        var self = this;
        this.value = ko.observable(url);
        this.selected = ko.observable(false);
        this.type = "img";
    }

    var fileModel = function(file) {
        var self = this;
        this.name = file.name;
        this.value = ko.observable();
        this.selected = ko.observable(false);
        this.target = file;
        this.type = "file";

        var reader = new FileReader();
        reader.onloadend = function() {
            self.value(reader.result);
        }

        reader.readAsDataURL(file);
    }

    $("#update-package").change(function() {
        var files = this.files;

        if (files.length > 0) {
            var name = files[0].name;

            if (name.indexOf(".") > -1 && name.split(".").reverse()[0] == "zip") {
                vm.updatePackage(files[0]);
            } else {
                alert(Kooboo.text.alert.uploadZipFile);
            }
        }
    })
})