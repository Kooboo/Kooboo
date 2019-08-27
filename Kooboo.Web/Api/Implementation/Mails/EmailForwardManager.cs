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
#if DEBUG
            return AppSettings.CurrentUsedPort == 80;
#else
            return AppSettings.IsOnlineServer;
#endif
        }

        private static string GetForwardServer()
        {
            //todo change
            return "http://local.kooboo:81";
        }

        public static T Get<T>(string modelName, string method,User user,Dictionary<string,string> param=null)
        {
            var url = GetForwardUrl(modelName, method);
            return HttpHelper.Get<T>(url, param,user.UserName,user.Password);
        }

        public static T Post<T>(string modelName, string method, User user, Dictionary<string, string> param)
        {
            var url = GetForwardUrl(modelName, method);
            return HttpHelper.Post<T>(url, param,user.UserName,user.Password);
        }

        public static T Post<T>(string modelName, string method, User user, string json, Dictionary<string, string> param = null)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return Post<T>(modelName, method, user, bytes, param);
        }
        public static T Post<T>(string modelName, string method, User user, byte[] bytes, Dictionary<string, string> param = null)
        {
            var url = GetForwardUrl(modelName, method);
            return HttpHelper.Post<T>(url, param, bytes, user.UserName, user.Password);
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
            return HttpHelper.ConvertKooboo(url, bytes, param, user.UserName, user.Password);
        }

        private static string GetForwardUrl(string modelName, string method)
        {
            var url = string.Format("_api/{0}/{1}", modelName, method);
            var domain = GetForwardServer();
            return string.Format("{0}/{1}",domain,url);
        }

    }
}
