$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: "Core settings"
          }
        ],
        settings: [],
        showModal: false,
        fields: [],
        currentSettingName: ""
      };
    },
    methods: {
      getList: function() {
        Kooboo.CoreSetting.getList().then(function(res) {
          if (res.success) {
            self.settings = res.model.map(function(item) {
              return {
                name: item.name,
                value: item.value
              };
            });
          }
        });
      },
      onSave: function() {
        Kooboo.CoreSetting.update({
          name: self.currentSettingName,
          model: Kooboo.arrToObj(self.fields, "name", "value")
        }).then(function(res) {
          if (res.success) {
            info.done(Kooboo.text.info.update.success);
            self.onClose();
            self.getList();
          }
        });
      },
      onClose: function() {
        self.currentSettingName = "";
        self.fields = [];
        self.showModal = false;
      },
      onEdit: function(name) {
        self.currentSettingName = name;
        Kooboo.CoreSetting.get({
          name: name
        }).then(function(res) {
          self.fields = Kooboo.objToArr(res.model, "name", "value");
          self.showModal = true;
          self.getList();
        });
      }
    },
    mounted: function() {
      this.getList();
    },
    beforeCreate: function() {
      self = this;
    },
    beforeDestory: function() {
      self = null;
    }
  });
});
