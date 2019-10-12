//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Web.Api.Implementation
{
    //public class KScriptApi : SiteObjectApi<KScript>
    //{
    //    //public List<IEmbeddableItemListViewModel> External(ApiCall call)
    //    //{
    //    //    var sitedb = call.WebSite.SiteDb();
    //    //    int storenameHash = Lib.Security.Hash.ComputeInt(sitedb.KScripts.StoreName);
    //    //    List<IEmbeddableItemListViewModel> result = new List<IEmbeddableItemListViewModel>();

    //    //    foreach (var item in sitedb.KScripts.GetExternals().OrderBy(o => o.Name))
    //    //    {
    //    //        IEmbeddableItemListViewModel model = new IEmbeddableItemListViewModel(sitedb, item);
    //    //        model.KeyHash = Sites.Service.LogService.GetKeyHash(item.Id);
    //    //        model.StoreNameHash = storenameHash;
    //    //        result.Add(model);
    //    //    }

    //    //    return result;
    //    //}

    //    //public List<IEmbeddableItemListViewModel> Embedded(ApiCall apiCall)
    //    //{
    //    //    return apiCall.WebSite.SiteDb().KScripts.GetEmbeddeds()
    //    //    .Select(o => new IEmbeddableItemListViewModel(apiCall.WebSite.SiteDb(), o)).ToList();
    //    //}

    //    //public Guid Update(ApiCall call)
    //    //{
    //    //    Guid id = call.ObjectId;
    //    //    string name = call.GetValue("name");
    //    //    string body = call.GetValue("body");

    //    //    if (id != default(Guid))
    //    //    {
    //    //        var script = call.WebSite.SiteDb().KScripts.Get(id);
    //    //        if (script != null)
    //    //        {
    //    //            script.Body = body;
    //    //            call.WebSite.SiteDb().KScripts.AddOrUpdate(script, true, true, call.Context.User.Id);
    //    //            return script.Id;
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        if (string.IsNullOrEmpty(name))
    //    //        {
    //    //            return default(Guid);
    //    //        }

    //    //        if (!name.ToLower().EndsWith(".js"))
    //    //        {
    //    //            name = name + ".js";
    //    //        }

    //    //        KScript newscript = new KScript();
    //    //        newscript.Name = name;
    //    //        newscript.Body = body;
    //    //        call.WebSite.SiteDb().KScripts.AddOrUpdate(newscript, true, true, call.Context.User.Id);
    //    //        return newscript.Id;
    //    //    }
    //    //    return default(Guid);
    //    //}

    //    //public override bool IsUniqueName(ApiCall call)
    //    //{
    //    //    string name = call.NameOrId;

    //    //    if (!string.IsNullOrEmpty(name))
    //    //    {
    //    //        var value = call.WebSite.SiteDb().KScripts.GetByNameOrId(name);
    //    //        if (value != null)
    //    //        {
    //    //            return false;
    //    //        }
    //    //        if (!name.ToLower().EndsWith(".js"))
    //    //        {
    //    //            value = call.WebSite.SiteDb().KScripts.GetByNameOrId(name + ".js");
    //    //            if (value != null)
    //    //            {
    //    //                return false;
    //    //            }
    //    //        }
    //    //    }

    //    //    return true;
    //    //}
    //}
}