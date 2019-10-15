//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Data.Service
{
    public static class StartService
    {
        public static string AfterLoginPage(RenderContext context)
        {
            if (context?.User != null)
            {
                if (Data.AppSettings.IsOnlineServer)
                {
                    var lasturl = Service.UserLoginService.GetLastPath(context.User.Id);
                    if (!string.IsNullOrEmpty(lasturl) && !lasturl.ToLower().StartsWith("/_admin/sites/edit"))
                    {
                        return lasturl;
                    }
                }
            }

            var host = context?.Request.Host;

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

            return relativeUrl.ToLower() == DefaultStartPage;
        }

        public static string DefaultStartPage => "/_admin/sites";
    }
}