$(function() {
    var ContentType = function() {
        var self = this;
        this.tableData = ko.observable({});
        this.createNewContentType = Kooboo.Route.Get(Kooboo.Route.ContentType.DetailPage);

        function dataMapping(data) {
            var d = [];
            data.forEach(function(item) {
                var ob = {};
                ob.name = {
                    text: item.name,
                    url: Kooboo.Route.Get(Kooboo.Route.ContentType.DetailPage, {
                        id: item.id
                    })
                };
                ob.field = {
                    text: item.propertyCount,
                    class: "blue"
                };
                ob.id = item.id;
                d.push(ob);
            })
            return d
        }

        Kooboo.ContentType.getList().then(function(data) {
            var ob = {
                columns: [{
                    displayName: Kooboo.text.common.name,
                    fieldName: "name",
                    type: "link"
                }, {
                    displayName: Kooboo.text.site.contentType.fields,
                    fieldName: "field",
                    type: "badge"
                }],
                kbType: "ContentType"
            }
            ob.docs = dataMapping(data.model)
            self.tableData(ob);
        })

        Kooboo.EventBus.subscribe("kb/table/delete/finish", function() {
            Kooboo.EventBus.publish("kb/sidebar/refresh");
        })
    }

    ContentType.prototype = new Kooboo.tableModel(Kooboo.Page.name);
    ko.applyBindings(new ContentType, document.getElementById("main"));
})