(function() {
  Kooboo.loadJS(["/_Admin/Scripts/vue-components/kbForm.js"]);
  var self;
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
      self = this;
      return {
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
        relationFormRules: {}
      };
    },
    mounted: function() {
      self.getTypeList();
    },
    computed: {
      isNew: function() {
        return !self.id;
      },
      hasContentType: function() {
        var cid = self.basicForm.contentTypeId,
          id = self.id;
        return (
          cid && cid !== Kooboo.Guid.Empty && id && id !== Kooboo.Guid.Empty
        );
      },
      contentTypeName: function() {
        return (_.find(self.contentTypes, { id: self.basicForm.contentTypeId }) || {})["name"];
      }
    },
    methods: {
      addCategoryFolders: function(e) {
        var newFolder = new Folder();
        self.relationForm.categoryFolders.push(newFolder);
        newFolder.folderId = self.availableFolders()[0].id;
      },
      removeCategoryFolders: function(f) {
        self.relationForm.categoryFolders = _.without(
          self.relationForm.categoryFolders,
          f
        );
        self.ableToAddRelationFolder = true;
      },
      addEmbeddedFolders: function(e) {
        var newFolder = new Folder();
        self.relationForm.embeddedFolders.push(newFolder);
        newFolder.folderId = self.availableFolders()[0].id;
      },
      removeEmbeddedFolders: function(f) {
        self.relationForm.embeddedFolders = _.without(
          self.relationForm.embeddedFolders,
          f
        );
        self.ableToAddRelationFolder = true;
      },
      submit: function(form) {
        if (self.isNew) {
          var isBasicValid = self.$refs.basicForm.validate();
          if (!isBasicValid) {
            if (
              !$("a[href=#tab_basic]")
                .parent()
                .hasClass("active")
            ) {
              $("a[href=#tab_basic]").tab("show");
              $('.error-container').hide();
              setTimeout(function() {
                $('.error-container').show();
                $('.has-error[data-container]').tooltip('show');
              }, 300);
            }
            return false;
          }
        }

        // if (self.isCatogoryAliasError() || self.isEmbeddedAliasError()) {
        //   if (
        //     !$("a[href=#tab_relation]")
        //       .parent()
        //       .hasClass("active")
        //   ) {
        //     $("a[href=#tab_relation]").tab("show");
        //     setTimeout(function() {
        //       self.showRelationFolderError();
        //     }, 300);
        //   } else {
        //     self.showRelationFolderError();
        //   }

        //   return false;
        // }

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
        self.$refs.basicForm.clearValid();
        self.basicForm.name = "";
        self.relationForm.categoryFolders = [];
        self.relationForm.embeddedFolders = [];
        self.$emit("update:visible", false);
        $("a[href=#tab_basic]").tab("show");
      },
      getTypeList: function() {
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
      },
      getAliasNames: function() {
        var names = [];
        _.forEach(
          _.concat(self.relationForm.categoryFolders, self.relationForm.embeddedFolders),
          function(folder) {
            folder.alias && names.push(folder.alias);
          }
        );
        return names;
      }
    },
    watch: {
      visible: function(val) {
        if (val) {
          self.init();
        }
      }
    },
    beforeDestroy: function() {
      self = null;
    }
  });

  function Folder(opt) {
    this.folderId = (opt && opt.folderId) || "";
    this.multiple = (opt && opt.multiple) || false;
    this.alias = (opt && opt.alias) || "";
    this.enableMultiple = !this.multiple;
  }

  var viewModel = function(params) {
    this.isValid = function() {
      if (self.isNew()) {
        return (
          self.name.isValid() &&
          self.contentTypeId.isValid() &&
          !self.isCatogoryAliasError() &&
          !self.isEmbeddedAliasError()
        );
      } else {
        return (
          self.contentTypeId.isValid() &&
          !self.isCatogoryAliasError() &&
          !self.isEmbeddedAliasError()
        );
      }
    };

    this.isCatogoryAliasError = function() {
      var hasCategoryAliasError = self.categoryFolders().some(function(o) {
        return !o.alias.isValid();
      });
      return hasCategoryAliasError;
    };

    this.isEmbeddedAliasError = function() {
      var hasEmbeddedAliasError = self.embeddedFolders().some(function(o) {
        return !o.alias.isValid();
      });
      return hasEmbeddedAliasError;
    };

    this.showRelationFolderError = function() {
      _.forEach(
        _.concat(self.categoryFolders(), self.embeddedFolders()),
        function(folder) {
          folder.showError(true);
        }
      );
    };

    function CategoryFolder(opt) {
      var _this = this;
      this.folderId = (opt && opt.folderId) || "";
      this.multiple = (opt && opt.multiple) || false;
      this.alias = ko.validateField(opt && opt.alias, {
        required: Kooboo.text.validation.required,
        regex: {
          pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
          message: Kooboo.text.validation.objectNameRegex
        },
        localUnique: {
          compare: function() {
            return self.getAliasNames();
          },
          message: Kooboo.text.validation.taken
        }
      });
      this.enableMultiple = ko.observable(!_this.multiple);
      this.alias.subscribe(function() {
        _.forEach(
          _.concat(self.categoryFolders(), self.embeddedFolders()),
          function(folder) {
            folder.folderId !== _this.folderId &&
              folder.showError() &&
              !folder.alias.isValid() &&
              folder.alias.valueHasMutated();
          }
        );
      });
      this.showError = ko.observable(false);
    }

    function EmbeddedFolder(opt) {
      var _this = this;
      this.folderId = (opt && opt.folderId) || "";
      this.alias = ko.validateField(opt && opt.alias, {
        required: Kooboo.text.validation.required,
        regex: {
          pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
          message: Kooboo.text.validation.objectNameRegex
        },
        localUnique: {
          compare: function() {
            return self.getAliasNames();
          },
          message: Kooboo.text.validation.taken
        }
      });
      this.alias.subscribe(function() {
        _.forEach(
          _.concat(self.categoryFolders(), self.embeddedFolders()),
          function(folder) {
            folder.folderId !== _this.folderId &&
              folder.showError() &&
              !folder.alias.isValid() &&
              folder.alias.valueHasMutated();
          }
        );
      });
      this.showError = ko.observable(false);
    }
  };
})();
