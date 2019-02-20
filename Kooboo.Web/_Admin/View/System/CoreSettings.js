$(function() {
    var CoreSetting = function() {
        var self = this;

        this.settings = ko.observableArray();

        this.getList = function() {
            Kooboo.CoreSetting.getList().then(function(res) {
                if (res.success) {
                    self.settings(res.model.map(function(item) {
                        return {
                            name: item.name,
                            value: item.value
                        }
                    }))
                }
            })
        }

        this.showModal = ko.observable(false);
        this.fields = ko.observableArray();
        this.currentSettingName = ko.observable();
        this.onSave = function() {
            Kooboo.CoreSetting.update({
                name: self.currentSettingName(),
                model: Kooboo.arrToObj(self.fields(), 'name', 'value')
            }).then(function(res) {
                if (res.success) {
                    info.done(Kooboo.text.info.update.success); 
                    self.onClose(); 
                    self.getList(); 
                }
            })
        }
        this.onClose = function() {
            self.currentSettingName('');
            self.fields([]);
            self.showModal(false);
        }

        this.onEdit = function(name) {
            self.currentSettingName(name);
            Kooboo.CoreSetting.get({
                name: name
            }).then(function(res) {
                self.fields(Kooboo.objToArr(res.model, 'name', 'value'))
                self.showModal(true);
                self.getList();
            })
        }

        this.getList();
    }

    var vm = new CoreSetting();
    ko.applyBindings(vm, document.getElementById('main'));
})