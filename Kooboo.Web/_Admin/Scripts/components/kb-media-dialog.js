(function() {
  Vue.component("kb-media-dialog", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/kb-media-dialog.html"
    ),
    props: {
      data: Object,
      multiple: Boolean
    },
    data: function() {
      var self = this;
      return {
        mediaDialog: false,
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
          callback: self.uploadMedia
        }
      };
    },
    methods: {
      onHideMediaDialog: function() {
        var self = this;
        self.folders = [];
        self.files = [];
        self.crumbPath = [];
        self.context = null;
        self.mediaDialog = false;
        self.selectedFiles = [];
        self.loading = true;
        self.$emit("update:data", null);
      },
      changeType: function(type) {
        var self = this;
        self.curType = type;
      },
      onChoosingFolder: function(path) {
        var self = this;
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
        var self = this;
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
        var self = this;
        var data = self.selectedFiles;
        var result = self.multiple ? data : data[0];
        // console.log(result)
        if (self.onAdd && typeof self.onAdd == "function") {
          self.onAdd(result);
        }
        self.$emit("on-add", result);
        self.onHideMediaDialog();
      },
      upload: function(data) {
        var self = this;
        var folders = _.cloneDeep(self.crumbPath);
        data.append("folder", folders.reverse()[0].fullPath);
        Kooboo.Upload.Images(data).then(function(res) {
          if (res.success) {
            self.onChoosingFolder(folders[0].fullPath);
          }
        });
      },
      uploadMedia: function(data, files) {
        var self = this;
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
        var self = this;
        if (data && data.show) {
          self.mediaDialog = true;
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
    }
  });

  var fileModel = function(file) {
    file.thumbnail = file.thumbnail + "&timestamp=" + +new Date();
    file.selected = false;
    return file;
  };
})();
