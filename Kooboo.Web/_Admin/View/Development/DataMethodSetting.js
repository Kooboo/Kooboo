$(function() {

    var folderSelected = false;

    ko.components.register('control-string', {
        template: Kooboo.getTemplate("/_Admin/Scripts/components/extensionEditor/string.html"),
        viewModel: function(params) {
            var self = this;
            this.value = ko.observable(params.value);
            this.key = params.key;
            this.value.subscribe(function() {
                Kooboo.EventBus.publish("parameterBinding change", self);
            });
        }
    });

    ko.components.register('control-textarea', {
        template: Kooboo.getTemplate("/_Admin/Scripts/components/extensionEditor/textarea.html"),
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
        })
        this.value.subscribe(function() {
            Kooboo.EventBus.publish("parameterBinding change", controlDictionaryVm);
        })
    }

    ko.components.register('control-dictionary', {
        template: Kooboo.getTemplate("/_Admin/Scripts/components/extensionEditor/dictionary.html"),
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
                self.value.push(new Dictionary({
                    key: "",
                    value: ""
                }, self));
            }
            this.remove = function(i, m, e) {
                var index = i;
                self.value.splice(index - 1, 1);
            }
        }
    });

    function Collection(ob, controlCollectionVm) {
        this.value = ko.observable(ob.value);
        this.value.subscribe(function() {
            Kooboo.EventBus.publish("parameterBinding change", controlCollectionVm);
        })
    }

    ko.components.register('control-collection', {
        template: Kooboo.getTemplate("/_Admin/Scripts/components/extensionEditor/collection.html"),
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
                self.value.push(new Collection({
                    value: ""
                }, self));
            }
            this.remove = function(i, m, e) {
                var index = i();
                self.value.splice(index - 1, 1);
            }
        }
    });

    String.prototype.toCamelCase = function() {
        return this[0].toLocaleLowerCase() + this.slice(1);
    }

    ko.components.register('control-checkbox', {
        template: Kooboo.getTemplate("/_Admin/Scripts/components/extensionEditor/checkbox.html"),
        viewModel: function(params) {
            var self = this;
            if (params && (params.value === "True" || params.value === "true" || params.value === "False" || params.value === "false")) {
                this.value = ko.observable(JSON.parse(params.value.toCamelCase()));
            } else {
                this.value = ko.observable(false);
            }
            this.key = params.key;
            this.value.subscribe(function() {
                Kooboo.EventBus.publish("parameterBinding change", self);
            });
        }
    });

    ko.components.register('control-order', {
        template: Kooboo.getTemplate("/_Admin/Scripts/components/extensionEditor/orderBy.html"),
        viewModel: function(params) {
            var self = this;
            this.key = params.key;
            this.fieldsOfCurrentFolder = params.fieldsOfCurrentFolder;
            this.value = ko.observable();
            this.fieldsOfCurrentFolder.subscribe(function() {
                self.value(params.value);
            })
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
                return item.name === ob.key
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
                var choosedOperator = m.fieldsOfCurrentFolder()
                    .filter(function(item) {
                        return item.name === e.target.value
                    })[0];
                self.operators(choosedOperator ? choosedOperator.operators : []);
            }
        }

        this.key.subscribe(function() {
            Kooboo.EventBus.publish("parameterBinding change", filterVm);
        })
        this.comparison.subscribe(function() {
            Kooboo.EventBus.publish("parameterBinding change", filterVm);
        })
        this.value.subscribe(function() {
            Kooboo.EventBus.publish("parameterBinding change", filterVm);
        })
    }

    ko.components.register('control-filter', {
        template: Kooboo.getTemplate("/_Admin/Scripts/components/extensionEditor/filter.html"),
        viewModel: function(params) {
            var self = this;
            this.fieldsOfCurrentFolder = params.fieldsOfCurrentFolder;
            this.value = ko.observableArray();
            this.key = params.key;

            this.fieldsOfCurrentFolder.subscribe(function() {
                self.value.removeAll();
                if (params.value !== undefined && params.value !== "" && params.value !== "[{}]") {
                    if (JSON.parse(params.value) instanceof Array && JSON.parse(params.value).length > 0) {
                        JSON.parse(params.value).forEach(function(item) {
                            self.value.push(new Filter({
                                key: item.FieldName,
                                value: item.FieldValue,
                                comparison: item.Comparer
                            }, self));
                        })
                    }
                }
            })

            this.add = function(m, e) {
                e.preventDefault();
                var newFilter = new Filter({
                    key: "Id",
                    value: "",
                    comparison: ""
                }, self)
                self.value.push(newFilter);
            }
            this.remove = function(i, m, e) {
                var index = i();
                self.value.splice(index, 1);
                Kooboo.EventBus.publish("parameterBinding change", self);
            }

        }
    });

    function methodSettingViewModel() {
        var self = this;
        this.showError = ko.observable(false);
        this.declareType = ko.observable();
        this.fieldsOfCurrentFolder = ko.observableArray();
        this.model = ko.observable();
        this.parameterBinding = ko.observableArray();
        this.supportedComparers = ko.observableArray();
        var methodId = Kooboo.getQueryString("id");
        var isNew = JSON.parse(Kooboo.getQueryString("isNew"));
        this.isNew = ko.observable(isNew);

        if (!!isNew) {
            this.methodName = ko.observable("").extend({
                validate: {
                    required: Kooboo.text.validation.required,
                    remote: {
                        url: Kooboo.DataSource.isUniqueName(),
                        message: Kooboo.text.validation.taken,
                        type: "get",
                        data: {
                            name: function() {
                                return self.methodName()
                            }
                        }
                    },
                    stringlength: {
                        min: 1,
                        max: 64,
                        message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
                    },
                    regex: {
                        pattern: /^\w+$/,
                        message: Kooboo.text.validation.nameInvalid
                    }
                }
            });
        } else {
            this.methodName = ko.observable("");
        }
        this.isFolder = ko.observable();
        this.isPublic = ko.observable(false);
        this.hasFolder = ko.observable(true);
        this.textContentsUrl = Kooboo.Route.Get(Kooboo.Route.ContentType.ListPage);

        this.dataSourceUrl = function() {
            window.location = Kooboo.Route.Get(Kooboo.Route.DataSource.ListPage);
        }

        this.submit = function(m, e) {
            if (!!isNew && !isValid()) {
                self.showError(true);
            } else if (self.isFolder() && !folderSelected) {
                window.alert(Kooboo.text.alert.pleaseChooseAFolder);
            } else {
                if (!!isNew) {
                    self.model().id(Kooboo.Guid.Empty);
                    self.model().isGlobal(false);
                } else {
                    self.model().isGlobal(false);
                }
                self.model().isPublic = true;
                self.model().methodName = self.methodName();
                var data = ko.mapping.toJS(self.model());

                for (var k in data.parameterBinding) {
                    if (data.parameterBinding[k].controlType === "contentFilter") {
                        if (data.parameterBinding[k].binding) {
                            var filter = JSON.parse(data.parameterBinding[k].binding);
                            var _filter = filter.map(function(o) {
                                return {
                                    FieldName: o.key || o.FieldName,
                                    FieldValue: o.value || o.FieldValue,
                                    Comparer: o.comparison || o.Comparer
                                }
                            })
                            data.parameterBinding[k].binding = JSON.stringify(_filter);
                        } else {
                            data.parameterBinding[k].binding = JSON.stringify([]);
                        }
                    }
                }
                Kooboo.DataSource.update(data).then(function(res) {
                    // console.log(res)
                    if (res.success) {
                        window.location.href = Kooboo.Route.Get(Kooboo.Route.DataSource.ListPage);
                    }
                })
            }
        }

        Kooboo.EventBus.subscribe("parameterBinding change", function(vm) {
            if (vm.value() !== undefined) {
                var value = ko.mapping.toJS(vm.value());
            } else {
                return false
            }
            if (typeof value === "string" || typeof value === "boolean") {
                self.model().parameterBinding[vm.key.toCamelCase()].binding(value);
            } else {
                self.model().parameterBinding[vm.key.toCamelCase()].binding(JSON.stringify(value));
            }
        });
        Kooboo.EventBus.subscribe("folder change", function(folderId) {
            var getFolder = self.parameterBinding().filter(function(item) {
                if (item.value.controlType === "contentFolder") {
                    return true;
                }
            })[0];
            self.model().parameterBinding[getFolder.key].binding(folderId);
        });

        // get method setting data
        Kooboo.DataSource.get({
            id: methodId
        }).then(function(data) {
            var model = data.model;

            var viewModel = ko.mapping.fromJS(data.model);

            self.model(viewModel)
            self.methodName(model.displayName);

            self.declareType(model.declareType);

            _.forEach(model.parameterBinding, function(value, key) {
                var ob = {};
                ob.key = key;
                ob.value = value;
                self.parameterBinding.push(ob);
            });

            var isFolder = self.parameterBinding().some(function(item) {
                if (item.value.controlType === "contentFolder") {
                    return true;
                }
            });
            self.isFolder(isFolder);

            if (isFolder) {
                var getFolder = self.parameterBinding().filter(function(item) {
                    if (item.value.controlType === "contentFolder") {
                        return true;
                    }
                });
                if (getFolder.length > 0) {
                    getFolder = getFolder[0];
                }
                // get content folder data
                Kooboo.ContentFolder.getList().then(function(data) {
                    var folders = data.model;
                    var d = [],
                        allowMultiple = getFolder.value.isCollection;
                    if (folders && folders.length > 0) {
                        self.hasFolder(true);
                    } else {
                        self.hasFolder(false);
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
                    $('#using_json').jstree({
                        'plugins': ['types', 'checkbox'],
                        'types': {
                            'default': {
                                icon: 'fa fa-file icon-state-warning'
                            }
                        },
                        'core': {
                            'strings': { 'Loading ...': Kooboo.text.common.loading + ' ...' },
                            'data': d,
                            "multiple": allowMultiple
                        }
                    }).on("changed.jstree", function(e, data) {
                        //判断是否有选中folder
                        var ContentFolderId;

                        if (!allowMultiple) {
                            ContentFolderId = data.selected[0];
                            if (!!ContentFolderId) {
                                folderSelected = true
                            } else {
                                folderSelected = false
                            }

                            //get content folder columns
                            if (folderSelected) {
                                Kooboo.ContentFolder.getColumnsById({
                                    id: ContentFolderId
                                }).then(function(res) {
                                    var model = res.model;
                                    self.fieldsOfCurrentFolder(model);
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
                    }).on("loaded.jstree", function() {
                        $("#using_json").parent().readOnly(!self.isNew());
                    });
                });
            }

            $(".wizard-nav-item").click(function(e) {
                //todo use ko
                $(".wizard-nav-item").removeClass("active");
                $(e.target).addClass("active");
                var target = $(e.target).data("step");
                if (target === "configure" && !folderSelected && isFolder) return false;
                $(".wizard-body").hide();
                $("div[data-step=" + target + "]").show();
            });
        });

        function isValid() {
            return self.methodName.isValid();
        }
    }
    ko.applyBindings(new methodSettingViewModel, $("#main")[0]);
});