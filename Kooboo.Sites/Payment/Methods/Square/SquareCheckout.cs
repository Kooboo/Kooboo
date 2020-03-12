using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Square;
using Kooboo.Sites.Payment.Methods.Square.lib;
using Kooboo.Sites.Payment.Methods.Square.lib.Models;
using Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout;
using Kooboo.Sites.Payment.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kooboo.Sites.Payment.Methods
{
    public class SquareCheckout : IPaymentMethod<SquareSetting>
    {
        public SquareSetting Setting { get; set; }

        public string Name => "SquareCheckout";

        public string DisplayName => Data.Language.Hardcoded.GetValue("square", Context);

        public string Icon => "/_Admin/View/Market/Images/payment-square.png";

        public string IconType => "img";

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

        [Description(@"<script engine='kscript'>
    var charge = {};
    charge.total = 2.60; 
    charge.currency='USD';
    charge.name = 'green tea order'; 
    charge.description = 'The best tea from Xiamen';  
    var resForm = k.payment.squareCheckout.charge(charge);
    k.response.redirect(resForm.RedirectUrl)
</script>")]
        [KDefineType(Return = typeof(RedirectResponse))]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            if (this.Setting == null)
            {
                return null;
            }

            CreateCheckoutRequest checkoutRequest = GetCheckoutRequest(request);
            var deserializeResult = PaymentsApi.CheckoutCreatOrder(checkoutRequest, Setting);

            // 把OrderID赋值到request referenceID 为了后面 checkStatus 使用
            request.ReferenceId = deserializeResult.Checkout.Order.ID;
            PaymentManager.UpdateRequest(request, Context);

            return new RedirectResponse(deserializeResult.Checkout.CheckoutPageURL, Guid.Empty);
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();

            // POST  https://connect.squareup.com/v2/locations/{location_id}/orders/batch-retrieve 
            if (string.IsNullOrEmpty(request.ReferenceId))
            {
                return result;
            }

            var orderRequest = new CheckOrderRequest { OrderIDs = new List<string>() };
            orderRequest.OrderIDs.Add(request.ReferenceId);

            var deserializeResult = PaymentsApi.CheckOrder(orderRequest, Setting);
            if (deserializeResult == null)
            {
                return result;
            }

            result = GetPaidStatus(result, deserializeResult.Orders[0].State);

            return result;
        }

        public PaymentCallback Notify(RenderContext context)
        {
            return SquareCommon.ProcessNotify(context);
        }

        private static PaymentStatusResponse GetPaidStatus(PaymentStatusResponse result, string orderStatus)
        {
            switch (orderStatus)
            {
                case "OPEN":
                case "CAPTURE":
                    result.Status = PaymentStatus.Pending;
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

        private CreateCheckoutRequest GetCheckoutRequest(PaymentRequest request)
        {
            string uuid = Guid.NewGuid().ToString();
            // square货币的最小面额指定。https://developer.squareup.com/docs/build-basics/working-with-monetary-amounts
            var amount = new Money { Amount = CurrencyDecimalPlaceConverter.ToMinorUnit(request.Currency, request.TotalAmount), Currency = request.Currency };

            return new CreateCheckoutRequest
            {
                IdempotencyKey = uuid,
                RedirectUrl = string.IsNullOrEmpty(request.ReturnUrl) ? Setting.RedirectURL : request.ReturnUrl,
                Order = new CreateOrderRequest
                {
                    ReferenceId = request.Id.ToString(),
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
