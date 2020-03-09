//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Repository;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Service;

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
                    var result=htmlBlock.GetValue(context.Culture).ToString();
                    if (context.Request.Channel == RequestChannel.InlineDesign)
                    {
                        result = DomService.ApplyKoobooId(result);
                    }

                    return Task.FromResult(result);
                }
            }

            return Task.FromResult(string.Empty);
        }

        public List<ComponentInfo> AvaiableObjects(SiteDb sitedb)
        {
            List<ComponentInfo> Models = new List<ComponentInfo>();
            var allblocks = sitedb.HtmlBlocks.All();
            foreach (var item in allblocks)
            {
                ComponentInfo comp = new ComponentInfo();
                comp.Id = item.Id;
                comp.Name = item.Name;
                Models.Add(comp);
            }
            return Models;
        }

        public string Preview(SiteDb SiteDb, string NameOrId)
        {
            if (string.IsNullOrEmpty(NameOrId))
            {
                return null;
            }
            var item = SiteDb.HtmlBlocks.GetByNameOrId(NameOrId);
            return item != null ? item.GetValue(SiteDb.WebSite.DefaultCulture).ToString() : null;
        }

        public string DisplayName(RenderContext Context)
        {
            return Data.Language.Hardcoded.GetValue("HtmlBlock", Context);
        }
    }
}
