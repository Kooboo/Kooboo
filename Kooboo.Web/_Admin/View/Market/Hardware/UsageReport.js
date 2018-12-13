$(function() {
    var chart = null,
        chartOption = {
            tooltip: {
                formatter: function(params) {
                    if (params.seriesIndex == 1) {
                        return params.name + "<br/>" + "Used: " + params.data.valueName;
                    } else {
                        return params.name + "<br/>" + "Total: " + params.data.valueName;
                    }
                }
            },
            xAxis: {
                axisLabel: {
                    inside: false,
                    textStyle: {
                        color: '#000'
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
                        color: '#999'
                    }
                }
            },
            grid: {
                top: '5%',
                left: '2%',
                right: '2%',
                bottom: '5%',
                containLabel: true
            },
            series: [{ // For shadow
                type: 'bar',
                itemStyle: {
                    normal: {
                        color: 'rgba(0,0,0,0.1)'
                    }
                },
                barGap: '-100%',
                barCategoryGap: '40%',
                animation: false
            }, {
                type: 'bar',
                itemStyle: {
                    normal: {
                        color: '#0087c2'
                    },
                    emphasis: {
                        color: '#205eac'
                    }
                }
            }]
        };

    var reportModel = function() {
        var self = this;

        this.types = ko.observableArray();
        this.currentType = ko.observable();
        this.currentType.subscribe(function(type) {
            Kooboo.Infrastructure.getMonthlyReport({
                infraType: type
            }).then(function(res) {
                if (res.success) {
                    var latest = res.model[res.model.length - 1];

                    self.currentData({
                        month: latest.month,
                        totalName:latest.totalName,
                        usedName:latest.usedName,
                        total: latest.total,
                        used: latest.used
                    })

                    var xData = [],
                        value = [],
                        dataShadow = [],
                        valueNames=[];

                    res.model.forEach(function(data) {
                        xData.push(data.month);
                        value.push({
                            value:data.used,
                            valueName:data.usedName
                        });

                        dataShadow.push({
                            value:data.total,
                            valueName:data.totalName
                        });
                    })

                    chartOption.xAxis.data = xData;
                    chartOption.series[0]['data'] = dataShadow;
                    chartOption.series[1]['data'] = value;
                    

                    chart = echarts.init(document.getElementById('report'));
                    chart.setOption(chartOption);

                    self.getLogs(self.currentData().month)

                    chart.off('click');
                    chart.on('click', function(params) {

                        var dataIdx = params.dataIndex;
                        
                        var total = chartOption.series[0].data[dataIdx],
                            used = chartOption.series[1].data[dataIdx];
                        
                        if (params.name) {
                            self.currentData({
                                month: params.name,
                                totalName:total.valueName,
                                total: total.value,
                                used: used.value,
                                usedName:used.valueName
                            })

                            self.getLogs(self.currentData().month)

                        }
                    })
                }
            })
        })
        this.changeType = function(m, e) {
            e.preventDefault();
            if (m.value !== self.currentType()) {
                self.currentType(m.value);
            }
        }

        this.currentData = ko.observable();
        this.logs = ko.observableArray();

        Kooboo.Infrastructure.getTypes().then(function(res) {
            if (res.success) {
                self.types(Kooboo.objToArr(res.model, 'value', 'displayName'));
                self.currentType(self.types()[0].value);
            }
        })

        this.pager = ko.observable();
        this.logsLoading = ko.observable(true);
        this.getLogs = function(month, page) {
            this.logsLoading(true);
            Kooboo.Infrastructure.getMonthlyLogs({
                infraType: self.currentType(),
                month: month,
                pageNr: page || 1
            }).then(function(res) {
                if (res.success) {
                    self.logsLoading(false);
                    self.pager(res.model);
                    self.logs(res.model.list.map(function(item) {
                        var date = new Date(item.creationDate);

                        return {
                            type: item.infraType,
                            content: item.content,
                            date: date.toDefaultLangString(),
                            count: item.count
                        }
                    }));
                }
            })
        }

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            self.getLogs(self.currentData().month, page);
        })
    }

    var vm = new reportModel();
    ko.applyBindings(vm, document.getElementById('main'));

    $(window).on("resize", _.debounce(function() {
        chart && chart.resize();
    }, 100))
})