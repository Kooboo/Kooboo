$(function() {
    var TableViewModel = function() {
        var self = this;
        this.publicDataSource = ko.observableArray([]);
        this.privateDataSource = ko.observableArray([]);

        this.deleteMethod = function(method) {
            if (confirm(Kooboo.text.confirm.deleteItem)) {
                Kooboo.DataSource.Delete({
                    Id: method.id
                }).then(function(res) {
                    if (res.success) {
                        self.init();
                    }
                });
            }
        }

        function getDataWithFilter(dataSource, excludeEmptyName) {
            dataSource.forEach(function(data, index) {
                data.level = "Level1-" + index;
                data.methods.forEach(function(method) {
                    method.des = renderDescription(method);
                    method.editUrl = getEditUrl(method);
                    method.newMethodUrl = getCreateUrl(method);
                });

                if (excludeEmptyName) {
                    data.methods = _.filter(data.methods, function(method) {
                        return !!method.viewName;
                    })
                }
            });
            return dataSource;
        }

        function renderDescription(method) {
            var str = "";
            if (!!method.description) {
                str += "<p>" + method.Description + "</p>";
            }
            if (method.parameters !== null) {
                str += "<table class=\"table table-condensed\">";
                _.forEach(method.parameters, function(value, key) {
                    str += "<tr><th>" + key + "</th><td>" + value + "</td></tr>";
                });
                str += "</table>";
            }
            return str;
        }

        function getUrl(method, type) {
            return Kooboo.Route.Get(Kooboo.Route.DataSource.DataMethodSetting, {
                id: method.id,
                isNew: (type.toLowerCase() == "edit") ? method.isNew : true
            });
        }

        function getEditUrl(method) {
            return getUrl(method, "edit");
        }

        function getCreateUrl(method) {
            return getUrl(method, "new");
        }

        this.init = function() {
            $.when(Kooboo.DataSource.getPublicData(), Kooboo.DataSource.getPrivateData()).then(function(publicRes, privateRes) {
                var publicData = getDataWithFilter(publicRes[0].model, false),
                    privateData = getDataWithFilter(privateRes[0].model, true);
                self.publicDataSource(publicData);
                self.privateDataSource(privateData);
            })
        }
        this.init();

        this.getRelation = function(ds, type) {
            Kooboo.EventBus.publish("kb/relation/modal/show", {
                id: ds.id,
                by: type,
                type: "DataMethodSetting"
            });
        }
    }
    var tableViewModel = new TableViewModel();
    ko.applyBindings(tableViewModel, $("#main")[0]);
    $('body').on("click", ".table-tree-toggle", function(e) {
        var t = e.target;
        e.preventDefault();
        $(t).find('.fa-caret-down, .fa-caret-right').toggleClass('fa-caret-right')
            .toggleClass('fa-caret-down');
        var nodeName = $(t).parents('tr').data('name');
        $(t).parents('table').find('[data-parent="' + nodeName + '"]').toggle();
    });
});