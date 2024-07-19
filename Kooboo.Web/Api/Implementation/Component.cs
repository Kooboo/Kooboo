//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Render.Components;
using Kooboo.Sites.Service;

namespace Kooboo.Web.Api.Implementation
{
    public class ComponentApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "component";
            }
        }

        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        public List<ComponentNames> List(ApiCall call)
        {
            List<ComponentNames> result = new List<ComponentNames>();
            foreach (var item in Container.List)
            {
                if (item.Value.TagName.ToLower() != "style")
                {
                    var display = item.Value.DisplayName(call.Context);

                    var model = new ComponentNames() { TagName = item.Key, DisplayName = display };

                    if (item.Value.IsRegularHtmlTag && !string.IsNullOrEmpty(item.Value.StoreEngineName))
                    {
                        model.RequireEngine = true;
                        model.EngineName = item.Value.StoreEngineName;
                    }

                    if (item.Value.IsRegularHtmlTag)
                    {
                        model.Attribute = "env='server'";
                    }
                    result.Add(model);
                }
            }

            ComponentNames layout = new ComponentNames() { TagName = "Layout", DisplayName = Data.Language.Hardcoded.GetValue("Layout", call.Context) };
            result.Add(layout);
            return result;
        }


        public List<ComponentInfo> TagObjects(string tag, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            if (tag.ToLower() == "layout")
            {
                List<ComponentInfo> Models = new List<ComponentInfo>();
                var alllayout = sitedb.Layouts.All();
                foreach (var item in alllayout)
                {
                    ComponentInfo comp = new ComponentInfo();
                    comp.Id = item.Id;
                    comp.Name = item.Name;
                    Models.Add(comp);
                }
                return Models;
            }
            else
            {
                return Manager.AvailableObjects(sitedb, tag);
            }

        }


        public string PreviewHtml(ApiCall call)
        {
            string tag = call.GetValue("tag");
            if (string.IsNullOrEmpty(tag))
            {
                return null;
            }

            if (call.ObjectId != default(Guid))
            {
                return Manager.Preview(call.Context.WebSite.SiteDb(), tag, call.ObjectId.ToString());
            }
            else
            {
                string NameOrId = call.GetValue("id", "NameOrId");
                if (!string.IsNullOrEmpty(NameOrId))
                {
                    return Manager.Preview(call.Context.WebSite.SiteDb(), tag, NameOrId);
                }
            }
            return null;
        }


        public ComponentSource GetSource(ApiCall call)
        {
            ComponentSource source = new ComponentSource();
            var sitedb = call.WebSite.SiteDb();

            string tag = call.GetValue("tag");
            if (string.IsNullOrEmpty(tag))
            {
                return null;
            }

            string id = call.ObjectId == default(Guid) ? call.GetValue("id") : call.ObjectId.ToString();

            if (tag.ToLower() == "layout")
            {
                var layout = sitedb.Layouts.GetByNameOrId(id);
                source.Body = layout.Body;
                return source;
            }
            else if (tag.ToLower() == "script")
            {
                return source;
            }
            else
            {
                source.Body = Manager.Preview(sitedb, tag, id);

                if (tag.ToLower() == "view")
                {
                    List<Guid> ids = new List<Guid>();
                    var view = call.WebSite.SiteDb().Views.GetByNameOrId(id);
                    if (view != null)
                    {
                        ids.Add(view.Id);
                        source.MetaBindings = PageService.GetMetaBindings(call.WebSite.SiteDb(), ids);
                        source.UrlParamsBindings = PageService.GetUrlParas(call.WebSite.SiteDb(), ids);
                    }
                }
                return source;
            }
        }


    }

    public class ComponentSource
    {
        public string Body { get; set; }

        public List<string> MetaBindings { get; set; } = new List<string>();

        public List<string> UrlParamsBindings { get; set; } = new List<string>();

    }

    public class ComponentNames
    {
        public string TagName { get; set; }
        public string DisplayName { get; set; }

        public bool RequireEngine { get; set; }

        public string EngineName { get; set; }


        public string Attribute { get; set; }
    }
}
