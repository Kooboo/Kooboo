//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Helper;
using Kooboo.Data.Context;
using System;
using Kooboo.Sites.Service;

namespace Kooboo.Sites.Render.Components
{
    public class MenuComponent : IComponent
    {
        private string MenuLevelSettingName = "MenuLevel";

        public MenuComponent()
        {
        }

        public string TagName
        {
            get
            {
                return "Menu";
            }
        }

        private Dictionary<string, string> _settings;


        public Dictionary<string, string> Setttings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Dictionary<string, string>();
                    _settings.Add(MenuLevelSettingName, "");
                }
                return _settings;
            }
        }

        public bool IsRegularHtmlTag { get { return false; } }

        public string StoreEngineName { get { return null; } }

        public byte StoreConstType { get { return ConstObjectType.Menu; } }

        public Task<string> RenderAsync(RenderContext context, ComponentSetting settings)
        {
            if (settings == null || string.IsNullOrEmpty(settings.NameOrId))
            {
                return Task.FromResult(string.Empty);
            }
            var frontcontext = context.GetItem<Render.FrontContext>();
            DateTime logstart = DateTime.UtcNow;

            Menu menu = frontcontext.SiteDb.Menus.GetByNameOrId(settings.NameOrId);

            if (menu == null)
            {
                frontcontext.AddLogEntry("menu", "", logstart, 404);
                return Task.FromResult(string.Empty);
            }

            string returnstring = MenuHelper.Render(menu, context, GetRenderLevel(settings));

            frontcontext.AddLogEntry("menu", menu.Name, logstart, 200);

            //if (context.Request.Channel == RequestChannel.InlineDesign)
            //{
            //    returnstring = DomService.ApplyKoobooId(returnstring);
            //}

            return Task.FromResult(returnstring);
        }

        private int GetRenderLevel(ComponentSetting setting)
        {
            int result = 999;

            if (setting.Settings != null)
            {
                if (setting.Settings.ContainsKey(MenuLevelSettingName))
                {
                    var strlevel = setting.Settings[MenuLevelSettingName];
                    int.TryParse(strlevel, out result);
                }
            }

            return result;
        }

        public List<ComponentInfo> AvaiableObjects(SiteDb SiteDb)
        {
            List<ComponentInfo> Models = new List<ComponentInfo>();
            var allmenu = SiteDb.Menus.List();
            foreach (var item in allmenu)
            {
                ComponentInfo comp = new ComponentInfo();
                comp.Id = item.Id;
                comp.Name = item.Name;
                comp.Settings.Add("MenuLevel", "");
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
            Menu menu = SiteDb.Menus.GetByNameOrId(NameOrId);

            if (menu == null)
            {
                return null;
            }
            RenderContext previewContext = new RenderContext();
            previewContext.WebSite = SiteDb.WebSite;
            return MenuHelper.Render(menu, previewContext, 99);
        }

        public string DisplayName(RenderContext Context)
        {
            return Data.Language.Hardcoded.GetValue("Menu", Context);
        }
    }
}
