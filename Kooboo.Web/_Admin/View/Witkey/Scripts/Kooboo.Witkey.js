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
            return this.executeGet('OrderBySupplierUser', para);
        }

        this.getOrdersByUser = function(para) {
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
    }
    extend(Supplier, Kooboo.BaseModel);

    Kooboo = Object.assign({
        Demand: new Demand(),
        Discussion: new Discussion(),
        Supplier: new Supplier()
    }, Kooboo);
})