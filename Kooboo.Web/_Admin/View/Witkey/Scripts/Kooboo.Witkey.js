$(function() {
    function extend(Child, Parent) {
        Child.prototype = Parent.__proto__
    }

    function Discussion() {
        this.name = 'Discussion';

        this.getCommentList = function(para) {
            return this.executeGet('CommentList', para);
        }

        this.getNestCommentList = function(para) {
            return this.executeGet('NestCommentList', para);
        }

        this.add = function(para) {
            return this.executePost('Add', para);
        }

        this.reply = function(para) {
            return this.executePost('Reply', para);
        }
    }
    extend(Discussion, Kooboo.BaseModel);

    Kooboo = Object.assign({
        Discussion: new Discussion()
    }, Kooboo);
})