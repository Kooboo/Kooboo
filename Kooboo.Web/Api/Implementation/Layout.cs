//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation
{
    public class LayoutApi : SiteObjectApi<Layout>
    {
        public override object Get(ApiCall call)
        {
            if (call.ObjectId != default(Guid))
            {
                var layoutobject = base.Get(call);
                if (layoutobject != null)
                {
                    var layout = layoutobject as Layout;
                    var layoutclone = layout.Clone<Layout>();


                    string basehrel = call.WebSite.BaseUrl();
                    basehrel = Kooboo.Data.Service.WebSiteService.EnsureHttpsBaseUrlOnServer(basehrel, call.WebSite); 
                     

                    if (!string.IsNullOrEmpty(basehrel))
                    {
                        layoutclone.Body = Sites.Service.HtmlHeadService.SetBaseHref(layoutclone.Body, basehrel);
                        return layoutclone;
                    }
                }
            }

            Layout newlayout = new Layout();
            newlayout.Body = getdummy(call);
            return newlayout;
        }

        [Kooboo.Attributes.RequireModel(typeof(Layout))]
        public override Guid Post(ApiCall call)
        {
            Layout value = (Layout)call.Context.Request.Model;
            value.Body = Sites.Service.HtmlHeadService.RemoveBaseHrel(value.Body);
            call.WebSite.SiteDb().Layouts.AddOrUpdate(value, call.Context.User.Id);
            return value.Id;
        }

        private string getdummy(ApiCall call)
        {
            string baseurl = call.WebSite.SiteDb().WebSite.BaseUrl();

            string template = "<!DOCTYPE html>\r\n<html><head>\r\n<base href=\"{{BaseUrl}}\"></head><body>\r\n<div k-placeholder=\"Main\"> Sample text inside the layout.. </div>\r\n</body>\r\n</html>";

            return template.Replace("{{BaseUrl}}", baseurl);
        }

        public override List<object> List(ApiCall call)
        {
            List<LayoutItemViewModel> result = new List<LayoutItemViewModel>();

            var sitedb = call.WebSite.SiteDb();

            int storenamehash =  Lib.Security.Hash.ComputeInt(sitedb.Layouts.StoreName);

            foreach (var item in sitedb.Layouts.All())
            {
                LayoutItemViewModel model = new LayoutItemViewModel();
                model.Id = item.Id;
                model.Name = item.Name;
                model.KeyHash = Sites.Service.LogService.GetKeyHash(item.Id);
                model.StoreNameHash = storenamehash;
                model.LastModified = item.LastModified;
                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.Layouts.GetUsedBy(item.Id));
                result.Add(model);
            }
            return result.ToList<object>();
        }

        [Kooboo.Attributes.RequireParameters("id", "name")]
        public LayoutItemViewModel Copy(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var layout = sitedb.Layouts.Get(call.ObjectId);
            if (layout != null)
            {
                var newlayout = Lib.Serializer.Copy.DeepCopy<Layout>(layout);
                newlayout.CreationDate = DateTime.UtcNow;
                newlayout.LastModified = DateTime.UtcNow; 

                newlayout.Name = call.GetValue("name");
                sitedb.Layouts.AddOrUpdate(newlayout, call.Context.User.Id);

                int storenamehash = Lib.Security.Hash.ComputeInt(call.WebSite.SiteDb().Layouts.StoreName); 

                LayoutItemViewModel model = new LayoutItemViewModel();
                model.Id = newlayout.Id;
                model.Name = newlayout.Name;
                model.KeyHash = Sites.Service.LogService.GetKeyHash(newlayout.Id);
                model.StoreNameHash = storenamehash;
                model.LastModified = newlayout.LastModified;
                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.Layouts.GetUsedBy(newlayout.Id));

                return model;
            }
            return null;
        }


        public override bool Deletes(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb(); 

            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }
            List<Guid> ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);

            if (ids != null && ids.Count() > 0)
            {
                foreach (var item in ids)
                {
                    var relations = sitedb.Relations.GetReferredBy(this.ModelType, item); 
                    if (relations !=null && relations.Count>0)
                    {
                        throw new Exception(Data.Language.Hardcoded.GetValue("Layout is being used, can not be deleted", call.Context)); 
                    }
                     
                   sitedb.Layouts.Delete(item, call.Context.User.Id);
                }
                return true;
            }
            return false;
        }

    }
}
