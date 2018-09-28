(function() {

    var _actions = [];

    var ActionStore = {
        init: function(actions) {
            _actions = actions;
            this.onChange();
        },
        getAll: function() {
            return _actions;
        },
        byId: function(id) {
            return _.chain(_actions).flatMap("methods").findLast({ "id": id }).value();
        },
        byFullName: function(fullName) {
            return _.chain(_actions).flatMap("methods").find({ "methodName": fullName }).value();
        },
        updateItemFieldsByMethodId: function(methodId, fields) {
            _actions.forEach(function(action) {
                action.methods.forEach(function(method) {
                    if (method.id == methodId) {
                        method.itemFields = fields;
                    }
                })
            })
            this.onChange();
        },
        add: function(action) {
            _actions.push(action);
            this.onChange();
        },
        addMethod: function(action) {
            var type = action.declareType;
            var ds = _.find(_actions, { type: type });
            if (ds) {
                ds.methods.push(action);
            }
            this.onChange();
        },
        deleteMethodById: function(id) {
            var ds = _.find(_actions, { type: this.byId(id).declareType });

            if (ds) {
                var idx = _.findIndex(ds.methods, function(method) {
                    return method.id == id;
                });

                if (idx > -1) {
                    ds.methods.splice(idx, 1);
                    this.onChange();
                    Kooboo.EventBus.publish("ActionStore/deleteMethod");
                }
            }
        },
        getAllMethods: function() {
            return _.chain(_actions).flatMap("methods").value();
        },
        onChange: function() {
            Kooboo.EventBus.publish("ActionStore/change");
        }
    };

    Kooboo.viewEditor.store.ActionStore = ActionStore;
})();