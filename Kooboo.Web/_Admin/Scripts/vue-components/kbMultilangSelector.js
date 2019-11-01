(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbMultilangSelector.html");

    ko.components.register("kb-multilang-selector", {
        viewModel: function(params) {
            /*
             * params:{
             *  cultures: {
             *      aaet: "Afar (Ethiopia)",
             *      enus: "English"
             *  },
             *  default: "enus",
             *  defaultName: "English"
             * }
             */

            var self = this;

            this.defaultCulture = ko.observable((params && params.default));

            var _culturesArr = Kooboo.objToArr((params && params.cultures) || []),
                _cultures = [];

            var defaultCultureIdx = _.findIndex(_culturesArr, function(c) {
                return c.key == self.defaultCulture();
            });

            if (defaultCultureIdx > -1) {
                _cultures.push(new cultureModel(_culturesArr[defaultCultureIdx], self));
                _culturesArr.splice(defaultCultureIdx, 1);
            }

            _.forEach(_culturesArr, function(culture) {
                _cultures.push(new cultureModel(culture, self));
            });

            this.cultures = ko.observableArray(_cultures);

            if (params) {

                Kooboo.EventBus.subscribe("kb/multilang/change", function(target) {
                    var lang = _.findLast(self.cultures(), function(lang) {
                        return lang.key() == target.name
                    })

                    lang && lang.selected(target.selected);
                })

                Kooboo.EventBus.subscribe("kb/multilang/get", function() {
                    _.forEach(self.cultures(), function(cul) {
                        cul.selected() && Kooboo.EventBus.publish("kb/multilang/change", {
                            name: cul.key(),
                            fullName: cul.value(),
                            selected: true
                        })
                    })
                })
            }
        },
        template: template
    });

    var cultureModel = function(cul, culList) {

        var self = this;

        ko.mapping.fromJS(cul, {}, self);

        self.selected = ko.observable(self.key() == culList.defaultCulture());
        self.selected.subscribe(function(selected) {
            /* publish language-change event */
            Kooboo.EventBus.publish("kb/multilang/change", {
                name: self.key(),
                fullName: self.value(),
                selected: selected
            })
        });
    }
})();