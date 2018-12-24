$(function() {
    var CURRENT_USER_ID = '';
    var INTERVAL = null;

    var viewModel = function() {
        var self = this;

        this.id = ko.observable(Kooboo.getQueryString('id'));

        this.breadcrumb = ko.observable();

        this.serviceName = ko.observable();

        this.remark = ko.observable();
        this.attachments = ko.observableArray();

        this.createdAt = ko.observable();
        this.price = ko.observable();
        this.isPaid = ko.observable();

        this.supplierName = ko.observable();
        this.buyerName = ko.observable();

        this.ImBuyer = ko.observable(false);
        this.isClose = ko.observable(false);

        this.getOrder = function(cb) {
            Kooboo.Supplier.getOrder({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    var data = res.model;
                    self.remark(data.remark && data.remark.split('\n').join('<br>'));
                    self.serviceName(data.name);
                    self.createdAt(new Date(data.creationDate).toDefaultLangString());
                    self.attachments(data.attachments ? data.attachments.map(function(item) {
                        item.downloadUrl = '/_api/attachment/getFile?id=' + item.id + '&fileName=' + item.fileName;
                        return item;
                    }) : []);
                    self.price(data.symbol + data.totalAmount);
                    self.isPaid(data.isPaid);
                    self.buyerName(data.buyerOrgName);
                    self.supplierName(data.orgName);
                    self.ImBuyer(data.buyerOrganizationId == localStorage.getItem('_kooboo_api_user'));
                    self.isClose(data.isClose);
                    if (data.isClose) {
                        self.getPublicCommentList();
                    } else {
                        INTERVAL = setInterval(function() {
                            self.getPublicCommentList();
                        }, 2000);
                    }

                    self.breadcrumb([{
                        name: 'MARKET'
                    }, {
                        name: Kooboo.text.common.Suppliers,
                        url: Kooboo.Route.Supplier.ListPage
                    }, {
                        name: Kooboo.text.market.supplier[self.ImBuyer() ? 'myOrders' : 'myOffers'],
                        url: Kooboo.Route.Supplier[self.ImBuyer() ? 'MyOrdersPage' : 'MyOffersPage']
                    }, {
                        name: Kooboo.text.common.detail
                    }]);
                }
            })
        }

        this.messages = ko.observableArray();
        this.getPublicCommentList = function(cb) {
            Kooboo.Supplier.getPublicCommentList({
                id: self.id()
            }).then(function(res) {
                if (res.success) {
                    self.messages(res.model.list.map(function(item) {
                        return new Message(item);
                    }));

                    cb && cb();
                }
            })
        }

        this.onComplete = function() {
            if(confirm(Kooboo.text.confirm.market.completedOrder)){
                Kooboo.Supplier.onComplete({
                    id: self.id(),
                    isAccept: true
                }).then(function(res) {
                    if (res.success) {
                        location.reload();
                    }
                })
            }
        }

        this.getOrder();

        Kooboo.EventBus.subscribe('kb/market/reply/sent', function(cb) {
            self.getPublicCommentList(cb);
        })

        Kooboo.SPA.beforeUnload = function() {
            INTERVAL && clearInterval(INTERVAL);
            return 'refresh';
        }
    }

    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));

    function Message(data) {
        if (!CURRENT_USER_ID) {
            CURRENT_USER_ID = localStorage.getItem('_kooboo_api_user');
        }

        var date = new Date(data.lastModified);

        this.firstLetter = ko.observable(data.userName.split('')[0].toUpperCase());
        this.content = ko.observable(data.content);
        this.isCurrentUser = ko.observable(data.userId == CURRENT_USER_ID);
        this.userName = ko.observable(this.isCurrentUser() ? Kooboo.text.market.supplier.me : data.userName);
        this.date = ko.observable(date.toDefaultLangString());
        this.attachment = ko.observable(data.attachments ? data.attachments.map(function(item) {
            item.url = '/_api/attachment/getFile?id=' + item.id + '&fileName=' + item.fileName;
            return item;
        })[0] : '');
    }
})