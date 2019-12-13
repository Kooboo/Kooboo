$(function() {

    var self;
    new Vue({
        el: "#app",
        data: function () {
            self = this;
            return {
                breads: [{
                    name: 'MARKET'
                }],
                username: undefined,
                isUserVerified:false,
                currentDataCenter:undefined,
                availableDataCenters:undefined,
                balance:undefined,
                currencySymbol:undefined,
                currencyCode:undefined,
                userEmail:undefined,
                showUserVerifyModal:false,
                showRechargeModal:false,
                showDataCenterModal:false



            };
        },
        created: function () {
            self.getData();
            self.getUser()
        },
        methods: {
            getData: function () {
                Kooboo.Balance.getBalance().then(function(res) {
                    if (res.success) {
                        self.organizationId = res.model.id;
                        self.username = res.model.name;
                        self.balance = res.model.balance;
                        self.currencySymbol = res.model.currency.symbol;
                        self.currencyCode = res.model.currency.code;

                    }
                })
            },
            getUser: function() {
                Kooboo.Organization.getDataCenter().then(function(res) {
                    if (res.success) {
                        self.username = res.model.organizationName;
                        self.currentDataCenter = res.model.currentDatacenterName;
                        self.availableDataCenters = res.model.availables.map(function(dc) {
                            return {
                                displayName: dc.displayName + ' (' + dc.country + ')',
                                value: dc.value
                            }
                        });
                    }
                });
                Kooboo.User.getUser().then(function(res) {
                    if (res.success) {
                        self.userEmail = res.model.emailAddress;
                        self.isUserVerified = res.model.isEmailVerified;
                    }
                })
            },

            onVerifyUser:function () {
                if (!self.isUserVerified) {
                    self.showUserVerifyModal = true;
                }
            },
            onShowRechargeModal:function () {
                self.showRechargeModal = true;
            },
            onShowTopupHistoryModal:function () {
                self.showTopupHistoryModal = true;
            },
            onShowCurrenies:function () {
                self.showCurrencyModal = true;
            },
            onChangeDC:function () {
                self.showDataCenterModal = true;
            }

            
        },
        computed: {
            displayBalance: function () {
                if (self.currencySymbol !== undefined && self.balance !== undefined) {
                    return self.currencySymbol + self.balance;
                } else {
                    return Kooboo.text.common.loading;
                }
            }
        }


    })


});
