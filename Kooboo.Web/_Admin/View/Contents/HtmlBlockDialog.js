$(function() {

    var blockViewModel = function() {

        var self = this;

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

        Kooboo.EventBus.subscribe("kb/multilang/change", function(target) {
            var content = _.findLast(self.contents(), function(c) {
                return c.abbr == target.name;
            });

            if (content) {
                content.show(target.selected);
                adjustFrameHeight();
            }
        });

        this.name = ko.observable();

        this.multiLangs = ko.observable();

        this.mediaDialogData = ko.observable();

        this.multiContents = ko.observable();
        this.multiContents.subscribe(function(multi) {
            var _contentList = [];
            Object.keys(multi.cultures).forEach(function(abbr) {
                var value = ko.observable((typeof self.contentsValue() == "object" && self.contentsValue().hasOwnProperty(abbr)) ?
                    self.contentsValue()[abbr] : "");

                if (!value()) {
                    if (typeof self.contentsValue() == "object" && self.contentsValue().hasOwnProperty("")) {
                        value = ko.observable(self.contentsValue()[""]);
                    }

                }
                var _content = {
                    show: ko.observable(multi.default == abbr),
                    value: value,
                    abbr: abbr
                };
                _contentList.push(_content);
            });

            self.contents(_contentList);
            adjustFrameHeight();
        });

        this.contents = ko.observableArray();

        this.contentsValue = ko.observable();

        this.blockId = ko.observable();

        this.getMultiConents = function() {
            var _values = {};
            _.forEach(self.contents(), function(c) {
                _values[c.abbr] = c.value();
            });

            return _values;
        }
        this.bindSaveHtmlBlockEvent = function() {

            if (window.parent.__gl) {
                window.parent.__gl.saveHtmlblock = this.onSubmit;
            }
        };
        this.onSubmit = function() {

            function submit() {
                var values = self.getMultiConents();
                Kooboo.HtmlBlock.syncPost({
                    id: Kooboo.Guid.Empty,
                    name: self.name(),
                    values: JSON.stringify(values)
                }).then(function(res) {
                    if (window.parent.__gl && window.parent.__gl.saveHtmlblockFinish) {
                        values && window.parent.__gl.saveHtmlblockFinish(values[self.multiLangs().default]);
                    }
                });
            }

            submit();
        }
    };

    var blockId = Kooboo.getQueryString("nameOrId"),
        vm = new blockViewModel();

    if (blockId) {
        vm.blockId(blockId);
    } else {
        window.info.show(Kooboo.text.info.versionLogParameterMissing, false);
    }

    function adjustFrameHeight() {
        setTimeout(function() {
            var height = window.document.body.clientHeight + 200;

            height = Math.min((parent.window.innerHeight - 200), height);

            window.parent.Kooboo.EventBus.publish("kb/component/modal/set/height", { height: height });
        }, 400)
    }

    $.when(Kooboo.HtmlBlock.Get({
        name: blockId
    }), Kooboo.Site.Langs()).then(function(hbRes, langRes) {
        var r1 = hbRes[0],
            r2 = langRes[0];

        if (r1.success && r2.success) {
            vm.name(r1.model.name);
            vm.contentsValue(r1.model.values);
            vm.multiLangs(r2.model);
            vm.multiContents(r2.model);
            vm.bindSaveHtmlBlockEvent();
        }
    })

    $(window).on('resize',function(){
        adjustFrameHeight();
    })

    ko.applyBindings(vm, document.getElementById("mainDialog"));
})