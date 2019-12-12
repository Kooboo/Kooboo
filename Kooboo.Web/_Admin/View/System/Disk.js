$(function() {
  var self;
  var chart;
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
            name: Kooboo.text.common.Disk
          }
        ],
        totalSize: 0,
        sizeArray: [],
        logData: {},
        showLogModal: false,
        tableData: []
      };
    },
    mounted: function() {
      this.initChart();
      this.$nextTick(function() {
        setTimeout(function() {
          chart.resize();
        }, 200);
      });
    },
    methods: {
      clearLogs: function() {
        if (confirm(Kooboo.text.confirm.disk.clearLogs)) {
          $.when(Kooboo.Disk.CleanRepository(), Kooboo.Disk.CleanLog()).then(
            function(r1, r2) {
              if (r1[0].success && r2[0].success) {
                window.info.show(Kooboo.text.info.clear.success, true);
                init();
              } else {
                window.info.show(Kooboo.text.info.clear.fail, false);
              }
            }
          );
        }
      },
      clearRepository: function() {
        if (confirm(Kooboo.text.confirm.disk.clearRepository)) {
          Kooboo.Disk.CleanRepository().then(function(res) {
            if (res.success) {
              window.info.show(Kooboo.text.info.clear.success, true);
              init();
            } else {
              window.info.show(Kooboo.text.info.clear.fail, false);
            }
          });
        }
      },
      clearVistorLog: function() {
        if (confirm(Kooboo.text.confirm.disk.clearVisitorLog)) {
          Kooboo.Disk.CleanLog().then(function(res) {
            if (res.success) {
              window.info.show(Kooboo.text.info.clear.success, true);
              init();
            } else {
              window.info.show(Kooboo.text.info.clear.fail, false);
            }
          });
        }
      },
      onCleanLog: function() {
        Kooboo.Disk.CleanLog({
          storeName: self.logData.name
        }).then(function(res) {
          window.info.done(Kooboo.text.info.clean.success);
          self.onHideLogModal();
        });
      },
      onHideLogModal: function() {
        self.logData = null;
        self.showLogModal = false;
      },
      initChart: function() {
        (chart = echarts.init(document.getElementById("chart"))),
          (chartOption = {});
        Kooboo.Disk.getList().then(function(res) {
          if (res.success) {
            var docs = [],
              sizeArray = res.model.repositorySize,
              chartOption = {
                tooltip: {
                  trigger: "item",
                  formatter: function(data) {
                    return (
                      data.data.name +
                      " - " +
                      Kooboo.bytesToSize(data.data.value) +
                      " (" +
                      data.percent +
                      "%)"
                    );
                  }
                },
                legend: {
                  orient: "vertical",
                  x: "right",
                  align: "left",
                  data: []
                },
                series: [
                  {
                    type: "pie",
                    radius: ["60%", "90%"],
                    center: ["26%", "50%"],
                    avoidLabelOverlap: false,
                    label: {
                      normal: {
                        show: false,
                        position: "center"
                      },
                      emphasis: {
                        show: true,
                        textStyle: {
                          fontSize: "30",
                          fontWeight: "bold"
                        }
                      }
                    },
                    labelLine: {
                      normal: {
                        show: false
                      }
                    },
                    data: []
                  }
                ]
              };

            sizeArray = _.sortBy(sizeArray, [
              function(o) {
                return o.length;
              }
            ]);

            sizeArray = sizeArray.reverse();

            sizeArray.forEach(function(rep) {
              docs.push({
                id: rep.name,
                name: rep.name,
                size: rep.size,
                itemCount: {
                  text: rep.itemCount,
                  class: "blue"
                },
                log: {
                  text: "Log",
                  class: "blue"
                }
              });

              chartOption.legend.data.push(rep.name);
              chartOption.series[0].data.push({
                value: rep.length,
                name: rep.name
              });
            });

            chart.setOption(chartOption);

            self.totalSize = res.model.totalSize;
            self.sizeArray = sizeArray;
            self.tableData = docs;
          }
        });
        $(window).on(
          "resize.chart",
          _.debounce(function() {
            chart && chart.resize();
          }, 100)
        );
      },
      showLog: function(data) {
        Kooboo.Disk.getSize({
          storeName: data.id
        }).then(function(res) {
          self.logData = res.model;
          self.showLogModal = true;
        });
      }
    },
    computed: {
      topThreeArray: function() {
        var _s = _.cloneDeep(this.sizeArray);
        return _s.splice(0, 5);
      }
    },
    beforeDestory: function() {
      $(window).off("resize.chart");
      self = null;
    }
  });
});
