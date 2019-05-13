(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/dataBase/columnEditor.html");
    var CONTROL_TYPES = Kooboo.columnEditor.controlTypes;

    ko.components.register("column-editor", {
        viewModel: function(params) {
            var self = this;
            var storedColumn = null;
            this.isNew = params.isNew;
            this.showModal = params.showModal;
            this.showModal.subscribe(function(show) {
                if (show) {
                    storedColumn = ko.mapping.toJS(self.data());

                    if (self.isNew()) {
                        self.availableControlTypes(CONTROL_TYPES);
                    } else {
                        var groups = _.groupBy(CONTROL_TYPES, 'dataType'),
                            find = _.find(CONTROL_TYPES, function(ct) {
                                return ct.value == self.data().controlType();
                            });
                        self.availableControlTypes(find ? groups[find.dataType] : []);

                        if (self.data().controlType() == "Number" && !self.data().isIncremental()) {
                            self.data().disableIncremental(true);
                        }
                    }

                    self.data().controlType.subscribe(function(type) {
                        self.data().options().forEach(function(opt) {
                            self.removeOption(opt);
                        })
                        self._optionRequired("");
                        self.showError(false);
                        if(self.showStringLengthForm()){
                            !self.data().length() && self.data().length(1024);
                        }
                    })
                }
            })
            this.data = params.data;
            this.onSave = params.onSave;
            this.onReset = function() {
                self.showError(false);
                self.data().hideOptionsError();
                self.availableControlTypes([]);
                self.data().controlType(storedColumn.controlType);
                self.data().dataType(storedColumn.dataType);
                self.data().isPrimaryKey(storedColumn.isPrimaryKey);
                self.data().isUnique(storedColumn.isUnique);
                self.data().isIndex(storedColumn.isIndex);
                self.data().isIncremental(storedColumn.isIncremental);
                self.data().seed(storedColumn.seed);
                self.data().scale(storedColumn.scale);
                self.data().resetOptions(storedColumn.options);
                // self.data().resetValidations(storedColumn.validations);

                self.showModal(false);
                self.data(null);
                storedColumn = null;
            }
            this.onEditorSave = function() {
                if (self.isValid()) {

                    var find = _.find(CONTROL_TYPES, function(ct) {
                        return ct.value == self.data().controlType();
                    })
                    find && (self.data().dataType(find.dataType));
                    self.availableControlTypes([]);

                    self.onSave();
                    self.showModal(false);
                } else {
                    self.showError(true);
                    self.data().showOptionsError();
                }
            }
            this.showOptionForm = function() {
                if (!self.data().controlType()) {
                    return false;
                } else {
                    return ['selection', 'checkbox', 'radiobox'].indexOf(self.data().controlType().toLowerCase()) > -1;
                }
            }
            this.showStringLengthForm = function(){
                if (!self.data().controlType()) {
                    return false;
                } else {
                    return ['textbox', 'textarea', 'tinymce'].indexOf(self.data().controlType().toLowerCase()) > -1;
                }
            }
            this.isValid = function() {
                self._optionRequired(self.data().options().length ? "requried" : "");

                var flag = self.data().name.isValid();

                if (self.showOptionForm()) {
                    flag = flag && self.data().isOptionsValid() && self._optionRequired.isValid();
                }

                if (self.data().isIncremental()) {
                    flag = flag &&
                        self.data().seed.isValid() &&
                        self.data().scale.isValid();
                }

                return flag;
            }

            this._optionRequired = ko.validateField("", {
                required: ""
            })
            this.addOption = function() {
                if (self.isAbleToAddOption()) {
                    self.data().addOption();
                }
                self._optionRequired("required");
            }

            this.removeOption = function(m) {
                m.showError(false);
                self.data().removeOption(m);
                self._optionRequired(self.data().options().length ? "requried" : "");
            }

            this.isAbleToAddOption = ko.pureComputed(function() {
                return self.data().isAbleToAddOption();
            });

            this.showError = ko.observable(false);
            this.availableControlTypes = ko.observableArray();


            this.inputNumberOnly = function(m, e) {
                if (e.keyCode >= 48 && e.keyCode <= 57 /*number*/ ) {
                    return true;
                } else if (e.keyCode >= 96 && e.keyCode <= 105 /*number*/ ) {
                    return true;
                } else if (e.keyCode == 8 /*BACKSPACE*/ ) {
                    return true;
                } else {
                    return false;
                }
            }
        },
        template: template
    })
})()