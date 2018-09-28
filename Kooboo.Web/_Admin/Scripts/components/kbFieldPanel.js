(function () {
    Kooboo.loadJS([
        "/_Admin/Scripts/tableModel.js",
        "/_Admin/Scripts/components/kbTable.js",
        "/_Admin/Scripts/components/kb-media-dialog.js",
        "/_Admin/Scripts/lib/tinymce/tinymceInitPath.js",
        "/_Admin/Scripts/lib/tinymce/tinymce.min.js",
        "/_Admin/Scripts/lib/jstree.min.js",
        "/_Admin/Scripts/lib/moment.min.js",
        "/_Admin/Scripts/viewEditor/components/modal.js",
        "/_Admin/Scripts/textContent/embeddedDialog.js",
        "/_Admin/Scripts/kobindings.richeditor.js",
        "/_Admin/Scripts/kobindings.textError.js",
        "/_Admin/Scripts/kobindings.datePicker.js",
        "/_Admin/Scripts/components/controlType/TextBox.js",
        "/_Admin/Scripts/components/controlType/TextArea.js",
        "/_Admin/Scripts/components/controlType/RichEditor.js",
        "/_Admin/Scripts/components/controlType/Number.js",
        "/_Admin/Scripts/components/controlType/Selection.js",
        "/_Admin/Scripts/components/controlType/CheckBox.js",
        "/_Admin/Scripts/components/controlType/RadioBox.js",
        "/_Admin/Scripts/components/controlType/Switch.js",
        "/_Admin/Scripts/components/controlType/MediaFile.js",
        "/_Admin/Scripts/components/controlType/DateTime.js",
    ]);
    Kooboo.loadCSS([
        "/_Admin/Styles/jstree/style.min.css",
        "/_Admin/Styles/bootstrap-datetimepicker.min.css",
    ]);

    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbFieldPanel.html");

    ko.components.register("kb-field-panel", {
        viewModel: function (params) {
            var self = this;

            var LANG = Kooboo.getQueryString("lang");
            this.isMultiContent = ko.observable(!!LANG);

            this.fields = ko.observableArray();
            this.categories = ko.observableArray();

            this.currentLangs = ko.observableArray();
            this.siteLangs = params.siteLangs;
            if (!this.siteLangs()) {
                this.siteLangs.subscribe(function (langs) {
                    if (langs) {
                        if (self.currentLangs().indexOf(langs.default) == -1) {
                            self.currentLangs.push(langs.default);
                        }
                    }
                })
            } else {
                self.currentLangs.push(this.siteLangs().default);
            }

            this.multilingualSite = ko.pureComputed(function () {
                return Object.keys(self.siteLangs().cultures).length > 1;
            });

            LANG && self.currentLangs.push(LANG);

            // field
            this._fields = params.fields;
            this._fields.subscribe(function (fields) {
                var defaultLang = self.siteLangs().default;
                var d = _.cloneDeep(fields),
                    fieldItem = [];
                d.forEach(function (item) {
                    item.fieldValue = item.values || '';
                    var valueArr = [];

                    var langKeys = Object.keys(item.fieldValue),
                        defaultLangIdx = langKeys.indexOf(defaultLang);

                    langKeys.splice(defaultLangIdx, 1);

                    valueArr.push({
                        lang: defaultLang,
                        value: item.fieldValue[defaultLang]
                    })

                    langKeys.forEach(function (key) {
                        valueArr.push({
                            lang: key,
                            value: item.fieldValue[key]
                        })
                    })

                    item.fieldValue = valueArr;
                    item.fieldValue.forEach(function (v) {

                        if (['boolean', 'switch'].indexOf(item.controlType.toLowerCase()) > -1) {
                            if (v.value && typeof v.value == 'string') {
                                v.value = JSON.parse(v.value.toLowerCase());
                            } else {
                                v.value = true;
                            }
                        }

                        fieldItem.push({
                            controlType: item.controlType,
                            lang: v.lang,
                            fieldValue: (item.multipleValue ? (!v.value ? [] : JSON.parse(v.value)) : v.value),
                            isMultilingual: item.isMultilingual,
                            name: item.name,
                            displayName: item.displayName,
                            validations: item.validations,
                            values: item.values,
                            tooltip: item.toolTip,
                            multipleValue: item.multipleValue,
                            selectionOptions: item.selectionOptions,
                            disabled: self.isMultiContent() ? (v.lang !== LANG) : false,
                            isMultilingualSite: self.multilingualSite(),
                            isShow: ko.pureComputed(function () {
                                return self.currentLangs().indexOf(v.lang) > -1;
                            })
                        })
                    })
                })

                self.fields(fieldItem.map(function (f) {
                    return new Field(f);
                }))
            })

            // category
            this._categories = params.categories || ko.observableArray();
            this._categories.subscribe(function (categories) {
                if (categories && categories.length) {
                    self.categories(categories.map(function (c) {
                        var newCate = new Category(c);
                        newCate.on('dialog/show', function (ctx) {
                            self.currentCategory(ctx);
                            self.showCategoryDialog(true);
                        })
                        newCate.on('category/choosed', function (choosed) {
                            self.choosedCategory(choosed);
                        })
                        return newCate;
                    }))
                } else {
                    self.categories([]);
                }
            })
            this.currentCategory = ko.observable();
            this.choosedCategory = ko.observableArray([]);
            this.showCategoryDialog = ko.observable(false);
            this.onSaveCategory = function () {
                self.currentCategory().contents(self.choosedCategory());
                self.onHideCategoryDialog();
            };
            this.onHideCategoryDialog = function () {
                $.jstree.reference("#categoryTree").destroy()
                self.showCategoryDialog(false);
                self.currentCategory(null);
            }

            // embedded
            this.embedded = params.embedded || ko.observableArray();
            this.currentEmbedded = ko.observable();
            this.addEmbedded = function (choosedEmbedded) {
                self.currentEmbedded(choosedEmbedded);
                $("#embeddedDialog").modal("show")
            }

            this.values = params.values;

            this.validationPassed = params.validationPassed;

            this.startValidating = params.startValidating;
            this.startValidating.subscribe(function (start) {
                if (start) {
                    var allValid = true;
                    self.fields().forEach(function (f) {
                        f.showError(false);
                        if (!f.isValid()) {
                            allValid = false;
                            f.showError(true);
                        }
                    })
                    self.validationPassed(allValid);

                    if (allValid) {
                        var values = {},
                            // showingField = _.filter(self.fields(), function(f) {
                            //     return f.isShow();
                            // }),
                            groups = _.groupBy(self.fields(), function (f) {
                                return f.lang();
                            });

                        var langs = Object.keys(groups);
                        langs.forEach(function (key) {
                            values[key] = {};
                            groups[key].forEach(function (v) {
                                if (v.isMultipleValue() || $.isArray(v.fieldValue())) {
                                    v.fieldValue(JSON.stringify(v.fieldValue()));
                                }
                                values[key][v.name()] = v.fieldValue();
                            })
                        })

                        var categories = {};
                        self.categories().forEach(function (cate) {
                            categories[cate.categoryFolder.id] = cate.contents().map(function (c) { return c.id });
                        })
                        var embedded = {};
                        self.embedded().forEach(function (emb) {
                            embedded[emb.embeddedFolder.id] = emb.contents.map(function (e) { return e.id });
                        })

                        self.values({
                            fieldsValue: values,
                            categories: categories,
                            embedded: embedded
                        });
                    }
                }
            })

            Kooboo.EventBus.subscribe("kb/multilang/change", function (lang) {
                if (lang.selected) {
                    self.currentLangs.push(lang.name);
                } else {
                    self.currentLangs.remove(lang.name);
                }
            })

            Kooboo.EventBus.subscribe("kb/textContent/embedded/edit", function (choosedEmbedded) {
                var embeddedFolderId = choosedEmbedded.embeddedFolder.id;
                var index = _.findIndex(self.embedded(), function (o) {
                    return o.embeddedFolder.id === embeddedFolderId;
                })

                self.embedded.splice(index, 1);
                self.embedded.splice(index, 0, choosedEmbedded);
            })

        },
        template: template
    })

    function Field(data) {

        this._id = ko.observable(Kooboo.getRandomId());

        this.showError = ko.observable(false);

        this.isShow = data.isShow;

        this.name = ko.observable(data.name);

        this.fieldName = ko.observable(data.displayName);

        this.fieldValue = ko.observable(data.fieldValue);

        this.lang = ko.observable(data.lang);

        this.controlType = ko.observable(data.controlType);

        this.disabled = ko.observable(data.disabled);

        this.tooltip = ko.observable(data.tooltip);

        this.options = ko.observableArray(JSON.parse(data.options || data.selectionOptions || '[]'));

        this.isMultilingual = ko.observable(data.isMultilingual && data.isMultilingualSite);

        this.isMultipleValue = ko.observable(data.multipleValue);

        var _validations = JSON.parse(data.validations || '[]') || [],
            validateRules = {};

        this.validations = ko.observableArray(_validations);
        for (var i = 0, len = _validations.length; i < len; i++) {
            var rule = _validations[i],
                type = (rule.type || rule.validateType),
                msg = (rule.msg || rule.errorMessage);

            switch (type) {
                case 'required':
                    validateRules[type] = msg || Kooboo.text.validation.required;
                    break;
                case 'regex':
                    validateRules[type] = {
                        pattern: new RegExp(rule.pattern),
                        message: msg || Kooboo.text.validation.inputError
                    };
                    break;
                case 'range':
                    validateRules[type] = {
                        from: Number(rule.min),
                        to: Number(rule.max),
                        message: msg || Kooboo.text.validation.inputError
                    };
                    break;
                case 'stringLength':
                    validateRules['stringlength'] = {
                        min: parseInt(rule.min),
                        max: parseInt(rule.max),
                        message: msg || Kooboo.text.validation.inputError
                    };
                    break;
                case 'min':
                case 'max':
                case 'minLength':
                case 'maxLength':
                case 'minChecked':
                case 'maxChecked':
                    validateRules[type] = {
                        value: Number(rule.value),
                        message: msg || Kooboo.text.validation.inputError
                    };
                    break;

            }
        }
        this.fieldValue.extend({ validate: validateRules })
        this.validateRules = validateRules;

        this.isValid = function () {
            return this.fieldValue.isValid();
        }
    }

    function Category(opt) {
        var _this = this;
        this.alias = opt.alias;
        this.contents = ko.observableArray(opt.contents);
        this.display = opt.alias || opt.display;
        this.multipleChoice = opt.multipleChoice;
        this.categoryFolder = opt.categoryFolder;

        this.onShowCategoryDialog = function () {
            Kooboo.TextContent.getByFolder({
                folderId: this.categoryFolder.id,
                pageSize: 99999
            }).then(function (res) {
                _this.emit('dialog/show', _this);
                var list = categoryMapping(res.model["list"]);
                var choosed = _.cloneDeep(_this.contents());
                var jsTreeData = list.map(function (o) {
                    var selected = choosed.some(function (choosed) {
                        return choosed.id === o.id;
                    })
                    o.state = {};
                    o.state.selected = selected;
                    return o;
                })
                $("#categoryTree").jstree({
                    'plugins': ['types', 'checkbox'],
                    'types': {
                        'default': {
                            icon: 'fa fa-file icon-state-warning'
                        }
                    },
                    'core': {
                        'strings': { 'Loading ...': Kooboo.text.common.loading + ' ...' },
                        'data': jsTreeData,
                        "multiple": _this.multipleChoice
                    }
                }).on("changed.jstree", function (e, data) {
                    var selectedCategoryId = data.selected;
                    var choosedCate = list.filter(function (o) {
                        return selectedCategoryId.indexOf(o.id) > -1;
                    })
                    _this.emit('category/choosed', choosedCate);
                });
            });
        }

        this.remove = function (item) {
            _this.contents.remove(item);
        }

        this.events = {};
        this.emit = function (event, data) {
            if (!this.events[event] || this.events[event].length == 0) return;
            this.events[event].forEach(function (fn) {
                fn(data);
            })
        }
        this.on = function (event, fn) {
            if (!this.events[event]) this.events[event] = [];
            this.events[event].push(fn);
        }

        function categoryMapping(list) {
            list.forEach(function (item) {
                item.text = item.values[Object.keys(list[0].values)[0]];
            })
            return list;
        }
    }
})()