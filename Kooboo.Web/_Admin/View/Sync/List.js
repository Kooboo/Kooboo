$(function() {

    var settingId = Kooboo.getQueryString("id") || location.hash.split("#")[1];

    var CONTIUNE_PULLING = true;

    var syncModel = function() {
        var self = this;

        function pollingPull(senderVersion) {
            if (CONTIUNE_PULLING) {
                Kooboo.Publish.pull({
                    id: Kooboo.getQueryString("id"),
                    senderVersion: senderVersion
                }).then(function(res) {
                    if (!res.model.isFinish) {
                        pollingPull(res.model.senderVersion);
                    } else {
                        self.isPulling(false);
                        window.onbeforeunload = function() {};
                        window.info.show(Kooboo.text.info.pull.success, true);
                        self.changeTab("pull");
                        DataCache.removeRelatedData("pull");
                    }
                })
            }
        }

        self.isPulling = ko.observable(false);
        self.isPushing = ko.observable(false);

        self.PullToLocal = function(m, e) {
            e.preventDefault();
            window.info.show(Kooboo.text.info.startPulling, true);
            pollingPull();
            self.isPulling(true);

            Kooboo.SPA.beforeUnload = function() {
                var flag = false;

                if (self.isPulling()) {
                    flag = confirm(Kooboo.text.confirm.siteSynchronizing);
                }

                if (flag) {
                    CONTIUNE_PULLING = false;
                    return 'refresh';
                } else {
                    return 'abort';
                }
            }
        }

        self.pushToRemote = function() {

            if (self.selectedItems().length) {
                self.isPushing(true);
                var logids = [];
                _.forEach(self.selectedItems(), function(item) {
                    logids.push(item.id);
                });

                Kooboo.Publish.push({
                    id: settingId,
                    logids: JSON.stringify(logids)
                }).then(function(res) {

                    if (res.success) {
                        self.isPushing(false);
                        self.changeTab("push");
                        window.info.show(Kooboo.text.info.push.success, true);
                        self.selectedItems.removeAll();
                        localStorage.clear();
                    } else {
                        window.info.show(Kooboo.text.info.push.fail, false);
                    }
                })
            } else {
                alert(Kooboo.text.alert.selectBeforePushing);
            }
        }

        this.selectItem = function(item) {
            item.selected(!item.selected());
            self.selectedItems[item.selected() ? "push" : "remove"](item);
        }

        this.selectedItems = ko.observableArray();

        this.getChangeClass = function(type) {
            var _class = "";
            switch (type.toLowerCase()) {
                case 'add':
                    _class = 'green'
                    break;
                case 'update':
                    _class = 'blue';
                    break;
                case 'delete':
                    _class = 'red';
                    break
            }
            return _class;
        }

        this.getDisplayTime = function(t) {
            var d = new Date(t);
            return d.toDefaultLangString();
        }

        this.localChanges = ko.observableArray();

        this.dynamicItems = ko.observableArray();

        this.pager = ko.observable();

        this.tabs = ko.observable([{
            displayName: Kooboo.text.site.sync.localChanges,
            value: "local"
        }, {
            displayName: Kooboo.text.site.sync.pullLog,
            value: "pull"
        }, {
            displayName: Kooboo.text.site.sync.pushLog,
            value: "push"
        }]);

        this.curTab = ko.observable();

        this.changeTab = function(tab) {
            if (tab !== self.curTab()) {

                switch (tab) {
                    case "local":
                        Kooboo.Publish.pushItems({
                            id: settingId
                        }).then(function(res) {
                            if (res.success) {
                                var _list = [];
                                _.forEach(res.model, function(change) {
                                    ko.mapping.fromJS(change);

                                    change.selected = ko.observable(false);

                                    _list.push(change);
                                })
                                self.localChanges(_list);
                            }
                        })
                        self.curTab(tab);
                        break;
                    case "push":
                        self.allSelected(false);
                        Kooboo.Publish.pushLog({
                            SyncSettingId: settingId
                        }).then(function(res) {

                            if (res.success) {
                                self.dynamicItems(res.model.list);
                                self.pager(res.model);
                                self.curTab(tab);
                            }
                        });
                        break;
                    case "pull":
                        self.allSelected(false);
                        Kooboo.Publish.pullLog({
                            SyncSettingId: settingId
                        }).then(function(res) {

                            if (res.success) {
                                self.dynamicItems(res.model.list);
                                self.pager(res.model);
                                self.curTab(tab);
                            }
                        })
                        break;
                }
            }
        }

        this.allSelected = ko.pureComputed({
            read: function() {
                if (self.localChanges().length === 0) {
                    return false;
                }
                return self.localChanges().length == self.selectedItems().length;
            },
            write: function(value) {

                if (typeof value == "boolean") {
                    self.selectedItems.removeAll();

                    _.forEach(self.localChanges(), function(change) {
                        change.selected(value);
                        value && self.selectedItems.push(change);
                    })
                }
            },
            owner: this
        })

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            Kooboo.Publish[self.curTab() + "Log"]({
                SyncSettingId: settingId,
                pageNr: page
            }).then(function(res) {

                if (res.success) {
                    self.dynamicItems(res.model.list);
                    self.pager(res.model);
                }
            })
        })
    };

    var vm = new syncModel();
    ko.applyBindings(vm, document.getElementById("main"));

    vm.changeTab("local");
});