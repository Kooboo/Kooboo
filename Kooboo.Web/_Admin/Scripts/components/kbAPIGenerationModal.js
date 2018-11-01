(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbAPIGeneretionModal.html");

    ko.components.register('kb-api-generation-modal', {
        viewModel: function(params) {
            var self = this;

            this.isShow = params.isShow;
            this.isShow.subscribe(function(show) {
                if (show) {
                    Kooboo.APIGeneration.getObjects().then(function(res) {
                        if (res.success) {
                            self.handleData(res.model);
                        }
                    })
                }
            })
            this.onHide = function() {
                self.currentStepIdx(0);
                self.types([]);
                self.isShow(false);
            }

            this.handleData = function(data) {
                var groupedData = _.groupBy(data, 'type');

                self.types(Kooboo.objToArr(groupedData).map(function(data) {
                    return new Type({
                        name: data.key,
                        displayName: data.value.length ? data.value[0].typeDisplayName : data.key,
                        items: data.value
                    })
                }))
            }

            this.types = ko.observableArray();

            this.steps = ko.observableArray(['Choose items', 'Choose actions', Kooboo.text.common.confirm])
            this.currentStepIdx = ko.observable(0);

            this.onPrev = function() {
                self.currentStepIdx(self.currentStepIdx() - 1);
            }
            this.onNext = function() {
                self.currentStepIdx(self.currentStepIdx() + 1);
            }
            this.onConfirm = function() {
                let list = [];
                self.types().forEach(function(type) {
                    type.items().forEach(function(item) {
                        if (item.selected()) {
                            var model = {
                                type: type.name(),
                                name: item.name(),
                                actions: item.actions().filter(function(action) {
                                    return action.selected()
                                }).map(function(action) {
                                    return action.type()
                                })
                            };

                            model.actions.length && list.push(model);
                        }
                    })
                })

                Kooboo.APIGeneration.Generate({
                    updateModel: list
                }).then(function(res) {
                    if (res.success) {
                        window.info.done('Generate successful');
                        self.onHide();
                        Kooboo.EventBus.publish('kb/code/refresh')
                    }
                })
            }
        },
        template: template
    })

    function Type(data) {
        this.name = ko.observable(data.name);
        this.displayName = ko.observable(data.displayName);
        this.items = ko.observableArray(data.items.map(function(item) {
            return new Item(item);
        }))
    }

    function Item(data) {
        var self = this;
        this.type = ko.observable(data.type);
        this.name = ko.observable(data.name);
        this.displayName = ko.observable(data.displayName);
        this.actions = ko.observableArray(data.actions.map(function(act) {
            return new Action(act);
        }))

        this.selected = ko.observable(false);
        this.onToggleItem = function() {
            self.selected(!self.selected());
        }
    }

    function Action(data) {
        this.type = ko.observable(data);

        this.selected = ko.observable(false);
    }
})()