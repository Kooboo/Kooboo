$(function() {
    var self;
    new Vue({
        el: "#app",
        data: function() {
            return {
                breads: [
                    {
                        name: Kooboo.text.component.breadCrumb.sites
                    },
                    {
                        name: Kooboo.text.component.breadCrumb.dashboard
                    },
                    {
                        name: Kooboo.text.common.ContentTypes
                    }
                ],
                tableData: [],
                tableDataSelected: [],
                showCreateModel: false,
                createNewContentTypeUrl: Kooboo.Route.Get(
                    Kooboo.Route.ContentType.DetailPage
                )
            };
        },
        created: function() {
            self = this;
            self.getTableData();
        },
        methods: {
            getTableData: function() {
                Kooboo.ContentType.getList().then(function(res) {
                    self.tableData = res.model;
                });
            },
            getConfirmMessage: function(doc) {
                if (doc.relations) {
                    doc.relationsTypes = _.sortBy(Object.keys(doc.relations));
                }
                var find = _.find(doc, function(item) {
                    return item.relations && Object.keys(item.relations).length;
                });

                if (!!find) {
                    return Kooboo.text.confirm.deleteItemsWithRef;
                } else {
                    return Kooboo.text.confirm.deleteItems;
                }
            },
            getDetailUrl: function(id){
               return  Kooboo.Route.Get(Kooboo.Route.ContentType.DetailPage, {
                    id: id
                })
            },
            onDelete: function() {
                if (confirm(this.getConfirmMessage(this.tableDataSelected))) {
                    var ids = this.tableDataSelected.map(function(m) {
                        return m.id;
                    });
                    Kooboo.ContentType.Deletes({
                        ids: ids
                    }).then(function(res) {
                        if (res.success) {
                            //self.getTableData();
                            location.reload();
                            window.info.done(Kooboo.text.info.delete.success);
                        } else {
                            window.info.done(Kooboo.text.info.delete.fail);
                        }
                    });
                }
            }
        }
    });
});
