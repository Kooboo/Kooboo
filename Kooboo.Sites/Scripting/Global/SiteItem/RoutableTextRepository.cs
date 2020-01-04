//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.ComponentModel;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;

namespace KScript.Sites
{
    public class RoutableTextRepository : TextRepository
    {
        public RoutableTextRepository(IRepository repo, RenderContext context) : base(repo, context)
        {

        }

        [Description(@"get the SiteObject by Url, SiteObject can be page, style or others.
      var page = k.site.pages.getByUrl(""/pagename"");
      var style = k.site.styles.getByUrl(""/style.css"");")]
        public ISiteObject GetByUrl(string url)
        {
            var route = this.context.WebSite.SiteDb().Routes.GetByUrl(url);
            if (route != null && route.objectId != default(Guid))
            {
                return this.repo.Get(route.objectId);
            }
            return null;
        }

        [KDefineType(Params = new[] { typeof(Parameter.RoutableText) })]
        [Description(@"Add a routable SiteObject, SiteObject can be page, style or others. 
  var page = {};
  page.name = ""pagename"";
  page.body = ""new  body"";
  page.url = ""/myurl""
  k.site.pages.add(page);")]
        public override void Add(object RoutableText)
        {
            string url = null;

            var data = Kooboo.Sites.Scripting.Global.kHelper.GetData(RoutableText);
            if (data.ContainsKey("url"))
            {
                url = data["url"].ToString();
            }

            var siteobject = Kooboo.Lib.Reflection.TypeHelper.ToObject(data, this.repo.ModelType);
            if (siteobject != null)
            {
                var routeobject = siteobject as Kooboo.Sites.Models.SiteObject;

                if (routeobject != null)
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        url = "/" + routeobject.Name;
                    }

                    if (routeobject is Kooboo.Sites.Models.Style || routeobject is Kooboo.Sites.Models.Script)
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

        [KIgnore]
        public string getObjectUrl(object siteobj)
        {
            var siteobject = siteobj as SiteObject;

            if (siteobject != null)
            {
                return getUrl(siteobject.Id.ToString());
            }

            if (siteobj is Guid || siteobj is String)
            {
                return getUrl(siteobj.ToString());
            }

            return null;
        }

        [Description(@"get the SiteObject relative url, SiteObject can be page, style or others.
      var page = k.site.pages.getByUrl(""/pagename"");
      var url = k.site.pages.getUrl(page.id);")]
        public string getUrl(object id)
        {
            if (id == null)
            {
                return null;
            }

            string strkey = id.ToString();

            Guid guid = default(Guid);

            if (System.Guid.TryParse(strkey, out guid))
            {
                var route = this.context.WebSite.SiteDb().Routes.GetByObjectId(guid);
                if (route != null)
                {
                    return route.Name;
                }
            }
            return null;
        }


        [Description(@"get the absolute Url of this object
      var page = k.site.pages.getByUrl(""/ pagename"");
      var url = k.site.pages.getAbsUrl(page.id);")]
        public string getAbsUrl(object id)
        {
            var relative = getUrl(id);
            if (!string.IsNullOrWhiteSpace(relative))
            {
                return this.context.WebSite.BaseUrl(relative);
            }
            return null;
        }
    }
}
