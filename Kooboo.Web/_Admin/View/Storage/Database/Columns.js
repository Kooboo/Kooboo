$(function() {
    var USED_NAMES = [];

    var columnsModel = function() {

        var self = this;

        this.name = ko.observable();

        this.from = ko.observable(Kooboo.getQueryString("from"));

        this.onFieldModalShow = ko.observable(false);
        this.onFieldSave = function() {}
        this.fieldData = ko.observable();

        this.columns = ko.observableArray();
        this.columns.subscribe(function(cols) {
            USED_NAMES = cols.map(function(col) {
                return col.name();
            })
        })

        this.showColumnModal = ko.observable(false);

        this.isNewColumn = ko.observable(false);
        this.editingColumn = ko.observable();

        this.addNewColumn = function(m) {
            if (m) {
                self.isNewColumn(false);
                self.editingColumn(m);
            } else {
                self.isNewColumn(true);
                self.editingColumn(new Column());
            }

            self.showColumnModal(true);
        }

        this.onSaveColumn = function() {
            if (self.isNewColumn()) {
                self.columns.push(self.editingColumn());
            }

            if (self.editingColumn().isPrimaryKey()) {
                self.columns().forEach(function(col) {
                    col.isPrimaryKey(false);
                })
                self.editingColumn().isPrimaryKey(true);
            }
        }

        this.removeColumn = function(m) {
            if (confirm(Kooboo.text.confirm.deleteItem)) {
                self.columns.remove(m);
            }
        }

        this.returnUrl = ko.pureComputed(function() {
            if (self.from()) {
                return Kooboo.Route.Get(Kooboo.Route.Database.DataPage, {
                    table: self.name()
                })
            } else {
                return Kooboo.Route.Database.TablePage;
            }
        })

        this.onSave = function() {
            var columns = ko.mapping.toJS(self.columns());
            columns.forEach(function(col) {

                for (var key in col) {
                    if (typeof col[key] == 'function') {
                        delete col[key];
                    }
                }

                delete col.controlTypeName;
                delete col.isAbleToAddOption;

                if (col.options && col.options.length) {
                    col.options.forEach(function(opt) {
                        delete opt.showError;
                        delete opt.isValid;
                    })
                }

                var setting = {
                    options: col.options,
                    validations: col.validations
                }

                col.setting = JSON.stringify(setting);

                delete col.options;
                delete col.validations;
            })
            console.log(columns);

            Kooboo.Database.updateColumn({
                tableName: self.name(),
                columns: columns
            }).then(function(res) {
                if (res.success) {
                    location.href = self.returnUrl();
                }
            })
        }

        var name = Kooboo.getQueryString("table");

        if (name) {
            self.name(name);
            Kooboo.Database.getColumns({
                table: name
            }).then(function(res) {
                if (res.success) {
                    var cols = res.model.map(function(col) {
                        return new Column(col);
                    })
                    self.columns(cols);
                } else {
                    setTimeout(function() {
                        location.href = self.returnUrl();
                    }, 1000);
                }
            })
        } else {
            window.info.fail(Kooboo.text.info.parameterMissing + ": table");
            setTimeout(function() {
                location.href = self.returnUrl();
            }, 1000);
        }
    }

    function Column(data) {
        var self = this;

        this.name = ko.validateField(data ? data.name : '', {
            required: '',
            localUnique: {
                compare: function() {
                    return self.name ? USED_NAMES.concat(self.name()) : USED_NAMES;
                },
                message: ''
            }
        });
        this.controlType = ko.observable(data ? data.controlType : '');
        this.controlType.subscribe(function(ct) {
            self.isIncremental(false);
            self.seed(1);
            self.scale(1);
        })
        this.length = ko.observable(data ? data.length : 1024);
        this.controlTypeName = ko.pureComputed(function() {
            return self.controlType() ? Kooboo.columnEditor.controlTypeNames[self.controlType().toLowerCase()] : '';
        })
        this.dataType = ko.observable(data ? data.dataType : null);
        this.isSystem = ko.observable(data ? data.isSystem : false);
        this.isPrimaryKey = ko.observable(data ? data.isPrimaryKey : false);
        this.isPrimaryKey.subscribe(function(ip) {
            if (ip) { self.isUnique(true); }
        })
        this.isUnique = ko.observable(data ? data.isUnique : false);
        this.isUnique.subscribe(function(iu) {
            if (!iu) {
                if (self.isPrimaryKey()) {
                    self.isUnique(true);
                }
                if (self.isIncremental()) {
                    self.isUnique(true);
                }
            } else {
                self.isIndex(true);
            }
        })
        this.isIndex = ko.observable(data ? data.isIndex : false);
        this.isIndex.subscribe(function(ix) {
            if (!ix) {
                if (self.isUnique()) {
                    self.isIndex(true);
                }
            }
        })

        this.isIncremental = ko.observable(data ? data.isIncremental : false);
        this.isIncremental.subscribe(function(bool) {
            self.isUnique(bool);
            self.isIndex(bool);
        })
        this.disableIncremental = ko.observable(false);
        this.seed = ko.validateField(data ? data.seed : 1, {
            required: '',
            min: { value: 1 },
            dataType: { type: 'Integer' }
        });
        this.scale = ko.validateField(data ? data.scale : 1, {
            required: '',
            min: { value: 1 },
            dataType: { type: 'Integer' }
        });

        var settings = data ? JSON.parse(data.setting) : null;
        this.options = ko.observableArray();
        this.resetOptions = function(arr) {
            self.options(arr.map(function(opt) {
                return new optionModel(opt)
            }))
        }
        this.validations = ko.observableArray();
        if (settings && typeof settings == 'object') {
            if (settings.options) {
                self.options(settings.options.map(function(opt) {
                    return new optionModel(opt);
                }));
            }

            if (settings.validations) {
                // TODO
            }
        }

        this.addOption = function() {
            self.options.push(new optionModel());
        }

        this.removeOption = function(m) {
            self.options.remove(m);
        }

        this.isAbleToAddOption = ko.pureComputed(function() {
            return self.isOptionsValid();
        })

        this.isOptionsValid = function() {
            var allValid = true;
            self.options().forEach(function(opt) {
                if (!opt.isValid()) { allValid = false; }
            })
            return allValid;
        }

        this.showOptionsError = function() {
            self.options().forEach(function(opt) {
                !opt.isValid() && opt.showError(true);
            })
        }

        this.hideOptionsError = function() {
            self.options().forEach(function(opt) {
                opt.showError(false);
            })
        }
    }

    function optionModel(opt) {
        var self = this;

        this.key = ko.validateField(opt ? opt.key : '', {
            required: ''
        });

        this.value = ko.validateField(opt ? opt.value : '', {
            required: ''
        });

        this.isValid = function() {
            return self.key.isValid() && self.value.isValid();
        }

        this.showError = ko.observable(false);
    }

    var vm = new columnsModel();

    ko.applyBindings(vm, document.getElementById('main'))
})