(function() {
    Kooboo.loadJS([
        "/_Admin/Scripts/lib/bootstrap-datetimepicker.js",
        "/_Admin/Scripts/kobindings.datePicker.js"
    ])

    var template = Kooboo.getTemplate("/_Admin/Scripts/components/controlType/DateTime.html");

    ko.components.register("date-time", {
        viewModel: function(params) {
            var self = this;
            _.assign(this, params);

            this.showValue = ko.observable(this.fieldValue() ? convertToTime(new Date(this.fieldValue())) : "");
            this.showValue.extend({ validate: this.validateRules });
            this.showValue.subscribe(function(v) {
                var d = new Date(v);
                self.fieldValue(d.toISOString());
            })

            this.preventInput = function(m, e) {
                e.preventDefault();
            }

            function convertToTime(date) {
                var y = date.getFullYear(),
                    m = (date.getMonth() + 1) < 10 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1),
                    d = date.getDate() < 10 ? "0" + date.getDate() : date.getDate(),
                    dateStr = y + "-" + m + "-" + d,
                    hh = date.getHours() < 10 ? "0" + date.getHours() : date.getHours(),
                    mm = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes(),
                    timeStr = hh + ":" + mm;
                return dateStr + " " + timeStr;
            }
        },
        template: template
    })
})()