using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Events;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Render.Cache
{
    public class SiteChangeTrigger : Kooboo.Data.Events.IHandler<Kooboo.Events.WebSiteChange>
    {
        public void Handle(WebSiteChange theEvent, RenderContext context)
        {
            if (theEvent !=null && theEvent.WebSite !=null)
            {
                SiteRender.ResetSite(theEvent.WebSite.Id);
            } 
        }
    }


    public class ContentChangeTrigger: Kooboo.Events.Cms.SiteObjectEvent<SiteObject>
    {

    }
}
