(function() {
  var DataStore = Kooboo.viewEditor.store.DataStore,
    ActionStore = Kooboo.viewEditor.store.ActionStore;

  function Action(data) {
    this.id = data.id;
    this.parentId = data.parentId;
    this.aliasName = data.aliasName;
    this.methodId = data.methodId;
    this.methodName = data.methodName;
    this.originalMethodName = data.originalMethodName;
    this.dataSourceName = data.dataSourceName;
    this.dataSourceDisplayName = data.dataSourceDisplayName;
    this.parameterMappings = data.parameterMappings;
    this.children = [];
    this.isOpen = true;
    this.isPublic = data.isPublic;
  }

  function getAllDataSources() {
    var list = DataStore.getAll(),
      hash = {},
      ret = [],
      item;
    _.forEach(list, function(it) {
      var act = ActionStore.byId(it.methodId);
      act &&
        (hash[it.id] = new Action({
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

  var DataSourcePanel = Vue.component("kb-view-datasource", {
    data: function() {
      return {
        cfg: {
          viewId: null
        },
        actions: getAllDataSources()
      };
    },
    created: function() {
      var self = this;
      Kooboo.EventBus.subscribe("DataStore/change", function() {
        self.actions = getAllDataSources();
      });
    },
    methods: {
      setViewId: function(id) {
        this.cfg.viewId = id;
      },
      add: function() {
        var self = this;
        Kooboo.EventBus.publish("action/edit", {
          viewId: self.cfg.viewId,
          parentId: Kooboo.Guid.Empty,
          context: self
        });
      },
      addChild: function(item) {
        var self = this;
        Kooboo.EventBus.publish("action/edit", {
          parentId: item.id,
          context: self
        });
      },
      edit: function(item) {
        var self = this;
        Kooboo.EventBus.publish("action/edit", {
          parentId: item.parentId,
          name: item.aliasName,
          item: item,
          context: self,
          viewId: self.cfg.viewId
        });
      },
      removeChildren: function(item, ids) {
        var self = this;
        if (item.children.length) {
          _.forEach(item.children, function(child) {
            self.removeChildren(child, ids);
            if (!child.isPublic) {
              ActionStore.deleteMethodById(child.methodId);
              ids.push(child.id);
            }
          });
        }
      },
      remove: function(item) {
        var ids = [];
        if (confirm(Kooboo.text.confirm.deleteItem)) {
          ids.push(item.id);
          if (!item.isPublic) {
            ActionStore.deleteMethodById(item.methodId);
          }
          this.removeChildren(item, ids);
          DataStore.remove(_.uniq(ids.reverse()));
        }
      },
      save: function(data) {
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
      },
      toggle: function(item) {
        item.isOpen = !item.isOpen;
      }
    }
  });

  Kooboo.viewEditor.viewModel.DataSourcePanel = DataSourcePanel;
})();
