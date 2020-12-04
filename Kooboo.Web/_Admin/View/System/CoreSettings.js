$(function () {
  var self;
  new Vue({
    el: "#app",
    data: function () {
      self = this;
      return {
        breads: [
          {
            name: "SITES",
          },
          {
            name: "DASHBOARD",
          },
          {
            name: "Core settings",
          },
        ],
        settings: [],
        showModal: false,
        fields: [],
        groups: [],
        alert: "",
        currentSettingName: "",
      };
    },
    methods: {
      setGroups: function () {
        var groupsBak = this.groups;
        this.groups = [];
        for (var i = 0; i < this.settings.length; i++) {
          const setting = this.settings[i];

          var group = this.groups.filter(function (f) {
            return f.name == setting.group;
          })[0];

          if (group) {
            group.items.push(setting);
          } else {
            var groupBak = groupsBak.filter(function (f) {
              return f.name == setting.group;
            })[0];

            this.groups.push({
              name: setting.group,
              items: [setting],
              expand: groupBak && groupBak.expand,
            });
          }
        }
      },
      getList: function () {
        Kooboo.CoreSetting.getList().then(function (res) {
          if (res.success) {
            self.settings = res.model.map(function (item) {
              return {
                name: item.name,
                group: item.group,
                alert: item.alert,
                value: item.value,
              };
            });

            self.setGroups();
          }
        });
      },
      onSave: function () {
        Kooboo.CoreSetting.update({
          name: self.currentSettingName,
          model: Kooboo.arrToObj(self.fields, "name", "value"),
        }).then(function (res) {
          if (res.success) {
            info.done(Kooboo.text.info.update.success);
            self.onClose();
            self.getList();
          }
        });
      },
      onClose: function () {
        self.currentSettingName = "";
        self.fields = [];
        self.showModal = false;
      },
      onEdit: function (name) {
        self.currentSettingName = name;

        self.alert = self.settings.filter(function (f) {
          return f.name == name;
        })[0].alert;

        Kooboo.CoreSetting.get({
          name: name,
        }).then(function (res) {
          self.fields = res.model;
          self.showModal = true;
          self.getList();
        });
      },
      getFile(event, field) {
        console.log(event);
        var file = event.target.files[0];
        if (!file) {
          return;
        }

        if (file.size > 10 * 1024) {
          info.fail(Kooboo.text.info.fileSizeLessThan + "10KB");
          event.target.value = "";
          return;
        }

        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function (e) {
          var b64 = e.target.result;
          field.value =
            file.name + "|" + b64.substr(b64.indexOf("base64,") + 7);
        };
      },
      getFileName(nameAndBase64) {
        if (!nameAndBase64) {
          return nameAndBase64;
        }
        var idx = nameAndBase64.indexOf("|");
        idx = idx < 0 ? 0 : idx;
        return nameAndBase64.substr(0, idx);
      },
    },
    mounted: function () {
      this.getList();
    },
    beforeDestory: function () {
      self = null;
    },
  });
});
