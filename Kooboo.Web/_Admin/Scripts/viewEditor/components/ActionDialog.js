(function() {
  var DataStore = Kooboo.viewEditor.store.DataStore,
    ActionStore = Kooboo.viewEditor.store.ActionStore,
    DataContext = Kooboo.viewEditor.DataContext,
    modal = Kooboo.viewEditor.component.modal;
  var self;
  Vue.component("kb-view-action-dialog", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/viewEditor/components/ActionDialog.html"
    ),
    data: function() {
      self = this;
      return {
        excludeEnumerable: false,
        viewId: "",
        model: {
          name: ""
        },
        rules: {
          name: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              pattern: /^[a-zA-Z]\w*$/,
              message: Kooboo.text.validation.nameInvalid
            },
            {
              validate: function(val) {
                if (self.isEdit) {
                  return true;
                }
                if (
                  !self.context ||
                  !self.context.actions ||
                  !self.context.actions.length
                ) {
                  return true;
                }
                return !_.some(self.context.actions, function(a) {
                  return a.aliasName.toLowerCase() === val.toLowerCase();
                });
              },
              message: Kooboo.text.validation.taken
            }
          ]
        },
        isEdit: false,
        dataItem: null,
        isShow: false,
        isGlobal: false,
        isPublic: false,
        parentId: null,
        fields: [],
        parameterMappings: [],
        methodId: null,
        context: {}
      };
    },
    mounted: function() {
      Kooboo.EventBus.subscribe(
        "ActionStore/change",
        _.debounce(function() {
          $("#action_tree").jstree("refresh");
        }, 100)
      );
      Kooboo.EventBus.subscribe("action/edit", function(data) {
        if (data.viewId) {
          self.viewId = data.viewId;
        }

        var find = _.find(DataStore.getAll(), function(action) {
          return action.id == data.parentId;
        });

        if (find) {
          if (!ActionStore.byId(find.methodId).enumerable) {
            self.excludeEnumerable = true;
          } else {
            self.excludeEnumerable = false;
          }
        }

        if (!$("#action_tree").data("jstree")) {
          self.renderTree();
        }

        setTimeout(function() {
          var tree = $("#action_tree").data("jstree");
          tree.deselect_all();
          var $container = $("#action_tree").parent();
          self.model.name = data.name;
          self.parentId = data.parentId;
          self.isShow = true;
          self.context = data.context;

          if (data.item) {
            self.isEdit = true;
            self.dataItem = data.item;

            if (data.item.methodId) {
              tree.select_node(data.item.methodId);
              self.methodId = data.item.methodId;
              $container.readOnly(true);
            } else {
              $container.readOnly(false);
            }
          } else {
            $container.readOnly(false);
            self.isEdit = false;
          }
        }, 100);
      });
    },
    methods: {
      getFields: function(fd, prefix) {
        if (fd.enumerable) {
          return [];
        }

        var ret = [],
          subPrefix;
        if (fd.isComplexType) {
          //Object
          subPrefix = prefix + fd.name + ".";
          _.forEach(fd.fields, function(it) {
            ret = ret.concat(self.getFields(it, subPrefix));
          });
        } else {
          //Plain
          ret.push({
            name: prefix + fd.name,
            value: "{" + prefix + fd.name + "}"
          });
        }

        return ret;
      },
      reset: function() {
        self.model.name = null;
        self.methodId = null;
        self.parentId = null;
        self.isShow = false;
        $.jstree.destroy();
        self.$refs.form.clearValid();
      },
      valid: function() {
        return self.$refs.form.validate();
      },
      next: function() {
        if (!self.valid()) {
          return;
        }
        var method = ActionStore.byId(self.methodId);

        self.isShow = false;
        self.methodId = null;

        if (_.toArray(method.parameters).length === 0) {
          self.methodId = method.id;
          modal.open({
            title: "Configure datasource",
            html:
              '<div class="alert alert-info">This datasource method does not require configureation.</div>',
            method: method,
            buttons: [
              {
                text: "Continue",
                cssClass: "green",
                click: function(options) {
                  self.save(method);
                  options.modal.destroy();
                }
              }
            ]
          });
        } else {
          window.dataSourceMethodSettingsContext = {
            onLoad: function(data) {
              var modal = window.dataSourceMethodSettingsModal;

              if (!$(window.document.body).hasClass("modal-open")) {
                // fix scroll bug
                $(window.document.body).addClass("modal-open");
              }

              modal.clearButtons();

              if (!data.useCustomButtons) {
                modal.addButton({
                  text: Kooboo.text.common.save,
                  cssClass: "green",
                  click: function(opts) {
                    $(window.document.body).removeClass("modal-open");
                    modal.find(
                      "iframe"
                    )[0].contentWindow.dataSourceMethodSettingsContext.displayName =
                      self.model.name;
                    modal
                      .find("iframe")[0]
                      .contentWindow.dataSourceMethodSettingsContext.submit();
                  }
                });
              } else {
                if (data.buttons) {
                  for (var i = 0, button; (button = data.buttons[i]); i++) {
                    modal.addButton(button);
                  }
                }
              }

              modal.addButton({
                text: Kooboo.text.common.cancel,
                cssClass: "gray",
                click: function(opts) {
                  $(window.document.body).removeClass("modal-open");
                  opts.modal.destroy();
                }
              });
            },
            onSubmit: function(data) {
              data["methodName"] = self.model.name;
              if (method.isGlobal) {
                ActionStore.addMethod(data);
              }
              self.methodId = data.id;

              window.dataSourceMethodSettingsModal.destroy();
              self.save(data);
            },
            data: {}
          };
          window.dataSourceMethodSettingsModal = modal.open({
            title: Kooboo.text.site.view.configureDataSource,
            width: 800,
            url: Kooboo.Route.Get(
              Kooboo.Route.DataSource.DataMethodSettingDialog,
              {
                isNew: !method.isPublic,
                id: method.id
              }
            ),
            buttons: [
              {
                id: "cancel",
                text: Kooboo.text.common.cancel,
                cssClass: "gray",
                click: function(context) {
                  context.modal.destroy();
                }
              }
            ]
          });
        }
      },
      edit: function() {
        var method = ActionStore.byId(self.methodId);
        self.isShow = false;

        window.dataSourceMethodSettingsContext = {
          onLoad: function(data) {
            var modal = window.dataSourceMethodSettingsModal;

            if (!$(window.document.body).hasClass("modal-open")) {
              $(window.document.body).addClass("modal-open");
            }

            modal.clearButtons();

            if (!data.useCustomButtons) {
              modal.addButton({
                text: Kooboo.text.common.save,
                cssClass: "green",
                click: function(opts) {
                  $(window.document.body).removeClass("modal-open");
                  modal.find(
                    "iframe"
                  )[0].contentWindow.dataSourceMethodSettingsContext.displayName =
                    self.model.name;
                  modal
                    .find("iframe")[0]
                    .contentWindow.dataSourceMethodSettingsContext.submit();
                }
              });
            } else {
              if (data.buttons) {
                for (var i = 0, button; (button = data.buttons[i]); i++) {
                  modal.addButton(button);
                }
              }
            }

            modal.addButton({
              text: Kooboo.text.common.cancel,
              cssClass: "gray",
              click: function(opts) {
                $(window.document.body).removeClass("modal-open");
                opts.modal.destroy();
              }
            });
          },
          onSubmit: function(data) {
            self.save(data);
            window.dataSourceMethodSettingsModal.destroy();
          },
          data: {}
        };

        window.dataSourceMethodSettingsModal = modal.open({
          title: Kooboo.text.site.view.configureDataSource,
          width: 800,
          url: Kooboo.Route.Get(
            Kooboo.Route.DataSource.DataMethodSettingDialog,
            {
              isNew: false,
              id: method.id
            }
          ),
          buttons: [
            {
              id: "cancel",
              text: Kooboo.text.common.cancel,
              cssClass: "gray",
              click: function(context) {
                context.modal.destroy();
              }
            }
          ]
        });
      },
      save: function(data) {
        if (!self.valid()) {
          return;
        }
        var name = self.model.name,
          methodId = self.methodId,
          parentId = self.parentId,
          params = {},
          isEdit = self.isEdit;
        self.parameterMappings.forEach(function(it) {
          params[it.fromParameter] = it.toParameter;
        });
        self.$emit("on-save", {
          isEdit: isEdit,
          id: isEdit ? self.dataItem.id : null,
          name: name,
          methodId: methodId,
          parentId: parentId,
          parameterMappings: params,
          itemFields: data ? data.itemFields : []
        });
        self.reset();
      },
      renderTree: function() {
        $("#action_tree")
          .jstree({
            plugins: ["types", "conditionalselect", "checkbox"],
            types: {
              default: {
                icon: "fa fa-file icon-state-warning"
              }
            },
            conditionalselect: function(node) {
              return !node.data.root;
            },
            core: {
              strings: { "Loading ...": Kooboo.text.common.loading + " ..." },
              data: function(obj, cb) {
                var acts = ActionStore.getAll(),
                  treeData = [];

                acts.forEach(function(it) {
                  if (!it.isPost) {
                    it.methods.forEach(function(m) {
                      if (!m.isPublic && !m.viewIds) {
                        m.viewIds = [];
                        m.viewIds.push(self.viewId);
                      }
                    });
                    var methods = _.filter(it.methods, function(m) {
                        return (
                          m.isGlobal ||
                          m.isPublic ||
                          (!m.isPublic &&
                            m.viewIds &&
                            m.viewIds.indexOf(self.viewId) > -1)
                        );
                      }),
                      list = _.map(methods, function(method) {
                        if (
                          (self.excludeEnumerable && !method.enumerable) ||
                          !self.excludeEnumerable
                        ) {
                          var localMethoad = DataStore.byMethodId(method.id);

                          var displayName =
                              (localMethoad
                                ? localMethoad.aliasName
                                : method.methodName) +
                              (method.description
                                ? " (" + method.description + ")"
                                : ""),
                            response = {
                              id: method.id,
                              data: {
                                methodId: method.id,
                                name: localMethoad
                                  ? localMethoad.aliasName
                                  : method.methodName,
                                isGlobal: method.isGlobal,
                                isPublic: method.isPublic,
                                description: method.description
                              },
                              state: {
                                disabled: method.isGlobal
                                  ? false
                                  : !method.isPublic
                              }
                            };
                          if (!method.isGlobal) {
                            response["icon"] = "fa fa-file icon-state-info";
                          }
                          response["text"] = displayName;
                          return response;
                        } else {
                          return null;
                        }
                      });
                    list = _.compact(list);

                    treeData.push({
                      text: it.displayName,
                      data: {
                        root: true
                      },
                      children: list
                    });
                  }
                });
                cb.call(this, treeData);
                self.excludeEnumerable = false;
              }
            }
          })
          .on("loaded.jstree", function(e, data) {
            // hide root checkboxes
            $("#action_tree>ul>li>a .jstree-checkbox").hide();
            var inst = $(this).data("jstree");
            inst.open_all();
          })
          .on("select_node.jstree", function(e, selected) {
            var selectedNode = selected.node,
              selectedData = selectedNode.data;
            if (selected.event) {
              $.each(selected.selected, function() {
                if (this.toString() !== selectedNode.id) {
                  selected.instance.uncheck_node(this.toString());
                }
              });
              self.isGlobal = selectedData.isGlobal;
              self.isPublic = selectedData.isPublic;
              self.model.name = selectedData.name;
              self.methodId = selectedData.methodId;
            }
          })
          .on("deselect_node.jstree", function() {
            self.methodId = null;
            self.model.name = "";
          });
      }
    },
    watch: {
      methodId: function(id) {
        var act = ActionStore.byId(id);
        self.parameterMappings = [];
        act &&
          self.parentId &&
          _.forEach(act.userVariables, function(paramName) {
            var toParam = null;

            if (self.dataItem) {
              toParam = self.dataItem.parameterMappings[paramName] || null;
            }

            if (!toParam) {
              // Guest best default
              var field = _.find(self.fields, function(field) {
                return (
                  field.name.replace(/\./g, "").toLowerCase() ===
                  paramName.toLowerCase()
                );
              });

              if (field) {
                toParam = field.value;
              }
            }

            self.parameterMappings.push({
              fromParameter: paramName,
              toParameter: toParam
            });
          });
      },
      parentId: function(id) {
        self.fields = [];
        var ds = DataStore.byId(id);

        if (ds) {
          var act = ActionStore.byId(ds.methodId),
            fields = [];

          if (act) {
            if (act.enumerable) {
              _.forEach(act.itemFields, function(fd) {
                fields = fields.concat(
                  self.getFields(fd, ds.aliasName + "Item")
                );
              });
            } else {
              _.forEach(act.fields, function(fd) {
                fields = fields.concat(self.getFields(fd, ds.aliasName + "."));
              });
            }

            self.fields = fields;
          }
        }
      }
    }
  });
})();
