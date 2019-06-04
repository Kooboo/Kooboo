//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Authorization;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation
{
    public class HtmlBlockApi : SiteObjectApi<HtmlBlock>
    {

        [Kooboo.Attributes.RequireParameters("id", "values")]
        public override Guid Post(ApiCall apiCall)
        { 
            Guid id = apiCall.ObjectId;  
            var strvalues = apiCall.GetValue("values");
            if (string.IsNullOrEmpty(strvalues))
            {
                return default(Guid); 
            }

            Dictionary<string, object> values = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, object>>(strvalues);
             
            if (id != default(Guid))
            {
                var current = apiCall.WebSite.SiteDb().HtmlBlocks.Get(id);
                if (current != null)
                {  
                    foreach (var item in values)
                    {
                        current.SetValue(item.Key, item.Value);
                    }
                    apiCall.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(current, apiCall.Context.User.Id);
                    return id;
                }
            }
            else
            {
                string name = apiCall.GetValue("name");
                HtmlBlock newblock = new HtmlBlock();
                newblock.Name = name;
                newblock.Values = values;

                apiCall.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(newblock, apiCall.Context.User.Id);

                return newblock.Id;                  
            } 
     
            return default(Guid); 
        }
          
        public override List<object> List(ApiCall call)
        {
            List<HtmlBlockItemViewModel> result = new List<HtmlBlockItemViewModel>();

            var sitedb = call.WebSite.SiteDb(); 

            int storenamehash = Lib.Security.Hash.ComputeInt(sitedb.HtmlBlocks.StoreName);

            foreach (var item in sitedb.HtmlBlocks.All())
            {
                HtmlBlockItemViewModel model = new HtmlBlockItemViewModel();
                model.Id = item.Id;
                model.Name = item.Name;
                model.KeyHash = Sites.Service.LogService.GetKeyHash(item.Id);
                model.StoreNameHash = storenamehash;
                model.LastModified = item.LastModified;
                model.Values = item.Values;
                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.HtmlBlocks.GetUsedBy(item.Id));
                result.Add(model);
            }
            return result.ToList<object>();
        }
         
    }
}
