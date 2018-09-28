(function() {

    var DataStore = Kooboo.viewEditor.store.DataStore,
        ActionStore = Kooboo.viewEditor.store.ActionStore;

    function Action(data) {
        var self = this;
        this.id = data.id;
        this.parentId = data.parentId;
        this.aliasName = data.aliasName;
        this.methodId = data.methodId;
        this.methodName = data.methodName;
        this.originalMethodName = data.originalMethodName;
        this.dataSourceName = data.dataSourceName;
        this.dataSourceDisplayName = data.dataSourceDisplayName;
        this.parameterMappings = data.parameterMappings;
        this.children = ko.observableArray();
        this.isOpen = ko.observable(true);
        this.isPublic = data.isPublic;
    }

    function getAllDataSources() {
        var list = DataStore.getAll(),
            hash = {},
            ret = [],
            item;
        _.forEach(list, function(it) {
            var act = ActionStore.byId(it.methodId);
            act && (hash[it.id] = new Action({
                id: it.id,
                methodId: act.id,
                methodName: act.methodName,
                originalMethodName: act.originalMethodName,
                aliasName: it.aliasName,
                dataSourceName: act.dataSourceName,
                dataSourceDisplayName: act.dataSourceDisplayName,
                parameterMappings: it.parameterMappings,
                parentId: it.parentId,
                isPublic: !!act.isPublic
            }));
        });
        _.forEach(hash, function(it, id) {
            var item;
            if (it.parentId !== Kooboo.Guid.Empty && (item = hash[it.parentId])) {
                item.children.push(it);
            } else {
                ret.push(it);
            }
        });

        return ret;
    }

    var DataSourcePanel = function() {
        var self = this;

        this.cfg = {
            viewId: null
        };

        this.setViewId = function(id) {
            self.cfg.viewId = id;
        }

        this.actions = ko.observableArray(getAllDataSources());

        this.add = function() {
            Kooboo.EventBus.publish("action/edit", {
                viewId: self.cfg.viewId,
                parentId: Kooboo.Guid.Empty,
                context: self
            })
        }

        this.addChild = function(item) {
            Kooboo.EventBus.publish("action/edit", {
                parentId: item.id,
                context: self
            });
        }

        this.edit = function(item) {
            Kooboo.EventBus.publish("action/edit", {
                parentId: item.parentId,
                name: item.aliasName,
                item: item,
                context: self,
                viewId: self.cfg.viewId,
            });
        }

        this.remove = function(item) {
            var ids = [];

            if (confirm(Kooboo.text.confirm.deleteItem)) {
                ids.push(item.id);
                if (!item.isPublic) {
                    ActionStore.deleteMethodById(item.methodId);
                }

                removeChildren(item, ids);

                DataStore.remove(_.uniq(ids.reverse()));
            }

            function removeChildren(item, ids) {
                if (item.children().length) {
                    _.forEach(item.children(), function(child) {
                        removeChildren(child, ids);
                        if (!child.isPublic) {
                            ActionStore.deleteMethodById(child.methodId);
                            ids.push(child.id)
                        }
                    })
                }
            }
        }

        this.save = function(data) {
            if (data.isEdit) {
                DataStore.updateParameterMappings({
                    id: data.id,
                    parameterMappings: data.parameterMappings
                });
            } else {
                DataStore.add({
                    methodId: data.methodId,
                    aliasName: data.name,
                    parentId: data.parentId || Kooboo.Guid.Empty,
                    parameterMappings: data.parameterMappings
                });
            }

            ActionStore.updateItemFieldsByMethodId(data.methodId, data.itemFields);

        }

        this.toggle = function(item) {
            var isOpen = item.isOpen();
            item.isOpen(!isOpen);
        }

        Kooboo.EventBus.subscribe("DataStore/change", function() {
            self.actions(getAllDataSources());
        })
    }

    Kooboo.viewEditor.viewModel.DataSourcePanel = DataSourcePanel;
})()