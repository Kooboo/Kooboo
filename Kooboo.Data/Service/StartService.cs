//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Service
{
    public static class StartService
    {

        public static string AfterLoginPage(RenderContext Context)
        {
            if (Context != null && Context.User != null)
            {
                if (Data.AppSettings.IsOnlineServer)
                {
                    var lasturl = Service.UserLoginService.GetLastPath(Context.User.Id);
                    if (!string.IsNullOrEmpty(lasturl) && !lasturl.ToLower().StartsWith("/_admin/sites/edit"))
                    {
                        return lasturl;
                    }
                }
            }

            var host = Context.Request.Host;

            if (host != null)
            {
                if (host.ToLower().StartsWith("mail."))
                {
                    return "/_Admin/Emails/Inbox";
                }
            }

            return DefaultStartPage;
        }

        public static bool IsDefaultStartPage(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                return false;
            }

            if (relativeUrl.ToLower() == DefaultStartPage)
            {
                return true;
            }
            return false;
        }

        public static string DefaultStartPage
        {
            get
            {
                return "/_admin/sites";
            }
        }

        public static CheckPortResult CheckInitPort()
        {
            CheckPortResult result = new CheckPortResult() { Ok = true }; 

            result.HttpPort = AppSettingsUtility.GetInt("Port", -1);

            if (result.HttpPort > 0)
            {
                // define port. 
                if (Lib.Helper.NetworkHelper.IsPortInUse(result.HttpPort))
                {
                    string message = Data.Language.Hardcoded.GetValue("Port") + " " + result.HttpPort.ToString() + " " + Data.Language.Hardcoded.GetValue("is in use");

                    result.Ok = false;
                    result.ErrorMessage = message;
                    return result;
                }
            }
            else
            {
                result.HttpPort = 80; // default port. 
                while (Lib.Helper.NetworkHelper.IsPortInUse(result.HttpPort) && result.HttpPort < 65535)
                {
                    result.HttpPort += 1;
                }
            }

            result.SslPort = AppSettingsUtility.GetInt("SslPort", -1);

            if (result.SslPort > 0)
            {
                // define port. 
                if (Lib.Helper.NetworkHelper.IsPortInUse(result.SslPort))
                {
                    string message = Data.Language.Hardcoded.GetValue("Port") + " " + result.SslPort.ToString() + " " + Data.Language.Hardcoded.GetValue("is in use");

                    result.Ok = false;
                    result.ErrorMessage = message;
                    return result;
                }
            }
            else
            {
                result.SslPort = 443;
                while (Lib.Helper.NetworkHelper.IsPortInUse(result.HttpPort) && result.HttpPort < 65535)
                {
                    result.HttpPort += 1;
                }
            }

            return result;
        }
    }


    public struct CheckPortResult
    {
        public bool Ok;
        public string ErrorMessage { get; set; }

        public int HttpPort { get; set; }

        public int SslPort { get; set; }
    }
}
