(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/viewEditor/components/ActionDialog.html"),
        DataStore = Kooboo.viewEditor.store.DataStore,
        ActionStore = Kooboo.viewEditor.store.ActionStore,
        DataContext = Kooboo.viewEditor.DataContext,
        modal = Kooboo.viewEditor.component.modal;

    ko.components.register("kb-view-action-dialog", {
        viewModel: function(params) {

            var self = this;

            this.excludeEnumerable = ko.observable(false);

            this.viewId = ko.observable();

            this.context = params.context;

            this.onSave = params.onSave;

            this.isEdit = ko.observable(false);

            this.dataItem = null;

            this.isShow = ko.observable(false);

            this.name = ko.validateField({
                required: Kooboo.text.validation.required,
                regex: {
                    pattern: /^[a-zA-Z]\w*$/,
                    message: Kooboo.text.validation.nameInvalid
                }
            });

            this.methodId = ko.validateField({
                required: Kooboo.text.validation.required
            });
            this.methodId.subscribe(function(id) {
                var act = ActionStore.byId(id);
                self.parameterMappings.removeAll();
                act && self.parentId() && _.forEach(act.userVariables, function(paramName) {
                    var toParam = null;

                    if (self.dataItem) {
                        toParam = self.dataItem.parameterMappings[paramName] || null;
                    }

                    if (!toParam) {
                        // Guest best default
                        var field = _.find(self.fields(), function(field) {
                            return field.name.replace(/\./g, '').toLowerCase() === paramName.toLowerCase();
                        })

                        if (field) {
                            toParam = field.value;
                        }
                    }

                    self.parameterMappings.push({
                        fromParameter: paramName,
                        toParameter: ko.observable(toParam)
                    })
                })
            })

            this.isGlobal = ko.observable(false);
            this.isPublic = ko.observable(false);

            this.parentId = ko.observable();
            this.parentId.subscribe(function(id) {
                self.fields.removeAll();
                var ds = DataStore.byId(id);

                if (ds) {
                    var act = ActionStore.byId(ds.methodId),
                        fields = [];

                    if (act) {

                        if (act.enumerable) {
                            _.forEach(act.itemFields, function(fd) {
                                fields = fields.concat(self.getFields(fd, ds.aliasName + 'Item'));
                            })
                        } else {
                            _.forEach(act.fields, function(fd) {
                                fields = fields.concat(self.getFields(fd, ds.aliasName + '.'));
                            })
                        }

                        self.fields(fields);
                    }
                }
            })

            this.getFields = function(fd, prefix) {
                if (fd.enumerable) {
                    return [];
                }

                var ret = [],
                    subPrefix;
                if (fd.isComplexType) { //Object
                    subPrefix = prefix + fd.name + '.';
                    _.forEach(fd.fields, function(it) {
                        ret = ret.concat(self.getFields(it, subPrefix))
                    });
                } else { //Plain
                    ret.push({ name: prefix + fd.name, value: '{' + prefix + fd.name + '}' });
                }

                return ret;
            }

            this.fields = ko.observableArray();

            this.showError = ko.observable(false);

            this.parameterMappings = ko.observableArray();

            Kooboo.EventBus.subscribe("action/edit", function(data) {

                if (data.viewId) {
                    self.viewId(data.viewId);
                }

                var find = _.find(DataStore.getAll(), function(action) {
                    return action.id == data.parentId;
                })

                if (find) {
                    if (!ActionStore.byId(find.methodId).enumerable) {
                        self.excludeEnumerable(true);
                    } else {
                        self.excludeEnumerable(false);
                    }
                }

                if (!$("#action_tree").data("jstree")) {
                    self.renderTree();
                }

                setTimeout(function() {
                    var tree = $("#action_tree").data("jstree");
                    tree.deselect_all();
                    var $container = $('#action_tree').parent();
                    self.name(data.name);
                    self.parentId(data.parentId);
                    self.isShow(true);
                    self.context = data.context;

                    if (data.item) {
                        self.isEdit(true);
                        self.dataItem = data.item;

                        if (data.item.methodId) {
                            tree.select_node(data.item.methodId);
                            self.methodId(data.item.methodId);
                            $container.readOnly(true);
                        } else {
                            $container.readOnly(false);
                        }
                    } else {
                        $container.readOnly(false);
                        self.isEdit(false);
                    }
                }, 100);
            });

            this.reset = function() {
                self.name(null);
                self.methodId(null);
                self.parentId(null);
                self.showError(false);
                self.isShow(false);
                $.jstree.destroy();
            }

            this.isValid = ko.pureComputed(function() {
                return this.valid();
            }, this);

            this.valid = function() {
                var result = self.name.valid() && self.methodId.valid();
                if (result && this.context && this.context.actions && typeof this.context.actions === "function") {
                    var existsActions = _.map(this.context.actions(), function(a) {
                        return a["aliasName"].toLowerCase();
                    });
                    if (existsActions.indexOf(self.name().toLowerCase()) > -1 && !self.isEdit()) {
                        self.name.error(Kooboo.text.validation.taken);
                        return false;
                    }
                }
                return result;
            };

            this.next = function() {
                if (!self.valid()) {
                    self.showError(true);
                    return;
                } else {
                    self.showError(false);
                }

                var method = ActionStore.byId(self.methodId());

                self.isShow(false);
                self.methodId(null);

                if (_.toArray(method.parameters).length === 0) {
                    self.methodId(method.id);
                    modal.open({
                        title: "Configure datasource",
                        html: '<div class="alert alert-info">This datasource method does not require configureation.</div>',
                        method: method,
                        buttons: [{
                            text: 'Continue',
                            cssClass: 'green',
                            click: function(options) {
                                self.save(method);
                                options.modal.destroy();
                            }
                        }]
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
                                    cssClass: 'green',
                                    click: function(opts) {
                                        $(window.document.body).removeClass("modal-open");
                                        modal.find('iframe')[0].contentWindow.dataSourceMethodSettingsContext.displayName = self.name();
                                        modal.find('iframe')[0].contentWindow.dataSourceMethodSettingsContext.submit();
                                    }
                                })
                            } else {
                                if (data.buttons) {
                                    for (var i = 0, button; button = data.buttons[i]; i++) {
                                        modal.addButton(button);
                                    }
                                }
                            }

                            modal.addButton({
                                text: Kooboo.text.common.cancel,
                                cssClass: 'gray',
                                click: function(opts) {
                                    $(window.document.body).removeClass("modal-open");
                                    opts.modal.destroy();
                                }
                            });
                        },
                        onSubmit: function(data) {
                            data["methodName"] = self.name();
                            if (method.isGlobal) {
                                ActionStore.addMethod(data);
                            }
                            self.methodId(data.id);

                            window.dataSourceMethodSettingsModal.destroy();
                            self.save(data);
                        },
                        data: {}
                    };
                    window.dataSourceMethodSettingsModal = modal.open({
                        title: Kooboo.text.site.view.configureDataSource,
                        width: 800,
                        url: Kooboo.Route.Get(Kooboo.Route.DataSource.DataMethodSettingDialog, {
                            isNew: !method.isPublic,
                            id: method.id
                        }),
                        buttons: [{
                            id: 'cancel',
                            text: Kooboo.text.common.cancel,
                            cssClass: 'gray',
                            click: function(context) {
                                context.modal.destroy();
                            }
                        }]
                    });
                }
            }

            this.edit = function() {
                var method = ActionStore.byId(self.methodId());
                self.isShow(false);

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
                                cssClass: 'green',
                                click: function(opts) {
                                    $(window.document.body).removeClass("modal-open");
                                    modal.find('iframe')[0].contentWindow.dataSourceMethodSettingsContext.displayName = self.name();
                                    modal.find('iframe')[0].contentWindow.dataSourceMethodSettingsContext.submit();
                                }
                            })
                        } else {
                            if (data.buttons) {
                                for (var i = 0, button; button = data.buttons[i]; i++) {
                                    modal.addButton(button);
                                }
                            }
                        }

                        modal.addButton({
                            text: Kooboo.text.common.cancel,
                            cssClass: 'gray',
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
                    url: Kooboo.Route.Get(Kooboo.Route.DataSource.DataMethodSettingDialog, {
                        isNew: false,
                        id: method.id
                    }),
                    buttons: [{
                        id: 'cancel',
                        text: Kooboo.text.common.cancel,
                        cssClass: 'gray',
                        click: function(context) {
                            context.modal.destroy();
                        }
                    }]
                });
            }

            this.save = function(data) {

                if (self.valid()) {
                    var name = self.name(),
                        methodId = self.methodId(),
                        parentId = self.parentId(),
                        params = {},
                        isEdit = self.isEdit();
                    self.parameterMappings.each(function(it) {
                        params[it.fromParameter] = it.toParameter();
                    });

                    self.onSave({
                        isEdit: isEdit,
                        id: isEdit ? self.dataItem.id : null,
                        name: name,
                        methodId: methodId,
                        parentId: parentId,
                        parameterMappings: params,
                        itemFields: data ? data.itemFields : []
                    });

                    self.reset();
                } else {
                    self.showError(true);
                }
            }

            this.renderTree = function() {
                $("#action_tree").jstree({
                    "plugins": ["types", "conditionalselect", "checkbox"],
                    "types": {
                        "default": {
                            "icon": "fa fa-file icon-state-warning"
                        }
                    },
                    "conditionalselect": function(node) {
                        return !node.data.root;
                    },
                    "core": {
                        "strings": { 'Loading ...': Kooboo.text.common.loading + ' ...' },
                        "data": function(obj, cb) {
                            var acts = ActionStore.getAll(),
                                treeData = [];

                            acts.forEach(function(it) {
                                if (!it.isPost) {

                                    it.methods.forEach(function(m) {
                                        if (!m.isPublic && !m.viewIds) {
                                            m.viewIds = [];
                                            m.viewIds.push(self.viewId());
                                        }
                                    });
                                    var methods = _.filter(it.methods, function(m) {
                                            return m.isGlobal || m.isPublic || (!m.isPublic && m.viewIds && m.viewIds.indexOf(self.viewId()) > -1)
                                        }),
                                        list = _.map(methods, function(method) {

                                            if ((self.excludeEnumerable() && !method.enumerable) ||
                                                !self.excludeEnumerable()) {
                                                var localMethoad = DataStore.byMethodId(method.id);

                                                var displayName = (localMethoad ? localMethoad.aliasName : method.methodName) + (method.description ? ' (' + method.description + ')' : ''),
                                                    response = {
                                                        id: method.id,
                                                        data: {
                                                            methodId: method.id,
                                                            name: localMethoad ? localMethoad.aliasName : method.methodName,
                                                            isGlobal: method.isGlobal,
                                                            isPublic: method.isPublic,
                                                            description: method.description
                                                        },
                                                        state: {
                                                            disabled: method.isGlobal ? false : !method.isPublic
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
                            self.excludeEnumerable(false);
                        }
                    }
                }).on("loaded.jstree", function(e, data) {
                    // hide root checkboxes
                    $('#action_tree>ul>li>a .jstree-checkbox').hide();
                    var inst = $(this).data("jstree");
                    inst.open_all();
                }).on("select_node.jstree", function(e, selected) {
                    var selectedNode = selected.node,
                        selectedData = selectedNode.data;
                    if (selected.event) {
                        $.each(selected.selected, function() {
                            if (this.toString() !== selectedNode.id) {
                                selected.instance.uncheck_node(this.toString());
                            }
                        });
                        self.isGlobal(selectedData.isGlobal);
                        self.isPublic(selectedData.isPublic);
                        self.name(selectedData.name);
                        self.methodId(selectedData.methodId);
                    }
                }).on("deselect_node.jstree", function() {
                    self.methodId(null);
                    self.name("");
                });
            }

            Kooboo.EventBus.subscribe("ActionStore/change", _.debounce(function() {
                $('#action_tree').jstree('refresh');
            }, 100));
        },
        template: template
    })
})()