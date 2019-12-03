(function() {
  Vue.component("control-string", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/string.html"
    ),
    inheritAttrs: false,
    props: ["name", "value"],
    methods: {
      change: function(value) {
        this.$emit("change", {
          name: this.name,
          value: value
        });
      }
    }
  });

  Vue.component("control-textarea", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/textarea.html"
    ),
    inheritAttrs: false,
    props: ["name", "value"],
    methods: {
      change: function(value) {
        this.$emit("change", {
          name: this.name,
          value: value
        });
      }
    }
  });

  Vue.component("control-dictionary", {
    inheritAttrs: false,
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/dictionary.html"
    ),
    props: ["name", "value"],
    inheritAttrs: false,
    data: function() {
      return {
        values: []
      };
    },
    mounted: function() {
      if (!this.value) {
        var dictionaries = JSON.parse(this.value);
        if (dictionaries instanceof Array && dictionaries.length > 0) {
          this.values = dictionaries;
        } else {
          this.values = [];
        }
      } else {
        this.values = [];
      }
    },
    methods: {
      add: function() {
        this.values.push({
          key: "",
          value: ""
        });
      },
      remove: function(i) {
        this.values.splice(i, 1);
      }
    },
    watch: {
      values: {
        handler: function(val) {
          this.$emit("change", {
            name: this.name,
            value: this.values
          });
        },
        deep: true
      }
    }
  });

  Vue.component("control-collection", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/collection.html"
    ),
    inheritAttrs: false,
    props: ["name", "value"],
    data: function() {
      return {
        values: []
      };
    },
    mounted: function() {
      if (!this.value) {
        var collections = JSON.parse(this.value);
        if (collections instanceof Array && collections.length > 0) {
          this.values = collections;
        } else {
          this.values = [];
        }
      } else {
        this.values = [];
      }
    },
    methods: {
      add: function() {
        this.values.push({
          value: ""
        });
      },
      remove: function(i) {
        this.values.splice(i, 1);
        this.$emit("change", {
          name: this.name,
          value: this.values
        });
      }
    },
    watch: {
      values: {
        handler: function(val) {
          this.$emit("change", {
            name: this.name,
            value: this.values
          });
        },
        deep: true
      }
    }
  });

  Vue.component("control-checkbox", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/checkbox.html"
    ),
    inheritAttrs: false,
    props: ["name", "value"],
    data: function() {
      return {
        valueBool: false
      };
    },
    mounted: function() {
      this.valueBool = this.value === "True" || this.value === "true";
    },
    watch: {
      valueBool: function(val) {
        this.$emit("change", {
          name: this.name,
          value: val
        });
      }
    }
  });

  Vue.component("control-order", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/orderBy.html"
    ),
    props: ["name", "value", "fields"],
    methods: {
      change: function(value) {
        this.$emit("change", {
          name: this.name,
          value: value
        });
      }
    }
  });

  function Filter(ob, fields) {
    var choosedOperator;
    if (ob.key) {
      choosedOperator = fields.filter(function(item) {
        return item.name === ob.key;
      })[0];
    }
    if (choosedOperator !== undefined) {
      ob.operators = choosedOperator.operators;
    } else {
      ob.operators = [];
    }
    return ob;
  }

  Vue.component("control-filter", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/extensionEditor/filter.html"
    ),
    props: ["name", "value", "fields"],
    data: function() {
      return {
        values: []
      };
    },
    methods: {
      chooseField: function(filter) {
        if (this.fields) {
          var choosedOperator = m.fields.filter(function(item) {
            return item.name === filter.key;
          })[0];
          filter.operators = choosedOperator ? choosedOperator.operators : [];
        }
      },
      add: function() {
        var newFilter = Filter(
          {
            key: "Id",
            value: "",
            comparison: ""
          },
          this.fields
        );
        this.values.push(newFilter);
      },
      remove: function(i) {
        this.values.splice(i, 1);
        this.$emit("change", {
          name: this.name,
          value: this.values
        });
      }
    },
    watch: {
      fields: {
        handler: function(val) {
          this.values = [];
          if (
            this.value !== undefined &&
            this.value !== "" &&
            this.value !== "[{}]"
          ) {
            if (
              JSON.parse(this.value) instanceof Array &&
              JSON.parse(this.value).length > 0
            ) {
              JSON.parse(this.value).forEach(function(item) {
                this.values.push(
                  Filter(
                    {
                      key: item.FieldName,
                      value: item.FieldValue,
                      comparison: item.Comparer
                    },
                    this.fields
                  )
                );
              });
            }
          }
        },
        immediate: true
      },
      watch: {
        values: {
          handler: function(val) {
            this.$emit("change", {
              name: this.name,
              value: this.values
            });
          },
          deep: true
        }
      }
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
      changeParameterBinding(data) {
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
