$(function() {
  var createDataModel = function() {
    var self = this;

    this.table = ko.observable(Kooboo.getQueryString("table"));
    this.id = ko.observable(Kooboo.getQueryString("id") || Kooboo.Guid.Empty);

    this.mediaDialogData = ko.observable();

    this.columns = ko.observableArray();

    this.onSave = function() {
      var values = self.columns().map(function(col) {
        return {
          name: col.name(),
          value: $.isArray(col.value())
            ? JSON.stringify(col.value())
            : col.value()
        };
      });

      Kooboo.Database.updateData({
        tableName: self.table(),
        id: self.id(),
        values: values
      }).then(function(res) {
        if (res.success) {
          location.href = self.dataPage;
        }
      });
    };

    this.dataPage = Kooboo.Route.Get(Kooboo.Route.Database.DataPage, {
      table: self.table()
    });

    Kooboo.Database.getEdit({
      tableName: self.table(),
      id: self.id()
    }).then(function(res) {
      if (res.success) {
        self.columns(
          res.model.map(function(col) {
            return new columnModel(col);
          })
        );
      }
    });

    Kooboo.EventBus.subscribe("ko/style/list/pickimage/show", function(ctx) {
      Kooboo.Media.getList().then(function(res) {
        if (res.success) {
          res.model["show"] = true;
          res.model["context"] = ctx;
          res.model["onAdd"] = function(selected) {
            ctx.settings.file_browser_callback(
              ctx.field_name,
              selected.url + "?SiteId=" + Kooboo.getQueryString("SiteId"),
              ctx.type,
              ctx.win,
              true
            );
          };
          self.mediaDialogData(res.model);
        }
      });
    });
  };

  function columnModel(data) {
    var self = this;

    this.name = ko.observable(data.name);
    this.controlType = ko.observable(data.controlType);
    if (data.controlType && data.controlType.toLowerCase() == "checkbox") {
      var value = null;
      if ($.isArray(data.value)) {
        value = data.value;
      } else {
        try {
          value = JSON.parse(data.value);
        } catch (ex) {
          value = [];
        }
      }
      this.value = ko.observableArray(value);
    } else if (
      data.controlType &&
      data.controlType.toLowerCase() == "datetime"
    ) {
      if (data.value.indexOf(".") > -1) {
        data.value = data.value.split(".")[0];
      }
      this.value = ko.observable(data.value);
    } else {
      this.value = ko.observable(data.value);
    }

    this.isSystem = ko.observable(data.isSystem);
    this.isIncremental = ko.observable(data.isIncremental);

    var setting = JSON.parse(data.setting);
    if (setting && setting.options && setting.options.length) {
      this.options = ko.observableArray(setting.options);
    }
    if (setting && setting.validations && setting.validations.length) {
      this.validations = ko.observableArray(setting.validations);
    }
  }

  var vm = new createDataModel();

  ko.applyBindings(vm, document.getElementById("main"));
});
