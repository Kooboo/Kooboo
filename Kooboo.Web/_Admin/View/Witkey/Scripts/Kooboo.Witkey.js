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

        this.isProposalUser = function(para) {
            return this.executeGet('IsProposalUser', para);
        }
    }
    extend(Demand, Kooboo.BaseModel);

    Kooboo = Object.assign({
        Demand: new Demand(),
        Discussion: new Discussion()
    }, Kooboo);
})