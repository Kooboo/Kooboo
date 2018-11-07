$(function() {
    var siteId = Kooboo.getQueryString("siteId");

    function SiteViewModel() {
        var self = this;
        this.scanPage = ko.observable();
        this.scanType = ko.observableArray();
        this.isScanningView = ko.observable(false);
        this.endScan = function() {
            endScan();
        };
        this.score = ko.observable(localStorage.getItem("score_" + siteId) || '<i class="glyphicon glyphicon-heart"></i>');
        this.issueNumber = ko.observable(0);
        this.percent = ko.observable('0%');
        this.cancelBtnText = ko.observable(Kooboo.text.common.cancel);
        this.diagnosticItems = ko.observableArray();
        this.messages = ko.observableArray();

        this.sessionId = ko.observable();
        this.isScanFinished = ko.observable(false);

        this.diagnosisCodeURL = Kooboo.Route.Get(Kooboo.Route.Code.EditPage, {
            codeType: 'diagnosis'
        })

        Kooboo.Diagnosis.getList().then(function(res) {
            if (res.success) {
                var items = Kooboo.objToArr(_.groupBy(res.model, function(item) {
                    return item.group;
                }), 'key', 'values')
                self.diagnosticItems(items);
                $("#J_ScanItems").jstree({
                    "core": {
                        "strings": { 'Loading ...': Kooboo.text.common.loading + ' ...' },
                    },
                    "themes": {
                        "responsive": true
                    },
                    "plugins": ["wholerow", "checkbox", "types"],
                    "types": {
                        "default": {
                            "icon": "fa fa-file icon-state-warning"
                        }
                    }
                }).on('changed.jstree', function(e, data) {
                    var i, j, r = [];
                    for (i = 0, j = data.selected.length; i < j; i++) {
                        data.instance.get_node(data.selected[i]).data.itemid && r.push(data.instance.get_node(data.selected[i]).data.itemid);
                    }
                    siteViewModel.scanType(r);
                }).on('ready.jstree', function() {
                    $(this).find('.jstree-anchor').each(function() {
                        if ($(this).parent().data("title")) {
                            $(this).popover({
                                container: "body",
                                placement: "top",
                                trigger: "hover",
                                title: $(this).parent().data("title"),
                                content: $(this).parent().data("content")
                            })
                        }
                    })
                }).on('after_open.jstree', function() {
                    $(this).find('.jstree-anchor').each(function() {
                        if ($(this).parent().data("title")) {
                            $(this).popover({
                                container: "body",
                                placement: "top",
                                trigger: "hover",
                                title: $(this).parent().data("title"),
                                content: $(this).parent().data("content")
                            })
                        }
                    })
                });
            }
        })

    }
    var siteViewModel = new SiteViewModel;

    ko.applyBindings(siteViewModel, document.getElementById('main'));

    $('#scan').on('click', function() {
        if (siteViewModel.scanType().length > 0) {
            $('#scan').tooltip('hide');
            startScan();
        }
    });

    function clearMessageStack() {
        siteViewModel.messages([]);
    }

    function getStatus(id) {
        if (!siteViewModel.isScanFinished()) {
            Kooboo.Diagnosis.getStatus({
                sessionId: id
            }).then(function(res) {
                if (res.success) {
                    siteViewModel.sessionId(id);
                    siteViewModel.issueNumber(res.model.criticalCount + res.model.infoCount + res.model.warningCount)
                    siteViewModel.scanPage(res.model.headLine);
                    res.model.messages.forEach(function(msg) {
                        switch (msg.type.toLowerCase()) {
                            case 'critical':
                                msg.class = 'danger';
                                msg.typeText = Kooboo.text.site.diagnostic.critical;
                                break;
                            case 'warning':
                                msg.class = 'warning';
                                msg.typeText = Kooboo.text.site.diagnostic.warning;
                                break;
                            case 'info':
                                msg.class = 'info';
                                msg.typeText = Kooboo.text.site.diagnostic.information;
                                break;
                        }
                        siteViewModel.messages.push(msg);
                    })

                    if (!res.model.isFinished) {
                        siteViewModel.isScanningView(true);
                        siteViewModel.percent('100%');

                        if (!siteViewModel.isScanFinished()) {
                            getStatus(id);
                        } else {
                            endScan(true);
                        }
                    } else {
                        renderFinished();
                    }
                }
            })
        }
    }

    function renderFinished() {
        siteViewModel.isScanFinished(true);
        window.onbeforeunload = function() {}
        localStorage.setItem("score_" + siteId, siteViewModel.score());

        siteViewModel.percent("100%");
        siteViewModel.scanPage('');
        siteViewModel.cancelBtnText(Kooboo.text.site.diagnostic.scanAgain);
        setTimeout(function() {
            $('#progressBar').fadeOut()
        }, 1000);
    }

    function endScan(insideCancel) {
        clearMessageStack();
        siteViewModel.isScanFinished(true);
        siteViewModel.isScanningView(false);
        siteViewModel.score(localStorage.getItem("score_" + siteId) || 100);
        siteViewModel.issueNumber(0);
        siteViewModel.percent("0%");
        window.onbeforeunload = function() {}

        if (!insideCancel) {
            Kooboo.Diagnosis.cancel({
                sessionId: siteViewModel.sessionId()
            })
        }
    }

    function startScan() {

        Kooboo.Diagnosis.startSession({
            checkers: JSON.stringify(siteViewModel.scanType())
        }).then(function(res) {
            if (res.success) {
                siteViewModel.isScanFinished(false);
                getStatus(res.model);
            }
        })

        window.onbeforeunload = function() {
            event.returnValue = Kooboo.text.site.diagnostic.scanning;
        }
        siteViewModel.isScanningView(true);
        siteViewModel.score(100);
        siteViewModel.issueNumber(0);
        siteViewModel.percent("0%");
        siteViewModel.cancelBtnText(Kooboo.text.common.cancel);
        $('#progressBar').fadeIn();
    }
})