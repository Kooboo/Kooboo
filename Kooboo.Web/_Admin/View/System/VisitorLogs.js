$(function() {
    var chart = null,
        chartOption = {
            tooltip: {
                formatter: function(params) {
                    if (params.seriesIndex == 1) {
                        return params.name + "<br/>" + "PV: " + params.value;
                    } else {
                        return "";
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
                        color: 'rgba(0,0,0,0.05)'
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

    function Domain() {
        var self = this;

        this.pager = ko.observable();

        this.logTypes = ko.observableArray([{
            displayName: Kooboo.text.site.visitorLog.all,
            value: "All"
        }, {
            displayName: Kooboo.text.site.visitorLog.topPages,
            value: "TopPages"
        }, {
            displayName: Kooboo.text.site.visitorLog.topReferer,
            value: "TopReferer"
        }, {
            displayName: Kooboo.text.site.visitorLog.topImages,
            value: "TopImages"
        }, {
            displayName: Kooboo.text.site.visitorLog.chart,
            value: "Graphs"
        }, {
            displayName: Kooboo.text.site.visitorLog.errorList,
            value: "ErrorList"
        }]);

        this.curLogType = ko.observable(location.hash ? location.hash.split("#")[1] : "All");

        this.changeLogType = function(type) {
            if (type !== self.curLogType() && type !== "Graphs") {
                Kooboo.VisitorLog[type]({
                    weekname: self.week()
                }).then(function(res) {
                    if (res.success) {
                        self.curLogType(type);
                        self.handleData(res.model, self.curLogType());
                    }
                })
            } else if (type == "Graphs") {
                self.curLogType(type);
                if (chart) {
                    chart.resize();
                } else {
                    Kooboo.VisitorLog.Monthly().then(function(res) {

                        if (res.success) {
                            var xData = [],
                                value = [],
                                dataShadow = [];

                            if (res.model.length > 16) {
                                _.forEach(res.model, function(item) {
                                    xData.push(item.name);
                                    value.push(item.count);
                                })
                            } else {
                                for (var i = 0; i <= 15 - res.model.length; i++) {
                                    xData.push("");
                                    value.push(0);
                                }

                                _.forEach(res.model, function(item) {
                                    xData.push(item.name);
                                    value.push(item.count);
                                })
                            }

                            chartOption.xAxis.data = xData;
                            chartOption.series[1]['data'] = value;

                            chart = echarts.init(document.getElementById("monthly"));
                            chart.setOption(chartOption);
                            var yMax = chart.getModel().getComponent('yAxis', 0).axis.scale.getExtent()[1];
                            value.forEach(function() {
                                dataShadow.push(yMax);
                            })
                            chartOption.series[0]['data'] = dataShadow;
                            chart.setOption(chartOption);
                        }
                    })
                }
            }
        }

        this.handleData = function(data, type) {
            switch (type) {
                case "All":
                    self.pager(data);
                    var list = [];
                    var isOnlineServer = false;
                    _.forEach(data.list, function(item) {
                        var begin = new Date(item.begin);
                        var model = {
                            id: item.entries,
                            clientIP: item.clientIP,
                            pageName: {
                                text: item.pageName,
                                url: item.referer,
                                newWindow: true
                            },
                            begin: begin.toDefaultLangString(),
                            timeElapsed: {
                                text: item.millionSecondTake + " ms",
                                class: "badge-success"
                            },
                            referer: item.referer,
                            size: Kooboo.bytesToSize(item.size),
                            detail: {
                                text: item.entries.length > 0 ? item.entries.length : "NO_VALUE",
                                url: "kb/visitor/log/entries",
                                class: item.entries.length ? "blue" : ""
                            }
                        }

                        if (item.hasOwnProperty('state')) {
                            isOnlineServer = true;
                            model.country = {
                                text: item.country,
                                class: "label-sm green"
                            }
                            model.state = {
                                text: item.state,
                                class: "label-sm green"
                            }
                        }

                        list.push(model);
                    })

                    var columns = [{
                        type: "text",
                        fieldName: "clientIP",
                        showClass: 'ip-cell',
                        displayName: Kooboo.text.site.visitorLog.ip,
                    }];

                    if (isOnlineServer) {
                        columns = _.concat(columns, [{
                            displayName: Kooboo.text.site.visitorLog.state,
                            fieldName: "state",
                            type: "label"
                        }, {
                            displayName: Kooboo.text.site.visitorLog.country,
                            fieldName: "country",
                            type: "label"
                        }])
                    }

                    columns = _.concat(columns, [{
                        displayName: Kooboo.text.site.visitorLog.page,
                        fieldName: "pageName",
                        type: "link"
                    }, {
                        displayName: Kooboo.text.site.visitorLog.startTime,
                        fieldName: "begin",
                        type: "text"
                    }, {
                        displayName: Kooboo.text.site.visitorLog.timeElapsed,
                        fieldName: "timeElapsed",
                        type: "badge"
                    }, {
                        displayName: Kooboo.text.site.visitorLog.referer,
                        fieldName: "referer",
                        type: "text"
                    }, {
                        displayName: Kooboo.text.common.size,
                        fieldName: "size",
                        type: "text"
                    }, {
                        displayName: Kooboo.text.common.detail,
                        fieldName: "detail",
                        type: "communication-badge"
                    }]);

                    self.tableData({
                        docs: list,
                        columns: columns,
                        kbType: "VisitorLog",
                        unselectable: true
                    });
                    break;
                case "TopPages":
                    var list = [];
                    _.forEach(data, function(item) {
                        list.push({
                            name: item.name,
                            count: item.count,
                            size: Kooboo.bytesToSize(item.size)
                        })
                    });

                    self.tableData({
                        columns: [{
                            displayName: Kooboo.text.common.name,
                            fieldName: "name",
                            type: "text"
                        }, {
                            displayName: Kooboo.text.common.size,
                            fieldName: "size",
                            type: "text"
                        }, {
                            displayName: Kooboo.text.common.pageView,
                            fieldName: "count",
                            type: "text"
                        }],
                        docs: list,
                        unselectable: true
                    })
                    break;
                case "TopReferer":
                    var list = [];
                    _.forEach(data, function(item) {
                        list.push({
                            name: item.name,
                            count: item.count,
                            size: Kooboo.bytesToSize(item.size)
                        })
                    });

                    self.tableData({
                        columns: [{
                            displayName: Kooboo.text.common.name,
                            fieldName: "name",
                            type: "text"
                        }, {
                            displayName: Kooboo.text.common.pageView,
                            fieldName: "count",
                            type: "text"
                        }],
                        docs: list,
                        unselectable: true
                    })
                    break;
                case "TopImages":
                    var list = [];
                    data.forEach(function(item) {
                        list.push({
                            thumbnail: {
                                src: item.thumbNail,
                                previewUrl: item.previewUrl,
                                newWindow: true
                            },
                            name: {
                                text: item.name,
                                url: item.previewUrl,
                                newWindow: true
                            },
                            count: item.count,
                            size: Kooboo.bytesToSize(item.size)
                        })
                    });

                    self.tableData({
                        columns: [{
                            displayName: "",
                            fieldName: "thumbnail",
                            type: "thumbnail"
                        }, {
                            displayName: Kooboo.text.common.name,
                            fieldName: "name",
                            type: "link"
                        }, {
                            displayName: Kooboo.text.common.size,
                            fieldName: "size",
                            type: "text"
                        }, {
                            displayName: Kooboo.text.site.visitorLog.views,
                            fieldName: "count",
                            type: "text"
                        }],
                        docs: list,
                        unselectable: true
                    })
                    break;
                case "ErrorList":
                    var list = [];
                    data.forEach(function(err) {
                        list.push({
                            id: err.id,
                            url: {
                                text: err.url,
                                url: err.previewUrl,
                                newWindow: true
                            },
                            count: {
                                text: err.count,
                                class: "badge-sm badge-warning",
                                url: "kb/visitor/log/error/detail"
                            }
                        })
                    })
                    self.tableData({
                        docs: list,
                        columns: [{
                            displayName: Kooboo.text.common.URL,
                            fieldName: "url",
                            type: "link"
                        }, {
                            displayName: Kooboo.text.site.visitorLog.errorCount,
                            fieldName: "count",
                            type: "communication-badge"
                        }],
                        kbType: "VisitorLog",
                        unselectable: true
                    })
                    break;
            }
        }

        this.weeks = ko.observableArray();

        this.week = ko.observable();
        this.week.subscribe(function(week) {
            if (self.curLogType() !== "Graphs") {
                if (week) {
                    !Kooboo.VisitorLog.hasOwnProperty(self.curLogType()) && self.curLogType("All");

                    Kooboo.VisitorLog[self.curLogType()]({
                        weekname: week
                    }).then(function(res) {
                        res.success && self.handleData(res.model, self.curLogType());
                    });
                }
            } else {
                self.changeLogType("Graphs");
            }
        })

        this.isShow = ko.observable();

        this.entries = ko.observableArray();

        this.getElapsedTime = function(endTime, startTime) {
            var t1 = new Date(endTime()),
                t2 = new Date(startTime());

            return t1.getTime() - t2.getTime() + " ms";
        }

        this.resetModal = function() {
            this.entries([]);
            this.isShow(false);
        }

        Kooboo.EventBus.subscribe("kb/visitor/log/entries", function(entries) {

            if (entries.length) {
                self.isShow(true);
                self.entries(entries);
            }
        })

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {

            Kooboo.VisitorLog[self.curLogType()]({
                pageNr: page
            }).then(function(res) {
                self.handleData(res.model, self.curLogType());
            })
        })

        this.showErrorDetailModal = ko.observable(false);

        this.hideErrorDetailModal = function() {
            this.showErrorDetailModal(false);
            this.errorDetails([]);
        }

        this.errorDetails = ko.observableArray();

        Kooboo.EventBus.subscribe("kb/visitor/log/error/detail", function(id) {
            Kooboo.VisitorLog.ErrorDetail({
                id: id
            }).then(function(res) {
                if (res.success) {
                    var list = []
                    res.model.forEach(function(item) {
                        var date = new Date(item.startTime);
                        list.push({
                            ip: item.clientIP,
                            message: item.message,
                            statusCode: item.statusCode,
                            displayDate: date.toDefaultLangString()
                        })
                    })
                    self.errorDetails(list);
                    self.showErrorDetailModal(true);
                }
            })
        })

        // Charts
        this.visitsCharts = ko.observable(true);

        this.toggleVisitsCharts = function() {
            self.visitsCharts(!self.visitsCharts());
        }
    }
    Domain.prototype = new Kooboo.tableModel();
    var vm = new Domain();

    ko.applyBindings(vm, document.getElementById("main"));

    Kooboo.VisitorLog.getWeekNames().then(function(res) {

        if (res.success) {
            vm.weeks(_.sortBy(res.model).reverse());
        }
    })

    $(window).on("resize", _.debounce(function() {
        if (vm.curLogType() == "Graphs") {
            chart && chart.resize();
        }
    }, 100))
})