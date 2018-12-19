$(function() {

    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.showDiscussionModal = ko.observable(false);

        this.editId = ko.observable();

        this.onCreateDiscussion = function() {
            self.showDiscussionModal(true);
        }

        this.getItems = function(page) {
            Kooboo.Discussion.ListByUser({
                pageNr: page || 1,
            }).then(function(res) {
                if (res.success) {
                    self.handleData(res.model);
                }
            })
        }

        this.handleData = function(data) {
            self.pager(data);

            var docs = data.list.map(function(item) {
                var date = new Date(item.lastModified);

                return {
                    id: item.id,
                    article: {
                        title: item.title,
                        description: item.content,
                        url: Kooboo.Route.Get(Kooboo.Route.Discussion.DetailPage, {
                            id: item.id
                        }),
                        class: "title",
                        newWindow: true
                    },
                    comments: {
                        text: item.commentCount,
                        class: 'badge badge-info'
                    },
                    views: {
                        text: item.viewCount,
                        class: 'badge badge-info'
                    },
                    user: {
                        text: item.userName,
                        class: 'lable label-sm gray'
                    },
                    
                    delete: {
                        iconClass: 'fa-trash',
                        class: 'btn-danger',
                        url: 'kb/discussion/my/delete'
                    },
                    lastModified: date.toDefaultLangString()
                }
            })

            self.tableData({
                docs: docs,
                columns: [
                {
                    displayName: Kooboo.text.common.Discussion,
                    fieldName: 'article',
                    type: 'summary'
                },
                {
                    displayName: Kooboo.text.market.discussion.Comments,
                    fieldName: 'comments',
                    showClass: 'table-short',
                    type: 'badge'
                }, {
                    displayName: Kooboo.text.common.visitors,
                    fieldName: 'views',
                    showClass: 'table-short',
                    type: 'badge'
                }, {
                    displayName: Kooboo.text.common.user,
                    fieldName: 'user',
                    showClass: 'table-short',
                    type: 'label'
                }, {
                    displayName: Kooboo.text.common.lastModified,
                    fieldName: 'lastModified',
                    showClass: 'table-short',
                    type: 'text'
                }],
                tableActions: [ {
                    fieldName: 'delete',
                    type: 'communication-icon-btn'
                }],
                kbType: Kooboo.Discussion.name,
                unselectable: true
            })
        }

        self.getItems();

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            self.getItems(page);
        })

        Kooboo.EventBus.subscribe('kb/component/discussion-modal/saved', function() {
            self.getItems();
        })

        Kooboo.EventBus.subscribe('kb/discussion/my/edit', function(data) {
            self.editId(data.id);
            self.showDiscussionModal(true);
        })
        Kooboo.EventBus.subscribe('kb/discussion/my/delete', function(data) {
            if (confirm(Kooboo.text.confirm.deleteItem)) {
                Kooboo.Discussion.Delete({
                    id: data.id
                }).then(function(res) {
                    if (res.success) {
                        window.info.done(Kooboo.text.info.delete.success);
                        self.getItems();
                    }
                })
            }
        })
    }

    viewModel.prototype = new Kooboo.tableModel(Kooboo.Discussion.name);
    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})