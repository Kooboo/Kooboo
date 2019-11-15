(function() {
  Kooboo.loadJS([
    "/_Admin/Scripts/lib/bootstrap-switch/bootstrap-switch.min.js",
    "/_Admin/Scripts/vue-components/kbSwitch.js"
  ]);
  Kooboo.loadCSS([
    "/_Admin/Scripts/lib/bootstrap-switch/bootstrap-switch.min.css"
  ]);
  // TODO: check
  Vue.component("kb-control-switch", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/controlType/Switch.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"]
  });
})();

// (function() {
//   if ($.fn.bootstrapSwitch) {
//     $.fn.bootstrapSwitch.defaults.onText = Kooboo.text.common.yes;
//     $.fn.bootstrapSwitch.defaults.offText = Kooboo.text.common.no;
//   }

//   Kooboo.loadJS([
//     "/_Admin/Scripts/lib/bootstrap-switch/bootstrap-switch.min.js"
//   ]);
//   Kooboo.loadCSS([
//     "/_Admin/Scripts/lib/bootstrap-switch/bootstrap-switch.min.css"
//   ]);

//   var template = Kooboo.getTemplate(
//     "/_Admin/Scripts/components/controlType/Switch.html"
//   );

//   ko.components.register("switch", {
//     viewModel: function(params) {
//       var self = this;
//       _.assign(this, params);
//       setTimeout(function() {
//         $("#" + self._id())
//           .bootstrapSwitch()
//           .on("switchChange.bootstrapSwitch", function(e, data) {
//             self.fieldValue(data);
//           });
//       }, 0);
//     },
//     template: template
//   });
// })();
