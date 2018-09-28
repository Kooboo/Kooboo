(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/pageEditor/components/parameters.html");

    ko.components.register("kb-page-parameters", {
        viewModel: function(params) {

            var self = this;

            this.defaultRouteValues = ko.observableArray();

            this.addDefaultRouteValue = function() {
                self.defaultRouteValues.push({
                    name: ko.observable(),
                    value: ko.observable()
                });
                Kooboo.EventBus.publish("kb/page/field/change", {
                    type: "url"
                })
            }

            this.removeDefaultRouteValue = function(routeValue) {
                self.defaultRouteValues.remove(routeValue);
                Kooboo.EventBus.publish("kb/page/field/change", {
                    type: "url"
                })
            }

            if (params) {

                if (params.parameters) {
                    for (var name in params.parameters) {
                        self.defaultRouteValues.push({
                            name: ko.observable(name),
                            value: ko.observable(params.parameters[name])
                        });
                    }
                }

                Kooboo.EventBus.subscribe("kb/page/save", function(res) {
                    res["parameters"] = {};

                    _.forEach(self.defaultRouteValues(), function(drv) {
                        res["parameters"][drv.name()] = drv.value();
                    })

                    Kooboo.EventBus.publish("kb/page/final/save", res);
                })
            }
        },
        template: template
    })
})()