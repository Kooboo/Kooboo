$(function() {

    var Product = function() {
        var self = this;

        var SITE_ID_QUERY_STRING = "?SiteId=" + Kooboo.getQueryString("SiteId");

        var typeId = Kooboo.getQueryString('type') || Kooboo.Guid.Empty;
        var initTimes = 0;

        this.productId = ko.observable(Kooboo.getQueryString('id') || Kooboo.Guid.Empty);
        this.isNew = ko.pureComputed(function() {
            return self.productId() == Kooboo.Guid.Empty;
        })
        this.fields = ko.observableArray();
        this.startValidating = ko.observable(false);
        this.validationPassed = ko.observable(false);
        this.contentValues = ko.observable();
        this.siteLangs = ko.observable();
        this.categories = ko.observableArray();
        this.categories.subscribe(function(cates) {
            var selected = getSelected(cates).map(function(c) {
                return ko.mapping.toJS(c);
            });
            self.selectedCategories(selected);

            function getSelected(cates) {
                var temp = [];
                cates.forEach(function(c) {
                    if (c.selected()) {
                        temp.push(c);
                    }

                    if (c.subCats() && c.subCats().length) {
                        temp = _.concat(temp, getSelected(c.subCats()));
                    }
                })
                return temp
            }
        })
        this.selectedCategories = ko.observableArray();

        this.multipleMedia = ko.observable(false);
        this.mediaDialogData = ko.observable();

        this.variants = ko.observableArray([]);

        this.specFields = ko.observableArray();
        this.specFields.subscribe(function(fields) {
            if (fields.length) {
                renderTable();
            }
        })
        this.dynamicFieldsChange = function(fields) {
            self.dynamicSpecFields(fields);
            fields.forEach(function(f) {
                if (f.options().length) {
                    if (self.specNames().indexOf(f.name()) == -1) {
                        self.specNames.push(f.name());
                    }
                } else {
                    self.specNames.remove(f.name());
                }
            })

            renderTable();
        }

        this.removeSkuPic = function(m) {
            m.skuImage('');
            m.skuThumbnail();
        }
        this.selectSkuPic = function(m) {
            self.multipleMedia(false);
            Kooboo.Media.getList().then(function(res) {
                if (res.success) {
                    res.model["show"] = true;
                    res.model["context"] = m;
                    res.model["onAdd"] = function(selected) {
                        m.skuImage(selected.url);
                        m.skuThumbnail(selected.thumbnail);
                    }
                    self.mediaDialogData(res.model);
                }
            });
        }
        this.selectImages = function(m) {
            self.multipleMedia(true);
            Kooboo.Media.getList().then(function(res) {
                if (res.success) {
                    res.model["show"] = true;
                    res.model["context"] = m;
                    res.model["onAdd"] = function(selected) {
                        m.images(selected.map(function(s) {
                            return {
                                url: s.url,
                                thumbnail: s.thumbnail
                            }
                        }))
                    }
                    self.mediaDialogData(res.model);
                }
            });
        }
        this.removeImg = function(m, data) {
            m.images.remove(data);
        }

        this.specNames = ko.observableArray();
        this.fixedSpecFields = ko.observableArray();
        this.dynamicSpecFields = ko.observableArray();

        this.typesMatrix = ko.observableArray();
        this.singleProduct = {
            showError: ko.observable(false),
            stock: ko.validateField({
                required: '',
                min: { value: 0 }
            }),
            price: ko.validateField({
                required: '',
                min: { value: 0 }
            }),
            sku: ko.observable(),
            skuImage: ko.observable(),
            skuThumbnail: ko.observable(),


        }

        this.onSaveAndReturn = function() {
            self.onSave(function() {
                location.href = Kooboo.Route.Product.ListPage
            })
        }

        this.onSave = function(cb) {
            if (self.isValid()) {
                self.startValidating(false);

                var variants = self.typesMatrix().map(function(row) {
                        var specs = {};
                        if (row.types && row.types.length) {
                            row.types.forEach(function(t) {
                                specs[t.name] = t.value;
                            })
                        }

                        return {
                            variants: specs,
                            stock: row.stock(),
                            price: row.price(),
                            sku: row.sku(),
                            thumbnail: row.skuImage(),
                            images: row.images().map(function(img) { return img.url }),
                            online: row.online()
                        }
                    }),
                    categories = self.selectedCategories().map(function(cate) {
                        return cate.id;
                    })

                Kooboo.Product.post({
                    id: self.productId(),
                    type: typeId,
                    values: self.contentValues().fieldsValue,
                    variants: variants,
                    categories: categories
                }).then(function(res) {
                    if (res.success) {

                        if (cb && typeof cb == 'function') {
                            cb();
                        } else {
                            location.href = Kooboo.Route.Get(Kooboo.Route.Product.DetailPage, {
                                id: res.model,
                                type: typeId
                            })
                        }
                    }
                })
            } else {
                self.typesMatrix().forEach(function(row) {
                    if (!row.isValid()) row.showError(true);
                })
            }
        }

        this.typesUrl = Kooboo.Route.Product.ListPage;

        this.isValid = function() {
            self.startValidating(true);
            var flag = true;
            this.typesMatrix().forEach(function(row) {
                if (!row.isValid()) flag = false;
            })

            return flag && self.validationPassed();
        }

        this.getCategories = function(cates, selectedIds) {
            var temp = [];
            cates.forEach(function(c) {
                if (selectedIds.indexOf(c.id) > -1) {
                    c.selected = true;
                }

                if (c.subCats && c.subCats.length) {
                    c.subCats = self.getCategories(c.subCats, selectedIds);
                }

                temp.push(new Category(c));
            })

            return temp;
        }

        function Category(cate) {
            cate.selected = ko.observable(cate.selected || false);
            ko.mapping.fromJS(cate, {}, this);
        }

        $.when(
            Kooboo.Site.Langs(),
            Kooboo.Product.getEdit({
                id: self.productId(),
                productTypeId: typeId
            }),
            Kooboo.ProductCategory.getList()
        ).then(function(r1, r2, r3) {
            var langRes = r1[0],
                productRes = r2[0],
                cateRes = r3[0];

            if (langRes.success && productRes.success && cateRes.success) {

                self.siteLangs(langRes.model);
                self.categories(self.getCategories(cateRes.model, productRes.model.categories ? productRes.model.categories.map(function(c) {
                    return c.categoryId;
                }) : []));

                self.variants(productRes.model.variants || []);

                var normalFields = [],
                    fixedSpecFields = [],
                    dynaSpecFields = [];

                productRes.model.properties.forEach(function(p) {
                    switch (p.controlType.toLowerCase()) {
                        case 'fixedspec':
                            self.specNames.push(p.name);
                            fixedSpecFields.push(p);
                            break;
                        case 'dynamicspec':
                            dynaSpecFields.push(p);
                            break;
                        default:
                            normalFields.push(p);
                            break;
                    }
                })

                if (productRes.model.variants && productRes.model.variants.length) {
                    var specs = [];
                    productRes.model.variants.forEach(function(va) {
                        var keys = va.variants ? Object.keys(va.variants) : [];
                        keys.forEach(function(key) {
                            if (self.specNames().indexOf(key) == -1) {
                                var idx = specs.length ? _.findIndex(specs, function(s) {
                                    return s.name == key && s.value == va.variants[key];
                                }) : -1;
                                if (idx == -1) {
                                    specs.push({
                                        name: key,
                                        value: va.variants[key]
                                    });
                                }
                            }
                        })
                    })

                    var dynaGroups = _.groupBy(specs, function(o) { return o.name }),
                        keys = Object.keys(dynaGroups);

                    keys.forEach(function(key) {
                        var dyna = _.find(dynaSpecFields, function(f) {
                            return f.name == key;
                        })

                        dyna && (dyna.selectionOptions = dynaGroups[key].map(function(v) { return v.value }));
                    })
                }

                self.fields(normalFields);
                self.fixedSpecFields(fixedSpecFields);
                self.specFields(_.concat(fixedSpecFields, dynaSpecFields));

                if (!self.specFields().length) {
                    renderTable();
                }
            }
        })

        function renderTable() {
            if (self.variants().length) {
                if (initTimes < 2) {
                    self.typesMatrix(self.variants().map(function(vari) {
                        var types = [];
                        self.specNames().forEach(function(name) {
                            types.push({ name: name, value: vari.variants[name] });
                        })

                        var images = [];
                        if (vari.images && vari.images.length) {
                            images = vari.images.map(function(img) {
                                return {
                                    url: img,
                                    thumbnail: "/_thumbnail/80/80" + img + SITE_ID_QUERY_STRING
                                }
                            })
                        }

                        return {
                            types: types,
                            showError: ko.observable(false),
                            stock: ko.validateField(vari.stock, {
                                required: '',
                                min: { value: 0 }
                            }),
                            price: ko.validateField(vari.price, {
                                required: '',
                                min: { value: 0 }
                            }),
                            sku: ko.observable(vari.sku),
                            skuImage: ko.observable(vari.thumbnail),
                            skuThumbnail: ko.observable("/_thumbnail/80/80" + vari.thumbnail + SITE_ID_QUERY_STRING),
                            images: ko.observableArray(images),
                            online: ko.observable(vari.online),
                            isValid: function() {
                                return this.stock.isValid() && this.price.isValid();
                            }
                        }
                    }))

                    initTimes++;
                } else {
                    var types = [];
                    self.fixedSpecFields().forEach(function(f) {
                        var options = JSON.parse(f.selectionOptions).map(function(opt) {
                            return {
                                name: f.name,
                                value: opt.key
                            };
                        })

                        types.push(options);
                    });

                    self.dynamicSpecFields().forEach(function(f) {
                        if (f.options().length) {
                            var options = f.options().map(function(opt) {
                                return {
                                    name: f.name(),
                                    value: opt
                                }
                            })
                            types.push(options);
                        }
                    })

                    var matrix = getTableDataByTypes(types);
                    self.typesMatrix(matrix.map(function(m) {
                        var find = _.find(self.typesMatrix(), function(row) {
                            return getValue(row.types) == getValue(m);

                            function getValue(list) {
                                return list.map(function(item) {
                                    return item.value
                                }).join(',');
                            }
                        })

                        return find || {
                            types: m,
                            showError: ko.observable(false),
                            stock: ko.validateField({
                                required: '',
                                min: { value: 0 }
                            }),
                            price: ko.validateField({
                                required: '',
                                min: { value: 0 }
                            }),
                            sku: ko.observable(),
                            skuImage: ko.observable(),
                            skuThumbnail: ko.observable(),
                            images: ko.observableArray(),
                            online: ko.observable(true),
                            isValid: function() {
                                return this.stock.isValid() && this.price.isValid();
                            }
                        }
                    }))
                }
            } else {
                self.typesMatrix([{
                    showError: ko.observable(false),
                    stock: ko.validateField({
                        required: '',
                        min: { value: 0 }
                    }),
                    price: ko.validateField({
                        required: '',
                        min: { value: 0 }
                    }),
                    sku: ko.observable(),
                    skuImage: ko.observable(),
                    skuThumbnail: ko.observable(),
                    images: ko.observableArray(),
                    online: ko.observable(true),
                    isValid: function() {
                        return this.stock.isValid() && this.price.isValid();
                    }
                }])
            }
        }

        this.toggleStatus = function(m, e) {
            m.online(!m.online());
        }

        this.showCategoriesModal = ko.observable(false);
        this.onShowCategoriesModal = function() {
            self.showCategoriesModal(true)
        }
        this.onHideCategoriesModal = function() {
            self.showCategoriesModal(false);
        }
        this.onSaveCategoriesModal = function() {
            self.selectedCategories(getSelected(self.categories()));
            self.onHideCategoriesModal();

            function getSelected(cates) {
                var temp = [];
                cates.forEach(function(c) {
                    if (c.selected()) {
                        temp.push(ko.mapping.toJS(c));
                    }

                    if (c.subCats() && c.subCats().length) {
                        temp = _.concat(temp, getSelected(c.subCats()));
                    }
                })
                return temp;
            }
        }

    }

    var vm = new Product();

    ko.applyBindings(vm, document.getElementById('main'));

    function getTableDataByTypes() {
        return Array.prototype.reduce.call(arguments[0], function(a, b) {
            var ret = [];
            a.forEach(function(a) {
                b.forEach(function(b) {
                    ret.push(a.concat([b]));
                });
            });
            return ret;
        }, [
            []
        ]);
    }
})