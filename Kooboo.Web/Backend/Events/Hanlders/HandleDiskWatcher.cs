//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Data.Events.Global;
using Kooboo.Events;

namespace Kooboo.Web.Events.Hanlders
{
    public class HandleInitWebsiteDiskSync : Data.Events.IHandler<ApplicationStartUp>
    { 
        public void Handle(ApplicationStartUp theEvent, RenderContext context)
        { 
            foreach (var item in Kooboo.Data.GlobalDb.WebSites.AllSites)
            {
                if (item.Value.EnableDiskSync)
                {
                    Kooboo.Sites.Sync.DiskSyncFolderWatcher.StartDiskWatcher(item.Value);
                }
            }
        }
    }
     
}
