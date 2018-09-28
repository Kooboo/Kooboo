$(function() {

    var labelViewModel = function() {

        var self = this;

        this.labels = ko.observableArray();
        this._labels = ko.observableArray();

        this.defaultLang = ko.observable();

        this.langs = ko.observableArray();

        this.editingLabel = ko.observable();

        this.editingContent = ko.observableArray();

        this.editLabel = function(m) {
            self.showEditModal(true);
            self.editingLabel(m);

            var langs = Object.keys(self.langs()),
                defaultLangIdx = langs.indexOf(self.defaultLang());

            if (defaultLangIdx > -1) {

                var value = m.values.hasOwnProperty(self.defaultLang()) ? m.values[self.defaultLang()]() : "";

                self.editingContent.push({
                    lang: self.defaultLang(),
                    label: self.defaultLang() + ' - ' + self.langs()[self.defaultLang()],
                    value: ko.observable(value)
                })
                langs.splice(defaultLangIdx, 1);
            }

            langs.forEach(function(lang) {
                self.editingContent.push({
                    lang: lang,
                    label: lang + ' - ' + self.langs()[lang],
                    value: ko.observable(m.values[lang] ? m.values[lang]() : "")
                })
            })

            setTimeout(function() {
                $(".autosize").textareaAutoSize().trigger("keyup");
            }, 300);
        }

        this.removeLabel = function(m) {
            if (confirm(Kooboo.text.confirm.deleteItem)) {
                Kooboo.Label.Deletes([m.id()]).then(function(res) {
                    if (res.success) {
                        self.labels.remove(m);
                        self._labels(self.labels());
                        self.rendered();
                    }
                })
            }
        }

        this.rendered = function () {
            try {
                waterfall('.label-list')
            } catch (e) {

            }
        }

        this.showRelationModal = function(m) {
            Kooboo.EventBus.publish("kb/relation/modal/show", {
                id: m.id,
                by: m.by,
                type: 'label'
            })
        }

        this.showEditModal = ko.observable(false);

        this.onHideEditModal = function() {
            self.showEditModal(false);
            self.editingContent.removeAll();
        }

        this.onSaveEditModal = function() {
            var values = {};
            self.editingContent().forEach(function(content) {
                values[content.lang] = content.value();
            });

            Kooboo.Label.Update({
                id: self.editingLabel().id(),
                values: values
            }).then(function(res) {
                if (res.success) {
                    self.editingContent().forEach(function(content) {
                        if (!self.editingLabel().values[content.lang]) {
                            self.editingLabel().values[content.lang] = ko.observable();
                        }
                        self.editingLabel().values[content.lang](content.value());
                    })
                    self.onHideEditModal();
                    window.info.done(Kooboo.text.info.update.success);
                }
            })
        }

        this.isSearching = ko.observable(false);

        this.keyword = ko.observable();
        this.keyword.subscribe(_.debounce(function(keyword) {
            if (!keyword) {
                self.isSearching(false);
                self.labels(self._labels());
                self.rendered()
            } else {
                var _keyword = keyword.toLowerCase();
                self.isSearching(true);
                var result = _.filter(self._labels(), function(label) {
                    var flag = false;
                    Object.keys(self.langs()).forEach(function(key) {
                        if (label.values[key] && label.values[key]().toLowerCase().indexOf(_keyword) > -1) {
                            flag = true;
                        }
                    })
                    return label.name().toLowerCase().indexOf(_keyword) > -1 || flag;
                })
                self.labels(result);
                result.length && self.rendered();
            }
        }, 300))

        $.when(Kooboo.Site.Langs(),
                Kooboo.Label.getList())
            .then(function(r1, r2) {

                var langRes = r1[0],
                    labelRes = r2[0];

                if (langRes.success && labelRes.success) {
                    self.defaultLang(langRes.model.default);
                    self.langs(langRes.model.cultures);
                    var labels = _.sortBy(labelRes.model, [function(o) { return o.lastModified }]);
                    labels.reverse().forEach(function(label) {
                        self.labels.push(new LabelModel(label));
                        self._labels(self.labels());
                    })
                }
            })
    }

    function LabelModel(data) {
        var self = this;

        var _d = new Date(data.lastModified);

        ko.mapping.fromJS(data, {}, self);

        this.date = _d.toDefaultLangString();

        this.refers = ko.observableArray();

        this.versionUrl = Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
            KeyHash: data.keyHash,
            storeNameHash: data.storeNameHash
        });

        Object.keys(self.relations).forEach(function(key) {
            self.refers.push({
                id: self.id(),
                by: key,
                text: self.relations[key]() + ' ' + Kooboo.text.component.table[key.toLowerCase()],
                bgColor: Kooboo.getLabelColor(key)
            })
        })
    }

    var vm = new labelViewModel();

    ko.applyBindings(vm, document.getElementById("main"));

    $(window).on('resize', function() {
        try {
            waterfall('.label-list')
        } catch (e) { }
    })
})