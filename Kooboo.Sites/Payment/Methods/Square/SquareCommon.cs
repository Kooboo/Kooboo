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

            if (data.Data == null || data.Data.Object == null || data.Data.Object.Payment == null)
            {
                return null;
            }

            PaymentRequest paymentRequest;
            Guid paymentRequestId;
            if (data.Data.Object.Payment.ReferenceId != null && Guid.TryParse(data.Data.Object.Payment.ReferenceId, out paymentRequestId))
            {
                paymentRequest = PaymentManager.GetRequest(paymentRequestId, context);
            }
            else
            {
                paymentRequest = PaymentManager.GetRequestByReferece(data.Data.Object.Payment.OrderId, context);
            }

            if (paymentRequest == null)
            {
                return null;
            }

            // https://developer.squareup.com/reference/square/objects/LaborShiftCreatedWebhookObject
            var status = new PaymentStatus();

            switch (data.Data.Object.Payment.Status)
            {
                case "OPEN":
                case "APPROVED":
                    status = PaymentStatus.Pending;
                    break;
                case "COMPLETED":
                    status = PaymentStatus.Paid;
                    break;
                case "CANCELED":
                    status = PaymentStatus.Cancelled;
                    break;
                case "FAILED":
                    status = PaymentStatus.Rejected;
                    break;
                default:
                    break;
            }

            var result = new PaymentCallback
            {
                RequestId = paymentRequest.Id,
                RawData = body,
                Status = status
            };

            return result;
        }
    }
}
