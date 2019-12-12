(function() {
  Vue.component("data-center-modal", {
    template: Kooboo.getTemplate(
      "/_Admin/View/Market/Scripts/components/DataCenterModal.html"
    ),
    props: {
      isShow: Boolean, // is-show.sync
      available: Array
    },
    data: function() {
      return {
        dataCenter: ""
      };
    },
    methods: {
      onSave: function() {
        var self = this;
        if (confirm(Kooboo.text.confirm.changeDataCenter)) {
          Kooboo.Organization.updateDataCenter({
            datacenter: self.dataCenter
          }).then(function(res) {
            if (res.success) {
              window.info.done(Kooboo.text.info.update.success);
              var loc = self.available.find(function(dc) {
                return dc.value == self.dataCenter;
              });
              Kooboo.EventBus.publish("kb/market/datacenter/updated", {
                loc: loc.displayName
              });
              self.onHide();
              setTimeout(function() {
                window.location.href =
                  res.model.redirectUrl || Kooboo.Route.User.LoginPage;
              }, 300);
            }
          });
        }
      },
      onHide: function() {
        this.dataCenter = "";
        this.$emit("update:isShow", false);
      }
    },
    watch: {
      isShow: function(val) {
        if (!val && this.available && this.available[0]) {
          this.dataCenter = this.available[0].value;
        }
      },
      available: function() {
        if (this.available && this.available[0]) {
          this.dataCenter = this.available[0].value;
        }
      }
    }
  });
})();
