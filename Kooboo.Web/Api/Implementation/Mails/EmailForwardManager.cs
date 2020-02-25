using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Lib.Helper;
using Kooboo.Data;
using Kooboo.Data.Models;
using Kooboo.Data.Context;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailForwardManager
    {
        public static bool RequireForward(RenderContext context)
        {
            return Kooboo.Mail.Settings.ForwardRequired;
        }

        public static string GetForwardUrl(string modelName, string method)
        {
            return "http://" + Kooboo.Mail.Settings.MailServerIP + "/_api/" + modelName + "/" + method;
        }

        public static T Get<T>(string modelName, string method, User user, Dictionary<string, string> param = null)
        {
            var url = GetForwardUrl(modelName, method);
            var headers = Kooboo.Data.Service.TwoFactorService.GetHeaders(user);
            return EmailHttpHelper.Get<T>(url, param, headers);
        }

        public static T Post<T>(string modelName, string method, User user, Dictionary<string, string> param)
        {
            var url = GetForwardUrl(modelName, method);
            var headers = Kooboo.Data.Service.TwoFactorService.GetHeaders(user);
            return EmailHttpHelper.Post<T>(url, param, headers);
        }

        public static T Post<T>(string modelName, string method, User user, string json, Dictionary<string, string> param = null)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return Post<T>(modelName, method, user, bytes, param);
        }
        public static T Post<T>(string modelName, string method, User user, byte[] bytes, Dictionary<string, string> param = null)
        {
            var url = GetForwardUrl(modelName, method);
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

            return EmailHttpHelper.Post<T>(url, headers, bytes);
        }

        public static byte[] Post(string modelName, string method, User user, byte[] bytes, Dictionary<string, string> param = null)
        {
            var url = GetForwardUrl(modelName, method);
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

}
