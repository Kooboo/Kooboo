using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Web.Payment.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Web.Payment
{
    public class KPaymentMethod
    {
        public RenderContext Context { get; set; }

        public IPaymentMethod PaymentMethod { get; set; }

        public IPaymentResponse Pay(object value)
        {
            var request = ParseRequest(value);
            Kooboo.Web.Payment.PaymentManager.SaveRequest(request, Context);
            return this.PaymentMethod.MakePayment(request, Context);
        }

        public PaymentStatusResponse Check(object requestId)
        {
            if (requestId == null)
            {
                string strid = requestId.ToString();
                if (System.Guid.TryParse(strid, out var id))
                {
                    var request = Kooboo.Web.Payment.PaymentManager.GetRequest(id, Context);

                    if (request != null)
                    {
                        return this.PaymentMethod.EnquireStatus(request, Context);
                    }
                }
            }

            return new PaymentStatusResponse();
        }

        public PaymentRequest ParseRequest(object dataobj)
        {
            PaymentRequest request = new PaymentRequest();

            System.Collections.IDictionary idict = dataobj as System.Collections.IDictionary;

            IDictionary<string, object> dynamicobj = null;

            if (idict == null)
            {
                dynamicobj = dataobj as IDictionary<string, object>;
            }

            request.Name = GetValue<string>(idict, dynamicobj, "name");
            request.Description = GetValue<string>(idict, dynamicobj, "des", "description", "detail");
            request.Currency = GetValue<string>(idict, dynamicobj, "currency");
            request.TotalAmount = GetValue<Decimal>(idict, dynamicobj, "amount", "total", "totalAmount", "totalamount");

            if (this.PaymentMethod != null)
            {
                request.PaymentMethod = PaymentMethod.Name;
            }

            request.OrderId = GetValue<Guid>(idict, dynamicobj, "orderId", "orderid");
            if (request.OrderId == default(Guid))
            {
                request.Order = GetValue<string>(idict, dynamicobj, "order", "orderId");
            }

            request.Code = GetValue<string>(idict, dynamicobj, "code");
            request.Reference = GetValue<string>(idict, dynamicobj, "ref", "reference");

            return request;
        }

        public T GetValue<T>(System.Collections.IDictionary idict, IDictionary<string, object> dynamic, params string[] fieldnames)
        {
            var type = typeof(T);

            object value = null;

            foreach (var item in fieldnames)
            {
                if (idict != null)
                {
                    value = Accessor.GetValueIDict(idict, item, type);
                }
                else if (dynamic != null)
                {
                    value = Accessor.GetValue(dynamic, item, type);
                }

                if (value != null)
                {
                    break;
                }
            }

            if (value != null)
            {
                return (T)value;
            }

            return default(T);
        }
    }
}