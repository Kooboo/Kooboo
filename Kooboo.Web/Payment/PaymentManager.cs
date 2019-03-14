using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.ServerData.Model;
using Kooboo.ServerData.Models;
using Kooboo.ServerData.Payment.Methods;
using Kooboo.ServerData.Payment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Web.Payment
{
  
    public static class PaymentManager
    {
        private static object _locker = new object();

        private static List<IPaymentMethod> _paymentmethods;

        private static List<IPaymentMethod> PaymentMethods
        {
            get
            {
                if (_paymentmethods == null)
                {
                    lock (_locker)
                    {
                        if (_paymentmethods == null)
                        {
                            _paymentmethods = new List<IPaymentMethod>();

                            var alltypes = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IPaymentMethod));

                            foreach (var item in alltypes)
                            {
                                var instance = Activator.CreateInstance(item) as IPaymentMethod;
                                _paymentmethods.Add(instance);
                            }
                        }
                    }
                }
                return _paymentmethods;
            }
        }
          
        public static List<IPaymentMethod> GetMethods(string currency, List<IPaymentMethod> methods)
        {
            List<IPaymentMethod> result = new List<IPaymentMethod>();

            foreach (var item in methods)
            {
                if (item.ForCurrency.Any())
                {
                    if (item.ForCurrency.Contains(currency))
                    {
                        result.Add(item);
                    }
                }
                else if (item.ExcludeCurrency.Any())
                {
                    if (!item.ExcludeCurrency.Contains(currency))
                    {
                        result.Add(item);
                    }
                }
                else
                {
                    result.Add(item);
                }

            }

            return result;
        }


        public static List<IPaymentMethod> GetSiteMethods(WebSite site)
        {
            return null; 
        }
         
        public static IPaymentMethod GetMethod(string paymentmethod)
        {
            // TODO: verify currency match. 
            paymentmethod = paymentmethod.ToLower();

            foreach (var item in PaymentMethods)
            {
                if (item.Name.ToLower() == paymentmethod)
                {
                    return item;
                }
            }
            return null;
        }

        public static IPaymentResponse MakePayment(PaymentRequest request, RenderContext context)
        {
            EnsurePaymentRequest(request);

            var method = GetMethod(request.PaymentMethod);

            // save the request data..  
            var payment = method.MakePayment(request);

            if (!string.IsNullOrEmpty(payment.PaymentReferenceId))
            {
                request.ProviderReference = payment.PaymentReferenceId;

                SavePaymentRequest(request, context); 
            }

            if (payment.PaymentRequestId == default(Guid))
            {
                payment.PaymentRequestId = request.Id;
            }

            if (payment is OKResponse)
            {
                CallBack(new PaymentCallback() { IsPaid = true, PaymentRequestId = payment.PaymentRequestId }, context);    
                /////Kooboo.Server.Commerce.PaymentRequestService.Paid(payment.PaymentRequestId);
            }
            else if (payment is NotOkResponse)
            {
                CallBack(new PaymentCallback() { IsCancel = true, PaymentRequestId = payment.PaymentRequestId }, context);  
              //Kooboo.Server.Commerce.PaymentRequestService.Cancel(payment.PaymentRequestId);
            }

            return payment;
        }

        public static void EnsurePaymentRequest(PaymentRequest request)
        {
            if (request.PaymentMethod == null)
            {
                request.PaymentMethod = "wechat";
            } 
            //TODO: ensure the currency. 
        }

        public static string GetCallbackUrl(IPaymentMethod method, string MethodName, WebSite site = null)
        {
            string baseurl = null;
            if (site != null)
            {
                baseurl = Kooboo.Data.Service.WebSiteService.GetBaseUrl(site);
            }
            else
            {
                if (Data.AppSettings.IsOnlineServer)
                {
                    if (Data.AppSettings.ServerSetting != null)
                    {
                        baseurl = "https://" + Data.AppSettings.ServerSetting.ServerId + "." + Data.AppSettings.ServerSetting.HostDomain;
                    }
                }
            }

            if (baseurl == null)
            {
                baseurl = "https://127.0.0.1"; // TODO: temp' 
            }

            return baseurl + "/_api/paymentcallback/" + method.Name + "_" + MethodName;

        }

        public static void  CallBack(PaymentCallback callback, RenderContext context)
        {
            foreach (var item in PaymentContainer.CallBackWorkers)
            {
                item.Process(callback, context); 
            }
        }

        public static void SavePaymentRequest(PaymentRequest request, RenderContext context)
        {
            //TODO: Kooboo.Server.Commerce.PaymentRequestService.SaveLocalAndRoot(request);

            foreach (var item in PaymentContainer.RequestStore)
            {
                item.Save(request, context); 
            }
        }
         
    }

}
