//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class ScriptApi : SiteObjectApi<Script>
    {
        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<IEmbeddableItemListViewModel> External(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            int storenameHash = Lib.Security.Hash.ComputeInt(sitedb.Scripts.StoreName);

            foreach (var item in sitedb.Scripts.GetExternals().SortByNameOrLastModified(call))
            {
                yield return new IEmbeddableItemListViewModel(sitedb, item)
                {
                    KeyHash = Sites.Service.LogService.GetKeyHash(item.Id),
                    StoreNameHash = storenameHash
                };
            }
        }

        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<IEmbeddableItemListViewModel> Embedded(ApiCall apiCall)
        {
            return apiCall
                .WebSite
                .SiteDb()
                .Scripts
                .GetEmbeddeds()
                .SortByBodyOrLastModified(apiCall)
                .Select(o => new IEmbeddableItemListViewModel(apiCall.WebSite.SiteDb(), o));
        }

        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.VIEW)]
        public object Relation(ApiCall call)
        {
            string type = call.GetValue("type", "by");
            if (string.IsNullOrEmpty(type))
            {
                return null;
            }
            byte consttype = ConstTypeContainer.GetConstType(type);

            if (call.ObjectId != default(Guid))
            {
                return call.WebSite.SiteDb().Scripts.GetUsedBy(call.ObjectId)
                       .Where(it => it.ConstType == consttype)
                       .Select(it =>
                       {
                           it.Url = call.WebSite.BaseUrl(it.Url);
                           return it;
                       });
            }

            return null;

        }

        [Kooboo.Attributes.RequireModel(typeof(ScriptEditViewModel))]
        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.EDIT)]
        public Guid Update(ApiCall call)
        {
            var model = call.Context.Request.Model as ScriptEditViewModel;

            if (string.IsNullOrEmpty(model.Extension))
            {
                model.Extension = "js";
            }

            if (model.Id != default(Guid))
            {
                var script = call.WebSite.SiteDb().Scripts.Get(model.Id);
                if (script != null)
                {
                    (model as IDiffChecker).CheckDiff(script);

                    script.Body = model.Body;
                    if (script.Extension == null)
                    {
                        script.Extension = model.Extension;
                    }
                    call.WebSite.SiteDb().Scripts.AddOrUpdate(script, true, true, call.Context.User.Id);
                    return script.Id;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    return default(Guid);
                }

                if (!model.Name.EndsWith("." + model.Extension))
                {
                    model.Name = model.Name + "." + model.Extension;
                }

                string url = model.Name;
                if (url.StartsWith("\\"))
                {
                    url = "/" + url.Substring(1);
                }
                if (!url.StartsWith("/"))
                {
                    url = "/" + url;
                }
                var route = call.WebSite.SiteDb().Routes.GetByUrl(url);
                if (route != null)
                {
                    var script = call.WebSite.SiteDb().Scripts.Get(route.objectId);
                    if (script != null)
                    {
                        script.Body = model.Body;
                        call.WebSite.SiteDb().Scripts.AddOrUpdate(script, true, true, call.Context.User.Id);
                        return script.Id;
                    }
                }

                Script newscript = new Script();
                newscript.Name = model.Name;
                newscript.Body = model.Body;
                newscript.Extension = model.Extension;
                call.WebSite.SiteDb().Routes.AddOrUpdate(url, newscript, call.Context.User.Id);
                call.WebSite.SiteDb().Scripts.AddOrUpdate(newscript, true, true, call.Context.User.Id);
                return newscript.Id;
            }
            return default(Guid);
        }

        public override bool IsUniqueName(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            string name = call.NameOrId;

            if (!string.IsNullOrEmpty(name))
            {
                var value = sitedb.Scripts.GetByNameOrId(name);
                if (value != null)
                {
                    return false;
                }

                List<string> dotextension = new List<string>();
                foreach (var item in GetExtensions(call))
                {
                    string dotitem = item;
                    if (!dotitem.StartsWith("."))
                    {
                        dotitem = "." + dotitem;
                    }
                    dotextension.Add(dotitem);
                }

                name = name.ToLower();

                var find = sitedb.Scripts.Store.FullScan(o => samename(o.Name, name, dotextension)).FirstOrDefault();

                if (find != null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool samename(string dbname, string name, List<string> extensionsWithDot)
        {
            if (dbname == null || name == null)
            {
                return false;
            }

            dbname = dbname.ToLower();
            if (dbname == name)
            {
                return true;
            }

            foreach (var item in extensionsWithDot)
            {

                if (dbname.EndsWith(item))
                {
                    dbname = dbname.Substring(0, dbname.Length - item.Length);
                    if (dbname == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<string> GetExtensions(ApiCall call)
        {
            HashSet<string> result = new HashSet<string>();
            result.Add("js");

            var list = Kooboo.Sites.Engine.Manager.GetScript();

            foreach (var item in list)
            {
                result.Add(item.Extension);
            }
            return result.ToList();
        }

        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }

        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.SCRIPT, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            return base.List(call);
        }
    }
}
