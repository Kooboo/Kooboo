$(function() {
  var folderSelected = false,
    productTypeSelected = false;
  var siteId = Kooboo.getQueryString("SiteId");
  var self;
  new Vue({
    el: "#basic-info",
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
        isProductType: false,
        isPublic: false,
        hasFolder: false,
        hasProductType: false,
        textContentsUrl: Kooboo.Route.Get(Kooboo.Route.ContentType.ListPage),
        folderListLoading: true
      };
    },
    mounted: function() {
      Kooboo.EventBus.subscribe("folder change", function(id) {
        var getFolder = self.parameterBinding.filter(function(item) {
            if (item.value.controlType === "contentFolder") {
              return true;
            }
          }),
          getProductType = self.parameterBinding.filter(function(item) {
            if (item.value.controlType.toLowerCase() == "producttype") {
              return true;
            }
          });

        if (getFolder && getFolder.length) {
          self.model.parameterBinding[getFolder[0].key].binding = id;
        }

        if (getProductType && getProductType.length) {
          self.model.parameterBinding[getProductType[0].key].binding = id;
        }
      });
      // get method setting data
      Kooboo.DataSource.get({
        id: self.methodId
      }).then(function(data) {
        var model = data.model;
        self.model = model;
        self.isPublic = model.isPublic;
        self.declareType = model.declareType;

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
          }),
          isProductType = self.parameterBinding.some(function(item) {
            if (item.value.controlType.toLowerCase() == "producttype") {
              return true;
            }
          });
        self.isFolder = isFolder;
        self.isProductType = isProductType;

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
              self.folderListLoading = false;
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
                if (self.hasFolder) {
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
                      self.setHeight();
                    });
                } else {
                  self.setHeight();
                }
              });
            }
          });
        } else if (isProductType) {
          // 如果是商品的话，在这里请求productType 的列表，并显示。
          var getProductType = self.parameterBinding.filter(function(item) {
            if (item.value.controlType.toLowerCase() == "producttype") {
              return true;
            }
          });

          if (getProductType.length) {
            getProductType = getProductType[0];
          }

          Kooboo.ProductType.getList().then(function(res) {
            if (res.success) {
              self.folderListLoading = false;
              var types = res.model;
              var d = [],
                allowMultiple = getProductType.value.isCollection;
              if (types && types.length > 0) {
                self.hasProductType = true;
              } else {
                self.hasProductType = false;
              }
              types.forEach(function(type) {
                if (getProductType.value.isCollection) {
                  var ids = JSON.parse(getProductType.value.binding);
                  if (ids.indexOf(type.id) > -1) {
                    d.push({
                      id: type.id,
                      text: type.displayName,
                      state: {
                        selected: true
                      }
                    });
                  } else {
                    d.push({
                      id: type.id,
                      text: type.displayName
                    });
                  }
                } else {
                  if (getProductType.value.binding === type.id) {
                    d.push({
                      id: type.id,
                      text: type.displayName,
                      state: {
                        selected: true
                      }
                    });
                  } else {
                    d.push({
                      id: type.id,
                      text: type.name
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
                    var ProductTypeId;
                    if (!allowMultiple) {
                      ProductTypeId = data.selected[0];
                      if (!!ProductTypeId) {
                        productTypeSelected = true;
                      } else {
                        productTypeSelected = false;
                      }

                      //get content folder columns
                      if (productTypeSelected) {
                        Kooboo.ProductType.getColumnsById({
                          id: ProductTypeId
                        }).then(function(res) {
                          if (res.success) {
                            var model = res.model;
                            self.fieldsOfCurrentFolder = model;
                          }
                        });
                      }
                    } else {
                      ProductTypeId = data.selected;
                      if (ProductTypeId && ProductTypeId.length) {
                        productTypeSelected = true;
                      } else {
                        productTypeSelected = false;
                      }
                      ProductTypeId = JSON.stringify(ProductTypeId);
                    }

                    Kooboo.EventBus.publish("folder change", ProductTypeId);
                  })
                  .on("loaded.jstree", function() {
                    $("#using_json")
                      .parent()
                      .readOnly(!self.isNew);
                    self.setHeight();
                  });
              });
            }
          });
        } else {
          setTimeout(function() {
            self.setHeight();
          }, 200);
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
          self.setHeight();
        });
      });

      window.dataSourceMethodSettingsContext = {
        submit: function() {
          if (self.submitable()) {
            self.submit().then(function(res) {
              res.success &&
                parent.dataSourceMethodSettingsContext.onSubmit(res.model);
            });
          }
        }
      };

      parent.dataSourceMethodSettingsContext.onLoad({
        useCustomButtons: false
      });
    },
    methods: {
      setHeight: function() {
        window.parent.Kooboo.EventBus.publish("kb/component/modal/set/height", {
          height: window.document.body.scrollHeight
        });
      },
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
          self.isPublic &&
          self.isNew &&
          self.$refs.methodForm &&
          !self.$refs.methodForm.validate()
        ) {
          return false;
        } else if (self.isFolder && !folderSelected) {
          window.alert(Kooboo.text.alert.pleaseChooseAFolder);
          return false;
        } else if (self.isProductType && !productTypeSelected) {
          window.alert(Kooboo.text.alert.pleaseChooseAFolder);
          return false;
        }
        return true;
      },
      submit: function(m, e) {
        if (self.isNew) {
          self.model.id = Kooboo.Guid.Empty;
        }
        self.model.isGlobal = false;
        self.model.isPublic = self.isPublic;
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
        self.$refs.methodForm && self.$refs.methodForm.clearValid();
        return Kooboo.DataSource.update(JSON.stringify(data));
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
