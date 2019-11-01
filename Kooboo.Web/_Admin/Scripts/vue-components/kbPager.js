(function() {
    var template = Kooboo.getTemplate("/_Admin/Scripts/components/kbPager.html");

    ko.components.register("kb-pager", {
        viewModel: function(params) {
            var self = this;

            var _pages = [];
            if (params) {

                if (params.totalPages > 5) {

                    if (params.pageNr > 3 && params.pageNr + 2 < params.totalPages) {
                        _pages.push({
                            count: "«",
                            target: params.pageNr - 3,
                            active: false
                        });

                        for (var i = 0; i < 5; i++) {
                            _pages.push({
                                count: params.pageNr - 2 + i,
                                target: params.pageNr - 2 + i,
                                active: i == 2
                            })
                        }

                        _pages.push({
                            count: "»",
                            target: params.pageNr + 3,
                            active: false
                        });
                    } else {

                        if (params.pageNr <= 3) {
                            for (var i = 1; i <= 5; i++) {
                                _pages.push({
                                    count: i,
                                    target: i,
                                    active: i == params.pageNr
                                })
                            }

                            _pages.push({
                                count: "»",
                                target: 6,
                                active: false
                            })
                        }

                        if (params.pageNr + 2 >= params.totalPages) {

                            for (var i = params.totalPages; i > params.totalPages - 5; i--) {
                                _pages.push({
                                    count: i,
                                    target: i,
                                    active: i == params.pageNr
                                })
                            }

                            _pages.push({
                                count: "«",
                                target: params.totalPages - 5,
                                active: false
                            })

                            _pages.reverse();
                        }
                    }

                } else if (params.totalPages <= 5) {
                    for (var i = 1; i <= params.totalPages; i++) {
                        _pages.push({
                            count: i,
                            target: i,
                            active: i == params.pageNr
                        })
                    }
                }
            }

            this.totalPages = ko.observableArray(params && _pages);

            this.currentPage = ko.observable(params && params.pageNr);

            this.changePage = function(page) {
                Kooboo.EventBus.publish("kb/pager/change", page.target);
            }
        },
        template: template
    });
})();