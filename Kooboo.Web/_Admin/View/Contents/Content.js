$(function() {
    var CACHE_STORAGE_KEY = "KB_CONTENT_GRIPPER_STATUS";

    var FOLDER_ID = Kooboo.getQueryString("folder"),
        CONTENT_ID = Kooboo.getQueryString("id"),
        LANG = Kooboo.getQueryString("lang");

    if (!FOLDER_ID && CONTENT_ID) {
        FOLDER_ID = Kooboo.TextContent.getFolderId({ id: CONTENT_ID });
    }

    var ContentMode = {
        NORMAL: 'normal',
        CONTENT_TYPE_REQUIRED: 'contentTypeRequired',
        FIELD_REQUIRED: 'fieldRequired',
        FOLDER_REQUIRED: 'folderRequired'
    };

    var TextContent = function() {
        var self = this;
        this.contentId = ko.observable(CONTENT_ID || Kooboo.Guid.Empty);
        this.folderId = ko.observable(FOLDER_ID || Kooboo.Guid.Empty);

        this.contentMode = ko.observable(); // normal, contentTypeRequired, fieldRequired, folderRequired

        this.isNewContent = ko.observable(!CONTENT_ID);
        this.hasFolder = ko.observable(!!FOLDER_ID);
        this.isMultiContent = ko.observable(!!LANG);

        this.contentTypes = ko.observableArray();
        this.contentTypeModel = ko.observable();
        this.contentType = ko.observable();
        this._contentType = ko.observable();
        if (!self.hasFolder()) {
            this._contentType.subscribe(function(typeId) {
                Kooboo.ContentType.Get({
                    id: typeId
                }).then(function(res) {
                    if (res.success) {
                        self.contentTypeModel(res.model);
                        self.typeProperties.removeAll();
                        self.typeProperties(res.model.properties.map(function(o) {
                            return ko.mapping.fromJS(o);
                        }))
                        self.changeAvaliableFolders();
                    }
                })
            })
        }

        this.showError = ko.observable(false);

        this.mediaDialogData = ko.observable();
        this.fields = ko.observableArray();
        this.siteLangs = ko.observable();
        this.categories = ko.observableArray();
        this.embedded = ko.observableArray();
        this.choosedEmbedded = ko.observable({});

        this.allowModify = ko.observable(false);
        this.onAllowModify = function() {
            self.allowModify(true);
        }

        this.allFolders = ko.observableArray();
        this.avaliableFolders = ko.observableArray();

        this.choosedFolder = ko.observable();
        this.choosedFolderId = ko.observable();

        this.changeAvaliableFolders = function() {
            self.avaliableFolders.removeAll();
            self.choosedFolderId(null);

            var list = _.filter(self.allFolders(), function(folder) {
                return folder.contentTypeId == self._contentType();
            })
            self.avaliableFolders(list);
            if (!list.length) {
                self.contentMode(ContentMode.FOLDER_REQUIRED);
                self.choosedFolder(null);
                self.choosedFolderId(null);
            } else {
                self.getContentFields();
            }
        }

        this.choosedFolderChange = function(m, e) {
            e.preventDefault();
            self.refreshContent();
        }

        this.typeProperties = ko.observableArray();
        this.editProperty = function(m, e) {
            self.isNewField(false);
            self.fieldData(ko.mapping.toJS(m));
            self.onFieldModalShow(true);
        }

        this.deleteProperty = function(m, e) {
            if (confirm(Kooboo.text.confirm.deleteItem)) {
                self.typeProperties.remove(m);
                self.saveContentFields(function() {
                    self.refreshContent();
                });
            }
        }

        this.refreshContent = function() {
            var tempSaveData = self.getSaveTextContent();

            var params;
            if (self.choosedFolderId()) {
                if (self.isNewContent()) {
                    params = {
                        folderId: self.choosedFolderId()
                    }
                } else {
                    params = {
                        folderId: self.choosedFolderId(),
                        id: self.contentId()
                    }
                }
            }

            if (params) {
                Kooboo.TextContent.getEdit(params).then(function(res) {
                    if (res.success) {
                        var data = res.model;
                        data.properties.forEach(function(prop) {
                            if (prop.name == "Online") {
                                var langs = Object.keys(prop.values);
                                langs.forEach(function(lang) {
                                    prop.values[lang] = tempSaveData.Online || prop.values[lang];
                                })

                                // FOR TEST ONLY
                                prop.controlType = "Boolean";
                            }
                        })

                        var langs = Object.keys(tempSaveData.values);
                        langs.forEach(function(lang) {
                            var keys = Object.keys(tempSaveData.values[lang]);
                            keys.forEach(function(key) {
                                data.properties.forEach(function(prop) {
                                    if (prop.name == key) {
                                        prop.values[lang] = tempSaveData.values[lang][key];
                                    }
                                })
                            })
                        })

                        self.fields(data.properties);
                        self.categories(data.categories || []);
                        self.embedded(data.embedded || []);
                        if (self.fields().length <= 1) {
                            self.allowModify(true);
                            self.contentMode(ContentMode.FIELD_REQUIRED);
                        } else {
                            self.contentMode(ContentMode.NORMAL);
                        }
                    }
                })
            }
        }

        this.contentTypeChanged = function(typeId, m, e) {
            if (e.originalEvent) {
                if (confirm(Kooboo.text.site.textContent.changeContentTypeConfirm)) {
                    self._contentType(typeId);
                } else {
                    self.contentType(self._contentType());
                }
            }
        }

        this.allFieldsShow = ko.observable(false);

        this.toggleAllFields = function() {
            self.allFieldsShow(!self.allFieldsShow());
        }

        this.onCreateField = function() {
            self.isNewField(true);
            self.fieldData({});
            self.onFieldModalShow(true);
        }

        this.onFieldModalShow = ko.observable(false);

        this.fieldData = ko.observable();

        this.isNewField = ko.observable();

        this.onFieldSave = function(fm) {
            var _properties = _.cloneDeep(self.typeProperties()),
                idx = _.findIndex(_properties, function(p) {
                    return p.name() == (self.isNewField() ? "Online" : fm.name)
                });

            if (idx > -1) {
                _properties.splice(idx, self.isNewField() ? 0 : 1, ko.mapping.fromJS(fm));
                self.typeProperties(_properties);
            }

            self.saveContentFields(function() {
                self.refreshContent();
            });
        }

        this.saveContentFields = function(cb) {
            var properties = self.typeProperties().map(function(p) {
                return ko.mapping.toJS(p);
            });

            var name = self.contentTypes().find(function(ct) {
                return ct.id == self.contentType();
            }).name;

            Kooboo.ContentType.save({
                id: self.contentType(),
                name: name,
                properties: properties
            }).then(function(res) {
                if (res.success) {
                    window.info.done(Kooboo.text.info.save.success);
                    if (cb && typeof cb == "function") { cb(); }
                }
            })
        }

        this.getFieldNames = function() {
            return self.typeProperties().map(function(f) {
                return f.name() == "UserKey" ? "" : f.name();
            })
        }

        // New folder
        this.showFolderModal = ko.observable(false);
        this.newFolderName = ko.validateField({
            required: "",
            regex: {
                pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                message: Kooboo.text.validation.objectNameRegex
            }
        });
        this.newFolderDisplayName = ko.observable();
        this.newFolderContentTypeId = ko.validateField({
            required: ''
        });
        this.onCreateFolder = function() {
            self.showFolderModal(true);
            self.newFolderContentTypeId(self._contentType());
        }
        this.onHideFolderModal = function() {
            self.showFolderModal(false);
            self.newFolderName("");
            self.showError(false);
        }
        this.isNewFolderValid = function() {
            return self.newFolderName.isValid() &&
                self.newFolderContentTypeId.isValid();
        }
        this.onCreateNewFolder = function() {
            if (self.isNewFolderValid()) {
                Kooboo.ContentFolder.post({
                    id: Kooboo.Guid.Empty,
                    name: self.newFolderName(),
                    displayName: self.newFolderDisplayName(),
                    contentTypeId: self.newFolderContentTypeId(),
                    embedded: [],
                    category: []
                }).then(function(res) {
                    if (res.success) {
                        Kooboo.ContentFolder.getList().then(function(r) {
                            if (r.success) {
                                var folders = _.sortBy(r.model, [function(o) { return o.creationDate }]);
                                self.allFolders.removeAll();
                                self.allFolders(folders.reverse());
                                self.changeAvaliableFolders();
                                self.getContentFields();
                                self.onHideFolderModal();
                            }
                        })
                    }
                })
            } else {
                self.showError(true);
            }
        }

        this.getContentFields = function() {
            var params = { folderId: FOLDER_ID || self.choosedFolderId() };
            if (CONTENT_ID) {
                params.id = CONTENT_ID
            }
            Kooboo.TextContent.getEdit(params).then(function(res) {
                if (res.success) {
                    var data = res.model;
                    // TEST ONLY
                    data.properties.forEach(function(prop) {
                        if (prop.name == "Online") {
                            prop.controlType = "Boolean"
                        }
                    })
                    self.contentMode((data.properties.length > 1) ? ContentMode.NORMAL : ContentMode.FIELD_REQUIRED);

                    self.fields(data.properties);
                    self.categories(data.categories || []);
                    self.embedded(data.embedded || [])
                    if (self.fields().length <= 1) {
                        self.allowModify(true);
                    }
                }
            })
        }

        // Content Type
        this.showContentTypeModal = ko.observable(false);

        this.createContentType = function() {
            self.showContentTypeModal(true);
        }

        this.onContentTypeModalHide = function() {
            self.showError(false);
            self.contentTypeName("");
            self.showContentTypeModal(false);
        };

        this.contentTypeName = ko.validateField({
            required: Kooboo.text.validation.required,
            stringlength: {
                min: 1,
                max: 64,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 64
            },
            remote: {
                url: Kooboo.ContentType.isUniqueName(),
                data: {
                    name: function() {
                        return self.contentTypeName()
                    }
                },
                message: Kooboo.text.validation.taken
            }
        });

        this.isAbleToCreateType = function() {
            return self.contentTypeName.isValid();
        }

        this.onCreateNewContentType = function() {
            if (self.isAbleToCreateType()) {
                Kooboo.ContentType.get({
                    id: Kooboo.Guid.Empty
                }).then(function(res) {
                    if (res.success) {
                        var data = res.model;
                        data.id = Kooboo.Guid.Empty;
                        data.name = self.contentTypeName();
                        Kooboo.ContentType.save(data).then(function(re) {
                            if (re.success) {
                                window.info.done(Kooboo.text.info.save.success);
                                refreshSidebar();
                                self.refreshContent();
                                self.allowModify(true);
                            } else {
                                window.info.done(Kooboo.text.info.save.fail);
                            }
                            self.onContentTypeModalHide();
                        })
                    }
                })
            } else {
                self.showError(true);
            }
        };

        this.getSaveTextContent = function() {
            return {
                id: self.contentId(),
                folderId: self.choosedFolderId(),
                values: self.contentValues().fieldsValue || {},
                categories: self.contentValues().categories || {},
                embedded: self.contentValues().embedded || {}
            };
        }

        this.isAbleToSaveTextContent = function() {
            if (!self.choosedFolderId()) {
                return false;
            } else {
                var flag = true;
                this.startValidating(true);
                if (this.validationPassed()) {
                    flag = this.fields().length > 1;
                } else {
                    flag = false;
                }
                this.startValidating(false);
                return flag;
            }
        }

        this.startValidating = ko.observable(false);

        this.validationPassed = ko.observable(true);

        this.contentValues = ko.observable({});

        this.onSubmit = function(cb) {
            if (self.isAbleToSaveTextContent()) {
                Kooboo.TextContent.langupdate(self.getSaveTextContent()).then(function(res) {
                    if (res.success) {
                        if (cb && typeof cb == "function") {
                            cb(res.model);
                        }
                    }
                })
            } else {
                if (!self.contentTypes().length) {
                    window.info.fail(Kooboo.text.site.textContent.createTypeFieldHint);
                } else if (!self.choosedFolderId()) {
                    window.info.fail(Kooboo.text.site.textContent.createFolderHint);
                } else if (self.fields().length == 1) {
                    window.info.fail(Kooboo.text.site.textContent.saveWidthNoFieldHint);
                }
            }
        }

        this.onContentSave = function() {
            self.onSubmit(function(id) {
                location.href = Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, _.assign({
                    folder: self.choosedFolderId(),
                    id: id
                }, LANG ? { lang: LANG } : {}))
            })
        }

        this.onContentSaveAndCreate = function() {
            self.onSubmit(function() {
                window.info.done(Kooboo.text.info.save.success);
                setTimeout(function() {
                    location.href = Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage, {
                        folder: self.choosedFolderId()
                    })
                }, 300);
            })
        }

        this.onContentSaveAndReturn = function() {
            self.onSubmit(function() {
                location.href = self.isMultiContent() ? Kooboo.Route.Get(Kooboo.Route.TextContent.ByLangFolder, {
                    folder: self.choosedFolderId(),
                    lang: LANG
                }) : Kooboo.Route.Get(Kooboo.Route.TextContent.ByFolder, {
                    folder: self.choosedFolderId()
                });
            })
        }

        this.userCancel = function() {
            if (self.hasFolder()) {
                location.href = self.isMultiContent() ? Kooboo.Route.Get(Kooboo.Route.TextContent.ByLangFolder, {
                    folder: self.choosedFolderId(),
                    lang: LANG
                }) : Kooboo.Route.Get(Kooboo.Route.TextContent.ByFolder, {
                    folder: self.choosedFolderId()
                });
            } else {
                location.href = Kooboo.Route.Get(Kooboo.Route.TextContent.ListPage);
            }
        }

        Kooboo.EventBus.subscribe("ko/style/list/pickimage/show", function(ctx) {
            Kooboo.Media.getList().then(function(res) {
                if (res.success) {
                    res.model["show"] = true;
                    res.model["context"] = ctx;
                    res.model["onAdd"] = function(selected) {
                        ctx.settings.file_browser_callback(ctx.field_name, selected.url + "?SiteId=" + Kooboo.getQueryString("SiteId"), ctx.type, ctx.win, true);
                    }
                    self.mediaDialogData(res.model);
                }
            });
        });

        Kooboo.EventBus.subscribe("ko/binding/sorted", function() {
            self.saveContentFields(function() {
                self.refreshContent();
            });
        })


        refreshSidebar(true);

        function refreshSidebar(initLoad) {
            $.when(Kooboo.ContentType.getList(),
                Kooboo.ContentFolder.getList(),
                Kooboo.Site.Langs()).then(function(r1, r2, r3) {
                var typeRes = r1[0],
                    folderRes = r2[0],
                    langRes = r3[0];

                if (typeRes.success && folderRes.success && langRes.success) {
                    URLFolderCheck(folderRes.model);

                    if (folderRes.model.length == 0) {
                        self.contentMode(ContentMode.FOLDER_REQUIRED);
                    } else {
                        var folders = _.sortBy(folderRes.model, [function(o) { return o.creationDate }]);
                        self.allFolders.removeAll();
                        self.allFolders(folders.reverse());
                    }

                    if (typeRes.model.length == 0) {
                        self.contentMode(ContentMode.CONTENT_TYPE_REQUIRED);
                    } else {
                        var types = _.sortBy(typeRes.model, [function(o) { return o.lastModified }]);
                        self.contentTypes.removeAll();
                        self.contentTypes(types.reverse());
                    }

                    self.siteLangs(langRes.model);

                    if (self.hasFolder()) {
                        var folder = _.find(self.allFolders(), function(f) {
                            return f.id == self.folderId();
                        })
                        if (folder) {
                            self.choosedFolder(folder);
                            self.choosedFolderId(folder.id);
                            self.contentType(folder.contentTypeId);
                            self._contentType(folder.contentTypeId);
                            Kooboo.ContentType.Get({
                                id: folder.contentTypeId
                            }).then(function(res) {
                                if (res.success) {
                                    self.contentTypeModel(res.model);
                                    self.typeProperties.removeAll();
                                    self.typeProperties(res.model.properties.map(function(o) {
                                        return ko.mapping.fromJS(o);
                                    }))
                                }
                            })
                        }
                    }

                    if (initLoad) {
                        Kooboo.EventBus.publish("content/ready");
                    }
                    self.refreshContent();
                }
            })

            function URLFolderCheck(folders) {

                if (FOLDER_ID) {
                    var selectedFolder = _.find(folders, function(folder) {
                        return folder.id == FOLDER_ID;
                    })

                    if (!selectedFolder) {
                        location.href = Kooboo.Route.TextContent.DetailPage;
                    }
                }
            }
        }

        var cachedFlag = localStorage.getItem(CACHE_STORAGE_KEY);
        if (cachedFlag == null) {
            cachedFlag = false;
        } else {
            cachedFlag = JSON.parse(cachedFlag);
        }
        this.isPanelHidden = ko.observable(cachedFlag || false);

        this.togglePanel = function() {
            var flag = this.isPanelHidden();
            this.isPanelHidden(!flag);
            localStorage.setItem(CACHE_STORAGE_KEY, JSON.stringify(this.isPanelHidden()));
        }
    }

    var vm = new TextContent();

    Kooboo.EventBus.subscribe("content/ready", function() {
        ko.applyBindings(vm, document.getElementById("main"));
    })
})