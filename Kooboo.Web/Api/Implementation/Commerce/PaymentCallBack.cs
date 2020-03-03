using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Api.Methods;
using Kooboo.Sites.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class PaymentCallBackApi : IApi, IDynamicApi
    {
        public string ModelName => "Paymentcallback";

        public bool RequireSite => true;

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
                        PaymentManager.CallBack(result, call.Context);

                        if (result.CallbackResponse != null)
                        {
                            var response = new PlainResponse();
                            response.ContentType = result.CallbackResponse.ContentType;
                            response.Content = result.CallbackResponse.Content;
                            response.statusCode = result.CallbackResponse.StatusCode;
                            return response;
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
}
