using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Api.Methods;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Payment;
using Kooboo.Sites.Payment.Repository;

namespace Kooboo.Web.Api.Implementation
{
    public class PaymentCallbackApi : IApi, IDynamicApi
    {
        public string ModelName => "PaymentCallback";

        public bool RequireSite => false;

        public bool RequireUser => false;

        public DynamicApi GetMethod(string name)
        {
            int index = name.IndexOf("_");
            if (index == -1)
            {
                return null;
            }

            var paymentname = name.Substring(0, index);
            var methodname = name.Substring(index + 1);

            var paymentmethod = PaymentManager.GetMethod(paymentname);
            if (paymentmethod != null)
            {
                DynamicApi api = new DynamicApi();
                api.Type = this.GetType();
                api.Method = this.GetType().GetMethod("Callback");
                return api;
            }
            return null;
        }

        // To be used for above method. 
        public IResponse Callback(ApiCall call)
        {
            log(call);

            string name = call.Command.Method;

            int index = name.IndexOf("_");
            if (index == -1)
            {
                return null;
            }
            var paymentname = name.Substring(0, index);
            var methodname = name.Substring(index + 1);
            var paymentmethod = PaymentManager.GetMethod(paymentname, call.Context);

            if (paymentmethod != null)
            {
                var method = paymentmethod.GetType().GetMethod(methodname);
                if (method != null)
                {
                    object[] para = new object[1];
                    para[0] = call.Context;
                    var result = method.Invoke(paymentmethod, para) as PaymentCallback;

                    if (result != null)
                    {
                        result.Name = paymentmethod.Name;
                        PaymentManager.CallBack(result, call.Context);

                        if (result.CallbackResponse != null)
                        {
                            if (result.CallbackResponse.StatusCode == 302)
                            {
                                var response = new MetaResponse();
                                response.Redirect(result.CallbackResponse.Content);
                                return response;
                            }
                            else
                            {
                                return new PlainResponse
                                {
                                    ContentType = result.CallbackResponse.ContentType,
                                    Content = result.CallbackResponse.Content,
                                    statusCode = result.CallbackResponse.StatusCode
                                };
                            }
                        }

                        else
                        {
                            var response = new PlainResponse();
                            response.ContentType = "text/html";
                            response.Content = "";
                            return response;
                        }
                    }
                }
            }

            return null;
        }

        public PaymentStatusResponse CheckStatus(ApiCall call)
        {
            var id = call.GetValue("paymentrequestid", "refid", "referenceId", "requestid", "id");

            if (!string.IsNullOrEmpty(id))
            {
                var repo = call.Context.WebSite.SiteDb().GetSiteRepository<PaymentRequestRepository>();

                var request = repo.Get(id);
                if (request != null)
                {
                    var paymentmethod = PaymentManager.GetMethod(request.PaymentMethod, call.Context);
                    if (paymentmethod != null)
                    {
                        paymentmethod.Context = call.Context;
                        var status = paymentmethod.CheckStatus(request);
                        if (status.Paid)
                        {
                            PaymentManager.CallBack(new PaymentCallback() { RequestId = request.Id, Status = PaymentStatus.Paid, ResponseMessage = "kscript check status" }, call.Context);
                        }
                        else if (status.Failed)
                        {
                            PaymentManager.CallBack(new PaymentCallback() { RequestId = request.Id, Status = PaymentStatus.Rejected, ResponseMessage = "kscript check status" }, call.Context);
                        }

                        return status;
                    }
                }
            }

            return new PaymentStatusResponse() { Message = "No result" };
        }
        private void log(ApiCall call)
        {
            try
            {
                Dictionary<string, string[]> query = new Dictionary<string, string[]>();
                foreach (var item in call.Context.Request.QueryString.Keys)
                {
                    var key = item.ToString();
                    var value = call.Context.Request.QueryString.GetValues(key);
                    if (value != null)
                    {
                        query[key] = value;
                    }
                }

                Dictionary<string, string[]> forms = new Dictionary<string, string[]>();
                foreach (var item in call.Context.Request.Forms.Keys)
                {
                    var key = item.ToString();
                    var value = call.Context.Request.Forms.GetValues(key);
                    if (value != null)
                    {
                        forms[key] = value;
                    }
                }

                Kooboo.Data.Log.Instance.Payment.Write("---" + call.Context.Request.Url);
                Kooboo.Data.Log.Instance.Payment.WriteObj(query);
                Kooboo.Data.Log.Instance.Payment.WriteObj(forms);
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex);
            }
        }
    }

    public class PaymentApi : IApi
    {
        public string ModelName => "Payment";

        public bool RequireSite => false;

        public bool RequireUser => false;

        public PaymentStatusResponse CheckStatus(ApiCall call)
        {
            var id = call.GetValue("paymentrequestid", "refid", "referenceId", "requestid", "id");

            if (!string.IsNullOrEmpty(id))
            {
                var repo = call.Context.WebSite.SiteDb().GetSiteRepository<PaymentRequestRepository>();

                var request = repo.Get(id);
                if (request != null)
                {
                    var paymentmethod = PaymentManager.GetMethod(request.PaymentMethod, call.Context);
                    if (paymentmethod != null)
                    {
                        paymentmethod.Context = call.Context;
                        var status = paymentmethod.CheckStatus(request);
                        if (status.Paid)
                        {
                            PaymentManager.CallBack(new PaymentCallback()
                            {
                                Name = paymentmethod.Name,
                                RequestId = request.Id,
                                Status = PaymentStatus.Paid,
                                ResponseMessage = "kscript check status"
                            }, call.Context);
                        }
                        else if (status.Failed)
                        {
                            PaymentManager.CallBack(new PaymentCallback()
                            {
                                Name = paymentmethod.Name,
                                RequestId = request.Id,
                                Status = PaymentStatus.Rejected,
                                ResponseMessage = "kscript check status"
                            }, call.Context);
                        }

                        return status;
                    }
                }
            }

            return new PaymentStatusResponse() { Message = "No result" };
        }
    }
}
