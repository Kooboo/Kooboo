(function() {
  function extend(Child, Parent) {
    Child.prototype = Parent.__proto__;
  }

  function Discussion() {
    this.name = "Discussion";

    this.get = function(para) {
      return this.executeGet("Get", para);
    };

    this.getCommentList = function(para) {
      return this.executeGet("CommentList", para);
    };

    this.getNestCommentList = function(para) {
      return this.executeGet("NestCommentList", para);
    };

    this.addOrUpdate = function(para) {
      return this.executePost("AddOrUpdate", para);
    };

    this.reply = function(para) {
      return this.executePost("Reply", para);
    };

    this.ListByUser = function(para) {
      return this.executeGet("ListByUser", para);
    };
  }
  extend(Discussion, Kooboo.BaseModel);

  function Demand() {
    this.name = "Demand";

    this.ListByUser = function(para) {
      return this.executeGet("ListByUser", para);
    };

    this.addOrUpdate = function(para) {
      return this.executePost("AddOrUpdate", para);
    };

    this.proposalListByDemand = function(para) {
      return this.executeGet("ProposalListByDemand", para);
    };

    this.getUserProposal = function(para) {
      return this.executeGet("GetUserProposal", para);
    };

    this.getProposal = function(para) {
      return this.executeGet("GetProposal", para);
    };

    this.acceptProposal = function(para) {
      return this.executeGet("AcceptProposal", para);
    };

    this.deleteProposal = function(para) {
      return this.executePost("DeleteProposal", para);
    };

    this.addOrUpdateProposal = function(para) {
      return this.executePost("AddOrUpdateProposal", para);
    };

    this.reply = function(para) {
      return this.executePost("ReplyDemand", para, true);
    };

    this.chat = function(para) {
      return this.executePost("ReplyChat", para, true);
    };

    this.complete = function(para) {
      return this.executePost("complete", para);
    };

    this.getPublicCommentList = function(para) {
      return this.executeGet("PublicCommentList", para);
    };

    this.getNestedPublicCommentList = function(para) {
      return this.executeGet("NestedPublicCommentList", para);
    };

    this.getPublicNestedCommentList = function(para) {
      return this.executeGet("PublicNestedCommentList", para);
    };

    this.getPrivateCommentList = function(para) {
      return this.executeGet("PrivateCommentList", para, true);
    };

    this.proposalTypes = function(para) {
      return this.executeGet("ProposalTypes", para);
    };

    this.MyProposals = function(para) {
      return this.executeGet("MyProposals", para);
    };

    this.raiseObjection = function(para) {
      return this.executePost("RaiseAnObjection", para);
    };

    this.getDemandObjection = function(para) {
      return this.executeGet("GetDemandObjection", para);
    };

    this.getDemandObejctionList = function(para) {
      return this.executeGet("GetDemandObjectionList", para);
    };
  }
  extend(Demand, Kooboo.BaseModel);

  function Supplier() {
    this.name = "Supplier";
    this.list = function(para) {
      return this.executeGet("List", para);
    };
    this.myList = function(para) {
      return this.executeGet("myList", para);
    };

    this.delete = function(para) {
      return this.executePost("Delete", para);
    };
    this.deletes = function(para) {
      return this.executePost("deletes", para);
    };

    this.addOrUpdate = function(para) {
      return this.executePost("addOrUpdate", para);
    };

    this.myOrdersIn = function() {
      return this.executePost("MyOrders", { in: true });
    };
    this.myOrdersOut = function() {
      return this.executePost("MyOrders", { in: false });
    };

    this.getOrder = function(para) {
      return this.executeGet("GetOrder", para);
    };

    this.get = function(para) {
      return this.executeGet("get", para);
    };

    this.isSupplier = function(para) {
      return this.executeGet("IsSupplier", para);
    };

    this.getByUser = function(para) {
      return this.executeGet("GetByUser", para);
    };

    this.getOrdersBySupplier = function(para) {
      // 我收到的所有 orders
      return this.executeGet("MySupplyOrders", para);
    };

    this.addOrUpdateOrder = function(para) {
      return this.executePost("AddOrUpdateOrder", para);
    };

    this.acceptOrder = function(para) {
      return this.executePost("AcceptSupplierOrder", para);
    };

    this.orderFinished = function(para) {
      return this.executePost("ConfirmSupplierOrder", para);
    };

    this.getMyOrdersInSupply = function(para) {
      // 在 supply 中发出的 orders
      return this.executeGet("MyOrdersFilterBySupplier", para);
    };

    this.reply = function(para) {
      return this.executePost("Reply", para);
    };

    this.getPublicCommentList = function(para) {
      return this.executeGet("PublicCommentList", para, true);
    };

    this.onComplete = function(para) {
      return this.executePost("Complete", para);
    };
  }
  extend(Supplier, Kooboo.BaseModel);

  function Infrastructure() {
    this.name = "Infrastructure";

    this.getSalesItems = function(para) {
      return this.executeGet("SalesItems", para);
    };

    this.order = function(para) {
      return this.executePost("Order", para);
    };

    this.getMonthlyReport = function(para) {
      return this.executeGet("MonthlyReport", para);
    };

    this.getMonthlyLogs = function(para) {
      return this.executeGet("MonthlyLogs", para, true);
    };

    this.getTypes = function(para) {
      return this.executeGet("Types", para);
    };

    this.getMyOrders = function(para) {
      return this.executeGet("MyOrders", para);
    };

    this.getSalesItem = function(para) {
      return this.executeGet("GetSalesItem", para);
    };
  }
  extend(Infrastructure, Kooboo.BaseModel);

  function Balance() {
    this.name = "Balance";

    this.getBalance = function(para) {
      return this.executeGet("GetBalance", para);
    };

    this.getChargePackages = function(para) {
      return this.executeGet("ChargePackages", para);
    };

    this.topup = function(para) {
      return this.executePost("Topup", para);
    };

    this.getTopupHistory = function(para) {
      return this.executeGet("TopupHistory", para);
    };

    this.getPaymentStatus = function(para) {
      return this.executeGet("PaymentStatus", para, true);
    };
  }
  extend(Balance, Kooboo.BaseModel);

  function App() {
    this.name = "App";

    this.getPersonal = function(para) {
      return this.executeGet("Personal", para);
    };

    this.getPrivate = function(para) {
      return this.executeGet("Private", para);
    };
  }
  extend(App, Kooboo.BaseModel);

  function Payment() {
    this.name = "ServerPayment";

    this.getMethods = function(para) {
      return this.executeGet("Methods", para);
    };

    this.getStatus = function(para) {
      return this.executeGet("Status", para, true);
    };
  }
  extend(Payment, Kooboo.BaseModel);

  function Order() {
    this.name = "ServerOrder";

    this.topup = function(para) {
      return this.executePost("Topup", para);
    };

    this.domain = function(para) {
      return this.executePost("Domain", para);
    };

    this.infra = function(para) {
      return this.executePost("Infra", para);
    };

    this.useCoupon = function(para) {
      return this.executePost("UseCoupon", para);
    };

    this.pay = function(para) {
      return this.executePost("Pay", para);
    };

    this.service = function(para) {
      return this.executePost("Service", para);
    };
  }
  extend(Order, Kooboo.BaseModel);

  function Market() {
    this.name = "Market";

    this.getMy = function(para) {
      return this.executeGet("My", para);
    };
  }
  extend(Market, Kooboo.BaseModel);

  function Attachment() {
    this.name = "Attachment";

    this.uploadFile = function(para) {
      return this.executeUpload("UploadFile", para);
    };
    this.deleteFile = function(para) {
      return this.executePost("DeleteFile", para);
    };
  }
  extend(Attachment, Kooboo.BaseModel);

  Kooboo = Object.assign(
    {
      App: new App(),
      Attachment: new Attachment(),
      Balance: new Balance(),
      Infrastructure: new Infrastructure(),
      Demand: new Demand(),
      Discussion: new Discussion(),
      Market: new Market(),
      Order: new Order(),
      Payment: new Payment(),
      Supplier: new Supplier()
    },
    Kooboo
  );
})();
