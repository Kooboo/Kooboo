(function() {
  var template = Kooboo.getTemplate(
    "/_Admin/Scripts/components/kbUploadProgressor.html"
  );

  Vue.component("kb-upload-progressor", {
    template: template,
    props: {
      percentage: Number
    },
    computed: {
      percentageString: function() {
        return (this.percentage * 100).toFixed(2) + "%";
      }
    }
  });
})();
