$(function() {
    var transferringModel = function() {
        var self = this;

        this.sitesUrl = ko.pureComputed(function() {
            return Kooboo.Route.Get(Kooboo.Route.Site.ListPage);
        });

        this.persentText = ko.observable();

        this.statusList = ko.observableArray();
    }

    var vm = new transferringModel(),
        taskId = Kooboo.getQueryString("TaskId");

    ko.applyBindings(vm, document.getElementById("main"));

    setInterval(function() {
        Kooboo.Transfer.getStatus({
            id: taskId
        }).then(function(res) {

            if (res.success) {
                vm.statusList(res.model);

                var total = res.model.length,
                    done = _.filter(res.model, function(m) {
                        return m.done;
                    }).length;

                var persent = done * 100 / total;
                if (persent !== 100) {
                    persent = Math.floor(persent);
                }
                vm.persentText(persent + "%");
                if (persent == 100) {
                    location.href = Kooboo.Route.Get(Kooboo.Route.Site.ListPage);
                }
            }
        })
    }, 500)
})