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
            return !string.IsNullOrWhiteSpace(Data.AppSettings.MailServer);  
        }
          
        public  static string GetForwardUrl(string modelName, string method)
        {
          return string.Format(Data.AppSettings.MailServer + "/_api/{0}/{1}", modelName, method); 
        }

        public static T Get<T>(string modelName, string method,User user,Dictionary<string,string> param=null)
        {
            var url = GetForwardUrl(modelName, method);
            var headers = TwoFactorUserCache.GetHeaders(user.Id);
            return EmailHttpHelper.Get<T>(url, param, headers);
            //return HttpHelper.Get<T>(url, param,user.UserName,user.Password);
        }

        public static T Post<T>(string modelName, string method, User user, Dictionary<string, string> param)
        {
            var url = GetForwardUrl(modelName, method);
            var headers = TwoFactorUserCache.GetHeaders(user.Id);
            return EmailHttpHelper.Post<T>(url, param, headers);
            //return HttpHelper.Post<T>(url, param,user.UserName,user.Password);
        }

        public static T Post<T>(string modelName, string method, User user, string json, Dictionary<string, string> param = null)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return Post<T>(modelName, method, user, bytes, param);
        }
        public static T Post<T>(string modelName, string method, User user, byte[] bytes, Dictionary<string, string> param = null)
        {
            var url = GetForwardUrl(modelName, method);
            var headers = TwoFactorUserCache.GetHeaders(user.Id);
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
            var url= GetForwardUrl(modelName, method);
            if (param == null)
            {
                param = new Dictionary<string, string>();
            }
            if (bytes == null)
            {
                bytes = new byte[0];
            }
            var headers = Kooboo.Data.Service.TwoFactorService.GetHeaders(user.Id);
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
            //return HttpHelper.ConvertKooboo(url, bytes, param, user.UserName, user.Password);
        }

         
    }

    public class TwoFactorUserCache
    {
        private static Dictionary<Guid, Dictionary<string, string>> Cache = new Dictionary<Guid, Dictionary<string, string>>();

        private static object _lockObj = new object();
        public static Dictionary<string,string> GetHeaders(Guid userId)
        {
            if (Cache.ContainsKey(userId))
            {
                return Cache[userId];
            }
            if (!Cache.ContainsKey(userId))
            {
                lock (_lockObj)
                {
                    if (!Cache.ContainsKey(userId))
                    {
                        Cache[userId] = Kooboo.Data.Service.TwoFactorService.GetHeaders(userId);

                    }
                }
            }

            return Cache[userId];
        }
    }


}
