using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment
{
    public static class PaymentHelper
    {
        public static string GetCallbackUrl(IPaymentMethod paymentMethod, string MethodName, RenderContext context)
        {
            var baseurl = GetBaseUrl(context).TrimEnd('/');
            return baseurl + "/_api/paymentcallback/" + paymentMethod.Name + "_" + MethodName;
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
                        : string.Format("{0}:{1}", context.Request.Host, context.Request.Port);
                baseurl = context.Request.Scheme + "://" + baseurl;
            }
            return baseurl;
        }

        public static string EnsureHttpUrl(string AbsOrRelativeUrl, RenderContext context)
        {
            if (string.IsNullOrWhiteSpace(AbsOrRelativeUrl))
            {
                return null;
            }
            if (AbsOrRelativeUrl.ToLower().StartsWith("http://") || AbsOrRelativeUrl.ToLower().StartsWith("https://"))
            {
                return AbsOrRelativeUrl;
            }

            var baseurl = GetBaseUrl(context);
            return Lib.Helper.UrlHelper.Combine(baseurl, AbsOrRelativeUrl);
        }
    }
}
