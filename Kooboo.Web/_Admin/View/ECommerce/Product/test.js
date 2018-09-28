$(function() {
    var testModel = function() {
        var self = this;

        this.onFieldModalShow = ko.observable(false);

        this.fieldData = ko.observable();

        this.onFieldSave = function(field) {
            debugger;
        }

        this.onShowFieldEditor = function() {
            this.fieldData({});
            this.onFieldModalShow(true);
        }
        this.onShowFieldEditor2 = function() {
            this.fieldData({
                name: "test",
                controlType: "TextBox",
                validations: '[{ "type": "required", "msg": "TEST" }]'
            })
            this.onFieldModalShow(true);
        }

    }

    var vm = new testModel();
    ko.applyBindings(vm, document.getElementById('main'));
})