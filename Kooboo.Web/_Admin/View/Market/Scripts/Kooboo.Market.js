$(function() {
    function extend(Child, Parent) {
        Child.prototype = Parent.__proto__
    }

    function Discussion() {
        this.name = 'Discussion';

        this.getEdit = function(para) {
            return this.executeGet('GetEdit', para);
        }

        this.getCommentList = function(para) {
            return this.executeGet('CommentList', para);
        }

        this.getNestCommentList = function(para) {
            return this.executeGet('NestCommentList', para);
        }

        this.addOrUpdate = function(para) {
            return this.executePost('AddOrUpdate', para);
        }

        this.reply = function(para) {
            return this.executePost('Reply', para);
        }

        this.getUserList = function(para) {
            return this.executeGet('UserList', para);
        }
    }
    extend(Discussion, Kooboo.BaseModel);

    function Demand() {
        this.name = 'Demand';

        this.getUserList = function(para) {
            return this.executeGet('UserList', para);
        }

        this.addOrUpdate = function(para) {
            return this.executePost('AddOrUpdate', para);
        }

        this.getProposalList = function(para) {
            return this.executeGet('ProposalList', para);
        }

        this.getUserProposal = function(para) {
            return this.executeGet('GetUserProposal', para);
        }

        this.getProposal = function(para) {
            return this.executeGet('GetProposal', para);
        }

        this.acceptProposal = function(para) {
            return this.executeGet('AcceptProposal', para);
        }

        this.deleteProposal = function(para) {
            return this.executePost('DeleteProposal', para);
        }

        this.addOrUpdateProposal = function(para) {
            return this.executePost('AddOrUpdateProposal', para);
        }

        this.reply = function(para) {
            return this.executePost('ReplyDemand', para, true);
        }

        this.chat = function(para) {
            return this.executePost('ReplyChat', para, true);
        }

        this.confirmDemandStatus = function(para) {
            return this.executePost('ConfirmDemandStatus', para);
        }

        this.getPublicCommentList = function(para) {
            return this.executeGet('PublicCommentList', para);
        }

        this.getNestedPublicCommentList = function(para) {
            return this.executeGet('NestedPublicCommentList', para);
        }

        this.getPublicNestedCommentList = function(para) {
            return this.executeGet('PublicNestedCommentList', para);
        }

        this.getPrivateCommentList = function(para) {
            return this.executeGet('PrivateCommentList', para, true);
        }

        this.uploadFile = function(para) {
            return this.executeUpload('UploadFile', para);
        }

        this.deleteFile = function(para) {
            return this.executePost('DeleteFile', para);
        }

        this.getProposalTypes = function(para) {
            return this.executeGet('GetProposalType', para);
        }

        this.getMyProposalList = function(para) {
            return this.executeGet('MyProposalList', para);
        }

        this.raiseObjection = function(para) {
            return this.executePost('RaiseAnObjection', para);
        }

        this.getDemandObjection = function(para) {
            return this.executeGet('GetDemandObjection', para);
        }

        this.getDemandObejctionList = function(para) {
            return this.executeGet('GetDemandObjectionList', para);
        }
    }
    extend(Demand, Kooboo.BaseModel);

    function Supplier() {
        this.name = 'Supplier';

        this.isSupplier = function(para) {
            return this.executeGet('IsSupplier', para);
        }

        this.getByUser = function(para) {
            return this.executeGet('GetByUser', para);
        }

        this.addOrUpdate = function(para) {
            return this.executePost('AddOrUpdate', para);
        }

        this.getOrdersBySupplier = function(para) {
            // 我收到的所有 orders
            return this.executeGet('OrderBySupplierUser', para);
        }

        this.getOrdersByUser = function(para) {
            // 我发出的所有 orders
            return this.executeGet('OrdersByUser', para);
        }

        this.addOrUpdateOrder = function(para) {
            return this.executePost('AddOrUpdateOrder', para);
        }

        this.acceptOrder = function(para) {
            return this.executePost('AcceptSupplierOrder', para);
        }

        this.orderFinished = function(para) {
            return this.executePost('ConfirmSupplierOrder', para);
        }

        this.getMyOrdersInSupply = function(para) {
            // 在 supply 中发出的 orders
            return this.executeGet('MyOrdersInSupply', para);
        }

        this.getUserExpertiseList = function(para) {
            return this.executeGet('UserExpertiseList', para);
        }

        this.deleteExpertise = function(para) {
            return this.executePost('DeleteExpertise', para);
        }

        this.addOrUpdateExpertise = function(para) {
            return this.executePost('AddOrUpdateExpertise', para);
        }

        this.getExpertiseList = function(para) {
            return this.executeGet('ExpertiseList', para);
        }

        this.getExpertise = function(para) {
            return this.executeGet('GetExpertise', para);
        }
    }
    extend(Supplier, Kooboo.BaseModel);

    function Infrastructure() {
        this.name = 'Infrastructure';

        this.getSalesItems = function(para) {
            return this.executeGet('SalesItems', para);
        }

        this.order = function(para) {
            return this.executePost('Order', para);
        }

        this.getMonthlyReport = function(para) {
            return this.executeGet('MonthlyReport', para);
        }

        this.getMonthlyLogs = function(para) {
            return this.executeGet('MonthlyLogs', para, true);
        }

        this.getTypes = function(para) {
            return this.executeGet('Types', para);
        }
    }
    extend(Infrastructure, Kooboo.BaseModel);

    function Balance() {
        this.name = "Balance";

        this.getBalance = function(para) {
            return this.executeGet('GetBalance', para);
        }

        this.getChargePackages = function(para) {
            return this.executeGet('ChargePackages', para);
        }

        this.topup = function(para) {
            return this.executePost('Topup', para);
        }

        this.useCoupon = function(para) {
            return this.executePost('UseCoupon', para);
        }

        this.getTopupHistory = function(para) {
            return this.executeGet('TopupHistory', para);
        }

        this.getPaymentStatus = function(para) {
            return this.executeGet('PaymentStatus', para, true);
        }
    }
    extend(Balance, Kooboo.BaseModel);

    function App() {
        this.name = 'App';

        this.getPersonal = function(para) {
            return this.executeGet('Personal', para);
        }

        this.getPrivate = function(para) {
            return this.executeGet('Private', para);
        }
    }
    extend(App, Kooboo.BaseModel);

    function Payment() {
        this.name = 'Payment';

        this.getMethods = function(para) {
            return this.executeGet('Methods', para);
        }

        this.getStatus = function(para) {
            return this.executeGet('Status', para, true);
        }
    }
    extend(Payment, Kooboo.BaseModel);

    function Order() {
        this.name = 'Order';

        this.topup = function(para) {
            return this.executePost('Topup', para);
        }

        this.domain = function(para) {
            return this.executePost('Domain', para);
        }

        this.infra = function(para) {
            return this.executePost('Infra', para);
        }

        this.useCoupon = function(para) {
            return this.executePost('UseCoupon', para);
        }
    }
    extend(Order, Kooboo.BaseModel);

    function Market() {
        this.name = 'Market';

        this.getMy = function(para) {
            return this.executeGet('My', para);
        }
    }
    extend(Market, Kooboo.BaseModel);

    Kooboo = Object.assign({
        App: new App(),
        Balance: new Balance(),
        Infrastructure: new Infrastructure(),
        Demand: new Demand(),
        Discussion: new Discussion(),
        Market: new Market(),
        Order: new Order(),
        Payment: new Payment(),
        Supplier: new Supplier(),
    }, Kooboo);
})