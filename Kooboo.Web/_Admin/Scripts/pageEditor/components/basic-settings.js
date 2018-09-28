(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/pageEditor/components/basic-settings.html"),
        ComponentStore = Kooboo.pageEditor.store.ComponentStore;

    ko.components.register('kb-page-basic-settings', {
        viewModel: function(params) {
            var self = this;

            this.multiContentTitle = ko.observableArray();

            this.metaBindingHelpers = ko.observableArray([]);

            this.routeHelpers = ko.observableArray();

            this.metaHelp = function(title, meta) {
                title.value((title.value() ? title.value() : "") + "{" + meta + "}")
            }

            this.showError = ko.observable(false);

            this.urlPath = ko.validateField(params && params.settings && params.settings.urlPath, {
                regex: {
                    pattern: /^[^\s|\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\||\[|\]|\;|\:|\"|\'|\,|\<|\>|\?]*$/,
                    message: Kooboo.text.validation.urlInvalid
                }
            });
            this.urlPath.subscribe(function() {
                Kooboo.EventBus.publish("kb/page/field/change", {
                    type: "url"
                })
            })

            this.routeHelpers = ko.observableArray();

            this.routeHelp = function(data) {
                self.urlPath(self.urlPath() + data);
            }

            this.removeDefaultRouteValue = function(routeValue) {
                Kooboo.EventBus.publish("kb/page/field/change", {
                    type: "url"
                })
            }

            if (params.settings) {

                var cultures = Object.keys(params.multiLangs.cultures),
                    defaultCultureIdx = cultures.indexOf(params.multiLangs.default);

                if (defaultCultureIdx > -1) {
                    var defaultCulture = cultures[defaultCultureIdx],
                        title = {
                            name: defaultCulture,
                            value: ko.observable(params.settings.contentTitle[""] || params.settings.contentTitle[defaultCulture]),
                            isDefault: true,
                            show: ko.observable(true)
                        }

                    if (window.__pageEditor.kbFrame) {
                        window.__pageEditor.kbFrame.setTitle(title.value() || "");
                    }

                    title.value.subscribe(function(t) {
                        if (window.__pageEditor.kbFrame) {
                            window.__pageEditor.kbFrame.setTitle(t);
                        }

                        Kooboo.EventBus.publish("kb/page/field/change", { type: "title" })
                    })

                    self.multiContentTitle.push(title)

                    cultures.splice(defaultCultureIdx, 1);
                }

                cultures.forEach(function(lang) {
                    title = {
                        name: lang,
                        value: ko.observable(params.settings.contentTitle[lang] || ""),
                        isDefault: false,
                        show: ko.observable(false)
                    };

                    title.value.subscribe(function(title) {
                        Kooboo.EventBus.publish("kb/page/field/change", {
                            type: "title"
                        })
                    })

                    self.multiContentTitle.push(title);
                });

                Kooboo.EventBus.subscribe("kb/page/title/set", function(title) {

                    var _default = _.findLast(self.multiContentTitle(), function(t) {
                        return t.isDefault
                    });

                    _default && _default.value(title);

                })

                Kooboo.EventBus.subscribe("kb/multilang/change", function(target) {
                    var title = _.findLast(self.multiContentTitle(), function(title) {
                        return target.name === title.name;
                    })

                    title && title.show(target.selected);
                });

                Kooboo.EventBus.subscribe("kb/page/layout/loaded", function(layout) {
                    self.layoutName(layout.name);
                })

                Kooboo.EventBus.subscribe("kb/page/ComponentStore/change", function() {
                    self.metaBindingHelpers(ComponentStore.getMetaBindings());
                    self.routeHelpers(ComponentStore.getUrlParamsBindings());
                })

                Kooboo.EventBus.subscribe("kb/page/url/route/set", function(name) {
                    if (!self.urlPath()) {
                        if (name.indexOf("/") !== 0) {
                            name = "/" + name;
                        };
                        self.urlPath(name);
                    }
                })

                Kooboo.EventBus.subscribe("kb/page/save", function(res) {
                    res["contentTitle"] = {}
                    _.forEach(self.multiContentTitle(), function(title) {

                        if (title.isDefault && $($("iframe")[0]).is(":visible")) {
                            if ($("title", $("iframe")[0].contentWindow.document.head).length) {
                                $("title", $("iframe")[0].contentWindow.document.head).innerHTML = title.value();
                            } else {
                                var el = $("<title>");
                                $(el).text(title.value());
                                $($("iframe")[0].contentWindow.document.head).prepend(el[0]);
                            }

                            Kooboo.EventBus.publish("kb/page/field/change", {
                                type: "resource"
                            })
                        }

                        // if (title.show()) {
                        res["contentTitle"][title.name] = title.value();
                        // }
                    })

                    res["urlPath"] = self.urlPath();

                    if (!self.urlPath.isValid()) {
                        if ($("#url-route-input").is(":visible")) {
                            self.showError(true);
                        } else {
                            window.info.fail(Kooboo.text.component.pageEditor.invalidRoute);
                        }
                        res.errorCount ? res.errorCount++ : (res.errorCount = 1);
                    }
                })

                self.metaBindingHelpers(ComponentStore.getMetaBindings());

                self.routeHelpers(ComponentStore.getUrlParamsBindings());
            }
        },
        template: template
    });
})();