$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      return {
        tableData: [],
        tableDataSelected: [],
        showCreateDialog: false,
        serverInfo: undefined,
        domain: undefined,
        domainValidateModel: { valid: true, msg: "" },
        registerPageUrl: Kooboo.Route.Domain.Register
      };
    },
    watch: {
      domain: function() {
        self.domainValidateModel = { valid: true, msg: "" };
      }
    },
    created: function() {
      self = this;
      self.getTableData();
    },
    methods: {
      getTableData: function() {
        Kooboo.Domain.getList().then(function(res) {
          if (res.success) {
            self.tableData = res.model;
          }
        });
      },
      getServerInfo: function() {
        Kooboo.Domain.serverInfo().then(function(res) {
          if (res.success) {
            self.serverInfo = {
              cName: res.model.cName,
              dnsServers: res.model.dnsServers,
              ipAddress: res.model.ipAddress
            };
          }
        });
      },
      onDialogCancel: function() {
        self.domain = "";
        self.domainValidateModel = { valid: true, msg: "" };
        this.showCreateDialog = false;
      },
      onDelete: function() {
        if (confirm(this.getConfirmMessage(this.tableDataSelected))) {
          var ids = this.tableDataSelected.map(function(m) {
            return m.id;
          });

          Kooboo.Domain.Deletes({
            ids: ids
          }).then(function(res) {
            if (res.success) {
              window.info.done(Kooboo.text.info.delete.success);
              self.getTableData();
            } else {
              window.info.fail(Kooboo.text.info.delete.failed);
            }
          });
        }
      },
      onCreate: function() {
        self.domain = "";
        self.getServerInfo();
        this.showCreateDialog = true;
      },
      onAdd: function() {
        var rule = [
          { required: true, message: Kooboo.text.validation.required },
          {
            pattern: /[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+\.?/,
            message: Kooboo.text.validate.domainInvalid
          }
        ];
        self.domainValidateModel = Kooboo.validField(self.domain, rule);
        if (self.domainValidateModel.valid) {
          var data = JSON.stringify({
            domainname: self.domain
          });
          Kooboo.Domain.creatDomain(data).then(function(res) {
            if (res.success) {
              self.getTableData();
              self.onDialogCancel();
            }
          });
        }
      },
      getConfirmMessage: function(doc) {
        if (doc.relations) {
          var reorderedKeys = _.sortBy(Object.keys(doc.relations));
          doc.relationsTypes = reorderedKeys;
        }
        var find = _.find(doc, function(item) {
          return item.relations && Object.keys(item.relations).length;
        });

        if (!!find) {
          return Kooboo.text.confirm.deleteItemsWithRef;
        } else {
          return Kooboo.text.confirm.deleteItems;
        }
      }
    }
  });
});
