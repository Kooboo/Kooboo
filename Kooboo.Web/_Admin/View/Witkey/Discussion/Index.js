$(function() {

    var viewModel = function() {
        var self = this;

        this.isUserList = ko.observable(Kooboo.getQueryString('type') ? Kooboo.getQueryString('type') == 'me' : false);

        this.list = ko.observableArray();
        this.pager = ko.observable();

        this.showDiscussionModal = ko.observable(false);
        this.editContent = ko.observable();
        this.onShowDiscussionModal = function() {
            self.showDiscussionModal(true);
        }

        this.onEdit = function(m, e) {
            self.editContent(m);
            self.showDiscussionModal(true);
        }
        this.onDelete = function(m, e) {
            if (confirm(Kooboo.text.confirm.deleteItem)) {
                let idx = _.findIndex(self.list(), function(item) {
                    return item.id == m.id;
                })
                Kooboo.Discussion.Delete({
                    id: m.id
                }).then(function(res) {
                    if (res.success) {
                        window.info.done(Kooboo.text.info.delete.success);
                        var list = _.cloneDeep(self.list());
                        list.splice(idx, 1);
                        self.list(list);
                    }
                })
            }
        }

        this.onViewType = function(type) {
            var url = '';
            if (type == 'all') {
                url = Kooboo.Route.Discussion.ListPage
            } else if (type == 'me') {
                url = Kooboo.Route.Get(Kooboo.Route.Discussion.ListPage, {
                    type: 'me'
                })
            }

            location.href = url;
        }

        this.getItems = function(page) {
            if (!self.isUserList()) {
                Kooboo.Discussion.getList({
                    pageNr: page || 1,
                }).then(function(res) {
                    if (res.success) {
                        self.pager(res.model);
                        self.list(res.model.list);
                    }
                })
            } else {
                Kooboo.Discussion.getUserList({
                    pageNr: page || 1
                }).then(function(res) {
                    if (res.success) {
                        self.pager(res.model);
                        self.list(res.model.list);
                    }
                })
            }
        }

        self.getItems();

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            self.getItems(page);
        })

        Kooboo.EventBus.subscribe('kb/component/discussion-modal/saved', function() {
            self.getItems();
        })
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})