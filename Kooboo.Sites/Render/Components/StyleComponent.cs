//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Components
{
    public class StyleComponent : IComponent
    {
        public string TagName
        {
            get
            {
                return "Style";
            }
        }

        public bool IsRegularHtmlTag { get { return true; } }

        public string StoreEngineName { get { return null; } }

        private Dictionary<string, string> _settings;

        public Dictionary<string, string> Setttings
        {
            get { return _settings ?? (_settings = new Dictionary<string, string>()); }
            set
            {
                _settings = value;
            }
        }

        public byte StoreConstType { get { return 0; } }

        public List<ComponentInfo> AvaiableObjects(SiteDb siteDb)
        {
            return new List<ComponentInfo>();
        }

        public string DisplayName(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Style", context);
        }

        public string Preview(SiteDb siteDb, string nameOrId)
        {
            return "<style></style>";
        }

        public Task<string> RenderAsync(RenderContext context, ComponentSetting settings)
        {
            // other engine only support inner html or dynamic render....
            string result = Kooboo.Sites.Engine.Manager.Execute(settings.Engine, context, settings.InnerHtml, settings.TagName, settings.TagAttributes);
            return Task.FromResult<string>(result);
        }
    }
}