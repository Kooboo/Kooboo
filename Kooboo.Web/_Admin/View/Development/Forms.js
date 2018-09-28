$(function() {

    var EXTERNAL_LINK = "__external_link";

    if ($.fn.bootstrapSwitch) {
        $.fn.bootstrapSwitch.defaults.onText = Kooboo.text.common.yes;
        $.fn.bootstrapSwitch.defaults.offText = Kooboo.text.common.no;
    }

    var formModel = function() {
        var self = this;

        this.createFormUrl = Kooboo.Route.Form.DetailPage;

        this.createKooFormUrl = Kooboo.Route.Form.KooFormPage;

        this.types = ko.observableArray([{
            displayName: Kooboo.text.common.external,
            value: "External"
        }, {
            displayName: Kooboo.text.common.Embedded,
            value: "Embedded"
        }]);

        this.curType = ko.observable();

        this.fetchDataByType = function(m, e) {
            if (m.value !== self.curType()) {
                Kooboo.Form['get' + m.value + 'List']().then(function(res) {
                    if (res.success) {
                        self.curType(m.value);
                        var _forms = [];
                        res.model.forEach(function(form) {
                            var date = new Date(form.lastModified),
                                editUrl = "";

                            if (m.value.toLowerCase() == "embedded" || form.formType.toLowerCase() == "normal") {
                                editUrl = Kooboo.Route.Form.DetailPage;
                            } else if (form.formType.toLowerCase() == "koobooform") {
                                editUrl = Kooboo.Route.Form.KooFormPage;
                            }

                            _forms.push({
                                id: form.id,
                                name: form.name,
                                date: date.toDefaultLangString(),
                                data: {
                                    text: form.valueCount,
                                    class: "blue",
                                    url: Kooboo.Route.Get(Kooboo.Route.Form.ValuePage, {
                                        id: form.id
                                    }),
                                    newWindow: true
                                },
                                relationsComm: "kb/relation/modal/show",
                                relationsTypes: Object.keys(form.references),
                                relations: form.references,
                                edit: {
                                    text: Kooboo.text.common.edit,
                                    url: Kooboo.Route.Get(editUrl, { Id: form.id })
                                },
                                setting: {
                                    text: Kooboo.text.common.setting,
                                    url: "kb/development/form/setting"
                                }
                            })
                        });

                        var _data = {
                            docs: _forms,
                            columns: [{
                                displayName: Kooboo.text.common.name,
                                fieldName: "name",
                                type: "text"
                            }, {
                                displayName: Kooboo.text.common.data,
                                fieldName: "data",
                                type: "link-badge"
                            }, {
                                displayName: Kooboo.text.common.usedBy,
                                fieldName: "relations",
                                type: "communication-refer"
                            }, {
                                displayName: Kooboo.text.common.lastModified,
                                fieldName: "date",
                                type: "text"
                            }],
                            tableActions: [{
                                fieldName: "edit",
                                type: "link-btn"
                            }, {
                                fieldName: "setting",
                                type: "communication-btn"
                            }],
                            kbType: Kooboo.Form.name,
                        }

                        self.tableData(_data);
                    }
                })
            }
        }

        this.id = ko.observable();

        this.formId = ko.observable();

        this.showSettingModal = ko.observable(false);

        this.onHideSettingModal = function() {
            self.id(null);
            self.formId(null);
            self.showSettingModal(false);
            $("#enable-setting-switch").bootstrapSwitch("destroy", true);
            $("#enable-setting-switch").bootstrapSwitch("state", false);
            self.enableSetting(false);
        }

        this.saveSettingModal = function() {
            var settings = {};
            _.forEach(self.settings(), function(setting) {
                settings[setting.name] = setting.defaultValue;
            })

            Kooboo.Form.updateSetting({
                id: self.id(),
                formId: self.formId(),
                method: self.method(),
                redirectUrl: self.showExternalLinkInput() ? self.externalLink() : self.redirect(),
                setting: settings,
                formSubmitter: self.formSubmitter(),
                enable: self.enableSetting()
            }).then(function(res) {
                if (res.success) {
                    window.info.show(Kooboo.text.info.update.success, true);
                    self.onHideSettingModal();
                } else {
                    window.info.show(Kooboo.text.info.update.fail, false);
                }
            })
        }

        this.enableSetting = ko.observable(false);

        this.method = ko.observable('get');

        this.showExternalLinkInput = ko.observable(false);
        this.redirect = ko.observable();
        this.redirect.subscribe(function(val) {
            self.showExternalLinkInput(val == EXTERNAL_LINK);
        })
        this.externalLink = ko.observable();

        this.linkList = ko.observableArray();

        this.availableSubmitters = ko.observableArray();

        this.formSubmitter = ko.observable();
        this.formSubmitter.subscribe(function(name) {
            if (name) {
                var _submitter = _.findLast(self.availableSubmitters(), function(as) {
                    return as.name == name;
                })

                if (_submitter.settings && _submitter.settings.length) {
                    self.settings(_submitter.settings);
                } else {
                    self.settings(null);
                }
            }
        })

        this.settings = ko.observableArray();

        Kooboo.EventBus.subscribe("kb/development/form/setting", function(data) {
            Kooboo.Form.getSetting({
                id: data.id
            }).then(function(res) {
                if (res.success) {
                    self.id(res.model.id);
                    self.formId(res.model.formId);
                    self.showSettingModal(true);
                    $("#enable-setting-switch").bootstrapSwitch("state", res.model.enable).on("switchChange.bootstrapSwitch", function(e, data) {
                        self.enableSetting(data);
                    });
                    self.enableSetting(res.model.enable);

                    self.method(res.model.method || "get");

                    if (res.model.redirectUrl) {
                        var _find = _.findLast(self.linkList(), function(link) {
                            return link.path == res.model.redirectUrl;
                        });

                        if (_find) {
                            self.redirect(res.model.redirectUrl);
                        } else {
                            self.redirect(EXTERNAL_LINK);
                            self.externalLink(res.model.redirectUrl);
                        }
                    } else {
                        self.redirect(self.linkList()[0].path);
                    }

                    _.forEach(res.model.availableSubmitters, function(submitter) {
                        _.forEach(submitter.settings, function(setting) {
                            setting.selectionValues = Kooboo.objToArr(setting.selectionValues);
                        })
                    })

                    self.availableSubmitters(res.model.availableSubmitters);
                    self.formSubmitter(res.model.formSubmitter);
                }
            })
        })

        self.fetchDataByType({ value: location.hash ? location.hash.split('#')[1] : 'External' });

        Kooboo.Page.getAll().then(function(res) {

            if (res.success) {
                var pageList = res.model.pages;
                pageList.push({
                    name: Kooboo.text.common.externalLink,
                    path: EXTERNAL_LINK
                })

                pageList.reverse();

                pageList.push({
                    name: Kooboo.text.site.form.refreshSelf,
                    path: "RefreshSelf()"
                })

                self.linkList(pageList.reverse());
            }
        })

    };

    formModel.prototype = new Kooboo.tableModel(Kooboo.Form.name);
    var vm = new formModel();

    ko.applyBindings(vm, document.getElementById("main"));
})