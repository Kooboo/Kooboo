$(function() {
  var self;

  new Vue({
    el: "#app",
    data: function() {
      return {
        text: {
          domains: Kooboo.text.common.Domains,
          sites: Kooboo.text.component.breadCrumb.sites,
          dashboard: Kooboo.text.component.breadCrumb.dashboard
        },
        siteName: "",
        modalShow: false,
        domainsData: [],
        rootDomain: [],
        defaultBinding: "domain",
        tableDataSelected: [],
        formModel: {
          subdomain: "",
          port: 81,
          root: ""
        },
        formRules: {
          subdomain: [
            {
              pattern: /^([A-Za-z][\w\-\.]*)*$/,
              message: Kooboo.text.validation.objectNameRegex
            },
            {
              min: 1,
              max: 63,
              message:
                Kooboo.text.validation.minLength +
                0 +
                ", " +
                Kooboo.text.validation.maxLength +
                63
            },
            {
              validate: function(value) {
                var exist = _.map(self.domainsData, function(dm) {
                  return dm.fullName;
                });
                if (
                  exist.indexOf(
                    self.formModel.subdomain + "." + self.formModel.root
                  ) > -1
                ) {
                  return false;
                }
                return true;
              },
              message: Kooboo.text.validation.taken
            },
            {
              remote: {
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                data: function() {
                  return {
                    SubDomain: self.formModel.subdomain,
                    RootDomain: self.formModel.root
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ],
          port: [
            { required: Kooboo.text.validation.required },
            {
              pattern: /^\d*$/,
              message: Kooboo.text.validation.invaildPort
            },
            {
              min: 0,
              max: 65535,
              message: Kooboo.text.validation.portRange
            }
          ]
        },
        validateModel: undefined
      };
    },
    created: function() {
      self = this;
      this.breads = [
        {
          name: "SITES"
        },
        {
          name: "DASHBOARD"
        },
        {
          name: this.text.domains
        }
      ];
      this.getDomainsData();
      this.getDomainData();
      this.getsiteNameData();
      self.createValidateModel();
    },
    watch: {
      formModel: {
        handler: function() {
          self.createValidateModel();
        },
        deep: true
      },
      defaultBinding: function() {
        self.createValidateModel();
      }
    },
    methods: {
      createValidateModel: function() {
        this.validateModel = {
          subdomain: { valid: true, msg: "" },
          port: { valid: true, msg: "" }
        };
      },
      getsiteNameData: function() {
        Kooboo.Site.getName().then(function(res) {
          if (res.success) {
            self.siteName = res.model;
          }
        });
      },
      getDomainsData: function() {
        Kooboo.Binding.listBySite().then(function(data) {
          self.domainsData = data.model;
        });
      },
      showDialog: function() {
        this.modalShow = true;
      },
      cancelDialog: function() {
        this.modalShow = false;
      },
      onSave: function() {
        var valid = true;
        if (self.defaultBinding === "domain") {
          self.validateModel.subdomain = Kooboo.validField(
            self.formModel.subdomain,
            self.formRules.subdomain
          );
          if (!self.validateModel.subdomain.valid) {
            valid = false;
          }
        } else if (self.defaultBinding === "port") {
          self.validateModel.port = Kooboo.validField(
            self.formModel.port,
            self.formRules.port
          );
          if (!self.validateModel.port.valid) {
            valid = false;
          }
        }
        if (valid) {
          Kooboo.Binding.post({
            subdomain: self.formModel.subdomain,
            rootdomain: self.formModel.root,
            port: self.formModel.port + "",
            defaultBinding: self.defaultBinding === "port"
          }).then(function() {
            self.formModel.subdomain = "";
            if (self.rootDomain[0] && self.rootDomain[0].domainName) {
              self.formModel.root = self.rootDomain[0].domainName;
            }

            self.getDomainsData();
            self.cancelDialog();
          });
        }
      },
      getConfirmMessage: function(doc) {
        if (doc.relations) {
          doc.relationsTypes = _.sortBy(Object.keys(doc.relations));
        }
        var find = _.find(doc, function(item) {
          return item.relations && Object.keys(item.relations).length;
        });

        if (!!find) {
          return Kooboo.text.confirm.deleteItemsWithRef;
        } else {
          return Kooboo.text.confirm.deleteItems;
        }
      },
      onDelete: function() {
        if (confirm(this.getConfirmMessage(this.tableDataSelected))) {
          var ids = this.tableDataSelected.map(function(m) {
            return m.id;
          });

          Kooboo[Kooboo.Binding.name]
            .Deletes({
              ids: ids
            })
            .then(function(res) {
              if (res.success) {
                window.info.done(Kooboo.text.info.delete.success);
                self.getDomainsData();
                self.cancelDialog();
              } else {
                window.info.fail(Kooboo.text.info.delete.failed);
              }
            });
        }
      },

      defaultBindingCheckboxHandle: function(event) {
        this.defaultBinding = event.target.value;
      },
      getDomainData: function() {
        Kooboo.Domain.getList().then(function(res) {
          self.rootDomain = res.model;
          if (self.rootDomain.length > 0) {
            self.formModel.root = self.rootDomain[0].domainName;
          }
        });
      },
      startSSLHandle: function(event, id) {
        //Prevent the event of the target element from bubbling to the parent element
        event.stopPropagation();
        this.changeSSl(id);
      },
      changeSSl: function(id) {
        var find = self.domainsData.find(function(domain) {
          return domain.id === id;
        });

        if (find) {
          var domain = self.rootDomain.find(function(domain) {
            return domain.id === find.domainId;
          });
          if (domain) {
            var rootDomain = domain.domainName,
              subDomain = find.subDomain;
            Kooboo.Binding.verifySSL({
              rootDomain: rootDomain,
              subDomain: subDomain
            }).then(function(res) {
              if (res.success) {
                Kooboo.Binding.setSSL({
                  rootDomain: rootDomain,
                  subDomain: subDomain
                }).then(function(resp) {
                  if (resp.success) {
                    self.getList();
                    window.info.done(Kooboo.text.info.enable.success);
                  } else {
                    window.info.fail(Kooboo.text.info.enable.failed);
                  }
                });
              }
            });
          } else {
            window.info.fail(Kooboo.text.info.domainMissing);
          }
        }
      }
    }
  });
});
