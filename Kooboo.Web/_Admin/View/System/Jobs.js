$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.Jobs
          }
        ],
        jobName: "",
        startTime: "",
        startDate: moment().format("YYYY-MM-DD HH:mm"),
        repeat: false,
        frequenceUnit: 0,
        frequence: "Second",
        script: "",
        selectedJobs: [],
        tabTypes: [
          {
            displayName: Kooboo.text.site.job.pending,
            value: "pending"
          },
          {
            displayName: Kooboo.text.site.job.completed,
            value: "completed"
          },
          {
            displayName: Kooboo.text.common.failed,
            value: "failed"
          }
        ],
        curTab: "pending",
        tableData: [],
        selectedJobs: [],
        showJobDialog: false,
        scheduleJobCode: []
      };
    },
    mounted: function() {
      self.getList();
    },
    methods: {
      changeTab: function(tab) {
        if (tab !== self.curTab) {
          switch (tab) {
            case "pending":
              self.getList(tab);
              break;
            case "completed":
            case "failed":
              self.getLogs(tab);
              break;
          }
        }
      },
      getList: function(tab) {
        Kooboo.Job.getList().then(function(res) {
          if (res.success) {
            self.curTab = "pending";
            var docs = res.model.map(function(item) {
              var date = new Date(item.startTime);
              return {
                id: item.id,
                codeId: item.codeId,
                name: item.jobName,
                startTime: date.toDefaultLangString(),
                codeName: {
                  text: item.codeName,
                  class: "label-sm green"
                },
                isRepeat: {
                  text: Kooboo.text.common[item.isRepeat ? "yes" : "no"],
                  class:
                    "label-sm " +
                    "label-" +
                    (item.isRepeat ? "success" : "default")
                }
              };
            });
            self.tableData = docs;
          }
        });
      },
      getLogs: function(tab) {
        Kooboo.Job.getLogs({
          success: tab == "completed"
        }).then(function(res) {
          if (res.success) {
            self.curTab = tab;
            var docs = res.model.map(function(item) {
              return {
                jobName: item.jobName,
                description: item.description,
                executionTime: item.executionTime,
                message: item.message
              };
            });
            self.tableData = docs;
          }
        });
      },
      onShowJobDialog: function() {
        if (self.scheduleJobCode.length) {
          self.showJobDialog = true;
        } else {
          Kooboo.Code.getListByType({
            codeType: "job"
          }).then(function(res) {
            if (res.success) {
              self.scheduleJobCode = res.model.map(function(item) {
                item.selected = false;
                return item;
              });
              self.showJobDialog = true;
            }
          });
        }
      },
      onHideJobDialog: function() {
        self.jobName = "";
        self.startTime = "";
        self.repeat = false;
        self.frequence = "Second";
        self.frequenceUnit = null;
        self.showJobDialog = false;
        self.showKscript = false;
        self.showCode = false;
      },
      onSaveJob: function() {
        var selectedCode = self.scheduleJobCode.find(function(code) {
          return code.selected;
        });

        var data = {
          name: self.jobName,
          script: self.script,
          codeId: selectedCode ? selectedCode.id : "",
          isRepeat: self.repeat,
          startTime: self.startTime || undefined,
          frequenceUnit: self.frequenceUnit || 0,
          frequence: self.frequence
        };

        Kooboo.Job.post(data).then(function(res) {
          if (res.success) {
            window.info.done(Kooboo.text.info.save.success);
            self.onHideJobDialog();
            self.getList();
          }
        });
      },
      selectCode: function(m) {
        self.scheduleJobCode.forEach(function(code) {
          code.selected = false;
        });
        m.selected = !m.selected;
      },
      onRun: function() {
        var id = self.selectedJobs.map(function(job) {
          return job.codeId;
        });

        Kooboo.Job.Run({
          id: id[0]
        }).then(function(res) {
          if (res.success) {
            window.info.done("Runing");
          }
        });
      },
      onDelete: function() {
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var ids = self.selectedJobs.map(function(job) {
            return job.id;
          });

          Kooboo.Job.Deletes({
            ids: JSON.stringify(ids)
          }).then(function(res) {
            if (res.success) {
              self.tableData = _.filter(self.tableData, function(row) {
                return ids.indexOf(row.id) === -1;
              });
              self.selectedJobs = [];
              window.info.show(Kooboo.text.info.delete.success, true);
            }
          });
        }
      }
    },
    beforeDestory: function() {
      self = null;
    }
  });
});
