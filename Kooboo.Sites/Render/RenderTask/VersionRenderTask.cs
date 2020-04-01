using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;

namespace Kooboo.Sites.Render.RenderTask
{
    public class VersionRenderTask : IRenderTask
    {
        // only has style or script now. 
        public bool IsStyle { get; set; }

        public string Url { get; set; }

        public Guid ObjectId { get; set; }

        public bool HasCheckId { get; set; } = false;

        public bool ClearBefore => false;

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }

        public string Render(RenderContext context)
        {
            var sitedb = context.WebSite.SiteDb();

            IRepository repo = null; 

            if (IsStyle)
            {
                repo = sitedb.Styles; 
            }
            else
            {
                repo = sitedb.Scripts; 
            }

            ISiteObject siteobject = null; 
            // get current version. 
            if (HasCheckId == false)
            {
                HasCheckId = true; 
                var route = sitedb.Routes.GetByUrl(this.Url); 
                if (route !=null && route.objectId != default(Guid))
                {
                    siteobject = repo.Get(route.objectId); 
                }
            }
            else
            {
                siteobject = repo.Get(this.ObjectId); 
            }

            if (siteobject !=null)
            {
                var core = siteobject as ICoreObject; 
                if (core !=null)
                { 
                    Url = Url += "?version=" + core.Version.ToString(); 
                } 
            }

            return Url; 
        }
    }
     
}
