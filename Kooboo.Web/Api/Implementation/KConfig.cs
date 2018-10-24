using Kooboo.Api;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Extensions; 

namespace Kooboo.Web.Api.Implementation
{                                       
    public class KConfigApi : SiteObjectApi<KConfig>
    {         
       
        public void Update(Guid Id, string Binding, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();    
            var config = sitedb.KConfig.Get(Id);
            if (config != null)
            {
                config.Binding = Binding;
                sitedb.KConfig.AddOrUpdate(config);     
            }
        }


        public override List<object> List(ApiCall call)
        {
            List<KConfigItemViewModel> result = new List<KConfigItemViewModel>();

            var sitedb = call.WebSite.SiteDb();

            int storenamehash = Lib.Security.Hash.ComputeInt(sitedb.Labels.StoreName);

            foreach (var item in sitedb.KConfig.All())
            {
                KConfigItemViewModel model = new KConfigItemViewModel();
                model.Id = item.Id;
                model.TagName = item.Name;
                model.KeyHash = Sites.Service.LogService.GetKeyHash(item.Id);
                model.StoreNameHash = storenamehash;
                model.LastModified = item.LastModified;
                model.Binding = item.Binding;
                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.KConfig.GetUsedBy(item.Id));
                result.Add(model);
            }
            return result.ToList<object>();
        }

        public List<string> Keys(ApiCall call)
        {
            List<string> keys = new List<string>();
            foreach (var item in call.WebSite.SiteDb().KConfig.All())
            {
                keys.Add(item.Name);
            }
            return keys;
        }

    }


}
