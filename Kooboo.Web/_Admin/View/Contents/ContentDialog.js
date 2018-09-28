$(function() {

    var Content = function() {
        var self = this;

        var id = Kooboo.getQueryString('id') || Kooboo.Guid.Empty;
        var folderId = Kooboo.getQueryString('folder');

        this.showError = ko.observable(false);

        this.mediaDialogData = ko.observable();
        this.fields = ko.observableArray();
        this.contentValues = ko.observable({});
        this.siteLangs = ko.observable();

        this.startValidating = ko.observable(false);
        this.validationPassed = ko.observable(true);

        this.categories = ko.observableArray();
        this.embedded = ko.observableArray();
        this.choosedEmbedded = ko.observable({});

        this.save = function() {
            if (self.isAbleToSave()) {
                Kooboo.TextContent.update({
                    id: id,
                    folderId: folderId,
                    values: self.contentValues().fieldsValue || {},
                    categories: self.contentValues().categories || {},
                    embedded: self.contentValues().embedded || {}
                }).then(function(res) {
                    if (res.success) {
                        if (window.parent.__gl &&
                            window.parent.__gl.saveContentFinish) {

                            var fields = {},
                                data = self.contentValues().fieldsValue[self.siteLangs().default],
                                keys = Object.keys(data);

                            keys.forEach(function(key) { fields[key] = data[key]; })

                            window.parent.__gl.saveContentFinish(fields, res.model, folderId);
                        }
                    }
                })
            }
        }

        this.isAbleToSave = function() {
            this.startValidating(true);
            var flag = this.validationPassed();
            this.startValidating(false);
            return flag;
        }

        Kooboo.EventBus.subscribe("ko/style/list/pickimage/show", function(ctx) {
            Kooboo.Media.getList().then(function(res) {
                if (res.success) {
                    res.model["show"] = true;
                    res.model["context"] = ctx;
                    res.model["onAdd"] = function(selected) {
                        ctx.settings.file_browser_callback(ctx.field_name, selected.url + "?SiteId=" + Kooboo.getQueryString("SiteId"), ctx.type, ctx.win, true);
                    }
                    self.mediaDialogData(res.model);
                }
            });
        });

        $.when(Kooboo.Site.Langs(), Kooboo.TextContent.getEdit({
            folderId: folderId,
            id: id
        })).then(function(r1, r2) {
            var langRes = r1[0],
                contentRes = r2[0];

            if (langRes.success && contentRes.success) {
                self.siteLangs(langRes.model);
                self.fields(contentRes.model.properties);
                self.categories(contentRes.model.categories || []);
                self.embedded(contentRes.model.embedded || [])

                setTimeout(function() {
                    adjustHeight();
                }, 300);
            }
        })

        function adjustHeight() {

            var hasTinyMceField = !!_.find(self.fields(), function(field) {
                return ["tinymce", "mediafile"].indexOf(field.controlType.toLowerCase()) > -1;
            });

            var data = {
                height: hasTinyMceField ? parent.window.innerHeight - 100 : window.document.body.scrollHeight,
                hasTinyMceField: hasTinyMceField
            }
            window.parent.Kooboo.EventBus.publish("kb/component/modal/set/height", data);
        }

        $(window).on('resize', function() {
            adjustHeight();
        })

        if (window.parent.__gl) {
            window.parent.__gl.saveContent = this.save;
        }
    }

    var vm = new Content();
    ko.applyBindings(vm, document.getElementById('main'));

})