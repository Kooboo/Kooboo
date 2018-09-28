(function() {

    var template = Kooboo.getTemplate("/_Admin/Scripts/pageEditor/widgets/component-selector.html"),
        ComponentStore = Kooboo.pageEditor.store.ComponentStore;

    ko.components.register("kb-page-widget-component-selector", {
        viewModel: function() {

            var self = this;

            this.isShow = ko.observable(false);

            this.title = ko.observable();

            this.selectContext = ko.observable();

            this.emptyMode = ko.observable(false);

            this.name = ko.validateField({
                required: Kooboo.text.validation.required
            });

            this.nameList = ko.observableArray();
            this.nameList.subscribe(function(list) {
                self.name(!list.length ? null : "vaild");
            })

            this.showError = ko.observable(false);

            this.showParameters = ko.observable();

            this.settings = ko.observable();

            this.reset = function() {
                self.showError(false);
                self.showParameters(false);
                self.isShow(false);
                self.name(null);
                self.nameList.removeAll();
            }

            this.isValid = function() {
                return self.name.isValid();
            }

            this.save = function() {
                if (self.isValid()) {
                    var list = [];
                    _.forEach(self.nameList(), function(data) {
                        data.type = self.selectContext().type;
                        data.engine = self.selectContext().engine;
                        list.push(data);
                    });
                    Kooboo.EventBus.publish("kb/page/layout/components/save", list);
                    Kooboo.EventBus.publish("kb/page/field/change", { type: "resource" });
                    self.reset();
                } else {
                    self.showError(true);
                }
            }

            Kooboo.EventBus.subscribe("kb/page/layout/component/select", function(context) {
                self.title(context.displayName);
                self.selectContext(context);
                $("#component_tree").jstree("destroy");
                if (context.data && context.data.length) {
                    self.emptyMode(false);
                    $("#component_tree").jstree({
                        "plugins": ["types", "conditionalselect", "checkbox"],
                        "types": {
                            "default": {
                                "icon": "fa fa-file icon-state-warning"
                            }
                        },
                        "core": {
                            "strings": { 'Loading ...': Kooboo.text.common.loading + ' ...' },
                            "data": function(obj, cb) {
                                var treeData = [];
                                _.forEach(context.data, function(data) {
                                    treeData.push({
                                        text: data.name,
                                        data: {
                                            id: data.id,
                                            name: data.name,
                                            settings: data.settings
                                        }
                                    })
                                })
                                cb.call(this, treeData);
                            }
                        }
                    }).on("select_node.jstree", function(e, selected) {
                        var selectedNode = selected.node,
                            selectedData = selectedNode.data;
                        self.nameList.push(selected.node.data);
                        self.settings(selectedData.settings);
                        self.showParameters(Object.keys(selectedData.settings).length);
                    }).on("deselect_node.jstree", function(e, selected) {
                        self.nameList.remove(selected.node.data);
                        self.showParameters(false);
                        self.settings(null);
                    });
                } else {
                    self.emptyMode(true);
                }
                self.isShow(true);
            })
        },
        template: template
    })
})()