$(function() {

    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.getItems = function(page) {
            Kooboo.Discussion.getList({
                pageNr: page || 1,
            }).then(function(res) {
                if (res.success) {
                    self.handleData(res.model);
                }
            })
        }

        this.onCreateDiscussion = function() {
            self.showDiscussionModal(true);
        }

        this.showDiscussionModal = ko.observable(false);

        this.handleData = function(data) {
            self.pager(data);

            var docs = data.list.map(function(item) {
                var date = new Date(item.lastModified);

                return {
                    id: item.id,
                    title: {
                        text: item.title,
                        url: Kooboo.Route.Get(Kooboo.Route.Discussion.DetailPage, {
                            id: item.id
                        })
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
                    view: {
                        iconClass: 'fa-eye',
                        url: Kooboo.Route.Get(Kooboo.Route.Discussion.DetailPage, {
                            id: item.id
                        })
                    },
                    lastModified: date.toDefaultLangString()
                }
            })

            self.tableData({
                docs: docs,
                columns: [{
                    displayName: "Title",
                    fieldName: "title",
                    type: 'link'
                }, {
                    displayName: 'Comments',
                    fieldName: 'comments',
                    showClass: 'table-short',
                    type: 'badge'
                }, {
                    displayName: 'Views',
                    fieldName: 'views',
                    showClass: 'table-short',
                    type: 'badge'
                }, {
                    displayName: 'User',
                    fieldName: 'user',
                    showClass: 'table-short',
                    type: 'label'
                }, {
                    displayName: 'Last modified',
                    fieldName: 'lastModified',
                    showClass: 'table-short',
                    type: 'text'
                }],
                tableActions: [{
                    fieldName: 'view',
                    type: 'link-icon'
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
    }

    viewModel.prototype = new Kooboo.tableModel(Kooboo.Discussion.name);
    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})