$(function() {
  var compareViewModel = function() {
    var self = this;

    this.dataType = ko.observable();

    this.title1 = ko.observable();

    this.title2 = ko.observable();

    this.id1 = ko.observable();

    this.id2 = ko.observable();

    this.source1 = ko.observable();

    this.source2 = ko.observable();

    this.compare = function() {
      switch (self.dataType()) {
        case 0:
          $("#compare").mergely({
            cmsettings: {
              readOnly: true,
              lineNumbers: true
            },
            width: "100%",
            height: "auto",
            sidebar: false,
            lhs: function(setValue) {
              setValue(self.source1() || "");
            },
            rhs: function(setValue) {
              setValue(self.source2() || "");
            }
          });
          break;
        case 1:

        default:
      }
    };

    this.backUrl = ko.pureComputed(function() {
      return Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
        KeyHash: Kooboo.getQueryString("KeyHash"),
        StoreNameHash: Kooboo.getQueryString("StoreNameHash"),
        tableNameHash: Kooboo.getQueryString("tableNameHash")
      });
    });
  };

  var vm = new compareViewModel();

  Kooboo.SiteLog.Compare({
    id1: Kooboo.getQueryString("id1"),
    id2: Kooboo.getQueryString("id2")
  }).then(function(res) {
    if (res.success) {
      vm.dataType(res.model.dataType);
      vm.title1(res.model.title1);
      vm.title2(res.model.title2);
      vm.id1(res.model.id1);
      vm.id2(res.model.id2);
      vm.source1(res.model.source1);
      vm.source2(res.model.source2);
      vm.compare();
    }
  });

  ko.applyBindings(vm, document.getElementById("main"));
});
