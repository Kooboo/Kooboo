using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailForwardManager
    {
        public static bool RequireForward(RenderContext context)
        {
            return Mail.Settings.ForwardRequired;
        }

        public static string GetForwardUrl(User user, string modelName, string method)
        {
            string mailserver = null;
            if (UrlSetting.MailServer == null)
            {
                mailserver = "http://" + OrgMailServerProvider.GetIP(user);
            }
            else
            {
                mailserver = UrlSetting.MailServer;
            }

            return mailserver + "/_api/" + modelName + "/" + method;
            //return UrlSetting.MailServer + "/_api/" + modelName + "/" + method;
        }

        public static T Get<T>(string modelName, string method, User user, Dictionary<string, string> param = null)
        {
            var url = GetForwardUrl(user, modelName, method);
            var headers = Data.Service.TwoFactorService.GetHeaders(user);
            return EmailHttpHelper.Get<T>(url, param, headers);
        }

        public static T Post<T>(string modelName, string method, User user, Dictionary<string, string> param)
        {
            var url = GetForwardUrl(user, modelName, method);
            var headers = Kooboo.Data.Service.TwoFactorService.GetHeaders(user);
            return EmailHttpHelper.Post<T>(url, param, headers);
        }

        public static T Post<T>(string modelName, string method, User user, string json, Dictionary<string, string> param = null)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return Post<T>(modelName, method, user, bytes, param, "application/json");
        }
        public static T Post<T>(string modelName, string method, User user, byte[] bytes, Dictionary<string, string> param = null, string contentType = null)
        {
            var url = GetForwardUrl(user, modelName, method);
            var headers = Kooboo.Data.Service.TwoFactorService.GetHeaders(user);
            if (param != null)
            {
                foreach (var p in param)
                {
                    if (!headers.ContainsKey(p.Key))
                    {
                        headers[p.Key] = p.Value;
                    }
                }
            }

            return EmailHttpHelper.Post<T>(url, headers, bytes, contentType);
        }

        public static byte[] Post(string modelName, string method, User user, byte[] bytes, Dictionary<string, string> param = null)
        {
            var url = GetForwardUrl(user, modelName, method);
            if (param == null)
            {
                param = new Dictionary<string, string>();
            }
            if (bytes == null)
            {
                bytes = new byte[0];
            }
            var headers = Kooboo.Data.Service.TwoFactorService.GetHeaders(user);
            if (param != null)
            {
                foreach (var p in param)
                {
                    if (!headers.ContainsKey(p.Key))
                    {
                        headers[p.Key] = p.Value;
                    }
                }
            }
            return EmailHttpHelper.PostBytes(url, bytes, headers);
        }

    }


    public static class OrgMailServerProvider
    {
        public static MemoryCache IpCache { get; set; } = new MemoryCache(new MemoryCacheOptions() { });

        public static string GetIP(User user)
        {
            if (IpCache.TryGetValue<string>(user.CurrentOrgId, out string result))
            {
                return result;
            }

            string url = Data.Helper.AccountUrlHelper.System("MailServer");
            url = url += "?OrgId=" + user.CurrentOrgId.ToString();

            try
            {
                var IP = Lib.Helper.HttpHelper.Get<string>(url);

                if (IP != null && System.Net.IPAddress.TryParse(IP, out var ipadd))
                {
                    IpCache.Set<string>(user.CurrentOrgId, IP, TimeSpan.FromHours(6));
                }
                else
                {
                    IpCache.Set<string>(user.CurrentOrgId, null, TimeSpan.FromHours(1));
                }

                return IP;
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex);
            }

            return null;
        }



    }
}
