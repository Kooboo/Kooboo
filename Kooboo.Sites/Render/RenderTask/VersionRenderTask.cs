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
         

        public bool ClearBefore => false;
         

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }

        public string Render(RenderContext context)
        { 
            var version = GetVersion(context); 
            if (Url.Contains("?"))
            {
                return Url + "&version=" + version;
            }
            else
            {
                return Url + "?version=" + version;
            } 
        }

        public string GetVersion(RenderContext context)
        {
            if (context == null || context.WebSite == null)
            {
                return Data.AppSettings.Version.ToString();
            }

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
            if (this.ObjectId == default(Guid))
            {  
                var route = sitedb.Routes.GetByUrl(this.Url);
                if (route != null && route.objectId != default(Guid))
                {
                    siteobject = repo.Get(route.objectId);

                    if (siteobject !=null)
                    {
                        this.ObjectId = siteobject.Id; 
                    }  
                }  
            }
            else
            {
                siteobject = repo.Get(this.ObjectId);
            }

            if (siteobject != null)
            {
                var core = siteobject as ICoreObject;
                if (core != null)
                {
                    return core.Version.ToString();
                }
            }

            return Data.AppSettings.Version.ToString();
        }
    }

}
