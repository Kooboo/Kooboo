(function() {
  var self;
  Vue.component("kb-media-dialog", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/kb-media-dialog.html"
    ),
    props: {
      mediaData: Object,
      multiple: Boolean
    },
    data: function() {
      self = this;
      return {
        mediaDialog: false,
        folders: [],
        crumbPath: [],
        files: [],
        loading: true,
        onAdd: null,
        context: {},
        curType: "list",
        selectedFiles: []
      };
    },
    methods: {
      onHideMediaDialog: function() {
        self.folders = [];
        self.files = [];
        self.crumbPath = [];
        self.context = null;
        self.mediaData = null;
        self.mediaDialog = false;
        self.selectedFiles = [];
        self.loading = true;
      },
      changeType: function(type) {
        self.curType = type;
      },
      onChoosingFolder: function(path) {
        self.loading = true;
        Kooboo.Media.getDialogList({ path: path }).then(function(res) {
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
        if (!self.multiple) {
          _.forEach(self.files, function(_f) {
            _f.selected = false;
          });
          self.selectedFiles = [];
        }
        file.selected = !file.selected;
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
        if (self.onAdd && typeof self.onAdd == "function") {
          var data = self.selectedFiles;
          self.onAdd(self.multiple ? data : data[0]);
        } else {
          console.warn("Uninitialize function: MediaDialog.onAdd");
        }
        self.onHideMediaDialog();
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
      uploadMedia: function(data, files) {
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
    beforeDestroy: function() {
      self = null;
    }
  });

  var viewModel = function(params) {
    var self = this;

    this.mediaData.subscribe(function(data) {
      self.mediaDialog(true);
      if (data && data.show) {
        self.loading(false);
        self.crumbPath(data.crumbPath);
        self.folders(data.folders);
        self.context(data.context);
        self.files([]);
        data.files.forEach(function(f) {
          self.files.push(fileModel(f));
        });

        self.onAdd = data.onAdd;
      }
    });

    this.selectedFiles.subscribe(function(selected) {});
  };

  var fileModel = function(file) {
    file.thumbnail = file.thumbnail + "&timestamp=" + +new Date();
    self.selected = false;
    return file;
    // self.onSelect = function() {
    //   self.selected(!self.selected());
    //   return true;
    // };
  };
})();
