using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Extensions;
using Kooboo.Web.Payment.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Web.Payment.Repository;
using System.ComponentModel;

namespace Kooboo.Web.Payment
{
    public class KPaymentMethodWrapper
    {
        public KPaymentMethodWrapper(IPaymentMethod paymentMethod, RenderContext context)
        {
            this.PaymentMethod = paymentMethod;
            this.Context = context;
        }

        private RenderContext Context { get; set; }

        private IPaymentMethod PaymentMethod { get; set; }

        public IPaymentResponse Charge(object value)
        {
            var request = ParseRequest(value);

            var sitedb = this.Context.WebSite.SiteDb();

            var repo = sitedb.GetSiteRepository<Kooboo.Web.Payment.Repository.TempPaymentRequestRepository>();
            repo.AddOrUpdate(request);

            //TODO, validate data.
            return this.PaymentMethod.Charge(request);
        }

        [Description("Submit additional data required by payment method")]
        public IPaymentResponse SubmitData(object data)
        {
            return null;
        }

        public PaymentStatusResponse Check(object requestId)
        {
            if (requestId == null)
            {
                string strid = requestId.ToString();
                Guid id;
                if (System.Guid.TryParse(strid, out id))
                {
                    var request = Kooboo.Web.Payment.PaymentManager.GetRequest(id, Context);

                    if (request != null)
                    {
                        return this.PaymentMethod.EnquireStatus(request);
                    }
                }
            }

            return new PaymentStatusResponse();
        }

        internal PaymentRequest ParseRequest(object dataobj)
        {
            PaymentRequest request = new PaymentRequest();

            System.Collections.IDictionary idict = dataobj as System.Collections.IDictionary;

            IDictionary<string, object> dynamicobj = null;

            if (idict == null)
            {
                dynamicobj = dataobj as IDictionary<string, object>;
            }

            request.Name = GetValue<string>(idict, dynamicobj, "name", "title");
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

        private T GetValue<T>(System.Collections.IDictionary idict, IDictionary<string, object> Dynamic, params string[] fieldnames)
        {
            var type = typeof(T);

            object Value = null;

            foreach (var item in fieldnames)
            {
                if (idict != null)
                {
                    Value = Accessor.GetValueIDict(idict, item, type);
                }
                else if (Dynamic != null)
                {
                    Value = Accessor.GetValue(Dynamic, item, type);
                }

                if (Value != null)
                {
                    break;
                }
            }

            if (Value != null)
            {
                return (T)Value;
            }

            return default(T);
        }

    }




}
