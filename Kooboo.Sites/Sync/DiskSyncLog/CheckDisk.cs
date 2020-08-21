using Kooboo.Data.Context;
using Kooboo.Data.Events.Global;
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Sync.DiskSyncLog
{
  

    public class HardCodeUpgrade : Data.Events.IHandler<ApplicationStartUp>
    {  
        public void Handle(ApplicationStartUp theEvent, RenderContext context)
        {
           if (context == null || context.WebSite == null)
            {
                var allsites = Kooboo.Data.GlobalDb.WebSites.AllSites;
                foreach (var item in allsites)
                {
                    CheckWebSiteDisk(item); 
                }

            }
           else
            {
                CheckWebSiteDisk(context.WebSite); 
            }
        }

        public void CheckWebSiteDisk(WebSite site)
        {
            if (site.EnableDiskSync)
            { 
                Kooboo.Sites.Sync.DiskSyncManager ma = new DiskSyncManager(site.Id);
                ma.InitSyncToDB();  
            }
        }
    }


}
