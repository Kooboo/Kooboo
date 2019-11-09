(function() {
  function trigger(el, type) {
    // copy from vue.js line 8578
    var e = document.createEvent("HTMLEvents");
    e.initEvent(type, true, true);
    el.dispatchEvent(e);
  }
  var defaultOptions = {
    autoclose: true,
    format: "yyyy-mm-dd hh:ii",
    minuteStep: 5
  };

  Vue.directive("kb-datetimepicker", {
    bind: function(element, binding) {
      var options = {};
      if (binding.value) {
        _.extend(options, defaultOptions, binding.value);
      }
      $(element)
        .datetimepicker(options)
        .on("changeDate", function(e) {
          // Seems datetimepicker assumes we are choosing utc dates :(
          // So e.date equals to toLocalDate(selected date)
          var selectedDate = moment(e.date)
            .utc()
            .format("YYYY-MM-DD HH:mm");
          element.value = selectedDate;
          trigger(element, "input");
        });
    },
    update: function(element, binding) {
      var options = {};
      if (binding.value) {
        _.extend(options, defaultOptions, binding.value);
      }
      $(element).datetimepicker("update");
    }
  });
})();
