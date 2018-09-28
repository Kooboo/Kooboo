(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/viewEditor/components/link.html"),
        PageStore = Kooboo.viewEditor.store.PageStore,
        DataContext = Kooboo.viewEditor.DataContext;

    var EXTERNAL_LINK = "__external_link",
        bindingType = "link";

    ko.components.register("kb-view-link", {
        viewModel: function(params) {

            var self = this;
            this.elem = null;
            this.showError = ko.observable(false);

            this.mode = ko.observable("normal");

            this.isShow = ko.observable(false);

            this.pages = ko.observableArray();

            this.views = ko.observableArray();

            this.externalLink = EXTERNAL_LINK;

            this.link = ko.observable("");
            this.link.subscribe(function(link) {
                self.showError(false);
                self.isExternalLink(link == EXTERNAL_LINK);

                var find = _.find(_.concat(self.pages(), self.views()), function(l) {
                    return l.url == link
                })

                find && self._paramsters(find.parameters);
            })

            this._link = ko.observable();
            this._link.subscribe(function(link) {
                self.link(link.url);
                self._paramsters(link.parameters || []);
            })

            this._paramsters = ko.observableArray();
            this._paramsters.subscribe(function(parameters) {
                var _list = [];
                _.forEach(parameters, function(para) {
                    if (typeof para == 'string') {
                        para = { name: para, value: '' }
                    }
                    _list.push(new parameterModel(para));
                });
                self.parameters(_list);
            });

            this.parameters = ko.observableArray();

            this.isExternalLink = ko.observable(false);

            this.extLink = ko.validateField({
                required: Kooboo.text.validation.required
            });

            this.showError = ko.observable(false);

            this.dataSourceId = ko.observable();

            this.reset = function() {
                self.elem = null;
                self.dynaParams([]);
                self.dataSourceId(null);
                self.isShow(false);
            }

            this.isValid = function() {

                if (self.mode() == "normal") {
                    if (self.isExternalLink()) {
                        return self.extLink.isValid();
                    } else {
                        return self.link();
                    }
                }
                return true;
            }

            this.save = function() {
                if (self.isValid()) {
                    params.onSave && params.onSave({
                        elem: self.elem,
                        bindingType: bindingType,
                        href: self.getFinalHref()
                    });
                    if (self.mode() == "pagination") {
                        Kooboo.EventBus.publish("kb/view/editor/data/pager", {
                            elem: self.elem,
                            pager: self.getRawData(),
                            dataSourceId: self.dataSourceId()
                        });
                    }
                    self.reset();
                } else {
                    self.showError(true);
                }
            }

            this.getRawData = function() {
                var temp = self.getFinalHref();
                return temp.match(/Pager\((\S*)\)/)[1];
            }

            this.getFinalHref = function() {
                if (self.mode() == "normal") {
                    if (self.isExternalLink()) {
                        return self.extLink();
                    } else {
                        if (self.parameters().length) {
                            var query = [];
                            self.parameters().forEach(function(para) {
                                if (para.value()) {
                                    query.push(para.name() + "=" + para.value());
                                }
                            })
                            return self.link() + (query.length ? ("?" + query.join("&")) : "");
                        } else {
                            return self.link();
                        }
                    }
                    return self.isExternalLink() ? self.extLink() : self.link();
                } else {
                    var paras = {};
                    self.dynaParams().forEach(function(dp) {
                        paras[dp.name] = dp.value;
                    })
                    return "Pager(" + paras.pageNumber + ")"
                }
            }

            this.dynaParams = ko.observableArray();

            this.href = ko.observable();

            Kooboo.EventBus.subscribe("binding/edit", function(data) {
                if (data.bindingType == bindingType) {
                    self.elem = data.elem;
                    self.href(data.href);

                    var dataSource = DataContext.get(data.elem).getDataSource();
                    if (dataSource && dataSource.length && paginationDS(dataSource)) {
                        self.mode("pagination");

                        var ds = dataSource[0];
                        self.dataSourceId(ds.dataId);
                        self.dynaParams.push({
                            name: 'pageNumber',
                            value: ds.name
                        })
                    } else {
                        self.mode("normal");
                    }

                    !self.pages().length && self.pages(PageStore.getAll().pages);
                    !self.views().length && self.views(PageStore.getAll().views);
                    setDefaultValue();
                    self.isShow(true);
                }

                function paginationDS(ds) {
                    var find = _.find(ds, function(d) {
                        return d.isPagedResult && (d.name.indexOf("Pages_Item") > -1);
                    })

                    return !!find;
                }
            })

            function setDefaultValue() {
                if (!self.href()) {
                    self.link(self.externalLink);
                } else {
                    var _find = _.findLast(_.concat(self.pages(), self.views()), function(link) {
                        return link.url == self.href();
                    })

                    if (!_find) {
                        if (self.href().indexOf("?") > -1) {
                            var temp = self.href().split("?");
                            var possibleLink = temp[0],
                                possibleParams = temp[1];

                            var __find = _.findLast(_.concat(self.pages(), self.views()), function(link) {
                                return link.url == possibleLink;
                            })

                            if (__find) {
                                var params = __find.parameters.map(function(p) {
                                        return { name: p, value: '' }
                                    }),
                                    exist = possibleParams.split("&").map(function(pa) {
                                        var sp = pa.split("=");
                                        return { name: sp[0], value: sp[1] }
                                    }),
                                    list = [];

                                params.forEach(function(p) {
                                    var find = _.find(exist, function(e) {
                                        return e.name == p.name;
                                    })

                                    list.push(find || p);
                                })

                                self._link({
                                    url: possibleLink,
                                    parameters: list
                                });
                            }

                        } else if (self.href().indexOf("Pager(") > -1) {

                            if (!self.dynaParams().length) {
                                var tester = self.href().match(/Pager\((\S*)\)/);
                                if (tester) {
                                    var paraStr = tester[1];

                                    self.dynaParams.push({
                                        name: "pageNumber",
                                        value: paraStr
                                    })
                                }
                            }
                        } else {
                            self.link(EXTERNAL_LINK);
                            self.extLink(self.href());
                        }
                    } else {
                        self._link(_find);
                    }
                }
            }
        },
        template: template
    })

    var parameterModel = function(para) {
        var self = this;

        this.name = ko.observable(para.name);

        this.value = ko.observable(para.value);
    }
})()