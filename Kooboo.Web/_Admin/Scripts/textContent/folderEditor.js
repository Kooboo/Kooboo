(function() {
  Kooboo.loadJS(["/_Admin/Scripts/components/kbForm.js"]);
  Vue.component("kb-folder-editor", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/textContent/folderEditor.html"
    ),
    props: {
      folders: Array,
      visible: Boolean,
      id: String
    },
    data: function() {
      var self = this;
      return {
        currentTab: "basic",
        showError: {
          basic: true,
          relation: true
        },
        contentTypes: [],
        ableToAddRelationFolder: true,
        basicForm: {
          name: "",
          displayName: "",
          contentTypeId: "",
          isTree: ""
        },
        basicFormRules: {
          name: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              remote: {
                url: Kooboo.ContentFolder.isUniqueName(),
                data: function() {
                  return {
                    name: self.basicForm.name
                  };
                }
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
              pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
              message: Kooboo.text.validation.contentTypeNameRegex
            }
          ],
          contentTypeId: [
            {
              required: true,
              message: Kooboo.text.validation.required
            }
          ]
        },
        relationForm: {
          categoryFolders: [],
          embeddedFolders: []
        },
        relationFormRules: {
          "categoryFolders[]": self.folderRules(),
          "embeddedFolders[]": self.folderRules()
        }
      };
    },
    mounted: function() {
      this.getTypeList();
    },
    computed: {
      isNew: function() {
        return !this.id;
      },
      hasContentType: function() {
        var self = this;
        var cid = self.basicForm.contentTypeId,
          id = self.id;
        return (
          cid && cid !== Kooboo.Guid.Empty && id && id !== Kooboo.Guid.Empty
        );
      },
      contentTypeName: function() {
        var self = this;
        contentTypeName = "";
        if (this.hasContentType) {
          var contentType = _.find(self.contentTypes, {
            id: self.basicForm.contentTypeId
          });
          if (contentType) {
            contentTypeName = contentType.name;
          }
        }
        return contentTypeName;
      }
    },
    methods: {
      folderRules: function() {
        var self = this;
        return {
          alias: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
              message: Kooboo.text.validation.objectNameRegex
            },
            {
              validate: function(value) {
                return (
                  _.filter(
                    _.concat(
                      self.relationForm.categoryFolders,
                      self.relationForm.embeddedFolders
                    ),
                    { alias: value }
                  ).length < 2
                );
              },
              message: Kooboo.text.validation.taken
            }
          ]
        };
      },
      changeTab: function(tab) {
        var self = this;
        if (tab !== self.currentTab) {
          self.currentTab = tab;
          $("a[href=#tab_" + tab + "]").tab("show");
          for (var key in self.showError) {
            self.showError[key] = false;
          }
          setTimeout(function() {
            self.showError[tab] = true;
          }, 300);
        }
      },
      addCategoryFolders: function(e) {
        var self = this;
        var newFolder = new Folder();
        self.relationForm.categoryFolders.push(newFolder);
        newFolder.folderId = self.availableFolders()[0].id;
      },
      removeCategoryFolders: function(f) {
        var self = this;
        self.relationForm.categoryFolders = _.without(
          self.relationForm.categoryFolders,
          f
        );
        self.ableToAddRelationFolder = true;
      },
      addEmbeddedFolders: function(e) {
        var self = this;
        var newFolder = new Folder();
        self.relationForm.embeddedFolders.push(newFolder);
        newFolder.folderId = self.availableFolders()[0].id;
      },
      removeEmbeddedFolders: function(f) {
        var self = this;
        self.relationForm.embeddedFolders = _.without(
          self.relationForm.embeddedFolders,
          f
        );
        self.ableToAddRelationFolder = true;
      },
      submit: function(form) {
        var self = this;
        if (self.isNew) {
          var isBasicValid = self.$refs.basicForm.validate();
          if (!isBasicValid) {
            if (self.currentTab != "basic") {
              self.changeTab("basic");
            }
            return false;
          }
        }
        var isRelationValid = self.$refs.relationForm.validate();
        if (!isRelationValid) {
          if (self.currentTab != "relation") {
            self.changeTab("relation");
          }
          return false;
        }
        var postData = {};
        if (!self.id) {
          postData.id = Kooboo.Guid.Empty;
        }
        postData.name = self.basicForm.name;
        postData.displayName = self.basicForm.displayName;
        postData.contentTypeId = self.basicForm.contentTypeId;
        postData.embedded = self.relationForm.embeddedFolders;
        postData.category = self.relationForm.categoryFolders;

        Kooboo.ContentFolder.post(postData).then(function(res) {
          if (res.success) {
            self.reset();
            self.$emit("after-edit");
          }
        });
      },
      availableFolders: function(id) {
        var self = this;
        var removeList = _.map(
          _.concat(
            self.relationForm.categoryFolders,
            self.relationForm.embeddedFolders
          ),
          "folderId"
        );
        var availableFolders = _.remove(_.cloneDeep(self.folders), function(
          folder
        ) {
          return folder.id !== self.id;
        });
        availableFolders = _.filter(availableFolders, function(folder) {
          return folder.id === id || removeList.indexOf(folder.id) == -1;
        });
        if (availableFolders.length == 1) {
          self.ableToAddRelationFolder = false;
        }
        return availableFolders;
      },
      reset: function() {
        var self = this;
        self.$refs.basicForm.clearValid();
        self.$refs.relationForm.clearValid();

        self.basicForm.name = "";
        self.basicForm.contentTypeId = "";
        self.relationForm.categoryFolders = [];
        self.relationForm.embeddedFolders = [];
        self.$emit("update:visible", false);
        self.changeTab("basic");
      },
      getTypeList: function() {
        var self = this;
        Kooboo.ContentType.getList().then(function(res) {
          self.contentTypes = _.concat(
            [
              {
                id: "",
                name: Kooboo.text.component.folderEditor.chooseItemBelow
              }
            ],
            res.model
          );
        });
      },
      init: function() {
        var self = this;
        if (!self.isNew) {
          var model = _.find(self.folders, { id: self.id }) || {},
            folderIds = [];
          self.basicForm.name = model.name;
          self.basicForm.isTree = model.isTree;
          self.basicForm.displayName = model.displayName;
          self.basicForm.contentTypeId = model.contentTypeId;
          self.relationForm.categoryFolders = [];
          model.category.forEach(function(o) {
            self.relationForm.categoryFolders.push(new Folder(o));
            folderIds.push(o.folderId);
          });
          self.relationForm.embeddedFolders = [];
          model.embedded.forEach(function(o) {
            self.relationForm.embeddedFolders.push(new Folder(o));
            folderIds.push(o.folderId);
          });
          var availableFolders = _.remove(_.cloneDeep(self.folders), function(
            folder
          ) {
            return folderIds.indexOf(folder.id) == -1;
          });
          self.ableToAddRelationFolder = availableFolders.length > 1;
        } else {
          self.basicForm.name = "";
          self.basicForm.isTree = "";
          self.basicForm.displayName = "";
          self.basicForm.contentTypeId = "";
          self.relationForm.categoryFolders = [];
          self.relationForm.embeddedFolders = [];
          self.ableToAddRelationFolder = self.folders.length;
        }
      }
    },
    watch: {
      visible: function(val) {
        if (val) {
          this.init();
        }
      }
    }
  });

  function Folder(opt) {
    this.folderId = (opt && opt.folderId) || "";
    this.multiple = (opt && opt.multiple) || false;
    this.alias = (opt && opt.alias) || "";
    this.enableMultiple = !this.multiple;
  }
})();
