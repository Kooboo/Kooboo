(function() {
    var Filter = function() {};

    Filter.prototype.getFieldList = function(field, list) {
        var self = this;
        !list && (list = []);
        if (field.enumerable) {
            list = list.concat([
                { name: field.name, enumerable: true, id: field.dataId },
                { name: field.name + '.Count()', enumerable: false, id: field.dataId, type: "Number" }
            ]);

            if (field.isPagedResult) {
                list = list.concat([
                    { name: field.name + '.CurrentPage', enumerable: false, id: field.dataId, type: "Number" },
                    { name: field.name + '.TotalPages', enumerable: false, id: field.dataId, type: "Number" },
                    { name: field.name + '.Pages', enumerable: true, id: field.dataId },
                ])
            }
        } else {
            if (field.isPagedResult && field.name.indexOf("_Pages_Item") > -1) {
                field.itemFields = [];
                list.push({ name: field.name, enumerable: false, type: "Number" })
            }

            if (field.isComplexType) {
                var itemFields = field["itemFields"] || field.fields;
                itemFields.forEach(function(item) {
                    itemList = self.getFieldList(item, []);
                    itemList.forEach(function(li) {
                        li.name = field.name + '.' + li.name;
                        li.id = field.dataId;
                        li.enumerable = li.hasOwnProperty("enumerable") ? li.enumerable : field.enumerable;
                        li.type = li.type
                    });
                    list = list.concat(itemList);
                })
            } else {
                list.push({ name: field.name, enumerable: field.enumerable, type: field.type });
            }

            if (field.children && field.children.length) {
                field.children.forEach(function(child) {
                    child.name = field.name + '.' + child.name;
                    list = list.concat(self.getFieldList(child, []));
                })
            }
        }

        return list;
    }

    Filter.prototype.getEnumerableList = function(field) {
        var allList = this.getFieldList(field, []),
            res = [];

        allList.forEach(function(li) {
            li.enumerable && res.push(li);
        })

        return res;
    };

    Filter.prototype.getNotEnumerableList = function(field) {
        var allList = this.getFieldList(field, []),
            res = [];

        allList.forEach(function(li) {
            !li.enumerable && res.push(li);
        })

        return res;
    }

    Kooboo.viewEditor.util.fieldFilter = new Filter();
})()