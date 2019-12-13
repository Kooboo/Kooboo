$(function() {
  var chart = null,
    chartOption = {
      tooltip: {
        formatter: function(params) {
          return (
            params.name +
            "<br/>" +
            Kooboo.text.market.Used +
            params.data.usedName +
            "<br/>" +
            Kooboo.text.market.Total +
            params.data.totalName
          );
        }
      },
      xAxis: {
        axisLabel: {
          inside: false,
          textStyle: {
            color: "#000"
          }
        },
        axisTick: {
          show: false
        },
        axisLine: {
          show: false
        },
        z: 10
      },
      yAxis: {
        axisLine: {
          show: false
        },
        axisTick: {
          show: false
        },
        axisLabel: {
          textStyle: {
            color: "#999"
          }
        }
      },
      grid: {
        top: "5%",
        left: "2%",
        right: "2%",
        bottom: "5%",
        containLabel: true
      },
      series: [
        {
          // For shadow
          type: "bar",
          itemStyle: {
            normal: {
              color: "rgba(0,0,0,0.1)"
            }
          },
          barGap: "-100%",
          barCategoryGap: "40%",
          animation: false
        },
        {
          type: "bar",
          itemStyle: {
            normal: {
              color: "#0087c2"
            },
            emphasis: {
              color: "#205eac"
            }
          }
        }
      ]
    };

  function getSize(filesize) {
    var gigabytes = 1024 * 1024 * 1024;
    var returnValue = filesize / gigabytes;

    if (returnValue > 1) {
      return {
        value: returnValue.toFixed(1),
        bytes: gigabytes,
        unit: "GB"
      };
    }
    var megabyte = 1024 * 1024;
    returnValue = filesize / megabyte;
    if (returnValue > 1) {
      return {
        value: returnValue.toFixed(1),
        bytes: megabyte,
        unit: "MB"
      };
    }

    var kilobyte = 1024;
    returnValue = filesize / kilobyte;
    return {
      value: returnValue.toFixed(1),
      bytes: kilobyte,
      unit: "KB"
    };
  }

  function getSizeWithUnit(value, unit) {
    var returnValue = value;
    if (unit == "GB") {
      returnValue = value / (1024 * 1024 * 1024);
    } else if (unit == "MB") {
      returnValue = value / (1024 * 1024);
    } else if (unit == "KB") {
      returnValue = value / 1024;
    }
    return returnValue;
  }

  //internal can be 0.1G or 100M
  function getInterval(maxValue) {
    var interval = 0;
    if (maxValue <= 10) {
      maxValue = parseFloat(maxValue);

      if (maxValue < 2) {
        //avoid y axis's scals too little.
        interval = maxValue / 10;
        interval = interval.toFixed(1);
        interval = interval * 2;
      } else if (maxValue > 6) {
        interval = 2;
      } else {
        interval = 1;
      }
    } else {
      var size = Math.floor(Math.log(maxValue) / Math.LN10);
      interval = Math.pow(10, size);
      if (maxValue / interval < 2) {
        //avoid y axis's scals too little.
        size = Math.floor(Math.log(maxValue / 10) / Math.LN10);
        interval = Math.pow(10, size);
      }

      if (maxValue / interval > 6) {
        interval = interval * 2; //reduce y axis's scale
      }
    }

    return interval;
  }

  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "MARKET"
          },
          {
            name: Kooboo.text.common.Hardwares,
            url: Kooboo.Route.Hardware.ListPage
          },
          {
            name: Kooboo.text.market.hardware.usageReport
          }
        ],
        types: [],
        currentType: null,
        currentData: null,
        logs: [],
        pager: {},
        logsLoading: true
      };
    },
    mounted: function() {
      Kooboo.Infrastructure.getTypes().then(function(res) {
        if (res.success) {
          self.types = Kooboo.objToArr(res.model, "value", "displayName");
          self.currentType = self.types[0] && self.types[0].value;
        }
      });
      $(window).on(
        "resize.chart",
        _.debounce(function() {
          chart && chart.resize();
        }, 100)
      );
    },
    methods: {
      changePage: function(page) {
        self.getLogs(self.currentData.month, page);
      },
      changeType: function(m) {
        if (m.value !== self.currentType) {
          self.currentType = m.value;
        }
      },
      getLogs: function(month, page) {
        this.logsLoading = true;
        Kooboo.Infrastructure.getMonthlyLogs({
          infraType: self.currentType,
          month: month,
          pageNr: page || 1
        }).then(function(res) {
          if (res.success) {
            self.logsLoading = false;
            self.pager = res.model;
            self.logs = res.model.list.map(function(item) {
              var date = new Date(item.creationDate);
              return {
                type: item.infraType,
                content: item.content,
                date: date.toDefaultLangString(),
                count: item.count
              };
            });
          }
        });
      }
    },
    watch: {
      currentType: function(type) {
        Kooboo.Infrastructure.getMonthlyReport({
          infraType: type
        }).then(function(res) {
          if (res.success) {
            var latest = res.model[res.model.length - 1];

            self.currentData = {
              month: latest.month,
              totalName: latest.totalName,
              usedName: latest.usedName,
              total: latest.total,
              used: latest.used
            };

            var xData = [],
              value = [],
              dataShadow = [],
              maxYData = 0;

            res.model.forEach(function(data) {
              xData.push(data.month);
              value.push({
                value: data.used,
                usedName: data.usedName,
                totalValue: data.total,
                totalName: data.totalName
              });
              if (data.total > maxYData) {
                maxYData = data.total;
              }
              dataShadow.push({
                usedvalue: data.used,
                usedName: data.usedName,
                value: data.total,
                totalName: data.totalName
              });
            });
            var maxSize = getSize(maxYData);
            var maxSizeUnit = maxSize.unit;

            chartOption.xAxis.data = xData;
            chartOption.series[0]["data"] = dataShadow;
            chartOption.series[1]["data"] = value;
            //format xAxis data
            chartOption.yAxis.axisLabel.formatter = function(value) {
              if (type == "email") {
                return value;
              } else {
                var unit = maxSizeUnit;
                var sizeValue = getSizeWithUnit(value, unit);
                return sizeValue + unit;
              }
            };

            if (type == "email") {
              var interval = getInterval(maxYData);
              var splitNumber = Math.ceil(maxYData / interval);
              chartOption.yAxis.min = 0;
              chartOption.yAxis.splitNumber = splitNumber;
              chartOption.yAxis.max = interval * splitNumber;
              chartOption.yAxis.interval = interval;
            } else {
              var size = getSize(maxYData);
              var interval = getInterval(size.value);
              var splitNumber = Math.ceil(size.value / interval);
              chartOption.yAxis.min = 0;
              chartOption.yAxis.splitNumber = splitNumber;

              chartOption.yAxis.max = interval * splitNumber * size.bytes;
              chartOption.yAxis.interval = interval * size.bytes;
            }

            chart = echarts.init(document.getElementById("report"));
            chart.setOption(chartOption);

            self.getLogs(self.currentData.month);

            chart.off("click");
            chart.on("click", function(params) {
              var dataIdx = params.dataIndex;

              var total = chartOption.series[0].data[dataIdx],
                used = chartOption.series[1].data[dataIdx];

              if (params.name) {
                self.currentData = {
                  month: params.name,
                  totalName: total.totalName,
                  total: total.value,
                  used: used.value,
                  usedName: used.usedName
                };

                self.getLogs(self.currentData.month);
              }
            });
          }
        });
      }
    }
  });
});
