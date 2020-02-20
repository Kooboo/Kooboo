using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Square.lib;
using Kooboo.Sites.Payment.Methods.Square.lib.Models;
using Kooboo.Sites.Payment.Response;
using System.Linq;
using Newtonsoft.Json;
using Kooboo.Sites.Payment.Models;

namespace Kooboo.Sites.Payment.Methods
{
    public class SquarePage : IPaymentMethod<SquareSetting>
    {
        public SquareSetting Setting { get; set; }

        public string Name => "SquarePage";

        public string DisplayName => throw new NotImplementedException();

        public string Icon => throw new NotImplementedException();

        public string IconType => throw new NotImplementedException();

        public List<string> supportedCurrency
        {
            get
            {
                var list = new List<string>();
                list.Add("USD");
                return list;
            }
        }

        public RenderContext Context { get; set; }

        public IPaymentResponse Charge(PaymentRequest request)
        {
            PaidResponse res = new PaidResponse();
            if (this.Setting == null)
            {
                return null;
            }

            // todo 需要转换为货币的最低单位 
            // square APi 使用decimal 类型会报错   必须以适用货币的最小面额指定。例如，美元金额以美分指定，https://developer.squareup.com/docs/build-basics/working-with-monetary-amounts
            Money amount = new Money((long)request.TotalAmount, request.Currency);

            var result = PaymentsApi.Pay(request.SquareResponseNonce, amount, Setting.AccessToken, Setting.PaymentURL);

            var deserializeResult = JsonConvert.DeserializeObject<PaymentResponse>(result);

            if (deserializeResult.Payment.Status == "APPROVED" || deserializeResult.Payment.Status == "COMPLETED")
            {
                res.Type = EnumResponseType.paid;
            }
            else if (deserializeResult.Payment.Status == "CANCELED" || deserializeResult.Payment.Status == "FAILED")
            {
                res.Type = EnumResponseType.failed;
            }

            return res;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();

            //https://connect.squareup.com/v2/payments/{payment_id} 

            // 创建订单后返回的订单编号  {payment_id}
            // request.Order = "Nq0fJPzhO84gNgG1NvUR288g9TLZY";
            if (!string.IsNullOrEmpty(request.Order))
            {
                var requestURL = Setting.PaymentURL + "/" + request.Order;

                var httpResult = PaymentsApi.DoHttpGetRequest(requestURL, Setting.AccessToken);

                var deserializeResult = JsonConvert.DeserializeObject<PaymentResponse>(httpResult);
                GetPaidStatus(result, deserializeResult.Payment.Status);
            }

            return result;
        }

        private static void GetPaidStatus(PaymentStatusResponse result, string paymentStatus)
        {
            switch (paymentStatus)
            {
                case "APPROVED":
                    result.Status = PaymentStatus.Authorized;
                    break;
                case "COMPLETED":
                    result.Status = PaymentStatus.Paid;
                    break;
                case "CANCELED":
                    result.Status = PaymentStatus.Cancelled;
                    break;
                case "FAILED":
                    result.Status = PaymentStatus.Rejected;
                    break;
                default:
                    break;
            }
        }
    }
}
