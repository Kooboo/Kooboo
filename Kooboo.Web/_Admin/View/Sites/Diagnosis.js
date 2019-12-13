$(function() {
  new Vue({
    el: "#app",
    data: function() {
      return {
        dataService: {
          getStatus: function(options) {
            return Kooboo.Diagnosis.getStatus(options);
          },

          getList: function() {
            return Kooboo.Diagnosis.getList();
          },

          cancel: function(options) {
            return Kooboo.Diagnosis.cancel(options);
          },

          objToArr: function(modelObject, key, value) {
            return Kooboo.objToArr(modelObject, key, value);
          },

          startSession: function(options) {
            return Kooboo.Diagnosis.startSession(options);
          }
        },
        siteId: Kooboo.getQueryString("siteId"),
        score:
          localStorage.getItem("score_" + Kooboo.getQueryString("siteId")) ||
          '<i class="glyphicon glyphicon-heart"></i>',

        diagnosisCodeURL: Kooboo.Route.Get(Kooboo.Route.Code.EditPage, {
          codeType: "diagnosis"
        }),
        text: {
          addCustomItem: Kooboo.text.site.diagnostic.addCustomItem,
          critical: Kooboo.text.site.diagnostic.critical,
          warning: Kooboo.text.site.diagnostic.warning,
          information: Kooboo.text.site.diagnostic.information,
          scanAgain: Kooboo.text.site.diagnostic.scanAgain,
          sites: Kooboo.text.component.breadCrumb.sites,
          dashboard: Kooboo.text.component.breadCrumb.dashboard,
          diagnosis: Kooboo.text.common.diagnosis,
          cancel: Kooboo.text.common.cancel,
          scanning: Kooboo.text.site.diagnostic.scanning,
          chooseTheTtemsToStartScanning:
            Kooboo.text.site.diagnostic.chooseTheTtemsToStartScanning,
          scanNow: Kooboo.text.site.diagnostic.scanNow
        },
        cancelBtnText: undefined,
        diagnosticItems: undefined,
        isScanFinished: false,
        isScanningView: false,
        scanPage: "",
        scanType: "",
        percent: 0,
        sessionId: undefined,
        issueNumber: undefined,
        messages: [],
        selectedTreeNodes: [],
        showProgress: false
      };
    },
    created: function() {
      this.cancelBtnText = this.text.cancel;
      this.breads = [
        {
          name: "SITES"
        },
        {
          name: "DASHBOARD"
        },
        {
          name: this.text.diagnosis
        }
      ];
      this.getList();
    },
    methods: {
      initTree: function() {
        var self = this;
        $("#J_ScanItems")
          .jstree({
            core: {
              strings: { "Loading ...": Kooboo.text.common.loading + " ..." }
            },
            themes: {
              responsive: true
            },
            plugins: ["wholerow", "checkbox", "types"],
            types: {
              default: {
                icon: "fa fa-file icon-state-warning"
              }
            }
          })
          .on("changed.jstree", function(e, data) {
            var i,
              j,
              r = [];
            for (i = 0, j = data.selected.length; i < j; i++) {
              data.instance.get_node(data.selected[i]).data.itemid &&
                r.push(data.instance.get_node(data.selected[i]).data.itemid);
            }
            self.scanType = r;
          })
          .on("ready.jstree", function() {
            $(this)
              .find(".jstree-anchor")
              .each(function() {
                if (
                  $(this)
                    .parent()
                    .data("title")
                ) {
                  $(this).popover({
                    container: "body",
                    placement: "top",
                    trigger: "hover",
                    title: $(this)
                      .parent()
                      .data("title"),
                    content: $(this)
                      .parent()
                      .data("content")
                  });
                }
              });
          })
          .on("after_open.jstree", function() {
            $(this)
              .find(".jstree-anchor")
              .each(function() {
                if (
                  $(this)
                    .parent()
                    .data("title")
                ) {
                  $(this).popover({
                    container: "body",
                    placement: "top",
                    trigger: "hover",
                    title: $(this)
                      .parent()
                      .data("title"),
                    content: $(this)
                      .parent()
                      .data("content")
                  });
                }
              });
          });
      },
      handleSelectTreeNode: function(event) {
        this.scanType = event;
      },
      addCustomItemHandle: function() {
        window.location.href = this.diagnosisCodeURL;
      },
      getList: function() {
        var self = this;
        this.dataService.getList().then(function(res) {
          if (res.success) {
            var modelObject = _.groupBy(res.model, function(item) {
              return item.group;
            });
            self.diagnosticItems = self.dataService.objToArr(
              modelObject,
              "key",
              "values"
            );
            self.$nextTick(function() {
              self.initTree();
            });
          } else {
          }
        });
      },

      clearMessageStack: function() {
        this.messages = [];
      },
      getStatus: function(id) {
        var self = this;
        if (!this.isScanFinished) {
          Kooboo.Diagnosis.getStatus({
            sessionId: id
          }).then(function(res) {
            if (res.success) {
              self.sessionId = id;
              self.issueNumber =
                res.model.criticalCount +
                res.model.infoCount +
                res.model.warningCount;
              self.scanPage = res.model.headLine;

              for (var i = 0; i < res.model.messages.length; i++) {
                var msg = res.model.messages[i];
                switch (msg.type.toLowerCase()) {
                  case "critical":
                    msg.class = "danger";
                    msg.typeText = self.text.critical;
                    break;
                  case "warning":
                    msg.class = "warning";
                    msg.typeText = self.text.warning;
                    break;
                  case "info":
                    msg.class = "info";
                    msg.typeText = self.text.information;
                    break;
                }
                self.messages.push(msg);
              }
              if (!res.model.isFinished) {
                self.isScanningView = true;
                self.percent = 100;

                if (!self.isScanFinished) {
                  self.getStatus(id);
                } else {
                  self.endScan(true);
                }
              } else {
                self.renderFinished();
              }
            }
          });
        }
      },
      renderFinished: function() {
        this.isScanFinished = true;
        window.onbeforeunload = function() {};
        if (this.score) {
          localStorage.setItem("score_" + this.siteId, this.score);
        }

        this.percent = 100;
        this.showProgress = false;
        this.scanPage = "";
        this.cancelBtnText = this.text.scanAgain;
        setTimeout(function() {
          $("#progressBar").fadeOut();
        }, 1000);
      },
      endScan: function(insideCancel) {
        this.clearMessageStack();
        this.isScanFinished = true;
        this.isScanningView = false;
        this.score = localStorage.getItem("score_" + this.siteId) || 100;
        this.issueNumber = 0;
        this.percent = 0;
        window.onbeforeunload = function() {};

        if (!insideCancel) {
          this.dataService.cancel({
            sessionId: this.sessionId
          });
        }
      },
      startScan: function() {
        var self = this;
        var ids = JSON.stringify(self.scanType);
        if (self.scanType.length > 0) {
          Kooboo.Diagnosis.startSession({
            checkers: ids
          }).then(function(res) {
            if (res.success) {
              self.isScanFinished = false;
              self.getStatus(res.model);
            }
          });

          window.onbeforeunload = function(event) {
            event.returnValue = self.text.scanning;
          };
          this.isScanningView = true;
          this.score = 100;
          this.issueNumber = 0;
          this.percent = 0;
          this.showProgress = true;
          this.cancelBtnText = self.text.cancel;
          $("#progressBar").fadeIn();
        }
      }
    }
  });
});
