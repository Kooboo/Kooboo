$(function() {

    var discussionModel = function() {
        var self = this;

        this.detail = ko.observable();

        this.comments = ko.observableArray();

        Kooboo.Discussion.get({
            id: Kooboo.getQueryString("id")
        }).then(function(res) {
            if (res.success) {
                self.detail(res.model);
            }
        })

        Kooboo.Discussion.getCommentList({
            id: Kooboo.getQueryString("id")
        }).then(function(res) {
            self.detail(res.model.list);
        })
    }
})