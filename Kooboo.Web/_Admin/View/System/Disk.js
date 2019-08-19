$(function() {
  var chart = echarts.init(document.getElementById("chart")),
    chartOption = {};

  var diskModel = function() {
    var self = this;

    this.totalSize = ko.observable();

    this.sizeArray = ko.observableArray();
    this.sizeArray.subscribe(function(s) {
      var _s = _.cloneDeep(s);
      self.topThreeArray(_s.splice(0, 5));
    });

    this.topThreeArray = ko.observableArray();

    this.clearLogs = function() {
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
    };

    this.clearRepository = function() {
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
    };

    this.clearVistorLog = function() {
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
    };

    Kooboo.EventBus.subscribe(
      "kb/table/rendered",
      _.throttle(function() {
        chart.resize();
      }, 200)
    );

    this.logData = ko.observable();
    this.showLogModal = ko.observable(false);
    this.onCleanLog = function() {
      Kooboo.Disk.CleanLog({
        storeName: self.logData().name
      }).then(function(res) {
        window.info.done(Kooboo.text.info.clean.success);
        self.onHideLogModal();
      });
    };
    this.onHideLogModal = function() {
      self.logData(null);
      self.showLogModal(false);
    };

    Kooboo.EventBus.subscribe("kb/disk/item/log", function(data) {
      Kooboo.Disk.getSize({
        storeName: data.id
      }).then(function(res) {
        self.logData(res.model);
        self.showLogModal(true);
      });
    });
  };

  diskModel.prototype = new Kooboo.tableModel(Kooboo.Disk.name);
  var vm = new diskModel();

  function init() {
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
              class: "blue",
              url: "kb/disk/item/log"
            }
          });

          chartOption.legend.data.push(rep.name);
          chartOption.series[0].data.push({
            value: rep.length,
            name: rep.name
          });
        });

        chart.setOption(chartOption);

        vm.totalSize(res.model.totalSize);
        vm.sizeArray(sizeArray);

        vm.tableData({
          unselectable: true,
          docs: docs,
          columns: [
            {
              displayName: Kooboo.text.common.name,
              fieldName: "name",
              type: "text"
            },
            {
              displayName: Kooboo.text.site.disk.count,
              fieldName: "itemCount",
              type: "badge"
            },
            {
              displayName: Kooboo.text.common.size,
              fieldName: "size",
              type: "text"
            }
          ],
          tableActions: [
            {
              fieldName: "log",
              type: "communication-btn"
            }
          ],
          kbType: Kooboo.Disk.name
        });
      }
    });
  }

  init();

  ko.applyBindings(vm, document.getElementById("main"));
  $(window).on(
    "resize",
    _.debounce(function() {
      chart && chart.resize();
    }, 100)
  );
});
