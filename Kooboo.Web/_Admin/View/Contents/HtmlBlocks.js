$(function() {

    var blockViewModel = function() {

        var self = this;

        this.onCreateUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.HtmlBlock.Create);
        });
    }

    blockViewModel.prototype = new Kooboo.tableModel(Kooboo.HtmlBlock.name);

    Kooboo.HtmlBlock.getList().then(function(res) {

        if (res.success) {
            var blockList = [];
            _.forEach(res.model, function(block) {
                var date = new Date(block.lastModified);

                var model = {
                    id: block.id,
                    name: {
                        text: block.name,
                        url: Kooboo.Route.Get(Kooboo.Route.HtmlBlock.DetailPage, {
                            id: block.id
                        })
                    },
                    date: date.toDefaultLangString(),
                    relationsComm: "kb/relation/modal/show",
                    relationsTypes: Object.keys(block.relations),
                    relations: block.relations,
                    versions: {
                        iconClass: "fa-clock-o",
                        title: Kooboo.text.common.viewAllVersions,
                        url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
                            KeyHash: block.keyHash,
                            storeNameHash: block.storeNameHash
                        }),
                        newWindow: true
                    }
                };

                blockList.push(model);
            });

            var columns = [{
                displayName: Kooboo.text.common.name,
                fieldName: "name",
                type: "link"
            }, {
                displayName: Kooboo.text.common.usedBy,
                fieldName: "relations",
                type: "communication-refer"
            }, {
                displayName: Kooboo.text.common.lastModified,
                fieldName: "date",
                type: "text"
            }];

            var cpnt = {
                docs: blockList,
                columns: columns,
                tableActions: [{
                    fieldName: "versions",
                    type: "link-icon"
                }],
                kbType: Kooboo.HtmlBlock.name
            };

            vm.tableData(cpnt);
        }
    });

    var vm = new blockViewModel();

    ko.applyBindings(vm, document.getElementById("main"));
});