//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
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
    public class ScriptApi : SiteObjectApi<Script>
    {
        public List<IEmbeddableItemListViewModel> External(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            int storenameHash = Lib.Security.Hash.ComputeInt(sitedb.Scripts.StoreName);
            List<IEmbeddableItemListViewModel> result = new List<IEmbeddableItemListViewModel>();

            foreach (var item in sitedb.Scripts.GetExternals().OrderBy(o => o.Name))
            {
                IEmbeddableItemListViewModel model = new IEmbeddableItemListViewModel(sitedb, item);
                model.KeyHash = Sites.Service.LogService.GetKeyHash(item.Id);
                model.StoreNameHash = storenameHash;
                result.Add(model);
            }

            return result;
        }

        public List<IEmbeddableItemListViewModel> Embedded(ApiCall apiCall)
        {
            return apiCall.WebSite.SiteDb().Scripts.GetEmbeddeds()
            .Select(o => new IEmbeddableItemListViewModel(apiCall.WebSite.SiteDb(), o)).ToList();
        }

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

        public Guid Update(ApiCall call)
        {
            Guid id = call.ObjectId;
            string name = call.GetValue("name");
            string body = call.GetValue("body");
            string extension = call.GetValue("extension");

            if (string.IsNullOrEmpty(extension))
            {
                extension = "js"; 
            }
                   
            if (id != default(Guid))
            {
                var script = call.WebSite.SiteDb().Scripts.Get(id);
                if (script != null)
                {
                    script.Body = body;
                    if (script.Extension == null)
                    {
                        script.Extension = extension; 
                    }
                    call.WebSite.SiteDb().Scripts.AddOrUpdate(script, true, true, call.Context.User.Id);
                    return script.Id;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                {
                    return default(Guid);
                }

                if (!name.EndsWith("." + extension))
                {
                    name = name + "." + extension;
                }

                string url = name;
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
                        script.Body = body;
                        call.WebSite.SiteDb().Scripts.AddOrUpdate(script, true, true, call.Context.User.Id);
                        return script.Id;
                    }
                }

                Script newscript = new Script();
                newscript.Name = name;
                newscript.Body = body;
                newscript.Extension = extension;
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

                if (find !=null)
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
    }
}
