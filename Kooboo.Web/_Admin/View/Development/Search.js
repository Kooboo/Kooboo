$(function() {
    var chart = echarts.init(document.getElementById("chart")),
        chartOption = {};

    if ($.fn.bootstrapSwitch) {
        $.fn.bootstrapSwitch.defaults.onText = Kooboo.text.common.yes;
        $.fn.bootstrapSwitch.defaults.offText = Kooboo.text.common.no;
    }

    var searchModel = function() {
        var self = this;

        this.enableSearch = ko.observable(false);

        this.wordCount = ko.observable();
        this.docCount = ko.observable();
        this.diskSize = ko.observable();

        this.weeks = ko.observableArray();
        this.week = ko.observable();
        this.week.subscribe(function(week) {
            Kooboo.Search.SearchStat({
                weekname: week
            }).then(function(res) {
                if (res.success) {
                    var model = Kooboo.objToArr(res.model);

                    var keys = model.map(function(m) {
                            return m.key
                        }),
                        values = model.map(function(m) {
                            return m.value;
                        });

                    var chartOption = {
                        color: ['#3398DB'],
                        tooltip: {
                            trigger: 'axis',
                            axisPointer: {
                                type: 'shadow'
                            }
                        },
                        grid: {
                            top: '2%',
                            left: '3%',
                            right: '4%',
                            bottom: '3%',
                            containLabel: true
                        },
                        yAxis: [{
                            type: 'category',
                            data: keys
                        }],
                        xAxis: [{
                            type: 'value',
                            axisTick: {
                                alignWithLabel: true
                            }
                        }],
                        series: [{
                            name: Kooboo.text.site.search.searchedCount,
                            type: 'bar',
                            barWidth: '60%',
                            data: values
                        }]
                    };

                    chart.setOption(chartOption);
                }
            })
        })

        this.rebuild = function() {
            Kooboo.Search.Rebuild().then(function(res) {
                if (res.success) {
                    self.wordCount(res.model.wordCount);
                    self.docCount(res.model.docCount);
                    self.diskSize(Kooboo.bytesToSize(res.model.diskSize));
                    window.info.done(Kooboo.text.info.rebuild.success);
                } else {
                    window.info.fail(Kooboo.text.info.rebuild.fail);
                }
            });
        }

        this.clean = function() {
            if (confirm(Kooboo.text.confirm.search.clean)) {
                Kooboo.Search.Clean().then(function(res) {
                    if (res.success) {
                        self.wordCount(res.model.wordCount);
                        self.docCount(res.model.docCount);
                        self.diskSize(Kooboo.bytesToSize(res.model.diskSize));
                        window.info.done(Kooboo.text.info.clean.success);
                    } else {
                        window.info.fail(Kooboo.text.info.clean.fail);
                    }
                });
            }
        }

        Kooboo.Search.getIndexStat().then(function(res) {
            if (res.success) {
                self.wordCount(res.model.wordCount);
                self.docCount(res.model.docCount);
                self.diskSize(Kooboo.bytesToSize(res.model.diskSize));

                self.enableSearch(res.model.enableFullTextSearch);
                self.enableSearch.subscribe(function(enable) {
                    if (enable) {
                        var rebuild = confirm(Kooboo.text.confirm.search.rebuild);
                        Kooboo.Search.Enable({
                            rebuild: rebuild
                        }).then(function(res) {
                            if (res.success) {
                                window.info.done(Kooboo.text.info.enable.success);
                                self.wordCount(res.model.wordCount);
                                self.docCount(res.model.docCount);
                                self.diskSize(Kooboo.bytesToSize(res.model.diskSize));
                            } else {
                                window.info.fail(Kooboo.text.info.enable.fail);
                            }
                        })
                    } else {
                        Kooboo.Search.Disable().then(function(res) {
                            if (res.success) {
                                window.info.done(Kooboo.text.info.disable.success);
                            } else {
                                window.info.fail(Kooboo.text.info.disable.fail);
                            }
                        })
                    }
                })

                $("#sync-switch").bootstrapSwitch("state", res.model.enableFullTextSearch).on("switchChange.bootstrapSwitch", function(e, data) {
                    self.enableSearch(data);
                });
            }
        })

        Kooboo.Search.getLastest().then(function(res) {
            if (res.success) {
                var docs = [];
                res.model.forEach(function(las) {
                    var d = new Date(las.time);
                    docs.push({
                        keywords: las.keywords,
                        docFound: {
                            text: las.docFound,
                            class: las.docFound > 0 ? 'blue' : 'badge-warning'
                        },
                        ip: las.ip,
                        date: d.toDefaultLangString()
                    })
                });

                self.tableData({
                    docs: docs,
                    columns: [{
                        displayName: Kooboo.text.site.visitorLog.ip,
                        fieldName: "ip",
                        type: "text"
                    }, {
                        displayName: Kooboo.text.site.search.keywords,
                        fieldName: "keywords",
                        type: "text"
                    }, {
                        displayName: Kooboo.text.site.search.docFound,
                        fieldName: "docFound",
                        type: "badge"
                    }, {
                        displayName: Kooboo.text.common.date,
                        fieldName: "date",
                        type: "text"
                    }],
                    kbType: Kooboo.Search.name,
                    unselectable: true
                })
            }
        });
        Kooboo.Search.getWeekNames().then(function(res) {
            res.success && self.weeks(res.model.reverse());
        });
    }
    searchModel.prototype = new Kooboo.tableModel(Kooboo.Search.name);
    var vm = new searchModel();

    ko.applyBindings(vm, document.getElementById("main"));

    $(window).on("resize", _.debounce(function() {
        chart && chart.resize();
    }, 100))

    $("#question-status").popover({
        container: "body",
        content: Kooboo.text.popover.searchStatusExplain,
        placement: "right",
        trigger: "hover"
    })
})