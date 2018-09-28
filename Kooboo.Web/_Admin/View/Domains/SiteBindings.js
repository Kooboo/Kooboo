$(function() {
    function Domain() {
        var self = this;
        this.rootDomain = ko.observableArray([]);
        this.root = ko.observable("");
        this.subdomain = ko.observable("");
        this.modalShow = ko.observable(false);


        function dataMapping(data) {
            var arr = [];
            arr = data.map(function(o) {
                return {
                    name: {
                        text: o.name,
                        url: Kooboo.Route.Get(Kooboo.Route.Domain.SiteBindingSettings, {
                            SiteId: o.id
                        })
                    },
                    bindingCount: {
                        text: o.bindingCount,
                        class: "blue"
                    }
                }
            })
            return arr;
        }

        this.save = function() {
            Kooboo.Binding.post({
                subdomain: self.subdomain(),
                rootdomain: self.root()
            }).then(function() {
                window.location.reload();
            })
        }

        Kooboo.Domain.getList().then(function(res) {
            self.rootDomain(res.model)
        })

        Kooboo.Binding.SiteBinding().then(function(res) {
            if (res.success) {
                var ob = {
                    columns: [{
                        displayName: Kooboo.text.site.domain.site,
                        fieldName: "name",
                        type: "link"
                    }, {
                        displayName: Kooboo.text.site.domain.domains,
                        fieldName: "bindingCount",
                        type: "badge"
                    }],
                    kbType: "Binding",
                    unselectable: true
                }

                ob.docs = res.model ? dataMapping(res.model) : [];
                self.tableData(ob);
            }
        })
    }
    Domain.prototype = new Kooboo.tableModel();
    ko.applyBindings(new Domain, document.getElementById("main"));
})