$(function() {

    var discussionModel = function() {
        var self = this;

        this.id = ko.observable(Kooboo.getQueryString("id"));

        this.title = ko.observable();
        this.userName = ko.observable();
        this.date = ko.observable();
        this.viewCount = ko.observable();
        this.commentCount = ko.observable();

        this.content = ko.observable();

        this.comments = ko.observableArray();
        this.pager = ko.observable();

        Kooboo.Discussion.get({
            id: self.id()
        }).then(function(res) {
            if (res.success) {
                self.title(res.model.title);
                self.userName(res.model.userName);
                self.date(new Date(res.model.lastModified).toDefaultLangString())
                self.content(res.model.content);
                self.viewCount(res.model.viewCount);
                self.commentCount(res.model.commentCount);
                self.getCommentList();
            }
        })

        this.getCommentList = function(page) {
            Kooboo.Discussion.getCommentList({
                id: self.id(),
                pageNr: page || 1
            }).then(function(res) {
                self.pager(res.model);
                self.comments(res.model.list.map(function(item) {
                    return new Comment(item)
                }));
            })
        }

        this.commentRendered = function() {
            Holder.run();
        }

        this.onToggleSubComment = function(m, e) {
            if (!m.showSubComment()) {
                self.getNestCommentList(m);
            } else {
                m.showSubComment(false);
            }
        }

        this.getNestCommentList = function(m) {
            Kooboo.Discussion.getNestCommentList({
                commentId: m.id()
            }).then(function(res) {
                if (res.success) {
                    m.showSubComment(true);
                    m.subCommentList(res.model.map(function(item) {
                        return new Comment(item);
                    }))
                    $(".autosize").textareaAutoSize().trigger("keyup");
                }
            })
        }

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            self.getCommentList(page);
        })

        Kooboo.EventBus.subscribe('kb/witkey/component/reply/refresh', function(data) {
            if (data.parentId == Kooboo.Guid.Empty) {
                self.getCommentList(-1);
            } else {
                var comment = self.comments().find(function(com) {
                    return com.id() == data.parentId;
                })

                if (comment) {
                    self.getNestCommentList(comment);
                }
            }
        })

    }

    var vm = new discussionModel();
    ko.applyBindings(vm, document.getElementById('main'))

    function Comment(data) {
        if (data.content.indexOf('\n') > -1) {
            data.content = data.content.split('\n').join('<br>')
        }
        var date = new Date(data.lastModified);
        this.id = ko.observable(data.id);
        this.firstLetter = data.userName.split('')[0].toUpperCase();
        this.userName = data.userName;
        this.content = data.content;
        this.date = date.toDefaultLangString();
        this.commentCount = data.commentCount;
        this.showSubComment = ko.observable(false);
        this.subCommentList = ko.observableArray([]);
    }
})