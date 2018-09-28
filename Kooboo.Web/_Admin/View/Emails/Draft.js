$(function() {
    var iframe;

    var draftModel = function() {
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
                        var _ids = self.selectedList().map(function(mail) {
                            return mail.id();
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
                folder: "Drafts",
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
                folder: "Drafts",
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

        this.onEdit = function(m) {
            location.href = Kooboo.Route.Get(Kooboo.Route.Email.Compose, {
                messageId: m.currentMail().id,
                type: "Drafts",
                folder: "Draft"
            })
        }

        this.onPrint = function() {
            window.open(Kooboo.Route.Get(Kooboo.Route.Email.PrintPage, {
                id: self.currentMail().id,
                folder: "Drafts"
            }))
        }

        this.onDelete = function() {
            Kooboo.EmailMessage.Moves({
                ids: JSON.stringify([self.currentMail().id]),
                folder: "Trash"
            }).then(function(res) {

                if (res.success) {
                    var _find = _.findLast(self.mailList(), function(mail) {
                        return mail.id() == self.currentMail().id;
                    })
                    _find && self.mailList.remove(_find);
                    self.selectMail(self.mailList()[0]);
                    window.info.show(Kooboo.text.info.moveTo.Trash.success, true);
                } else {
                    window.info.show(Kooboo.text.info.moveTo.Trash.fail, false);
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
    }

    var vm = new draftModel();

    ko.applyBindings(vm, document.getElementById("main"));

    Kooboo.EmailMessage.getList({
        folder: "Drafts",
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