$(function() {
    var jobsModel = function() {
        var self = this;

        this.jobName = ko.observable();
        this.startTime = ko.observable();
        this.repeat = ko.observable(false);
        this.frequenceUnit = ko.observable();
        this.frequence = ko.observable();
        this.script = ko.observable('');

        this.selectedJobs = ko.observableArray();

        this.tabTypes = ko.observableArray([{
            displayName: Kooboo.text.site.job.pending,
            value: 'pending'
        }, {
            displayName: Kooboo.text.site.job.completed,
            value: 'completed'
        }, {
            displayName: Kooboo.text.common.failed,
            value: 'failed'
        }])

        this.curTab = ko.observable('pending');

        this.changeTab = function(tab) {

            if (tab !== self.curTab()) {
                switch (tab) {
                    case 'pending':
                        getList();
                        break;
                    case 'completed':
                    case 'failed':
                        Kooboo.Job.getLogs({
                            success: tab == 'completed'
                        }).then(function(res) {
                            if (res.success) {
                                self.curTab(tab);
                                var docs = res.model.map(function(item) {
                                    return {
                                        jobName: item.name,
                                        description: item.description,
                                        executionTime: '',
                                        message: ''
                                    }
                                })

                                self.tableData({
                                    docs: docs,
                                    columns: [{
                                        displayName: Kooboo.text.site.job.jobName,
                                        fieldName: "jobName",
                                        type: "text"
                                    }, {
                                        displayName: Kooboo.text.site.job.description,
                                        fieldName: "description",
                                        type: "text"
                                    }, {
                                        displayName: Kooboo.text.site.job.executionTime,
                                        fieldName: "executionTime",
                                        type: "text"
                                    }, {
                                        displayName: Kooboo.text.site.job.message,
                                        fieldName: "message",
                                        type: "text"
                                    }],
                                    unselectable: true,
                                    kbType: Kooboo.Job.name
                                })
                            }
                        })
                        break;
                }
            }
        }

        this.showJobDialog = ko.observable(false);

        this.scheduleJobCode = ko.observableArray();

        this.onShowJobDialog = function() {
            if (self.scheduleJobCode().length) {
                self.showJobDialog(true);
            } else {
                Kooboo.Code.getListByType({
                    codeType: 'job'
                }).then(function(res) {
                    if (res.success) {
                        self.scheduleJobCode(res.model.map(function(item) {
                            return new codeModel(item)
                        }));
                        self.showJobDialog(true);
                    }
                })
            }
        }

        this.onHideJobDialog = function() {
            self.jobName('');
            self.startTime('');
            self.repeat(false);
            self.frequence('Second');
            self.frequenceUnit(null);
            self.showJobDialog(false);
            self.showKscript(false);
            self.showCode(false);
        }

        this.onSaveJob = function() {
            var selectedCode = self.scheduleJobCode().find(function(code) {
                return code.selected();
            })

            var data = {
                name: self.jobName(),
                script: self.script(),
                codeId: selectedCode ? selectedCode.id() : '',
                isRepeat: self.repeat(),
                startTime: self.startTime() || undefined,
                frequenceUnit: self.frequenceUnit() || 0,
                frequence: self.frequence()
            }

            Kooboo.Job.post(data).then(function(res) {
                if (res.success) {
                    window.info.done(Kooboo.text.info.save.success);
                    self.onHideJobDialog();
                    getList();
                }
            })
        }

        this.selectCode = function(m) {
            var selected = m.selected();
            self.scheduleJobCode().forEach(function(code) {
                code.selected(false);
            })
            m.selected(!selected);
        }

        function getList() {
            Kooboo.Job.getList().then(function(res) {
                if (res.success) {
                    self.curTab('pending')
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
                                text: Kooboo.text.common[item.isRepeat ? 'yes' : 'no'],
                                class: 'label-sm ' + 'label-' + (item.isRepeat ? 'success' : 'default')
                            }
                        }
                    })

                    self.tableData({
                        docs: docs,
                        columns: [{
                            displayName: Kooboo.text.common.name,
                            fieldName: 'name',
                            type: 'text'
                        }, {
                            displayName: "Code name",
                            fieldName: "codeName",
                            type: "label"
                        }, {
                            displayName: Kooboo.text.site.job.startTime,
                            fieldName: 'startTime',
                            type: 'text'
                        }, {
                            displayName: Kooboo.text.site.job.repeat,
                            fieldName: 'isRepeat',
                            type: 'label'
                        }],
                        kbType: Kooboo.Job.name
                    })
                }
            })
        }

        getList();

        Kooboo.EventBus.subscribe('ko/table/docs/selected', function(selected) {
            self.selectedJobs(selected);
        })

        this.onRun = function() {
            var id = self.selectedJobs().map(function(job) {
                return job.codeId();
            })

            Kooboo.Job.Run({
                id: id[0]
            }).then(function(res) {
                if (res.success) {
                    window.info.done("Runing");
                }
            })
        }
    }

    jobsModel.prototype = new Kooboo.tableModel(Kooboo.Job.name);

    var vm = new jobsModel();
    ko.applyBindings(vm, document.getElementById('main'));

    var codeModel = function(data) {
        var self = this;
        data.selected = false;
        ko.mapping.fromJS(data, {}, self);
    }
})