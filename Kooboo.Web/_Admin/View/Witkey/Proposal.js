$(function() {

    var viewModel = function() {
        var self = this;

        this.curType = ko.observable();
        this.curType.subscribe(function(type) {
            if (type) {
                Kooboo.Demand.getMyProposalList({
                    type: type
                }).then(function(res) {
                    if (res.success) {
                        self.handleRecords(res.model);
                    }
                })
            }
        })
        this.proposalTypes = ko.observableArray();

        this.pager = ko.observable();
        this.records = ko.observableArray();

        this.getProposalTypes = function(cb) {
            Kooboo.Demand.getProposalTypes().then(function(res) {
                if (res.success) {
                    self.proposalTypes(Kooboo.objToArr(res.model, 'value', 'displayName'));
                    cb && cb();
                }
            })
        }

        this.getProposalTypes(function() {
            self.curType(self.proposalTypes()[0].value);
        });

        this.handleRecords = function(data) {
            self.pager(data);
            self.records(data.list);
        }

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            Kooboo.Demand.getMyProposalList({
                type: self.curType(),
                pageNr: page
            }).then(function(res) {
                if (res.success) {
                    self.handleRecords(res.model);
                }
            })
        })
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})