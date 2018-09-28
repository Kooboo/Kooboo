$(function() {
    var iframe;

    var sentModel = function() {
        var self = this;

        this.checkTypes = ko.observableArray([{
            displayName: Kooboo.text.common.all,
            value: "All"
        }, {
            displayName: Kooboo.text.mail.read,
            value: "Read"
        }, {
            displayName: Kooboo.text.mail.unread,
            value: "Unread"
        }]);

        this.onCheckType = function(type) {
            switch (type) {
                case "All":
                    var isAllSelected = true;
                    _.forEach(self.mailList(), function(mail) {
                        isAllSelected = isAllSelected && mail.selected();
                    });

                    _.forEach(self.mailList(), function(mail) {
                        mail.selected(!isAllSelected)
                    })
                    break;
                case "Read":
                    _.forEach(self.mailList(), function(mail) {
                        mail.selected(false);
                        mail.read() && mail.selected(true);
                    });
                    break;
                case "Unread":
                    _.forEach(self.mailList(), function(mail) {
                        mail.selected(false);
                        (!mail.read()) && mail.selected(true);
                    });
                    break;
            }
        }

        this.actions = ko.observableArray([{
            displayText: Kooboo.text.common.delete,
            value: "delete"
        }, {
            displayText: Kooboo.text.mail.moveTo,
            value: "move"
        }, {
            displayText: Kooboo.text.mail.markAsRead,
            value: "read"
        }, {
            displayText: Kooboo.text.mail.markAsUnread,
            value: "unread"
        }]);

        this.onAction = function(type) {

            if (self.selectedList().length) {
                switch (type) {
                    case "delete":
                        var _ids = [];
                        _.forEach(self.selectedList(), function(mail) {
                            _ids.push(mail.id());
                        });
                        Kooboo.EmailMessage.Moves({
                            ids: JSON.stringify(_ids),
                            folder: "Trash"
                        }).then(function(res) {

                            if (res.success) {
                                var list = _.cloneDeep(self.selectedList());
                                _.forEach(list, function(mail) {
                                    var find = _.findLast(self.mailList(), function(m) {
                                        return m.id() == mail.id();
                                    })
                                    find && find.selected(false) && self.mailList.remove(find);

                                    if (self.currentMail()) {
                                        if (find.id() == self.currentMail().id) {
                                            self.currentMail(null)
                                        }
                                    }
                                })
                                window.info.show(Kooboo.text.info.moveTo.Trash.success, true);
                            } else {
                                window.info.show(Kooboo.text.info.moveTo.Trash.fail, false);
                            }
                        })
                        break;
                    case "move":
                        self.showMoveModal(true);
                        break;
                    case "read":
                    case "unread":
                        var _ids = [];
                        _.forEach(self.selectedList(), function(mail) {
                            _ids.push(mail.id());
                        })

                        Kooboo.EmailMessage.MarkReads({
                            ids: JSON.stringify(_ids),
                            value: type == "read"
                        }).then(function(res) {

                            if (res.success) {
                                var list = _.cloneDeep(self.selectedList());
                                _.forEach(list, function(mail) {
                                    mail.read(type == "read");
                                    mail.selected(false);
                                })
                                self.selectedList.removeAll();
                            }
                        })
                        break;
                }
            }
        }

        this.adjustIframe = function() {
            $(iframe).removeAttr("style");
            $(iframe).height(iframe.contentWindow.document.body.scrollHeight + 20);
        }

        this.currentMail = ko.observable();
        this.currentMail.subscribe(function(mail) {

            if (mail) {
                var setHTML = function(code) {
                    iframe.contentWindow.document.documentElement.innerHTML = code;
                    $("img", iframe.contentWindow.document).load(function() {
                        self.adjustIframe();
                    }).error(function() {
                        self.adjustIframe();
                    })
                    $("a", iframe.contentWindow.document).on("click", function (e) {
                        e.preventDefault();
                        parent.window.open($(this).attr("href"))
                    })
                    self.adjustIframe();
                }

                iframe = $("iframe.auto-height")[0];

                if (!iframe) {
                    setTimeout(function() {
                        iframe = $("iframe.auto-height")[0];
                        setHTML(mail.html);
                    }, 300);
                } else {
                    setHTML(mail.html);
                }
            }
        })

        this.mailList = ko.observableArray();
        this.selectedList = ko.observableArray();

        this.refreshList = function() {
            vm.mailList.removeAll();
            Kooboo.EmailMessage.getList({
                folder: "Sent",
                address: Kooboo.getQueryString("address") || ""
            }).then(function(res) {

                if (res.success) {
                    _.forEach(res.model, function(mail) {
                        vm.mailList.push(new mailModel(mail));
                    })
                    self.currentMail(null);
                }
            })
        }

        this.loadMore = function() {
            var last = _.cloneDeep(self.mailList()).reverse()[0];
            last && Kooboo.EmailMessage.More({
                folder: "Sent",
                address: Kooboo.getQueryString("address") || "",
                messageId: last.id()
            }).then(function(res) {

                if (res.success) {
                    _.forEach(res.model, function(mail) {
                        vm.mailList.push(new mailModel(mail));
                    })
                }
            })
        }

        this.selectMail = function(m) {
            _.forEach(self.mailList(), function(mail) {
                mail.reading(false);
            })

            if (m) {
                m.reading(true) && m.read(true);

                Kooboo.EmailMessage.getContent({
                    messageId: m.id()
                }).then(function(res) {

                    if (res.success) {
                        self.currentMail(res.model);
                    }
                })
            } else {
                self.currentMail(null);
            }
        }

        this.onReply = function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.Email.Compose, {
                type: "Reply",
                sourceId: self.currentMail().id,
                from: self.currentMail().from.address,
                folder: "Sent"
            })
        }

        this.onForward = function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.Email.Compose, {
                type: "Forward",
                sourceId: self.currentMail().id,
                from: self.currentMail().from.address,
                folder: "Sent"
            })
        }

        this.onPrint = function() {
            window.open(Kooboo.Route.Get(Kooboo.Route.Email.PrintPage, {
                id: self.currentMail().id,
                folder: "Sent"
            }))
        }

        this.moveToFolder = function(folder) {
            Kooboo.EmailMessage.Moves({
                ids: JSON.stringify([self.currentMail().id]),
                folder: folder
            }).then(function(res) {

                if (res.success) {
                    var _find = _.findLast(self.mailList(), function(mail) {
                        return mail.id() == self.currentMail().id;
                    })
                    _find && self.mailList.remove(_find);
                    self.selectMail(self.mailList()[0]);
                    window.info.show(Kooboo.text.info.moveTo[folder].success, true);
                } else {
                    window.info.show(Kooboo.text.info.moveTo[folder].fail, false);
                }
            })
        }

        this.showPrevMail = function() {
            var idx = _.findIndex(self.mailList(), function(mail) {
                return mail.id() == self.currentMail().id;
            })

            if (idx !== 0) {
                self.selectMail(self.mailList()[idx - 1]);
            }
        }

        this.showNextMail = function() {
            var idx = _.findIndex(self.mailList(), function(mail) {
                return mail.id() == self.currentMail().id;
            })

            if (idx !== self.mailList().length - 1) {
                self.selectMail(self.mailList()[idx + 1]);
            }
        }

        this.getDetailDate = ko.pureComputed(function() {
            var date = new Date(self.currentMail().date);
            return date.toDefaultLangString();
        })

        this.downloadAttachment = function(attachment) {
            window.open(Kooboo.EmailAttachment.downloadAttachment() +
                '/' + self.currentMail().id +
                (attachment ? '/' + attachment.fileName : ''))
        }

        this.showMoveModal = ko.observable(false);

        this.getDisplayDate = function(d) {
            var date = new Date(d);
            return date.toDefaultLangString();
        }

        this.folders = ko.observableArray([{
            displayName: Kooboo.text.mail.Inbox,
            value: "Inbox"
        }]);

        this.targetFolder = ko.observable("Inbox");

        this.resetModal = function() {
            self.showMoveModal(false);
        }

        this.startMove = function() {
            var _ids = [];
            _.forEach(self.selectedList(), function(mail) {
                _ids.push(mail.id());
            })
            Kooboo.EmailMessage.Moves({
                ids: JSON.stringify(_ids),
                folder: self.targetFolder() || "Inbox"
            }).then(function(res) {

                if (res.success) {
                    _.forEach(self.selectedList(), function(mail) {
                        self.mailList.remove(mail);
                    });
                    self.selectedList.removeAll();
                    self.selectMail(self.mailList()[0]);

                    window.info.show(Kooboo.text.info.moveTo[self.targetFolder() || "Inbox"].success, true);
                    self.resetModal();
                } else {
                    window.info.show(Kooboo.text.info.moveTo[self.targetFolder() || "Inbox"].fail, false);
                }
            })
        }
    }

    var vm = new sentModel();

    ko.applyBindings(vm, document.getElementById("main"));

    Kooboo.EmailMessage.getList({
        folder: "Sent",
        address: Kooboo.getQueryString("address") || ""
    }).then(function(res) {

        if (res.success) {
            _.forEach(res.model, function(mail) {
                vm.mailList.push(new mailModel(mail));
            })
        }
    })

    var mailModel = function(mail) {
        var self = this;

        ko.mapping.fromJS(mail, {}, self);
        this.selected = ko.observable(false);
        this.selected.subscribe(function(checked) {

            if (checked) {
                vm.selectedList.push(self);
            } else {
                vm.selectedList.remove(self);
            }
        })

        this.reading = ko.observable(false);

        var date = new Date(mail.date);
        this.displayDate = ko.observable(date.toDefaultLangString());
    }
})