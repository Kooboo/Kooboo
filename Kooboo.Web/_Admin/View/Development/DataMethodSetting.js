$(function() {
  var folderSelected = false;

  var self;
  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        methodId: Kooboo.getQueryString("id"),
        rules: {
          methodName: [
            {
              required: true,
              message: Kooboo.text.validation.required
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
              pattern: /^\w+$/,
              message: Kooboo.text.validation.nameInvalid
            },
            {
              remote: {
                url: Kooboo.DataSource.isUniqueName(),
                data: function() {
                  return {
                    name: self.model.methodName
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ]
        },
        declareType: "",
        fieldsOfCurrentFolder: [],
        model: {},
        parameterBinding: [],
        supportedComparers: [],
        isNew: JSON.parse(Kooboo.getQueryString("isNew")),
        isFolder: false,
        hasFolder: false,
        textContentsUrl: Kooboo.Route.Get(Kooboo.Route.ContentType.ListPage)
      };
    },
    mounted: function() {
      Kooboo.EventBus.subscribe("folder change", function(id) {
        var getFolder = self.parameterBinding.filter(function(item) {
          if (item.value.controlType === "contentFolder") {
            return true;
          }
        })[0];
        self.model.parameterBinding[getFolder.key].binding = id;
      });
      // get method setting data
      Kooboo.DataSource.get({
        id: self.methodId
      }).then(function(data) {
        var model = data.model;
        self.model = model;
        self.declareType = model.declareType;
        self.model.methodName = model.displayName || self.model.methodName;

        _.forEach(model.parameterBinding, function(value, key) {
          var ob = {};
          ob.key = key;
          ob.value = value;
          self.parameterBinding.push(ob);
        });

        var isFolder = self.parameterBinding.some(function(item) {
          if (item.value.controlType === "contentFolder") {
            return true;
          }
        });
        self.isFolder = isFolder;
        if (isFolder) {
          var getFolder = self.parameterBinding.filter(function(item) {
            if (item.value.controlType === "contentFolder") {
              return true;
            }
          });
          if (getFolder.length > 0) {
            getFolder = getFolder[0];
          }
          // get content folder data
          Kooboo.ContentFolder.getList().then(function(data) {
            if (data.success) {
              var folders = data.model;
              var d = [],
                allowMultiple = getFolder.value.isCollection;
              if (folders && folders.length > 0) {
                self.hasFolder = true;
              } else {
                self.hasFolder = false;
              }
              folders.forEach(function(folder) {
                if (getFolder.value.isCollection) {
                  var ids = JSON.parse(getFolder.value.binding);
                  if (ids.indexOf(folder.id) > -1) {
                    d.push({
                      id: folder.id,
                      text: folder.displayName,
                      state: {
                        selected: true
                      }
                    });
                  } else {
                    d.push({
                      id: folder.id,
                      text: folder.displayName
                    });
                  }
                } else {
                  if (getFolder.value.binding === folder.id) {
                    d.push({
                      id: folder.id,
                      text: folder.displayName,
                      state: {
                        selected: true
                      }
                    });
                  } else {
                    d.push({
                      id: folder.id,
                      text: folder.displayName
                    });
                  }
                }
              });
              setTimeout(function() {
                $("#using_json")
                  .jstree({
                    plugins: ["types", "checkbox"],
                    types: {
                      default: {
                        icon: "fa fa-file icon-state-warning"
                      }
                    },
                    core: {
                      strings: {
                        "Loading ...": Kooboo.text.common.loading + " ..."
                      },
                      data: d,
                      multiple: allowMultiple
                    }
                  })
                  .on("changed.jstree", function(e, data) {
                    //判断是否有选中folder
                    var ContentFolderId;

                    if (!allowMultiple) {
                      ContentFolderId = data.selected[0];
                      if (!!ContentFolderId) {
                        folderSelected = true;
                      } else {
                        folderSelected = false;
                      }

                      //get content folder columns
                      if (folderSelected) {
                        Kooboo.ContentFolder.getColumnsById({
                          id: ContentFolderId
                        }).then(function(res) {
                          var model = res.model;
                          self.fieldsOfCurrentFolder = model;
                        });
                      }
                    } else {
                      ContentFolderId = data.selected;
                      if (ContentFolderId && ContentFolderId.length) {
                        folderSelected = true;
                      } else {
                        folderSelected = false;
                      }
                      ContentFolderId = JSON.stringify(ContentFolderId);
                    }

                    Kooboo.EventBus.publish("folder change", ContentFolderId);
                  })
                  .on("loaded.jstree", function() {
                    $("#using_json")
                      .parent()
                      .readOnly(!self.isNew);
                  });
              });
            }
          });
        }

        $(".wizard-nav-item").click(function(e) {
          //todo use ko
          $(".wizard-nav-item").removeClass("active");
          $(e.target).addClass("active");
          var target = $(e.target).data("step");
          if (target === "configure" && !folderSelected && isFolder)
            return false;
          $(".wizard-body").hide();
          $("div[data-step=" + target + "]").show();
        });
      });
    },
    methods: {
      changeParameterBinding: function(data) {
        var value = data.value;
        if (value === undefined) {
          return;
        }
        if (typeof value === "string" || typeof value === "boolean") {
          this.model.parameterBinding[data.name.toCamelCase()].binding = value;
        } else {
          this.model.parameterBinding[
            data.name.toCamelCase()
          ].binding = JSON.stringify(value);
        }
      },
      dataSourceUrl: function() {
        window.location = Kooboo.Route.Get(Kooboo.Route.DataSource.ListPage);
      },
      submitable: function() {
        if (
          self.isNew &&
          self.$refs.methodForm &&
          !self.$refs.methodForm.validate()
        ) {
          return false;
        } else if (self.isFolder && !folderSelected) {
          window.alert(Kooboo.text.alert.pleaseChooseAFolder);
          return false;
        }
        return true;
      },
      submit: function(m, e) {
        if (!self.submitable()) {
          return;
        }
        if (self.isNew) {
          self.model.id = Kooboo.Guid.Empty;
        }
        self.model.isGlobal = false;
        self.model.isPublic = true;
        var data = self.model;
        for (var k in data.parameterBinding) {
          if (data.parameterBinding[k].controlType === "contentFilter") {
            if (data.parameterBinding[k].binding) {
              var filter = JSON.parse(data.parameterBinding[k].binding);
              var _filter = filter.map(function(o) {
                return {
                  FieldName: o.key || o.FieldName,
                  FieldValue: o.value || o.FieldValue,
                  Comparer: o.comparison || o.Comparer
                };
              });
              data.parameterBinding[k].binding = JSON.stringify(_filter);
            } else {
              data.parameterBinding[k].binding = JSON.stringify([]);
            }
          }
        }
        Kooboo.DataSource.update(data).then(function(res) {
          // console.log(res)
          if (res.success) {
            window.location.href = Kooboo.Route.Get(
              Kooboo.Route.DataSource.ListPage
            );
          }
        });
      },
      getControlName: function(controlType) {
        controlType = controlType.toLowerCase();
        if (
          [
            "normal",
            "textarea",
            "checkbox",
            "dictionary",
            "collection",
            "orderby",
            "contentfilter"
          ].indexOf(controlType) === -1
        ) {
          return;
        }
        switch (controlType) {
          case "normal":
            controlType = "string";
            break;
          case "orderby":
            controlType = "order";
            break;
          case "contentfilter":
            controlType = "filter";
            break;
        }
        return "control-" + controlType;
      }
    }
  });
});
