using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Web.Payment.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Payment
{
    public static class PaymentManager
    {
        private static object _locker = new object();

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

        public static List<IPaymentMethod> GetSiteMethods(RenderContext context)
        {
            var all = PaymentContainer.PaymentMethods;

            List<IPaymentMethod> result = new List<IPaymentMethod>();

            foreach (var item in all)
            {
                if (item is IKoobooPayment)
                {
                    // skip
                }
                else
                {
                    if (item.CanUse(context))
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public static IPaymentMethod GetMethod(string paymentmethod)
        {
            paymentmethod = paymentmethod.ToLower();

            foreach (var item in PaymentContainer.PaymentMethods)
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
            var payment = method.MakePayment(request, context);

            if (!string.IsNullOrEmpty(payment.PaymentReferenceId))
            {
                request.Reference = payment.PaymentReferenceId;

                SaveRequest(request, context);
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

        private static string GetBaseUrl(RenderContext context)
        {
            string baseurl = null;
            if (context.WebSite != null && context.WebSite.OrganizationId != default(Guid))
            {
                baseurl = Kooboo.Data.Service.WebSiteService.GetBaseUrl(context.WebSite, true);
            }
            else
            {
                if (Data.AppSettings.IsOnlineServer && Data.AppSettings.ServerSetting != null)
                {
                    baseurl = "https://" + Data.AppSettings.ServerSetting.ServerId + "." + Data.AppSettings.ServerSetting.HostDomain;
                }
            }

            if (baseurl == null)
            {
                baseurl = context.Request.Port == 80 || context.Request.Port == 443
                        ? context.Request.Host
                        : $"{context.Request.Host}:{context.Request.Port}";
                baseurl = context.Request.Scheme + "://" + baseurl;
            }
            return baseurl;
        }

        public static string EnsureHttp(string url, RenderContext context)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }
            if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
            {
                return url;
            }

            var baseurl = GetBaseUrl(context);
            return Lib.Helper.UrlHelper.Combine(baseurl, url);
        }

        public static string GetCallbackUrl(IPaymentMethod method, string methodName, RenderContext context)
        {
            var baseurl = GetBaseUrl(context);
            return baseurl + "/_api/paymentcallback/" + method.Name + "_" + methodName;
        }

        public static void CallBack(PaymentCallback callback, RenderContext context)
        {
            foreach (var item in PaymentContainer.CallBackWorkers)
            {
                item.Process(callback, context);
            }
        }

        public static void SaveRequest(PaymentRequest request, RenderContext context)
        {
            foreach (var item in PaymentContainer.RequestStore)
            {
                item.Save(request, context);
            }
        }

        public static PaymentRequest GetRequest(Guid paymentRequestId, RenderContext context)
        {
            foreach (var item in PaymentContainer.RequestStore)
            {
                var result = item.Get(paymentRequestId, context);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static T GetPaymentSetting<T>(RenderContext context) where T : IPaymentSetting
        {
            if (context.WebSite != null && context.WebSite.OrganizationId != default(Guid))
            {
                var sitedb = context.WebSite.SiteDb();
                var setting = sitedb.CoreSetting.GetSetting<T>();
                if (setting != null)
                {
                    return setting;
                }
            }

            return GetDefaultSetting<T>();
        }

        public static void SetDefaultPaymentSetting(IPaymentSetting input)
        {
            string typename = input.GetType().Name;

            DefaultSettings[typename] = input;
        }

        private static Dictionary<string, IPaymentSetting> DefaultSettings { get; set; } = new Dictionary<string, IPaymentSetting>(StringComparer.OrdinalIgnoreCase);

        private static T GetDefaultSetting<T>()
        {
            var name = typeof(T).Name;
            if (DefaultSettings.ContainsKey(name))
            {
                var setting = DefaultSettings[name];
                return (T)setting;
            }
            return default(T);
        }
    }
}