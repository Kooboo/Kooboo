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

        this.getPaymentMethods = function(para) {
            return this.executeGet('PaymentMethods', para);
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

        this.TopupHistory = function(para) {
            return this.executeGet('TopupHistory', para);
        }

        this.getPaymentStatus = function(para) {
            return this.executeGet('PaymentStatus', para, true);
        }
    }
    extend(Balance, Kooboo.BaseModel);

    Kooboo = Object.assign({
        Balance: new Balance(),
        Infrastructure: new Infrastructure()
    }, Kooboo);
})