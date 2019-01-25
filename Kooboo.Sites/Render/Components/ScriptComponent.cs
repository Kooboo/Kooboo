//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.Render.Components
{
    public class ScriptComponent : IComponent
    {
        public string TagName
        {
            get
            {
                return "Script";
            }
        }

        public bool IsRegularHtmlTag { get { return true; } }

        public string StoreEngineName { get { return "kscript"; } }

        private Dictionary<string, string> _settings;

        public Dictionary<string, string> Setttings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Dictionary<string, string>();
                }
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        public byte StoreConstType { get { return ConstObjectType.Code; } }

        // only kscript can be inserted into page. 
        public List<ComponentInfo> AvaiableObjects(SiteDb SiteDb)
        {
            List<ComponentInfo> Models = new List<ComponentInfo>();
            var allScript = SiteDb.Code.ListByCodeType(Sites.Models.CodeType.PageScript).Where(o=>o.IsEmbedded== false);

            foreach (var item in allScript)
            {
                ComponentInfo comp = new ComponentInfo();
                comp.Id = item.Id;
                comp.Name = item.Name;
                Models.Add(comp);
            }
            return Models;
        }

        public string DisplayName(RenderContext Context)
        {
            return Data.Language.Hardcoded.GetValue("Script", Context);
        }

        public string Preview(SiteDb SiteDb, string NameOrId)
        {
            var k = SiteDb.Code.Get(NameOrId); 
            if (k !=null)
            {
                return k.Body; 
            }
            return null;
        }

        public Task<string> RenderAsync(RenderContext context, ComponentSetting settings)
        {
            string code = settings.InnerHtml;
            string result = null; 

            if (settings.Engine == "kscript")
            {
                if (!string.IsNullOrWhiteSpace(settings.NameOrId))
                {
                    var kscript = context.WebSite.SiteDb().Code.Get(settings.NameOrId);
                    if (kscript != null)
                    {
                        result = Scripting.Manager.ExecuteCode(context, kscript.Body, kscript.Id);  ///  code = kscript.Body; 
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(code))
                    { 
                        result = Scripting.Manager.ExecuteInnerScript(context, code);
                    }
                }
               
            }
            else
            {
                // other engine only support inner html or dynamic render....  
                if (!string.IsNullOrWhiteSpace(settings.Engine) && !string.IsNullOrWhiteSpace(code))
                {
                    result = Kooboo.Sites.Engine.Manager.Execute(settings.Engine, context, code, settings.TagName, settings.TagAttributes);
                }
                else
                {
                    result = code; 
                }    
            }
              
            return Task.FromResult<string>(result);

        }
    }
}
