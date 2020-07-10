$(function() {
    var self;
    new Vue({
        el: "#main",
        data: function() {
            return {
                isNewCode: false,
                codeId: Kooboo.getQueryString("Id") || Kooboo.Guid.Empty,
                name: undefined,
                codeContent: "",
                compareTarget: "",
                sourceChange: false,
                supportExtensions: undefined,
                codeChange: false,
                nameValidateModel: { valid: true, msg: "" },
                codeTypeValidateModel:{ valid: true, msg: "" },
                lang: "javascript",
                codeType: undefined,
                eventType: undefined,
                url: undefined,
                configContent: undefined,
                availableCodeType: undefined,
                availableEventType: undefined,
                activeTab: 'code'


            };
        },
        watch: {
            codeContent: function(value) {
                if (value !== self.compareTarget) {
                    self.codeChange = true;
                } else {
                    self.codeChange = false;
                }
            },
            name: function() {
                self.nameValidateModel = { valid: true, msg: "" };
            },
            codeType: function () {
                self.codeTypeValidateModel = { valid: true, msg: "" };
            }
        },
        created: function() {
            self = this;
            if (self.codeId === Kooboo.Guid.Empty) {
                self.isNewCode = true;
            }
            self.init();
        },
        mounted: function() {
            $(document).keydown(function(e) {
              if (e.keyCode == 83 && e.ctrlKey) {
                //Ctrl + S
                e.preventDefault();
                self.onSave();
              }
            });
        },
        methods: {
            init: function() {
                Kooboo.Code.getEdit({
                    Id: self.codeId,
                    codeType: Kooboo.getQueryString("codeType") || "all"
                }).then(function(res) {

                    if (res.success) {
                        self.name = res.model.name;
                        self.codeContent = res.model.body || "";
                        self.compareTarget = self.codeContent;
                        self.url = res.model.url;

                        if (res.model.availableCodeType) {
                            self.availableCodeType = res.model.availableCodeType.map(function(item) {
                                return {
                                    displayText: item,
                                    value: item.toLowerCase()
                                }
                            });
                            self.availableCodeType.splice(0,0,{
                                displayText: Kooboo.text.site.code.chooseCodeType,
                                value: undefined
                            })
                        } else {
                            self.availableCodeType = [];
                            self.codeType = res.model.codeType;
                        }

                        if (res.model.availableCodeType) {
                            self.availableEventType = res.model.availableEventType.map(function(item) {
                                return {
                                    displayText: item,
                                    value: item
                                }
                            });
                            if(self.availableEventType.length && self.availableEventType[0].value ){
                                self.eventType = self.availableEventType[0].value
                            }
                            var codeTypeParams = Kooboo.getQueryString('codeType'),
                                eventTypeParma = Kooboo.getQueryString('eventType');

                            if (codeTypeParams) {
                                self.codeType = codeTypeParams.toLowerCase();
                            }
                            if (eventTypeParma) {
                                self.codeType = 'event';
                                self.eventType = eventTypeParma;
                            }

                        } else {
                            self.availableEventType = [];
                            self.eventType = res.model.eventType;
                        }

                        self.configContent = res.model.config || "";
                    }
                })
            },
            formatCode: function() {
                this.$refs[this.activeTab].formatCode();
            },
            onSaveAndReturn: function() {
                self.onSubmitCode(function(model) {
                    self.goBack();
                });
            },
            onSave: function() {
                self.onSubmitCode(function(id) {
                    if (self.isNewCode) {
                        location.href = Kooboo.Route.Get(Kooboo.Route.Code.EditPage, {
                            Id: id
                        });
                    } else {
                        window.info.show(Kooboo.text.info.save.success, true);
                        self.compareTarget = self.codeContent;
                    }
                });
            },
            onCancel: function() {
                if (self.codeChange) {
                    if (confirm(Kooboo.text.confirm.beforeReturn)) {
                        self.goBack();
                    }
                } else {
                    self.goBack();
                }
            },
            goBack: function() {
                location.href = Kooboo.Route.Get(Kooboo.Route.Code.ListPage);
            },
            validate: function() {
                var nameRule = [
                    {
                        required: true,
                        message: Kooboo.text.validation.required

                    },
                    {
                        pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
                        message: Kooboo.text.validation.objectNameRegex
                    },
                    {
                        min: 1,
                        max: 64,
                        message:
                            Kooboo.text.validation.minLength +
                            1 +
                            ", " +
                            Kooboo.text.validation.maxLength +
                            64
                    },
                    {
                        remote: {
                            url: Kooboo.Code.isUniqueName(),
                            data: function() {
                               return {name: self.name}
                            }
                        },
                        message: Kooboo.text.validation.taken
                    }
                ];
                var codeTypeRule = [
                    {
                        required: true,
                        message: Kooboo.text.validation.required

                    }
                ];
                self.nameValidateModel = Kooboo.validField(self.name, nameRule);
                self.codeTypeValidateModel = Kooboo.validField(self.codeType, codeTypeRule);
                return self.nameValidateModel.valid && self.codeTypeValidateModel.valid;
            },
            onSubmitCode: function(callback) {
                if ((self.isNewCode && self.validate()) || !self.isNewCode) {
                    Kooboo.Code.post({
                        Id: self.isNewCode ? Kooboo.Guid.Empty : self.codeId,
                        name: self.name,
                        body: self.codeContent,
                        config: self.configContent,
                        codeType: self.codeType,
                        eventType: self.eventType,
                        url: self.codeType.toLowerCase() === 'api' ? self.url : ''
                    }).then(function(res) {
                        if (res.success) {
                            callback && typeof callback == "function" && callback(res.model);
                        } else {
                            window.info.show(Kooboo.text.info.save.fail, false);
                        }
                    });
                }
            }
        }
    });
});
