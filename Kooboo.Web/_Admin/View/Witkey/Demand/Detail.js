$(function() {

    var detailModel = function() {
        var self = this;

        this.id = ko.observable(Kooboo.getQueryString('id'));

        this.title = ko.observable();
        this.description = ko.observable();
        this.budget = ko.observable();
        this.skills = ko.observableArray();
        this.startDate = ko.observable();
        this.endDate = ko.observable();
        this.duration = ko.pureComputed(function() {
            return self.startDate() + ' - ' + self.endDate();
        })

        this.proposals = ko.observableArray();

        this.getDetail = function() {
            Kooboo.Demand.get({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.title(res.model.title);
                    self.description(res.model.description);
                    self.budget(res.model.budget);
                    self.skills(['Kooboo', 'CMS', 'Development', 'Views']);
                    // self.skills(res.model.skills);

                    var start = new Date(res.model.startDate);
                    self.startDate(start.toLocaleDateString());
                    var end = new Date(res.model.endDate);
                    self.endDate(end.toLocaleDateString());
                }
            })

            Kooboo.Demand.isProposalUser({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    debugger
                }
            })

            Kooboo.Demand.getProposalList({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.proposals(res.model.list);
                }
            })
        }

        this.getDetail();
    }

    var vm = new detailModel();
    ko.applyBindings(vm, document.getElementById('main'))
})