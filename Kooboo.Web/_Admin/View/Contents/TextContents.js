$(function() {
    var TextContents = function() {
        var self = this;
        this.isSettingShow = ko.observable(false);

        this.folders = ko.observableArray();

        function dataMapping(data) {
            var tempArr = [];
            data.forEach(function(item) {
                tempArr.push({
                    id: item.id,
                    relationsComm: "kb/relation/modal/show",
                    relationsTypes: Object.keys(item.relations),
                    relations: item.relations,
                    folderName: {
                        text: item.displayName,
                        url: Kooboo.Route.Get(Kooboo.Route.TextContent.ByFolder, {
                            folder: item.id
                        })
                    },
                    edit: {
                        text: Kooboo.text.common.setting,
                        url: "/textcontent/contentfolder/edit"
                    }
                })
            })
            return tempArr;
        }

        Kooboo.EventBus.subscribe("/textcontent/contentfolder/edit", function(selectedFolder) {
            Kooboo.EventBus.publish("ko/textContent/folderSetting", selectedFolder.id);
        });

        self.getList = function() {
            Kooboo.ContentFolder.getList().then(function(data) {
                var ob = {
                    columns: [{
                        displayName: Kooboo.text.common.name,
                        fieldName: "folderName",
                        type: "link"
                    }, {
                        displayName: Kooboo.text.common.usedBy,
                        fieldName: "relations",
                        type: "communication-refer"
                    }],
                    tableActions: [{
                        fieldName: "edit",
                        type: "communication-btn"
                    }],
                    kbType: "ContentFolder"
                };

                ob.docs = dataMapping(data.model)
                self.tableData(ob);
                self.folders(data.model);
            })
        }

        this.newFolder = function() {
            Kooboo.EventBus.publish("ko/textContent/newFolder");
        }

        this.newContent = Kooboo.Route.Get(Kooboo.Route.TextContent.DetailPage);

        Kooboo.EventBus.subscribe("kb/textcontents/new/folder", function() {
            self.getList();
            Kooboo.EventBus.publish("kb/sidebar/refresh");
        })

        $("#J_NewFolder").on("show.bs.modal", function() {
            var $folder = $('table.table > tbody input:checkbox:checked[data-check-model="folders"]'),
                isExists = !$("#J_NewFolder").data("isnew") && $folder.length > 0,
                data = {};
            if (isExists) {
                data["Id"] = $folder[0].value;
                vm.init($folder[0].value);
            } else {
                vm.init(null);
            }
        });
    }

    Kooboo.EventBus.subscribe("kb/table/delete/finish", function() {
        Kooboo.EventBus.publish("kb/sidebar/refresh");
    })

    TextContents.prototype = new Kooboo.tableModel(Kooboo.ContentFolder.name);
    var vm = new TextContents();
    ko.applyBindings(vm, document.getElementById("main"));
    vm.getList();
})