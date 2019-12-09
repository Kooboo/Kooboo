$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.mediaLibrary
          }
        ],
        curType: "list",
        uploadSetting: {
          allowMultiple: true,
          acceptTypes: [
            "image/bmp",
            "image/x-windows-bmp",
            "image/png",
            "image/jpeg",
            "image/gif",
            "image/webp",
            "image/svg+xml",
            "image/x-icon"
          ],
          acceptSuffix: [
            "bmp",
            "png",
            "jpg",
            "jpeg",
            "gif",
            "svg",
            "ico",
            "webp"
          ],
          callback: self.uploadImage
        },
        pager: {},
        currentPath: undefined,
        imgTypes: [
          {
            displayName: Kooboo.text.site.images.all,
            value: "all"
          },
          {
            displayName: Kooboo.text.site.images.page,
            value: "page"
          },
          {
            displayName: Kooboo.text.site.images.style,
            value: "style"
          },
          {
            displayName: Kooboo.text.site.images.view,
            value: "view"
          },
          {
            displayName: Kooboo.text.site.images.layout,
            value: "layout"
          },
          {
            displayName: Kooboo.text.common.HTMLblock,
            value: "HTMLBlock"
          },
          {
            displayName: Kooboo.text.site.images.content,
            value: "TextContent"
          }
        ],
        curImgType: "all",
        crumbPath: [],
        folders: [],
        _folders: [],
        files: [],
        _files: [],
        currentSort: "url",
        isAsc: false,
        selectedFiles: [],
        newFolderModal: false,
        folderForm: {
          name: ""
        },
        folderRules: {
          name: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              validate: function(value) {
                return !_.some(self.folders, { name: value });
              },
              message: Kooboo.text.validation.taken
            },
            {
              min: 1,
              max: 64,
              message:
                Kooboo.text.validation.minLength +
                1 +
                ", " +
                Kooboo.text.validation.maxLength +
                64
            },
            {
              pattern: /^([a-zA-Z\w\-\.])*$/,
              message: Kooboo.text.validation.folderNameInvalid
            }
          ]
        }
      };
    },
    created: function() {
      self.debouceChangeType = _.debounce(
        function(type) {
          self.curType = type;
        },
        300,
        {
          leading: true
        }
      );
    },
    methods: {
      changeType: function(type) {
        self.debouceChangeType(type);
      },
      changeImgType: function(type) {
        if (type !== self.curImgType) {
          if (type !== "all") {
            self.currentPath = undefined;
            Kooboo.Media.getPagedListBy({
              by: type
            }).then(function(res) {
              if (res.success) {
                var _folders = [];
                _.forEach(res.model.folders, function(folder) {
                  _folders.push(new folderModel(folder));
                });
                self.folders = _folders;
                self._folders = _folders;

                var _files = [];
                _.forEach(res.model.files.list, function(file) {
                  _files.push(new fileModel(file));
                });
                self.files = _files;
                self._files = _files;

                self.pager = res.model.files;

                self.crumbPath = res.model.crumbPath;
                self.curImgType = type;

                self.reorder();
                location.hash = "";
              }
            });
          } else {
            self.onChoosingFolder("/");
            self.curImgType = type;
          }
        }
      },
      resetCurImgType: function() {
        this.changeImgType("all");
      },
      currentOrderCSS: function(type) {
        if (self.currentSort == type) {
          return this.isAsc ? "asc" : "desc";
        } else {
          return "";
        }
      },
      changeSort: function(sort) {
        self.isAsc = self.currentSort == sort ? !self.isAsc : false;
        self.currentSort = sort;
        self.reorder();
      },
      reorder: function() {
        var _folders = _.cloneDeep(self.folders),
          _files = _.cloneDeep(self.files);

        switch (self.currentSort) {
          case "url":
            _folders = _.sortBy(self.folders, [
              function(f) {
                return f.name;
              }
            ]);
            _files = _.sortBy(self.files, [
              function(f) {
                return f.name;
              }
            ]);
            break;
          case "size":
            _files = _.sortBy(self.files, [
              function(f) {
                return f.size;
              }
            ]);
            break;
          case "date":
            _folders = _.sortBy(self.folders, [
              function(f) {
                return f.lastModified;
              }
            ]);
            _files = _.sortBy(self.files, [
              function(f) {
                return f.lastModified;
              }
            ]);
            break;
        }

        self.folders = _folders;
        self.files = _files;

        if (!self.isAsc) {
          self.folders.reverse();
          self.files.reverse();
        }
      },
      selectDoc: function(doc) {
        doc.selected = !doc.selected;
        if (doc.selected) {
          self.selectedFiles.push(doc);
        } else {
          self.selectedFiles = _.without(self.selectedFiles, doc);
        }
        return true;
      },
      editImage: function(m) {
        var id = "";

        if (self.curType == "grid") {
          id = self.selectedFiles[0].id;
        } else {
          id = m.id;
        }

        var crumbPath = _.cloneDeep(self.crumbPath);

        location.href =
          Kooboo.Route.Get(Kooboo.Route.Image.Edit, {
            Id: id
          }) +
          "#" +
          crumbPath.reverse()[0].fullPath;
      },
      onChoosingFolder: function(path, page) {
        if (
          !self.currentPath ||
          self.currentPath !== path ||
          (page && self.pager.pageNr !== page)
        ) {
          Kooboo.Media.getPagedList({
            path: path,
            pageNr: page ? (typeof page == "number" ? page : 1) : 1
          }).then(function(res) {
            if (res.success) {
              self.selectAll = false;
              self.selectedFiles = [];
              self.crumbPath = res.model.crumbPath;

              var _folders = [];
              _.forEach(res.model.folders, function(folder) {
                _folders.push(new folderModel(folder));
              });
              self.folders = _folders;
              self._folders = _folders;

              var _files = [];
              _.forEach(res.model.files.list, function(file) {
                _files.push(new fileModel(file));
              });
              self.files = _files;
              self._files = _files;

              self.pager = res.model.files;

              self.reorder();

              self.currentPath = path;
              if (path !== "/") {
                location.hash = path;
              } else {
                location.hash = "";
              }
            }
          });
        }
      },
      getRelation: function(file, type) {
        Kooboo.EventBus.publish("kb/relation/modal/show", {
          id: file.id,
          by: type,
          type: "Image"
        });
      },
      localDate: function(date) {
        var d = new Date(date);
        return d.toDefaultLangString();
      },
      onDelete: function() {
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var folders = [],
            files = [];

          _.forEach(self.selectedFiles, function(selected) {
            if (selected.type == "folder") {
              folders.push(selected.fullPath);
            } else if (selected.type == "file") {
              files.push(selected.id);
            }
          });

          if (folders.length) {
            Kooboo.Media.deleteFolders(JSON.stringify(folders)).then(function(
              res
            ) {
              if (res.success) {
                _.forEach(folders, function(fullPath) {
                  var _find = _.find(self.folders, function(folder) {
                    return folder.fullPath == fullPath;
                  });
                  if (_find) {
                    self.folders = _.without(self.folders, _find);
                  }
                });
                window.info.done(Kooboo.text.info.delete.success);
              } else {
                window.info.done(Kooboo.text.info.delete.fail);
              }
            });
          }
          if (files.length) {
            Kooboo.Media.deleteImages(JSON.stringify(files)).then(function(
              res
            ) {
              if (res.success) {
                _.forEach(files, function(id) {
                  var _find = _.find(self.files, function(files) {
                    return files.id == id;
                  });
                  if (_find) {
                    self.files = _.without(self.files, _find);
                  }
                });
                window.info.done(Kooboo.text.info.delete.success);
              } else {
                window.info.done(Kooboo.text.info.delete.fail);
              }
            });
          }
          self.selectedFiles = [];
        }
      },
      uploadImage: function(data, files) {
        function upload() {
          var folders = _.cloneDeep(self.crumbPath);
          data.append("folder", folders.reverse()[0].fullPath);
          Kooboo.Upload.Images(data).then(function(res) {
            if (res.success) {
              self.currentPath = "";
              self.onChoosingFolder(folders[0].fullPath);
            }
          });
        }

        if (!Kooboo.isFileNameExist(files, self.files)) {
          upload();
        } else {
          if (confirm(Kooboo.text.confirm.overrideFile)) {
            upload();
          }
        }
      },
      onCreateFolder: function() {
        self.folderForm.name = "";
        self.newFolderModal = true;
      },
      onNewFolderModalReset: function() {
        self.newFolderModal = false;
        self.folderForm.name = "";
        self.$refs.folderForm.clearValid();
      },
      onNewFolderModalSubmit: function() {
        var isValid = self.$refs.folderForm.validate();
        if (isValid) {
          Kooboo.Media.createFolder({
            path: self.crumbPath[self.crumbPath.length - 1].fullPath,
            name: self.folderForm.name
          }).then(function(res) {
            if (res.success) {
              self.folders.push(new folderModel(res.model));
              self.onNewFolderModalReset();
            }
          });
        }
      },
      changePage: function(page) {
        if (self.curImgType == "all") {
          self.onChoosingFolder(self.currentPath, page);
        } else {
          Kooboo.Media.getPagedListBy({
            by: self.curImgType, // type
            pageNr: page
          }).then(function(res) {
            if (res.success) {
              var _folders = [];
              _.forEach(res.model.folders, function(folder) {
                _folders.push(new folderModel(folder));
              });
              self.folders = _folders;
              self._folders = _folders;

              var _files = [];
              _.forEach(res.model.files.list, function(file) {
                _files.push(new fileModel(file));
              });
              self.files = _files;
              self._files = _files;

              self.pager = res.model.files;

              self.crumbPath = res.model.crumbPath;
              // self.curImgType = type;

              self.reorder();
              location.hash = "";
            }
          });
        }
      }
    },
    computed: {
      selectAll: {
        get: function() {
          var allLength = self.folders.length + self.files.length;
          if (allLength === 0) {
            return false;
          }
          return self.selectedFiles.length == allLength;
        },
        set: function(checked) {
          self.selectedFiles = [];
          _.forEach(self.folders, function(folder) {
            folder.selected = checked;
            checked && self.selectedFiles.push(folder);
          });
          _.forEach(self.files, function(file) {
            file.selected = checked;
            checked && self.selectedFiles.push(file);
          });
        }
      },
      showDeleteBtn: function() {
        return this.selectedFiles.length;
      }
    },
    mounted: function() {
      self.onChoosingFolder(location.hash ? location.hash.split("#")[1] : "");
      Kooboo.EventBus.subscribe("window/popstate", function() {
        $(".modal").modal("hide");
        if (location.hash) {
          var path = location.hash.split("#")[1];
          self.onChoosingFolder(path);
        } else {
          self.curImgType == "all" && self.onChoosingFolder("/");
        }
      });
    },
    beforeDestory: function() {
      self = null;
    }
  });

  var fileModel = function(file) {
    file.thumbnail = file.thumbnail + "&timestamp=" + +new Date();
    file.type = "file";
    file.selected = false;
    file.editUrl = Kooboo.Route.Get(Kooboo.Route.Image.Edit, { Id: file.id });
    return file;
  };

  var folderModel = function(folder) {
    folder.type = "folder";
    folder.selected = false;
    return folder;
  };
});
