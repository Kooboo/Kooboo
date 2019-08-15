(function() {
  var template = Kooboo.getTemplate(
    "/_Admin/Scripts/textContent/folderEditor.html"
  );

  ko.components.register("folder-editor", {
    viewModel: function(params) {
      var contentTypes = [];

      this.hasInit = false;

      var self = this;
      self.id = ko.observable();
      self.isNew = ko.pureComputed(function() {
        return !this.id();
      }, self);
      self.name = ko.validateField({
        required: Kooboo.text.validation.required,
        remote: {
          message: Kooboo.text.validation.taken,
          url: Kooboo.ContentFolder.isUniqueName(),
          type: "get",
          data: {
            name: function() {
              return self.name();
            }
          }
        },
        stringlength: {
          min: 1,
          max: 64,
          message:
            Kooboo.text.validation.minLength +
            1 +
            ", " +
            Kooboo.text.validation.maxLength +
            64
        },
        regex: {
          pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
          message: Kooboo.text.validation.contentTypeNameRegex
        }
      });

      self.allFolders = params.allFolders;
      self.isTree = ko.observable(false);
      self.displayName = ko.observable();
      self.contentTypeId = ko.validateField({
        required: Kooboo.text.validation.required
      });
      self.hasContentType = ko.pureComputed(function() {
        var cid = self.contentTypeId(),
          id = self.id();
        return (
          cid && cid !== Kooboo.Guid.Empty && id && id !== Kooboo.Guid.Empty
        );
      });
      self.contentTypeName = ko.pureComputed(function() {
        return (_.find(contentTypes, { id: self.contentTypeId() }) ||
          {})["name"];
      }, self);
      self.contentTypes = ko.observableArray(contentTypes);
      self.categoryFolders = ko.observableArray();
      self.embeddedFolders = ko.observableArray();
      self.addCategoryFolders = function(e) {
        self.categoryFolders.push(new CategoryFolder());
      };
      self.removeCategoryFolders = function(f) {
        self.categoryFolders.remove(f);
        self.ableToAddRelationFolder(true);
      };
      self.addEmbeddedFolders = function(e) {
        self.embeddedFolders.push(new EmbeddedFolder());
      };
      self.removeEmbeddedFolders = function(f) {
        self.embeddedFolders.remove(f);
        self.ableToAddRelationFolder(true);
      };

      self.newFolderModalShow = ko.observable(false);

      this.showError = ko.observable(false);

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

      self.submit = function(form) {
        if (self.isNew()) {
          if (!self.name.isValid() || !self.contentTypeId.isValid()) {
            if (
              !$("a[href=#tab_basic]")
                .parent()
                .hasClass("active")
            ) {
              $("a[href=#tab_basic]").tab("show");
              setTimeout(function() {
                self.showError(true);
              }, 300);
            } else {
              self.showError(true);
            }

            return false;
          }
        }

        if (self.isCatogoryAliasError() || self.isEmbeddedAliasError()) {
          if (
            !$("a[href=#tab_relation]")
              .parent()
              .hasClass("active")
          ) {
            $("a[href=#tab_relation]").tab("show");
            setTimeout(function() {
              self.showRelationFolderError();
            }, 300);
          } else {
            self.showRelationFolderError();
          }

          return false;
        }

        var data = ko.mapping.toJS(form);
        var postData = {};

        if (!data.id) {
          data.id = Kooboo.Guid.Empty;
        }
        postData.name = data["name"];
        postData.id = data["id"];
        postData.displayName = data["displayName"];
        postData.contentTypeId = data["contentTypeId"];
        postData.embedded = data["embeddedFolders"];
        postData.category = data["categoryFolders"];

        Kooboo.ContentFolder.post(postData).then(function(res) {
          if (res.success) {
            init();
            self.reset();
            Kooboo.EventBus.publish("kb/textcontents/new/folder");
            getFolderList();
          }
        });
      };

      self.availableFolders = function(id) {
        var removeList = _.map(
          _.concat(self.categoryFolders(), self.embeddedFolders()),
          "folderId"
        );
        var availableFolders = _.remove(
          _.cloneDeep(self.allFolders()),
          function(folder) {
            return folder.id !== self.id();
          }
        );
        availableFolders = _.filter(availableFolders, function(folder) {
          return folder.id === id || removeList.indexOf(folder.id) == -1;
        });
        if (availableFolders.length == 1) {
          self.ableToAddRelationFolder(false);
        }
        return availableFolders;
      };

      self.ableToAddRelationFolder = ko.observable(true);

      self.reset = function() {
        self.name("");
        _.forEach(
          _.concat(self.categoryFolders(), self.embeddedFolders()),
          function(folder) {
            folder.showError(false);
          }
        );
        self.categoryFolders([]);
        self.embeddedFolders([]);
        self.newFolderModalShow(false);
        self.showError(false);
        $("a[href=#tab_basic]").tab("show");
      };

      self.init = function(id) {
        if (id) {
          var model = _.find(self.allFolders(), { id: id }) || {},
            folderIds = [];
          self.id(id);
          self.name(model["name"]);
          self.isTree(model["isTree"]);
          self.displayName(model["displayName"]);
          self.contentTypeId(model["contentTypeId"]);
          self.categoryFolders([]);
          self.embeddedFolders([]);
          model["category"].forEach(function(o) {
            self.categoryFolders.push(new CategoryFolder(o));
            folderIds.push(o.folderId);
          });
          model["embedded"].forEach(function(o) {
            self.embeddedFolders.push(new EmbeddedFolder(o));
            folderIds.push(o.folderId);
          });
          self.newFolderModalShow(true);

          var availableFolders = _.remove(
            _.cloneDeep(self.allFolders()),
            function(folder) {
              return folderIds.indexOf(folder.id) == -1;
            }
          );
          self.ableToAddRelationFolder(availableFolders.length > 1);
        } else {
          if (!self.hasInit) {
            init().done(function() {
              self.hasInit = true;
              reset();
            });
          } else {
            reset();
          }

          function reset() {
            self.id("");
            self.name("");
            self.isTree("");
            self.displayName("");
            self.categoryFolders([]);
            self.embeddedFolders([]);
            self.newFolderModalShow(true);
            self.contentTypeId("");
            self.ableToAddRelationFolder(self.allFolders().length);
          }
        }
      };

      Kooboo.EventBus.subscribe("ko/textContent/folderSetting", function(
        selectedFolderId
      ) {
        if (!!selectedFolderId) {
          self.init(selectedFolderId);
        }
      });

      Kooboo.EventBus.subscribe("ko/textContent/newFolder", function(
        selectedFolder
      ) {
        self.init(null);
      });

      Kooboo.EventBus.subscribe("kb/table/delete/finish", function() {
        getFolderList();
      });

      self.getAliasNames = function() {
        var names = [];
        _.forEach(
          _.concat(self.categoryFolders(), self.embeddedFolders()),
          function(folder) {
            folder.alias() && names.push(folder.alias());
          }
        );
        return names;
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

      function init() {
        return $.when(getFolderList(), getTypeList());
      }

      function getFolderList() {
        Kooboo.ContentFolder.getList().then(function(res) {
          self.allFolders(res.model);
        });
      }

      function getTypeList() {
        Kooboo.ContentType.getList().then(function(res) {
          contentTypes = _.concat(
            [
              {
                id: "",
                name: Kooboo.text.component.folderEditor.chooseItemBelow
              }
            ],
            res.model
          );
          self.contentTypes(contentTypes);
        });
      }

      getTypeList();
    },
    template: template
  });
})();
