$(function() {
  var self;
  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        dataType: "",
        title1: "",
        title2: "",
        id1: "",
        id2: "",
        source1: "",
        source2: ""
      };
    },
    methods: {
      compare: function() {
        switch (self.dataType) {
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
                setValue(self.source1 || "");
              },
              rhs: function(setValue) {
                setValue(self.source2 || "");
              }
            });
            break;
          case 1:

          default:
        }
      }
    },
    mounted: function() {
      Kooboo.SiteLog.Compare({
        id1: Kooboo.getQueryString("id1"),
        id2: Kooboo.getQueryString("id2")
      }).then(function(res) {
        if (res.success) {
          self.dataType = res.model.dataType;
          self.title1 = res.model.title1;
          self.title2 = res.model.title2;
          self.id1 = res.model.id1;
          self.id2 = res.model.id2;
          self.source1 = res.model.source1;
          self.source2 = res.model.source2;
          self.compare();
        }
      });
    },
    computed: {
      backUrl() {
        return Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
          KeyHash: Kooboo.getQueryString("KeyHash"),
          StoreNameHash: Kooboo.getQueryString("StoreNameHash"),
          tableNameHash: Kooboo.getQueryString("tableNameHash")
        });
      }
    },
    beforeDestory: function() {
      self = null;
    }
  });
});
