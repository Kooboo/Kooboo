using Kooboo.Data.Context;
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



    }
}
