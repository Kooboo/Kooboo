(function() {
  var self;
  Vue.component("kb-field-validation", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/kbFieldValidation.html"
    ),
    props: {
      field: {}
    },
    data: function() {
      return {};
    },
    created: function() {
      self = this;
    },
    watch: {},
    methods: {}
  });
})();

