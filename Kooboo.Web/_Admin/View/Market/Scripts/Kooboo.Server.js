$(function() {
    function extend(Child, Parent) {
        Child.prototype = Parent.__proto__
    }

    function Infrastructure() {
        this.name = 'Infrastructure';

        this.getSalesItems = function(para) {
            return this.executeGet('SalesItems', para);
        }

        this.order = function(para) {
            return this.executePost('Order', para);
        }
    }
    extend(Infrastructure, Kooboo.BaseModel);

    Kooboo = Object.assign({
        Infrastructure: new Infrastructure()
    }, Kooboo);
})