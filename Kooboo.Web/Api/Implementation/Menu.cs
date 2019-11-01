//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class MenuApi : SiteObjectApi<Menu>
    {
        [Kooboo.Attributes.RequireParameters("data", "rootid")]
        public JsonResponse Update(ApiCall call)
        {
            string data = call.GetValue("Data");
            string strRootId = call.GetValue("RootId");
            if (!System.Guid.TryParse(strRootId, out var RootId))
            {
                List<string> message = new List<string>();
                message.Add("rootid is not a valid guid");
                return new JsonResponse { Success = false, Messages = message };
            }
            try
            {
                var menu = Lib.Helper.JsonHelper.Deserialize<Menu>(data);
                menu.Id = RootId;
                call.WebSite.SiteDb().Menus.AddOrUpdate(menu, call.Context.User.Id);
                return new JsonResponse { Success = true };
            }
            catch (Exception ex)
            {
                List<string> message = new List<string> {ex.Message};
                return new JsonResponse { Success = false, Messages = message };
            }
        }

        [Kooboo.Attributes.RequireParameters("rootid", "id", "Value")]
        public bool UpdateText(ApiCall call)
        {
            Guid id = call.ObjectId;
            string strroot = call.GetValue("RootId");

            if (System.Guid.TryParse(strroot, out var RootId))
            {
                var root = call.WebSite.SiteDb().Menus.Get(RootId);

                var sub = GetSubMenu(root, id);

                if (sub != null)
                {
                    string text = call.GetValue("Value");
                    sub.Name = text;
                    sub.Values[call.WebSite.DefaultCulture] = text;

                    call.WebSite.SiteDb().Menus.AddOrUpdate(root, call.Context.User.Id);
                    return true;
                }
            }
            return false;
        }

        [Kooboo.Attributes.RequireParameters("Id", "RootId", "Url")]
        public bool UpdateUrl(ApiCall call)
        {
            Guid id = call.ObjectId;
            string strroot = call.GetValue("RootId");

            if (System.Guid.TryParse(strroot, out var rootId))
            {
                var root = call.WebSite.SiteDb().Menus.Get(rootId);

                var sub = GetSubMenu(root, id);

                if (sub != null)
                {
                    string text = call.GetValue("Url");
                    sub.Url = text;
                    call.WebSite.SiteDb().Menus.AddOrUpdate(root, call.Context.User.Id);
                    return true;
                }
            }
            return false;
        }

        //[Kooboo.Attributes.RequireParameters("Id", "RootId", "SubItemTemplate", "SubItemContainer")]
        public bool UpdateTemplate(ApiCall call)
        {
            string template = call.GetValue("Template");
            string subItemTemplate = call.GetValue("SubItemTemplate");
            string subItemContainer = call.GetValue("SubItemContainer");

            if (string.IsNullOrWhiteSpace(template) && string.IsNullOrWhiteSpace(subItemTemplate) && string.IsNullOrWhiteSpace(subItemContainer))
            {
                return false;
            }

            Guid id = call.ObjectId;
            string strroot = call.GetValue("RootId");

            if (System.Guid.TryParse(strroot, out var rootId))
            {
                var root = call.WebSite.SiteDb().Menus.Get(rootId);

                var sub = GetSubMenu(root, id);

                if (sub != null)
                {
                    sub.Template = template;
                    sub.SubItemContainer = subItemContainer;
                    sub.SubItemTemplate = subItemTemplate;
                    call.WebSite.SiteDb().Menus.AddOrUpdate(root, call.Context.User.Id);
                    return true;
                }
            }
            return false;
        }

        [Kooboo.Attributes.RequireParameters("Id", "RootId")]
        public Dictionary<string, string> GetLang(ApiCall call)
        {
            Guid id = call.ObjectId;
            string strroot = call.GetValue("RootId");
            call.GetGuidValue("rootid");

            Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (System.Guid.TryParse(strroot, out var rootId))
            {
                var root = call.WebSite.SiteDb().Menus.Get(rootId);

                var sub = GetSubMenu(root, id);

                if (sub != null)
                {
                    foreach (var item in call.WebSite.Culture.Keys.ToList())
                    {
                        string itemvalue = string.Empty;
                        if (sub.Values != null && sub.Values.ContainsKey(item))
                        {
                            itemvalue = sub.Values[item];
                        }

                        values[item] = itemvalue;
                    }

                    var defaultvalue = values[call.WebSite.DefaultCulture];
                    if (string.IsNullOrEmpty(defaultvalue) && !string.IsNullOrEmpty(sub.Name))
                    {
                        values[call.WebSite.DefaultCulture] = sub.Name;
                    }
                }
            }
            return values;
        }

        [Kooboo.Attributes.RequireParameters("Id", "RootId", "Values")]
        public string UpdateLang(ApiCall call)
        {
            Guid id = call.ObjectId;
            string strroot = call.GetValue("RootId");
            call.GetGuidValue("rootid");
            string values = call.GetValue("Values");

            if (System.Guid.TryParse(strroot, out var rootId))
            {
                var root = call.WebSite.SiteDb().Menus.Get(rootId);

                var sub = GetSubMenu(root, id);

                if (sub != null)
                {
                    var dict = Kooboo.Lib.Helper.JsonHelper.Deserialize<Dictionary<string, string>>(values);

                    sub.Values = dict;

                    call.WebSite.SiteDb().Menus.AddOrUpdate(root, call.Context.User.Id);

                    string defaultlangcode = call.WebSite.DefaultCulture;

                    if (dict.ContainsKey(defaultlangcode))
                    {
                        return dict[defaultlangcode];
                    }
                }
            }
            return null;
        }

        public override List<object> List(ApiCall call)
        {
            List<MainMenuItemViewModel> result = new List<MainMenuItemViewModel>();
            var sitedb = call.WebSite.SiteDb();
            int storeNameHash = Lib.Security.Hash.ComputeInt(sitedb.Menus.StoreName);
            foreach (var item in sitedb.Menus.All())
            {
                MainMenuItemViewModel model = new MainMenuItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    KeyHash = Sites.Service.LogService.GetKeyHash(item.Id),
                    StoreNameHash = storeNameHash,
                    LastModified = item.LastModified,
                    Relations = Sites.Helper.RelationHelper.Sum(sitedb.Menus.GetUsedBy(item.Id))
                };
                result.Add(model);
            }
            return result.ToList<object>();
        }

        [Kooboo.Attributes.RequireParameters("Name")]
        public Menu Create(ApiCall call)
        {
            string name = call.GetValue("Name");
            var sitedb = call.WebSite.SiteDb();

            if (!string.IsNullOrEmpty(name))
            {
                var old = sitedb.Menus.GetByNameOrId(name);
                if (old != null)
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("Name already exists", call.Context));
                }

                var menu = new Menu {Name = name};
                sitedb.Menus.AddOrUpdate(menu);
                return menu;
            }
            return null;
        }

        public override bool IsUniqueName(ApiCall call)
        {
            return base.IsUniqueName(call);
        }

        [Kooboo.Attributes.RequireParameters("ParentId", "RootId", "Name", "Url")]
        public Menu CreateSub(ApiCall call)
        {
            Guid parentId = call.GetGuidValue("ParentId");
            Guid rootId = call.GetGuidValue("RootId");

            if (parentId != default(Guid) && rootId != default(Guid))
            {
                var root = call.WebSite.SiteDb().Menus.Get(rootId);
                if (root != null)
                {
                    var parentmenu = GetSubMenu(root, parentId);
                    if (parentmenu != null)
                    {
                        string name = call.GetValue("Name");
                        string url = call.GetValue("Url");
                        var submenu = new Menu {ParentId = parentId, Name = name};
                        submenu.Values.Add(call.WebSite.DefaultCulture, name);
                        submenu.RootId = rootId;
                        submenu.Url = url;
                        parentmenu.children.Insert(0, submenu);
                        call.WebSite.SiteDb().Menus.AddOrUpdate(root, call.Context.User.Id);
                        return submenu;
                    }
                }
            }
            return null;
        }

        private Menu GetSubMenu(Menu menu, Guid subId)
        {
            if (menu.Id == subId)
            {
                return menu;
            }

            foreach (var item in menu.children)
            {
                var result = GetSubMenu(item, subId);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public List<SiteMenuItemViewModel> GetFlat(ApiCall call)
        {
            Menu menu = null;
            if (call.ObjectId != default(Guid))
            {
                menu = call.WebSite.SiteDb().Menus.Get(call.ObjectId);
            }
            else
            {
                string name = call.GetValue("name");
                if (!string.IsNullOrEmpty(name))
                {
                    menu = call.WebSite.SiteDb().Menus.GetByNameOrId(name);
                }
            }

            if (menu != null)
            {
                List<SiteMenuItemViewModel> flatList = new List<SiteMenuItemViewModel>();
                MakeFlat(menu, flatList, menu.Id, call.WebSite);
                return flatList;
            }
            return null;
        }

        private SiteMenuItemViewModel MakeFlat(Menu topmenu, List<SiteMenuItemViewModel> result, Guid rootId, WebSite site)
        {
            var menuview = ToViewModel(topmenu, site);
            menuview.RootId = rootId;
            result.Add(menuview);
            foreach (var item in topmenu.children)
            {
                var subitem = MakeFlat(item, result, rootId, site);
                menuview.Children.Add(subitem);
            }

            return menuview;
        }

        [Kooboo.Attributes.RequireParameters("Id", "RootId")]
        public override bool Delete(ApiCall call)
        {
            string strrootid = call.GetValue("rootid");
            string strid = call.GetValue("id");

            if (Guid.TryParse(strrootid, out var rootId) && Guid.TryParse(strid, out var id))
            {
                if (id == rootId)
                {
                    call.WebSite.SiteDb().Menus.Delete(id, call.Context.User.Id);
                }
                else
                {
                    var rootmenu = call.WebSite.SiteDb().Menus.Get(rootId);
                    if (rootmenu != null)
                    {
                        RemoveSubItem(rootmenu, id);
                    }
                    call.WebSite.SiteDb().Menus.AddOrUpdate(rootmenu, call.Context.User.Id);
                }
                return true;
            }
            return false;
        }

        private bool RemoveSubItem(Menu menu, Guid subId)
        {
            Menu founditem = menu.children.Find(o => o.Id == subId);
            if (founditem != null)
            {
                menu.children.Remove(founditem);
                return true;
            }
            else
            {
                foreach (var item in menu.children)
                {
                    if (RemoveSubItem(item, subId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private SiteMenuItemViewModel ToViewModel(Menu menu, WebSite site)
        {
            SiteMenuItemViewModel item = new SiteMenuItemViewModel
            {
                Id = menu.Id,
                ParentId = menu.ParentId,
                RootId = menu.RootId,
                SubItemContainer = menu.SubItemContainer,
                SubItemTemplate = menu.SubItemTemplate,
                Template = menu.Template,
                Name = Kooboo.Sites.Helper.MenuHelper.GetName(menu, site.DefaultCulture),
                Url = menu.Url
            };

            return item;
        }

        [Kooboo.Attributes.RequireParameters("rootid", "ida", "idb")]
        public void Swap(ApiCall call)
        {
            Guid rootid = call.GetGuidValue("rootid");
            Guid ida = call.GetGuidValue("ida");
            Guid idb = call.GetGuidValue("idb");

            call.WebSite.SiteDb().Menus.Swap(rootid, ida, idb, call.Context.User.Id);
        }
    }
}