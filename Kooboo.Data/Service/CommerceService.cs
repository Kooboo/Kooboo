using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using Kooboo.Data.Helper;
using Kooboo.Data.ViewModel;

namespace Kooboo.Data.Service
{
   public static class CommerceService
    {
        private static string RedeemVoucherUrl = AccountUrlHelper.Commerce("redeemvoucher"); 
        private static string RechargeUrl = AccountUrlHelper.Commerce("Recharge");
        private static string PaymentStatusUrl = AccountUrlHelper.Commerce("PaymentStatus");
        private static string PaypalReturnUrl = AccountUrlHelper.Commerce("PayPalReturn");
        private static string PayDomainUrl = AccountUrlHelper.Commerce("PayDomain");
        private static string PayTwoCheckoutUrl = AccountUrlHelper.Commerce("TwoCheckoutTest");

        public static Organization RedeemVoucher(Guid OrganizationId, string code)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("organizationId", OrganizationId.ToString());
            para.Add("code", code);

            var paramStr = Lib.Helper.JsonHelper.Serialize(para);
            var org = HttpHelper.Post<Organization>(RedeemVoucherUrl, paramStr);

            if (org == null) return null;  
            return org;
        }

        public static PaymentResponse Recharge(RechargeRequest request)
        {
            var json = Lib.Helper.JsonHelper.Serialize(request);
            return HttpHelper.Post<PaymentResponse>(RechargeUrl, json);
        }

        public static PaymentStatusResponse PaymentStatus(Guid id, Guid paymentId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("paymentId", paymentId.ToString());
            var paramStr = Lib.Helper.JsonHelper.Serialize(para);
            var result = HttpHelper.Post<PaymentStatusResponse>(PaymentStatusUrl, paramStr); 
            return result;
        }

        public static PaymentStatusResponse PaypalReturn(string payerID,Guid guid,bool cancel)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("payerID", payerID);
            para.Add("guid", guid.ToString());
            para.Add("cancel", cancel.ToString());
            var paramStr = Lib.Helper.JsonHelper.Serialize(para);
            var result = HttpHelper.Post<PaymentStatusResponse>(PaypalReturnUrl, paramStr);
            return result;
        }

        public static PaymentResponse PayDomain(PaymentRequest request)
        {
            var json = Lib.Helper.JsonHelper.Serialize(request);
            return HttpHelper.Post<PaymentResponse>(PayDomainUrl, json);
        }
        public static bool PayTwoCheckoutTest(string token)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("token", token);
            var paramStr = Lib.Helper.JsonHelper.Serialize(para);
            return HttpHelper.Post<bool>(PayTwoCheckoutUrl, paramStr);
        }
    }
}
