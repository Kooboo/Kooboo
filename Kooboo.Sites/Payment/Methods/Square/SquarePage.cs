using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Methods.Square;
using Kooboo.Sites.Payment.Methods.Square.lib;
using Kooboo.Sites.Payment.Methods.Square.lib.Models;
using Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout;
using Kooboo.Sites.Payment.Models;
using Kooboo.Sites.Payment.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
            var amount = new Money { Amount = (long)request.TotalAmount * 100, Currency = request.Currency };

            var result = PaymentsApi.CreatPayment(request.SquareResponseNonce, amount, Setting);

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
            // request.ReferenceId = "nn2I3hGkDrBqU2MPQy103dzET5UZY";
            if (string.IsNullOrEmpty(request.ReferenceId))
            {
                return result;
            }

            var requestURL = Setting.BaseURL + "/v2/payments/" + request.ReferenceId;

            var httpResult = PaymentsApi.DoHttpGetRequest(requestURL, Setting.AccessToken);

            var deserializeResult = JsonConvert.DeserializeObject<PaymentResponse>(httpResult);

            if (deserializeResult == null)
            {
                return null;
            }

            result = GetPaidStatus(result, deserializeResult.Payment.Status);

            return result;
        }

        public PaymentCallback Notify(RenderContext context)
        {
            return SquareCommon.ProcessNotify(context);
        }

        private static PaymentStatusResponse GetPaidStatus(PaymentStatusResponse result, string paymentStatus)
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

            return result;
        }
    }
}
