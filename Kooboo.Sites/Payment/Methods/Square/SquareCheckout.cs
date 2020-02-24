using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Square.lib;
using Kooboo.Sites.Payment.Methods.Square.lib.Models;
using Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout;
using Kooboo.Sites.Payment.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Payment.Methods
{
    public class SquareCheckout : IPaymentMethod<SquareSetting>
    {
        public SquareSetting Setting { get; set; }

        public string Name => "SquareCheckout";

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
            if (this.Setting == null)
            {
                return null;
            }

            // todo 需要转换为货币的最低单位 
            CreateCheckoutRequest checkoutRequest = GetCheckoutRequest(request);

            var result = PaymentsApi.CheckoutCreatOrder(checkoutRequest, Setting);

            var deserializeResult = JsonConvert.DeserializeObject<CreateCheckoutResponse>(result);

            return new RedirectResponse(deserializeResult.Checkout.CheckoutPageURL, Guid.Empty);
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();

            // POST  https://connect.squareup.com/v2/locations/{location_id}/orders/batch-retrieve 
            // Body string[] order_ids

            // order_id    this line to be remove
            // request.ReferenceId = "oNtObOW0XqUxAKEU9a6xCC6VxvbZY";
            if (string.IsNullOrEmpty(request.ReferenceId))
            {
                return result;
            }

            var orderRequest = new CheckOrderRequest { OrderIDs = new List<string>() };
            orderRequest.OrderIDs.Add(request.ReferenceId);

            var httpResult = PaymentsApi.CheckOrder(orderRequest, Setting);

            var deserializeResult = JsonConvert.DeserializeObject<CheckOrderResponse>(httpResult);
            if (deserializeResult == null)
            {
                return result;
            }

            result = GetPaidStatus(result, deserializeResult.Orders[0].State);

            return result;
        }

        private static PaymentStatusResponse GetPaidStatus(PaymentStatusResponse result, string orderStatus)
        {
            switch (orderStatus)
            {
                case "OPEN":
                    result.Status = PaymentStatus.NotAvailable;
                    break;
                case "CAPTURE":
                    result.Status = PaymentStatus.Pending;
                    break;
                case "COMPLETED":
                    result.Status = PaymentStatus.Paid;
                    break;
                case "CANCELED":
                    result.Status = PaymentStatus.Cancelled;
                    break;
                default:
                    break;
            }

            return result;
        }

        private CreateCheckoutRequest GetCheckoutRequest(PaymentRequest request)
        {
            string uuid = Guid.NewGuid().ToString();
            // square APi 使用decimal 类型会报错   必须以适用货币的最小面额指定。例如，美元金额以美分指定，https://developer.squareup.com/docs/build-basics/working-with-monetary-amounts
            var amount = new Money { Amount = (long)(request.TotalAmount * 100), Currency = request.Currency };

            return new CreateCheckoutRequest
            {
                IdempotencyKey = uuid,
                RedirectUrl = Setting.SucceedRedirectURL,
                Order = new CreateOrderRequest
                {
                    LineItems = new List<CreateOrderRequestLineItem> {
                        new CreateOrderRequestLineItem
                        {
                            BasePriceMoney = amount,
                            Name = request.Name,
                            Quantity= "1",
                            Note = request.Description
                        }
                    }
                }
            };
        }
    }
}
