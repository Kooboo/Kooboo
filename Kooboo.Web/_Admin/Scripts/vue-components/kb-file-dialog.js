(function() {
    var self;
    Vue.component("kb-file-dialog", {
      template: Kooboo.getTemplate(
        "/_Admin/Scripts/vue-components/kb-file-dialog.html"
      ),
      props: {
        data: Object,
        multiple: Boolean
      },
      data: function() {
        self = this;
        return {
          fileDialog: false,
          folders: [],
          crumbPath: [],
          files: [],
          loading: true,
          onAdd: null,
          context: null,
          curType: "list",
          selectedFiles: [],
          uploadSetting: {
            allowMultiple: true,
            acceptTypes: ['*/*'],
            callback: self.uploadFile
          }
        };
      },
      methods: {
        onHideFileDialog: function() {
          self.folders = [];
          self.files = [];
          self.crumbPath = [];
          self.context = null;
          self.fileDialog = false;
          self.selectedFiles = [];
          self.loading = true;
          self.$emit("update:data", null);
        },
        changeType: function(type) {
          self.curType = type;
        },
        onChoosingFolder: function(path) {
          self.loading = true;
          Kooboo.File.getList({ path: path }).then(function(res) {
            if (res.success) {
              if (!$("body").hasClass("modal-open")) {
                $("body").addClass("modal-open");
              }
              self.crumbPath = res.model.crumbPath;
              self.folders = res.model.folders;
              self.files = [];
              _.forEach(res.model.files, function(file) {
                self.files.push(new fileModel(file));
              });
              self.loading = false;
            }
          });
        },
        onChoosingFile: function(file) {
          var currentSelectedStatus = file.selected;
          if (!self.multiple) {
            _.forEach(self.files, function(_f) {
              _f.selected = false;
            });
            self.selectedFiles = [];
          }
          file.selected = !currentSelectedStatus;
          if (file.selected) {
            self.selectedFiles.push(file);
          } else {
            self.selectedFiles = _.without(self.selectedFiles, file);
          }
        },
        localDate: function(date) {
          var d = new Date(date);
          return d.toDefaultLangString();
        },
        save: function() {
          var data = self.selectedFiles;
          var result = self.multiple ? data : data[0];
          // console.log(result)
          if (self.onAdd && typeof self.onAdd == "function") {
            self.onAdd(result);
          }
          self.$emit("on-add", result);
          self.onHideFileDialog();
        },
        upload: function(data) {
          var folders = _.cloneDeep(self.crumbPath);
          data.append("folder", folders.reverse()[0].fullPath);
          Kooboo.Upload.Images(data).then(function(res) {
            if (res.success) {
              self.onChoosingFolder(folders[0].fullPath);
            }
          });
        },
        uploadFile: function(data, files) {
          if (files.length) {
            if (!Kooboo.isFileNameExist(files, self.files)) {
              self.upload(data);
            } else {
              if (confirm(Kooboo.text.confirm.overrideFile)) {
                self.upload(data);
              }
            }
          }
        }
      },
      watch: {
        data: function(data) {
          if (data && data.show) {
            self.fileDialog = true;
            self.loading = false;
            self.crumbPath = data.crumbPath;
            self.folders = data.folders;
            self.context = data.context;
            self.files = [];
            data.files.forEach(function(f) {
              self.files.push(fileModel(f));
            });
            self.onAdd = data.onAdd;
          }
        }
      },
      beforeDestroy: function() {
        self = null;
      }
    });
  
    var fileModel = function(file) {
      file.thumbnail = file.thumbnail + "&timestamp=" + +new Date();
      file.selected = false;
      return file;
    };
  })();
  



(function() {
    var url = "/_Admin/Scripts/components/kb-file-dialog.html";
    var template = Kooboo.getTemplate(url);
    var mapping = ko.mapping;
    ko.components.register("kb-file-dialog", {
        viewModel: function(params) {
            var self = this;

            this.multiple = ko.observable(!!params.multiple);
            this.fileDialog = ko.observable(false);
            this.folders = ko.observableArray();
            this.crumbPath = ko.observableArray();
            this.files = ko.observableArray();

            this.loading = ko.observable(true);

            this.onAdd = null;

            this.context = ko.observable();

            this.fileData = params.data;
            this.fileData.subscribe(function(data) {
                self.fileDialog(true);
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

            this.onHideFileDialog = function() {
                self.folders([]);
                self.files([]);
                self.crumbPath([]);
                self.context(null);
                self.fileData(null);
                self.fileDialog(false);
                self.selectedFiles([]);
                self.loading(true);
            }

            this.curType = ko.observable("list");

            this.changeType = function(type) {
                self.curType(type);
            }

            this.onChoosingFolder = function(path) {
                self.loading(true);
                Kooboo.File.getList({ path: path }).then(function(res) {
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
                    console.warn("Uninitialize function: FileDialog.onAdd");
                }
                self.onHideFileDialog();
            }

            this.selectedFiles = ko.observableArray();
            this.selectedFiles.subscribe(function(selected) {

            });

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

        mapping.fromJS(file, {}, self);

        self.selected = ko.observable(false);

        self.onSelect = function() {
            self.selected(!self.selected());
            return true;
        }
    }
})();