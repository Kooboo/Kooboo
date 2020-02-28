using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square
{
    public static class SquareCommon
    {
        public static PaymentCallback ProcessNotify(RenderContext context)
        {
            var body = context.Request.Body;
            var data = JsonHelper.Deserialize<CallbackRequest>(body);

            // to refactor charge order ID to match webhook order_id
            var orderID = "";  // get form somewhere
            if (data.Data.Object.Payment.OrderId != orderID)
            {
                return null;
            }

            // https://developer.squareup.com/reference/square/objects/LaborShiftCreatedWebhookObject
            var status = new PaymentStatus();
            if (data.Data.Object.Payment.Status == "COMPLETED")
            {
                status = PaymentStatus.Pending;
            }
            if (data.Data.Object.Payment.Status == "APPROVED")
            {
                status = PaymentStatus.Paid;
            }

            var result = new PaymentCallback
            {
                // to do  该字段不是guid，无法转换赋值
                //  "merchant_id": "6SSW7HV8K2ST5",
                //RequestId = data.MerchantID,
                RawData = body,
                Status = status
            };

            return result;
        }

        public static long GetSquareAmount(decimal totalAmount)
        {
            return (long)(totalAmount * 100);
        }
    }
}
