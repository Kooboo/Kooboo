$(function() {
  var configViewModel = function() {
    var self = this;

    this.getList = function() {
      Kooboo.KConfig.getList().then(function(res) {
        if (res.success) {
          self.tableData(self.getTableData(res.model));
        }
      });
    };

    this.showConfigModal = ko.observable(false);

    this.configItem = ko.observable();

    this.getTableData = function(data) {
      var docs = [];
      data.forEach(function(item) {
        var date = new Date(item.lastModified);
        var firstKey = Object.keys(item.binding)[0];

        var obj = {
          id: item.id,
          name: {
            title: item.name,
            description: firstKey + ": " + item.binding[firstKey]
          },
          thumbnail: {
            src:
              firstKey == "src"
                ? item.binding[firstKey] +
                  "?SiteId=" +
                  Kooboo.getQueryString("siteid")
                : "",
            previewUrl:
              firstKey == "src"
                ? item.binding[firstKey] +
                  "?SiteId=" +
                  Kooboo.getQueryString("siteid")
                : "",
            newWindow: true
          },
          tagName: {
            text: "<" + item.tagName + ">",
            tooltip: item.tagHtml,
            class: "label-sm blue"
          },
          date: date.toDefaultLangString(),
          relationsComm: "kb/relation/modal/show",
          relationsTypes: Object.keys(item.relations),
          relations: item.relations,
          edit: {
            iconClass: "fa-pencil",
            title: Kooboo.text.common.edit,
            url: "kb/config/edit"
          },
          versions: {
            iconClass: "fa-clock-o",
            title: Kooboo.text.common.viewAllVersions,
            url: Kooboo.Route.Get(Kooboo.Route.SiteLog.LogVersions, {
              KeyHash: item.keyHash,
              storeNameHash: item.storeNameHash
            }),
            newWindow: true
          }
        };
        docs.push(obj);
      });

      return {
        columns: [
          {
            displayName: Kooboo.text.common.name,
            fieldName: "name",
            type: "summary"
          },
          {
            displayName: Kooboo.text.common.preview,
            fieldName: "thumbnail",
            type: "thumbnail"
          },
          {
            displayName: "tagName",
            fieldName: "tagName",
            type: "label"
          },
          {
            displayName: Kooboo.text.common.usedBy,
            fieldName: "relations",
            type: "communication-refer"
          },
          {
            displayName: Kooboo.text.common.lastModified,
            fieldName: "date",
            type: "text"
          }
        ],
        docs: docs,
        tableActions: [
          {
            fieldName: "edit",
            type: "communication-icon-btn"
          },
          {
            fieldName: "versions",
            type: "link-icon"
          }
        ],
        kbType: Kooboo.KConfig.name
      };
    };

    this.getList();

    this.mediaDialogData = ko.observable();

    Kooboo.EventBus.subscribe("kb/config/edit", function(data) {
      Kooboo.KConfig.Get({
        id: data.id
      }).then(function(res) {
        if (res.success) {
          switch (res.model.controlType.toLowerCase()) {
            case "textbox":
              self.configItem(res.model);
              self.showConfigModal(true);
              break;
            case "mediafile":
              Kooboo.Media.getList().then(function(res) {
                if (res.success) {
                  res.model["show"] = true;
                  res.model["context"] = self;
                  res.model["onAdd"] = function(selected) {
                    Kooboo.KConfig.update({
                      id: data.id,
                      binding: {
                        src: selected.url
                      }
                    }).then(function(res) {
                      self.getList();
                    });
                  };
                  self.mediaDialogData(res.model);
                }
              });
              break;
          }
        }
      });
    });

    Kooboo.EventBus.subscribe("kb/config/attribute/update", function() {
      self.getList();
    });
  };

  configViewModel.prototype = new Kooboo.tableModel(Kooboo.KConfig.name);
  var vm = new configViewModel();
  ko.applyBindings(vm, document.getElementById("main"));
});
