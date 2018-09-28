$(function() {
    function CertificateModel() {
        var self = this;
        this.dialogShow = ko.observable(false);

        this.save = function() {
            Kooboo.Certificate.post({
                fullDomain: self.subDomain() + self.selectedDomain()
            }).then(function(res) {
                if (res.success) {
                    window.info.done(res.model);
                    self.cancelDialog();
                }
            })
        }

        this.showDialog = function() {
            self.dialogShow(true);
        }

        this.cancelDialog = function() {
            self.subDomain("");
            self.selectedDomain("");
            self.dialogShow(false);
        }

        this.getList = function() {
            Kooboo.Certificate.getList().then(function(res) {
                if (res.success) {
                    var docs = res.model.map(function(d) {
                        return {
                            id: d.id,
                            name: d.domainName,
                            expires: {
                                text: d.expires,
                                // class: 'label-sm label-green'
                                class: 'label-sm green'
                            }
                        }
                    })

                    self.tableData({
                        docs: docs,
                        columns: [{
                            fieldName: 'name',
                            displayName: Kooboo.text.common.name,
                            type: 'text'
                        }, {
                            fieldName: 'expires',
                            displayName: Kooboo.text.site.domain.expires,
                            type: 'label'
                        }],
                        actions: [],
                        kbType: Kooboo.Certificate.name
                    })
                }
            })
        }

        this.domainList = ko.observableArray();
        this.selectedDomain = ko.observable();

        this.subDomain = ko.observable("");

        Kooboo.Domain.getList().then(function(res) {
            if (res.success) {
                self.domainList(res.model.map(function(d) {
                    return {
                        id: d.id,
                        name: '.' + d.domainName
                    }
                }))
            }
        })
    }

    CertificateModel.prototype = new Kooboo.tableModel(Kooboo.Certificate.name);

    var vm = new CertificateModel();
    ko.applyBindings(vm, document.getElementById("main"));
    vm.getList();
})