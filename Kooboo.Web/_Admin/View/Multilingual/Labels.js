$(function() {

    var labelViewModel = function() {

        var self = this;

        this.lang = ko.observable();
        this.langName = ko.observable();

        this.multiLang = ko.observable();
        this.mutliLangName = ko.observable();

        this.labels = ko.observableArray();

        this.rendered = function() {
            waterfall('.label-list')
        }

        this.editingLabel = ko.observable();

        this.newMultiLangValue = ko.observable();

        this.editLabel = function(m, e) {
            self.showEditModal(true);
            self.editingLabel(m);

            self.newMultiLangValue(m.multilingual());

            setTimeout(function() {
                $(".autosize").textareaAutoSize().trigger("keyup");
            }, 300);
        }

        $.when(Kooboo.Site.Langs(),
                Kooboo.Label.getList())
            .then(function(r1, r2) {

                var langRes = r1[0],
                    labelRes = r2[0];

                if (langRes.success && labelRes.success) {

                    self.lang(langRes.model.default);
                    self.langName(langRes.model.defaultName);
                    self.multiLang(Kooboo.getQueryString("lang"));
                    self.mutliLangName(langRes.model.cultures[self.multiLang()]);

                    labelRes.model.forEach(function(label) {
                        label.defaultLang = self.lang();
                        label.multiLang = self.multiLang();
                        self.labels.push(new MultilingualLabelModel(label));
                    })
                }
            })

        this.showEditModal = ko.observable(false);

        this.onHideEditModal = function() {
            self.showEditModal(false);
            self.newMultiLangValue('');
        }

        this.onSaveEditModal = function() {
            var values = {};

            values[self.multiLang()] = self.newMultiLangValue() || "";

            Kooboo.Label.Update({
                id: self.editingLabel().id(),
                values: values
            }).then(function(res) {
                if (res.success) {
                    self.editingLabel().multilingual(self.newMultiLangValue());
                    self.editingLabel().date(new Date().toDefaultLangString());
                    self.onHideEditModal();
                    window.info.done(Kooboo.text.info.update.success);
                }
            })
        }
    }

    var vm = new labelViewModel();

    var MultilingualLabelModel = function(info) {
        var self = this;

        var _d = new Date(info.lastModified);

        ko.mapping.fromJS(info, {}, self);

        this.date = ko.observable(_d.toDefaultLangString());

        this.defaultValue = ko.observable(info.values[info.defaultLang]);

        this.multilingual = ko.observable(info.values[info.multiLang]);
    }

    ko.applyBindings(vm, document.getElementById("main"));

    $(window).on('resize', function() {
        waterfall('.label-list')
    })
});