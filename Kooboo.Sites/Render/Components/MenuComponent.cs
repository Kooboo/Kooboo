//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            get { return _settings ?? (_settings = new Dictionary<string, string> {{MenuLevelSettingName, ""}}); }
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

        public List<ComponentInfo> AvaiableObjects(SiteDb siteDb)
        {
            List<ComponentInfo> models = new List<ComponentInfo>();
            var allmenu = siteDb.Menus.List();
            foreach (var item in allmenu)
            {
                ComponentInfo comp = new ComponentInfo {Id = item.Id, Name = item.Name};
                comp.Settings.Add("MenuLevel", "");
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
            Menu menu = siteDb.Menus.GetByNameOrId(nameOrId);

            if (menu == null)
            {
                return null;
            }

            RenderContext previewContext = new RenderContext {WebSite = siteDb.WebSite};
            return MenuHelper.Render(menu, previewContext, 99);
        }

        public string DisplayName(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Menu", context);
        }
    }
}