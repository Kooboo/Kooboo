(function() {
  ko.components.register("control-string", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/string.html"
    ),
    viewModel: function(params) {
      var self = this;
      this.value = ko.observable(params.value);
      this.key = params.key;
      this.value.subscribe(function() {
        Kooboo.EventBus.publish("parameterBinding change", self);
      });
    }
  });

  ko.components.register("control-textarea", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/textarea.html"
    ),
    viewModel: function(params) {
      var self = this;
      this.value = ko.observable(params.value);
      this.key = params.key;
      this.value.subscribe(function() {
        Kooboo.EventBus.publish("parameterBinding change", self);
      });
    }
  });

  function Dictionary(ob, controlDictionaryVm) {
    this.value = ko.observable(ob.value);
    this.key = ko.observable(ob.key);
    this.key.subscribe(function() {
      Kooboo.EventBus.publish("parameterBinding change", controlDictionaryVm);
    });
    this.value.subscribe(function() {
      Kooboo.EventBus.publish("parameterBinding change", controlDictionaryVm);
    });
  }

  ko.components.register("control-dictionary", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/dictionary.html"
    ),
    viewModel: function(params) {
      var self = this;
      this.key = params.key;

      if (params.value !== "") {
        var dictionaries = JSON.parse(params.value);
        if (dictionaries instanceof Array && dictionaries.length > 0) {
          this.value = ko.observableArray(dictionaries);
        } else {
          this.value = ko.observableArray([]);
        }
      } else {
        this.value = ko.observableArray([]);
      }

      this.add = function(m, e) {
        e.preventDefault();
        self.value.push(
          new Dictionary(
            {
              key: "",
              value: ""
            },
            self
          )
        );
      };
      this.remove = function(i, m, e) {
        var index = i;
        self.value.splice(index - 1, 1);
      };
    }
  });

  function Collection(ob, controlCollectionVm) {
    this.value = ko.observable(ob.value);
    this.value.subscribe(function() {
      Kooboo.EventBus.publish("parameterBinding change", controlCollectionVm);
    });
  }

  ko.components.register("control-collection", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/collection.html"
    ),
    viewModel: function(params) {
      var self = this;
      this.key = params.key;

      if (params.value !== "") {
        var collections = JSON.parse(params.value);
        if (collections instanceof Array && collections.length > 0) {
          this.value = ko.observableArray(collections);
        } else {
          this.value = ko.observableArray([]);
        }
      } else {
        this.value = ko.observableArray([]);
      }

      this.add = function(m, e) {
        e.preventDefault();
        self.value.push(
          new Collection(
            {
              value: ""
            },
            self
          )
        );
      };
      this.remove = function(i, m, e) {
        var index = i();
        self.value.splice(index - 1, 1);
      };
    }
  });

  ko.components.register("control-checkbox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/checkbox.html"
    ),
    viewModel: function(params) {
      var self = this;
      if (
        params &&
        (params.value === "True" ||
          params.value === "true" ||
          params.value === "False" ||
          params.value === "false")
      ) {
        this.value = ko.observable(JSON.parse(params.value.toCamelCase()));
      } else {
        this.value = false;
      }
      this.key = params.key;
      this.value.subscribe(function() {
        Kooboo.EventBus.publish("parameterBinding change", self);
      });
    }
  });

  ko.components.register("control-order", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/orderBy.html"
    ),
    viewModel: function(params) {
      var self = this;
      this.key = params.key;
      this.fieldsOfCurrentFolder = params.fieldsOfCurrentFolder;
      this.value = ko.observable();
      this.fieldsOfCurrentFolder.subscribe(function() {
        self.value(params.value);
      });
      this.value.subscribe(function() {
        Kooboo.EventBus.publish("parameterBinding change", self);
      });
    }
  });

  function Filter(ob, filterVm) {
    var self = this;
    this.key = ko.observable(ob.key);
    var choosedOperator;
    this.value = ko.observable(ob.value);
    if (ob.key) {
      choosedOperator = filterVm.fieldsOfCurrentFolder().filter(function(item) {
        return item.name === ob.key;
      })[0];
    }

    if (choosedOperator !== undefined) {
      this.operators = ko.observableArray(choosedOperator.operators);
    } else {
      this.operators = ko.observableArray([]);
    }
    this.comparison = ko.observable(ob.comparison);

    this.chooseField = function(m, e) {
      if (m.fieldsOfCurrentFolder) {
        var choosedOperator = m.fieldsOfCurrentFolder().filter(function(item) {
          return item.name === e.target.value;
        })[0];
        self.operators(choosedOperator ? choosedOperator.operators : []);
      }
    };

    this.key.subscribe(function() {
      Kooboo.EventBus.publish("parameterBinding change", filterVm);
    });
    this.comparison.subscribe(function() {
      Kooboo.EventBus.publish("parameterBinding change", filterVm);
    });
    this.value.subscribe(function() {
      Kooboo.EventBus.publish("parameterBinding change", filterVm);
    });
  }

  ko.components.register("control-filter", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/extensionEditor/filter.html"
    ),
    viewModel: function(params) {
      var self = this;
      this.fieldsOfCurrentFolder = params.fieldsOfCurrentFolder;
      this.value = ko.observableArray();
      this.key = params.key;

      this.fieldsOfCurrentFolder.subscribe(function() {
        self.value.removeAll();
        if (
          params.value !== undefined &&
          params.value !== "" &&
          params.value !== "[{}]"
        ) {
          if (
            JSON.parse(params.value) instanceof Array &&
            JSON.parse(params.value).length > 0
          ) {
            JSON.parse(params.value).forEach(function(item) {
              self.value.push(
                new Filter(
                  {
                    key: item.FieldName,
                    value: item.FieldValue,
                    comparison: item.Comparer
                  },
                  self
                )
              );
            });
          }
        }
      });

      this.add = function(m, e) {
        e.preventDefault();
        var newFilter = new Filter(
          {
            key: "Id",
            value: "",
            comparison: ""
          },
          self
        );
        self.value.push(newFilter);
      };
      this.remove = function(i, m, e) {
        var index = i();
        self.value.splice(index, 1);
        Kooboo.EventBus.publish("parameterBinding change", self);
      };
    }
  });
})();
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
        methodName: "",
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
      Kooboo.EventBus.subscribe("parameterBinding change", function(vm) {
        if (vm.value !== undefined) {
          var value = vm.value;
        } else {
          return false;
        }
        if (typeof value === "string" || typeof value === "boolean") {
          self.model.parameterBinding[vm.key.toCamelCase()].binding = value;
        } else {
          self.model.parameterBinding[
            vm.key.toCamelCase()
          ].binding = JSON.stringify(value);
        }
      });
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
        self.methodName = model.methodName;

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
                    window.parent.Kooboo.EventBus.publish(
                      "kb/component/modal/set/height",
                      {
                        height: window.document.body.scrollHeight
                      }
                    );
                  });
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
                    window.parent.Kooboo.EventBus.publish(
                      "kb/component/modal/set/height",
                      {
                        height: window.document.body.scrollHeight
                      }
                    );
                  });
              });
            }
          });
        } else {
          setTimeout(function() {
            window.parent.Kooboo.EventBus.publish(
              "kb/component/modal/set/height",
              {
                height: window.document.body.scrollHeight
              }
            );
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
          window.parent.Kooboo.EventBus.publish(
            "kb/component/modal/set/height",
            {
              height: window.document.body.scrollHeight
            }
          );
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
      dataSourceUrl: function() {
        window.location = Kooboo.Route.Get(Kooboo.Route.DataSource.ListPage);
      },
      submitable: function() {
        return true;
        // TODO: nj validate
        if (!!self.isPublic && !self.methodName.isValid() && !!self.isNew) {
          self.showError(true);
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
        // self.showError(false);
        if (self.isNew) {
          self.model.id = Kooboo.Guid.Empty;
          self.model.isGlobal = false;
        } else {
          self.model.isGlobal = false;
        }
        self.model.isPublic = self.isPublic;
        self.model.methodName = self.methodName;
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
        return Kooboo.DataSource.update(JSON.stringify(data));
      }
    }
  });
});

(function() {
  function methodSettingViewModel() {
    var self = this;
    this.showError = ko.observable();
    self.methodName.extend({
      validate: {
        required: Kooboo.text.validation.required,
        remote: {
          url: Kooboo.DataSource.isUniqueName(),
          message: Kooboo.text.validation.taken,
          type: "get",
          data: {
            name: function() {
              return self.methodName();
            }
          }
        },
        regex: {
          pattern: /^\w+$/,
          message: Kooboo.text.validation.nameInvalid
        }
      }
    });
    this.isPublic.subscribe(function(isPublic) {
      if (!isPublic) {
        self.showError(false);
      }
    });
  }
});
