//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Events;

namespace Kooboo.Sites.Events.Handlers
{
    public class HandleWebSiteChange : Data.Events.IHandler<Kooboo.Events.WebSiteChange>
    {  
        public void Handle(WebSiteChange theEvent, RenderContext context)
        { 
             
            Kooboo.Sites.Cache.RenderPlan.RemoveSiteDb(theEvent.WebSite.Id);  
        }
    }
}
