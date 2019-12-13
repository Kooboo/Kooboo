(function() {
  var defaultOptions = {
    autoclose: true,
    format: "yyyy-mm-dd hh:ii",
    minuteStep: 5
  };

  Vue.directive("kb-datetimepicker", {
    bind: function(element, binding) {
      var options = {};
      _.extend(options, defaultOptions, binding.value || {});

      $(element)
        .datetimepicker(options)
        .on("changeDate", function(e) {
          // Seems datetimepicker assumes we are choosing utc dates :(
          // So e.date equals to toLocalDate(selected date)
          var selectedDate = moment(e.date)
            .utc()
            .format("YYYY-MM-DD HH:mm");
          element.value = selectedDate;
          Kooboo.trigger(element, "input");
        });
    },
    update: function(element, binding) {
      // if (!_.isEqual(binding.value, binding.oldValue)) {
      // var options = {};
      // _.extend(options, defaultOptions, binding.value || {});
      // }
      $(element).datetimepicker("update");
    }
  });
})();
