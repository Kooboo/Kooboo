(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/viewEditor/components/form.html"),
        ActionStore = Kooboo.viewEditor.store.ActionStore,
        PageStore = Kooboo.viewEditor.store.PageStore,
        FormBindingStore = Kooboo.viewEditor.store.FormBindingStore,
        DataContext = Kooboo.viewEditor.DataContext;

    var bindingType = "form";

    function createSubmitTargets() {
        var acts = ActionStore.getAll(),
            ret = [];
        _.forEach(acts, function(it) {
            if (it.isPost) {
                ret.push({
                    name: it.group.replace(/DataSource$/, '') + '.' + it.methodName,
                    value: it.methodId
                });
            }
        });
        return ret;
    }

    ko.components.register("kb-view-form", {
        viewModel: function(params) {
            var self = this;
            this.onSave = params.onSave;

            this.elem = null;
            this.isShow = ko.observable(false);
            this.formBindingId = ko.observable();
            this.dataSourceMethodId = ko.validateField({
                required: "This field is required"
            });
            this.redirect = ko.observable();
            this.method = ko.observable();
            this.callback = ko.observable();
            this.showError = ko.observable(false);

            this.ajax = ko.computed(function() {
                return self.method() === 'ajax-get' || self.method() === 'ajax-post';
            });

            this.pages = ko.observableArray(_.cloneDeep(PageStore.getAll()).pages);

            this.dataSourceMethodReturnFields = ko.observableArray();

            this.redirectParameters = ko.observableArray();

            this.redirect.subscribe(function(url) {
                self.redirectParameters.removeAll();
                if (!url) {
                    return;
                }

                var params = [];
                $.each(PageStore.getParameters(url), function() {
                    params.push({
                        name: this.name,
                        value: ko.observable(self.redirectParameterValues[this.name])
                    });
                });

                self.redirectParameters(params);
            });

            this.dataSourceMethodId.subscribe(function(id) {
                self.dataSourceMethodReturnFields.removeAll();

                if (!id) return;

                var method = ActionStore.byId(id);
                if (!method) throw new Error('Cannot find data source method with id "' + id + '".');

                var fields = DataContext.flatternFields(method).map(function(x) {
                    return {
                        name: 'Result.' + x.name,
                        value: x.name
                    };
                });

                self.dataSourceMethodReturnFields(fields);
            });

            Kooboo.EventBus.subscribe("PageStore/change", function() {
                self.pages(_.cloneDeep(PageStore.getAll()));
            });

            this.submitTargets = ko.observableArray(createSubmitTargets());

            this.redirectParameterValues = {};

            Kooboo.EventBus.subscribe("binding/edit", function(data) {
                if (data.bindingType == bindingType) {
                    self.elem = data.elem;
                    self.isShow(true);

                    self.formBindingId(data.formBindingId);
                    self.dataSourceMethodId(data.dataSourceMethodId);
                    self.method(data.method || 'post');
                    self.callback(data.callback);

                    self.redirectParameterValues = {};

                    if (data.redirect) {
                        var match = PageStore.match(data.redirect);
                        if (match) {
                            $.each(match.params, function() {
                                self.redirectParameterValues[this.name] = this.value;
                            });
                            self.redirect(match.page.route);
                        } else {
                            self.redirect(data.redirect);
                        }
                    }
                }
            });

            this.valid = function() {
                return self.dataSourceMethodId.valid();
            };

            this.save = function() {
                if (self.valid()) {
                    var redirectParams = {};
                    $.each(self.redirectParameters(), function() {
                        redirectParams[this.name] = '{' + this.value() + '}';
                    });

                    self.ensureFormBindingId().then(function(id) {
                        var formBinding = {
                            id: id,
                            dataSourceMethodId: self.dataSourceMethodId(),
                            method: self.method(),
                            redirect: PageStore.replaceParameters(self.redirect(), redirectParams),
                            callback: self.callback()
                        };

                        FormBindingStore.save(formBinding);

                        self.onSave({
                            bindingType: bindingType,
                            elem: self.elem,
                            formBindingId: id,
                            dataSourceMethodId: self.dataSourceMethodId(),
                            method: self.method(),
                            redirect: formBinding.redirect,
                            callback: self.callback()
                        });

                        self.reset();
                    });
                } else {
                    self.showError(true);
                }
            };

            this.ensureFormBindingId = function() {
                var defer = $.Deferred();
                if (self.formBindingId()) {
                    defer.resolve(self.formBindingId());
                } else {
                    $.get('/api/admin/guid', function(id) {
                        self.formBindingId(id);
                        defer.resolve(id);
                    });
                }

                return defer.promise();
            }

            this.reset = function() {
                self.elem = null;
                self.formBindingId(null);
                self.dataSourceMethodId(null);
                self.method(null);
                self.redirect(null);
                self.callback(null);
                self.showError(false);
                self.isShow(false);
            };
        },
        template: template
    });

})();