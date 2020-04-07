using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.Render.RenderTask
{
    public class VersionRenderTask : IRenderTask
    {
        public VersionRenderTask(string url, bool isStyle = false, bool IsImage = false)
        {
            if (url != null)
            {
                IsGroup = Kooboo.Sites.Service.GroupService.IsGroupUrl(url);
                this.Url = url;
                this.IsStyle = isStyle;
                this.IsImage = IsImage;
            }
        }

        // only has style or script now. 
        public bool IsStyle { get; set; }

        public bool IsGroup { get; set; }

        public bool IsImage { get; set; }

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

            // get the store.lastkey is faster because it is cached. 
            if (this.LastMaxVersion > 0 && sitedb.Log.Store.LastKey == this.LastMaxVersion)
            {
                if (this.LastObjectVersion >= 0)
                {
                    return this.LastObjectVersion.ToString();
                }
            }

            IRepository repo = null;

            if (this.IsGroup)
            {
                repo = sitedb.ResourceGroups;
            }
            else if (this.IsImage)
            {
                repo = sitedb.Images;
            }

            else if (IsStyle)
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

                    if (siteobject != null)
                    {
                        var core = siteobject as ICoreObject;

                        if (core != null)
                        {
                            this.ObjectId = siteobject.Id; 
                            this.LastObjectVersion = GetVersion(sitedb, core); 
                            this.LastMaxVersion = sitedb.Log.Store.LastKey; 
                            return this.LastObjectVersion.ToString();
                        }
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
                    this.ObjectId = siteobject.Id;
                    this.LastObjectVersion = GetVersion(sitedb, core);
                    this.LastMaxVersion = sitedb.Log.Store.LastKey;
                    return this.LastObjectVersion.ToString();
                }
            }

            return Data.AppSettings.Version.ToString();
        }
         
        // get object version. when it is group. it is complicated!
        public long GetVersion(SiteDb sitedb, ICoreObject coreobject)
        {
            long version = 0;

            if (this.IsGroup)
            {
                var group = coreobject as ResourceGroup;

                IRepository repo = null;
                if (group.Type == ConstObjectType.Style)
                {
                    repo = sitedb.Styles;
                }
                else if (group.Type == ConstObjectType.Script)
                {
                    repo = sitedb.Scripts;
                }

                foreach (var item in group.Children.OrderBy(o => o.Value))
                {
                    var route = sitedb.Routes.Get(item.Key);
                    if (route != null)
                    {
                        var siteobject = repo.Get(route.objectId);
                        if (siteobject != null)
                        {
                            var core = siteobject as ICoreObject;
                            if (core != null)
                            {
                                version += core.Version;
                            }
                        }
                    }
                }

            }
            else
            {
                version = coreobject.Version;
            }

            return version;
        }

        public long LastMaxVersion { get; set; }

        public long LastObjectVersion { get; set; } = -1;
    }

}
