using Kooboo.Data.Context;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using Kooboo.Data.Attributes;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment
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

            PaymentManager.ValidateRequest(this.PaymentMethod, request, this.Context);

            var sitedb = this.Context.WebSite.SiteDb();

            var repo = sitedb.GetSiteRepository<Repository.PaymentRequestRepository>();
            repo.AddOrUpdate(request);

            var result = this.PaymentMethod.Charge(request);

            if (!string.IsNullOrWhiteSpace(result.paymemtMethodReferenceId))
            {
                request.ReferenceId = result.paymemtMethodReferenceId;
            }

            if (!string.IsNullOrWhiteSpace(request.Code) || !string.IsNullOrWhiteSpace(request.ReferenceId))
            {
                repo.AddOrUpdate(request);
            }

            if (result is PaidResponse)
            {
                PaymentManager.CallBack(new PaymentCallback() { RequestId = request.Id, Status = PaymentStatus.Paid }, this.Context);
            }
            else if (result is FailedResponse)
            {
                PaymentManager.CallBack(new PaymentCallback() { RequestId = request.Id, Status = PaymentStatus.Rejected }, this.Context);
            }

            return result;
        }

        public PaymentStatusResponse checkStatus(object requestId)
        {
            if (requestId != null)
            {
                string strid = requestId.ToString();
                Guid id;
                if (System.Guid.TryParse(strid, out id))
                {
                    var request = PaymentManager.GetRequest(id, Context);

                    if (request != null)
                    {
                        bool notsupport = false; 
                        try
                        {
                            var status = this.PaymentMethod.checkStatus(request);
                            if (status.Paid)
                            {
                                PaymentManager.CallBack(new PaymentCallback() { RequestId = request.Id, Status = PaymentStatus.Paid, ResponseMessage = "kscript check status" }, this.Context);
                            }
                            else if (status.Failed)
                            {
                                PaymentManager.CallBack(new PaymentCallback() { RequestId = request.Id, Status = PaymentStatus.Rejected, ResponseMessage = "kscript check status" }, this.Context);
                            }

                            return status;
                        }
                        catch (Exception ex)
                        {
                             if (ex is NotImplementedException || ex is NotSupportedException)
                            {
                                notsupport = true; 
                            }
                        }

                        if (notsupport)
                        {
                            // TODO: check paymentrequest or callback for information... 

                        }
                    
                    }
                }
            }

            return new PaymentStatusResponse();
        }

        [KIgnore]
        public PaymentRequest ParseRequest(object dataobj)
        {
            Dictionary<string, object> additionals = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            PaymentRequest request = new PaymentRequest();

            System.Collections.IDictionary idict = dataobj as System.Collections.IDictionary;

            IDictionary<string, object> dynamicobj = null;

            if (idict == null)
            {
                dynamicobj = dataobj as IDictionary<string, object>;
                foreach (var item in dynamicobj)
                {
                    additionals[item.Key] = item.Value;
                }
            }
            else
            {
                foreach (var item in idict.Keys)
                {
                    if (item != null)
                    {
                        additionals[item.ToString()] = idict[item];
                    }
                }
            }

            request.Additional = additionals;
             
            var id = GetValue<string>(idict, dynamicobj, "id", "requestId", "paymentrequestid");
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (Guid.TryParse(id, out Guid requestid))
                {
                    request.Id = requestid;
                }
            }

            request.Name = GetValue<string>(idict, dynamicobj, "name", "title");
            request.Description = GetValue<string>(idict, dynamicobj, "des", "description", "detail");
            request.Currency = GetValue<string>(idict, dynamicobj, "currency");
            request.Country = GetValue<string>(idict, dynamicobj, "country", "countryCode");
            request.TotalAmount = GetValue<Decimal>(idict, dynamicobj, "amount", "total", "totalAmount", "totalamount");

            request.ReturnUrl = GetValue<string>(idict, dynamicobj, "return", "returnurl", "returnpath");
            request.CancelUrl = GetValue<string>(idict, dynamicobj, "return", "cancelurl", "cancelurl");

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

            request.ReferenceId = GetValue<string>(idict, dynamicobj, "ref", "reference");

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
