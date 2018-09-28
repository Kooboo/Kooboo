$(function() {

    var labelViewModel = function() {

        var self = this;

        this.lang = ko.observable();

        this._lang = ko.observable();
        this._lang.subscribe(function(l) {
            self.lang(self.multiLangs().cultures[l]);
        });

        this.multiLangs = ko.observable();

        this.multiBlocks = ko.observableArray();
        this.multiBlocks.subscribe(function(bls) {

            var blockList = [];
            _.forEach(bls, function(block) {
                var date = new Date(block.lastModified);

                var model = {
                    id: block.id,
                    name: {
                        text: block.name,
                        url: Kooboo.Route.Get(Kooboo.Route.HtmlBlock.MultiLangDetailPage, {
                            id: block.id,
                            lang: self._lang()
                        })
                    },
                    hasContent: {
                        text: block.values.hasOwnProperty(self._lang()) && block.values[self._lang()] ? Kooboo.text.common.yes : Kooboo.text.common.no,
                        class: "label-sm " + (block.values.hasOwnProperty(self._lang()) && block.values[self._lang()] ? "label-success" : "label-default")
                    },
                    date: date.toDefaultLangString()
                };

                blockList.push(model);
            });

            var columns = [{
                displayName: Kooboo.text.common.name,
                fieldName: "name",
                type: "link"
            }, {
                displayName: Kooboo.text.site.multiLang.translationState,
                fieldName: "hasContent",
                type: "label"
            }, {
                displayName: Kooboo.text.common.lastModified,
                fieldName: "date",
                type: "text"
            }];

            vm.tableData({
                docs: blockList,
                columns: columns,
                kbType: Kooboo.HtmlBlock.name
            })
        });
    }
    labelViewModel.prototype = new Kooboo.tableModel(Kooboo.HtmlBlock.name);

    var vm = new labelViewModel();

    var _lang = Kooboo.getQueryString("lang");

    Kooboo.Site.Langs().then(function(res) {

        if (res.success) {
            vm.multiLangs(res.model);
            vm._lang(_lang);

            Kooboo.HtmlBlock.getList().then(function(res) {

                if (res.success) {
                    vm.multiBlocks(res.model);
                }
            })
        }
    })


    ko.applyBindings(vm, document.getElementById("main"));
});