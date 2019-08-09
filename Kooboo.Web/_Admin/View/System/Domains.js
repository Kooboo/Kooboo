$(function() {
  function Domain() {
    var self = this;
    this.siteName = ko.observable();
    this.tableData = ko.observable({});
    this.rootDomain = ko.observableArray([]);
    this.root = ko.observable("");
    this.root.subscribe(function() {
      self.subdomain.valueHasMutated();
    });
    this.domains = ko.observableArray();
    this.docs = ko.observableArray();
    this.subdomain = ko.observable();
    this.subdomain = ko.validateField("", {
      regex: {
        pattern: /^([A-Za-z][\w\-\.]*)*$/,
        message: Kooboo.text.validation.objectNameRegex
      },
      localUnique: {
        compare: function() {
          var exist = _.map(self.domains(), function(dm) {
            return dm.fullName;
          });
          var res = [self.subdomain()];
          if (exist.indexOf(self.subdomain() + "." + self.root()) > -1) {
            res.push(self.subdomain());
          }
          return res;
        }
      },
      stringlength: {
        min: 0,
        max: 63,
        message:
          Kooboo.text.validation.minLength +
          0 +
          ", " +
          Kooboo.text.validation.maxLength +
          63
      },
      remote: {
        url: Kooboo.Site.CheckDomainBindingAvailable(),
        message: Kooboo.text.validation.taken,
        type: "get",
        data: {
          SubDomain: function() {
            return self.subdomain();
          },
          RootDomain: function() {
            return self.root();
          }
        }
      }
    });
    this.port = ko.validateField({
      required: Kooboo.text.validation.required,
      regex: {
        pattern: /^\d*$/,
        message: Kooboo.text.validation.invaildPort
      },
      range: {
        from: 0,
        to: 65535,
        message: Kooboo.text.validation.portRange
      }
    });
    this.defaultBinding = ko.observable("domain");
    this.defaultBinding.subscribe(function(db) {
      self.showError(false);
    });
    this.modalShow = ko.observable(false);
    this.showError = ko.observable(false);

    function dataMapping(data) {
      var arr = [];
      arr = data.map(function(o) {
        return {
          id: o.id,
          domain: o.fullName,
          subDomain: o.subDomain,
          sslEnabled: {
            text: o.enableSsl ? Kooboo.text.common.yes : Kooboo.text.common.no,
            class: "label-sm " + (o.enableSsl ? "green" : "gray")
          },
          enable: {
            text: o.enableSsl
              ? Kooboo.text.common.enabled
              : Kooboo.text.common.enable,
            class: o.enableSsl ? "gray disabled" : "blue",
            url: "kb/domain/enable/domain"
          }
        };
      });
      return arr;
    }

    function isValid() {
      if (self.defaultBinding() == "port") {
        return self.port.isValid();
      } else {
        return self.subdomain.isValid();
      }
    }

    this.save = function() {
      if (!isValid()) {
        self.showError(true);
      } else {
        Kooboo.Binding.post({
          subdomain: self.subdomain(),
          rootdomain: self.root(),
          port: self.port(),
          defaultBinding: self.defaultBinding() == "port"
        }).then(function() {
          getList();
          self.cancelDialog();
        });
      }
    };

    this.showDialog = function() {
      self.modalShow(true);
    };

    this.cancelDialog = function() {
      self.subdomain("");
      self.port("");
      self.defaultBinding("domain");
      self.showError(false);
      self.modalShow(false);
    };

    Kooboo.Domain.getList().then(function(res) {
      self.rootDomain(res.model);
    });

    getList();

    function getList() {
      Kooboo.Binding.listBySite().then(function(data) {
        var ob = {
          columns: [
            {
              displayName: Kooboo.text.site.domain.name,
              fieldName: "domain",
              type: "text"
            },
            {
              displayName: Kooboo.text.site.domain.sslEnabled,
              fieldName: "sslEnabled",
              type: "label"
            }
          ],
          tableActions: [
            {
              fieldName: "enable",
              type: "communication-btn"
            }
          ],
          kbType: "Binding"
        };

        ob.docs = dataMapping(data.model);
        self.tableData(ob);

        self.domains(data.model);
      });
    }

    Kooboo.EventBus.subscribe("kb/table/delete/finish", function(data) {
      var newDomains = self.domains().filter(function(domain) {
        return data.ids.indexOf(domain.id) == -1;
      });

      self.domains(newDomains);
    });

    Kooboo.Site.getName().then(function(res) {
      res.success && self.siteName(res.model);
    });

    Kooboo.EventBus.subscribe("kb/domain/enable/domain", function(doc) {
      var find = self.domains().find(function(domain) {
        return domain.id == doc.id;
      });

      if (find) {
        var domain = self.rootDomain().find(function(domain) {
          return domain.id == find.domainId;
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
                  getList();
                  window.info.done(Kooboo.text.info.enable.success);
                } else {
                  window.info.fail(Kooboo.text.info.enable.failed);
                }
              });
            }
          });
        }else{
          window.info.fail(Kooboo.text.info.domainMissing)
        }
      }
    });
  }
  Domain.prototype = new Kooboo.tableModel();
  ko.applyBindings(new Domain(), document.getElementById("main"));
});
