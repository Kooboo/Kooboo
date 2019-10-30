//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;

namespace Kooboo.Sites.Scripting.Global.SiteItem
{
    public class RoutableTextRepository : TextRepository
    {
        public RoutableTextRepository(IRepository repo, RenderContext context) : base(repo, context)
        {
        }

        public ISiteObject GetByUrl(string url)
        {
            var route = this.context.WebSite.SiteDb().Routes.GetByUrl(url);
            if (route != null && route.objectId != default(Guid))
            {
                return this.repo.Get(route.objectId);
            }
            return null;
        }

        public override void Add(object siteObject)
        {
            string url = null;

            var data = kHelper.GetData(siteObject);
            if (data.ContainsKey("url"))
            {
                url = data["url"].ToString();
            }

            var siteobject = Lib.Reflection.TypeHelper.ToObject(data, this.repo.ModelType);
            if (siteobject != null)
            {
                if (siteobject is SiteObject routeobject)
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        url = "/" + routeobject.Name;
                    }

                    if (routeobject is Models.Style || routeobject is Models.Script)
                    {
                        var ext = routeobject as IExtensionable;
                        if (!url.Contains("."))
                        {
                            if (ext.Extension.StartsWith("."))
                            {
                                url = url + ext.Extension;
                            }
                            else
                            {
                                url = url + "." + ext.Extension;
                            }
                        }
                    }

                    context.WebSite.SiteDb().Routes.AddOrUpdate(url, routeobject);
                    this.repo.AddOrUpdate(routeobject);
                }
            }
        }

        public string getObjectUrl(object siteobj)
        {
            if (siteobj is SiteObject siteobject)
            {
                return getUrl(siteobject.Id.ToString());
            }

            if (siteobj is Guid || siteobj is String)
            {
                return getUrl(siteobj.ToString());
            }

            return null;
        }

        public string getUrl(object id)
        {
            if (id == null)
            {
                return null;
            }

            string strkey = id.ToString();

            if (System.Guid.TryParse(strkey, out var guid))
            {
                var route = this.context.WebSite.SiteDb().Routes.GetByObjectId(guid);
                if (route != null)
                {
                    return route.Name;
                }
            }
            return null;
        }

        public string getAbsUrl(object id)
        {
            var relative = getUrl(id);
            return !string.IsNullOrWhiteSpace(relative) ? this.context.WebSite.BaseUrl(relative) : null;
        }
    }
}