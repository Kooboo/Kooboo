//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Components
{
    public class HtmlBlockComponent : IComponent
    {
        public string TagName
        {
            get { return "HtmlBlock"; }
        }

        public Dictionary<string, string> Setttings
        {
            get
            {
                return null;
            }
        }

        public bool IsRegularHtmlTag { get { return false; } }

        public string StoreEngineName { get { return null; } }

        public byte StoreConstType { get { return ConstObjectType.HtmlBlock; } }

        public Task<string> RenderAsync(RenderContext context, ComponentSetting settings)
        {
            if (settings != null && !string.IsNullOrEmpty(settings.NameOrId))
            {
                HtmlBlock htmlBlock = context.WebSite.SiteDb().HtmlBlocks.GetByNameOrId(settings.NameOrId);

                if (htmlBlock != null)
                {
                    return Task.FromResult(htmlBlock.GetValue(context.Culture).ToString());
                }
            }

            return Task.FromResult(string.Empty);
        }

        public List<ComponentInfo> AvaiableObjects(SiteDb sitedb)
        {
            List<ComponentInfo> models = new List<ComponentInfo>();
            var allblocks = sitedb.HtmlBlocks.All();
            foreach (var item in allblocks)
            {
                ComponentInfo comp = new ComponentInfo {Id = item.Id, Name = item.Name};
                models.Add(comp);
            }
            return models;
        }

        public string Preview(SiteDb siteDb, string nameOrId)
        {
            if (string.IsNullOrEmpty(nameOrId))
            {
                return null;
            }
            var item = siteDb.HtmlBlocks.GetByNameOrId(nameOrId);
            return item?.GetValue(siteDb.WebSite.DefaultCulture).ToString();
        }

        public string DisplayName(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("HtmlBlock", context);
        }
    }
}