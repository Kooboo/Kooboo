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
            return this.executeGet('ReplyDemand', para, true);
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
    }
    extend(Demand, Kooboo.BaseModel);

    Kooboo = Object.assign({
        Demand: new Demand(),
        Discussion: new Discussion()
    }, Kooboo);
})