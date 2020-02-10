//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
    public static class ApiHelper
    {
        private static Dictionary<string, bool> _serverips;
        public static Dictionary<string, bool> ServerIps
        {
            get
            {
                if (_serverips == null)
                {
                    _serverips = new Dictionary<string, bool>();
                }
                return _serverips;
            }
        }

        public static bool IsOnlineSever(string IP)
        {
            if (ServerIps.ContainsKey(IP))
            {
                return ServerIps[IP];
            }

            string url = Data.Helper.AccountUrlHelper.System("VerifyServer");
            url = url += "?IP=" + IP;
            try
            {
                var isServer = Lib.Helper.HttpHelper.Get<bool>(url);
                ServerIps[IP] = isServer;
                return isServer;
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex); 
            }

            return false; 
        }




        public static void EnsureAccountUrl(ApiResource res)
        {
            if (ShouldChangeToHttp(res))
            {
                res.AccountUrl = ChangeHttpsToHttp(res.AccountUrl);
            }
        }
        public static string ChangeHttpsToHttp(string url)
        {
            return "http://" + url.Substring("https://".Length);
        }

        public static bool TestAccountRequest(string baseurl, int trytime = 1)
        {
            ApiResource testresult = null;

            if (baseurl.EndsWith("/"))
            {
                baseurl = baseurl.TrimEnd('/');
            }

            string url = baseurl + "/account/system/apiresource";
            bool requestok = true;

            for (int i = 0; i < trytime; i++)
            {
                requestok = true;
                try
                {
                    testresult = Lib.Helper.HttpHelper.Get<ApiResource>(url);
                }
                catch (Exception)
                {
                    requestok = false;
                }
                if (requestok && testresult != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ShouldChangeToHttp(ApiResource res)
        {
            string sbase = res.AccountUrl.ToLower();

            if (sbase.StartsWith("https://"))
            {
                string httpsurl = res.AccountUrl + "/account/system/apiresource";
                if (TestAccountRequest(res.AccountUrl, 2))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
