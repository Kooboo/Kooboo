//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Service
{
 public static   class StartService
    {
        public static string AfterLoginPage(RenderContext Context)
        {  
            // TODO: 
            if (Data.AppSettings.Global.EnableLog)
            {
                // try to get user last log.. 
            }

            var host = Context.Request.Host; 
            
            if (host !=null)
            {
                if (host.ToLower().StartsWith("mail."))
                {
                    return "/_Admin/Emails/Inbox"; 
                }
            }

            return "/_admin/sites"; 
            ////_Admin/Emails/Inbox 
        }
        
        public static string DefaultStartPage()
        {
            return "/_admin/sites"; 
        }
         
    }
}
