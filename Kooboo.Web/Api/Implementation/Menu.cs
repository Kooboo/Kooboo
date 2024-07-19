//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Models;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class MenuApi : SiteObjectApi<Menu>
    {
        [Kooboo.Attributes.RequireParameters("data", "rootid")]
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public JsonResponse Update(ApiCall call)
        {
            string data = call.GetValue("Data");
            string strRootId = call.GetValue("RootId");
            Guid RootId;
            if (!System.Guid.TryParse(strRootId, out RootId))
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
                List<string> message = new List<string>();
                message.Add(ex.Message);
                return new JsonResponse { Success = false, Messages = message };
            }
        }

        [Kooboo.Attributes.RequireParameters("rootid", "id", "Value")]
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public bool UpdateText(ApiCall call)
        {
            Guid id = call.ObjectId;
            string strRoot = call.GetValue("RootId");
            Guid RootId = default(Guid);

            if (System.Guid.TryParse(strRoot, out RootId))
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
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public bool UpdateUrl(ApiCall call)
        {
            Guid id = call.ObjectId;
            string strroot = call.GetValue("RootId");
            Guid RootId = default(Guid);

            if (System.Guid.TryParse(strroot, out RootId))
            {
                var root = call.WebSite.SiteDb().Menus.Get(RootId);

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
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
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
            Guid RootId = default(Guid);

            if (System.Guid.TryParse(strroot, out RootId))
            {
                var root = call.WebSite.SiteDb().Menus.Get(RootId);

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
        [Permission(Feature.MENU, Action = Data.Permission.Action.VIEW)]
        public Dictionary<string, string> GetLang(ApiCall call)
        {
            Guid id = call.ObjectId;
            string strroot = call.GetValue("RootId");
            Guid RootId = call.GetGuidValue("rootid");

            Dictionary<string, string> Values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (System.Guid.TryParse(strroot, out RootId))
            {
                var root = call.WebSite.SiteDb().Menus.Get(RootId);

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

                        Values[item] = itemvalue;
                    }

                    var defaultvalue = Values[call.WebSite.DefaultCulture];
                    if (string.IsNullOrEmpty(defaultvalue) && !string.IsNullOrEmpty(sub.Name))
                    {
                        Values[call.WebSite.DefaultCulture] = sub.Name;
                    }
                }

            }
            return Values;
        }

        [Kooboo.Attributes.RequireParameters("Id", "RootId", "Values")]
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public string UpdateLang(ApiCall call)
        {
            Guid id = call.ObjectId;
            string strroot = call.GetValue("RootId");
            Guid RootId = call.GetGuidValue("rootid");
            string values = call.GetValue("Values");

            if (System.Guid.TryParse(strroot, out RootId))
            {
                var root = call.WebSite.SiteDb().Menus.Get(RootId);

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

        [Permission(Feature.MENU, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            int StoreNameHash = Lib.Security.Hash.ComputeInt(sitedb.Menus.StoreName);

            return sitedb.Menus
                .All()
                .SortByNameOrLastModified(call)
                .Select(item => new MainMenuItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    KeyHash = Sites.Service.LogService.GetKeyHash(item.Id),
                    StoreNameHash = StoreNameHash,
                    LastModified = item.LastModified,
                    Relations = Sites.Helper.RelationHelper.Sum(sitedb.Menus.GetUsedBy(item.Id))
                })
                .ToList<object>();
        }

        [Kooboo.Attributes.RequireParameters("Name")]
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public Menu Create(ApiCall call)
        {
            string Name = call.GetValue("Name");
            var sitedb = call.WebSite.SiteDb();

            if (!string.IsNullOrEmpty(Name))
            {

                var old = sitedb.Menus.GetByNameOrId(Name);
                if (old != null)
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("Name already exists", call.Context));

                }

                var menu = new Menu();
                menu.Name = Name;
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
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public virtual Menu CreateSub(ApiCall call)
        {
            Guid ParentId = call.GetGuidValue("ParentId");
            Guid RootId = call.GetGuidValue("RootId");

            if (ParentId != default(Guid) && RootId != default(Guid))
            {
                var root = call.WebSite.SiteDb().Menus.Get(RootId);
                if (root != null)
                {
                    var parentmenu = GetSubMenu(root, ParentId);
                    if (parentmenu != null)
                    {
                        string Name = call.GetValue("Name");
                        string Url = call.GetValue("Url");
                        var Submenu = new Menu();
                        Submenu.ParentId = ParentId;
                        Submenu.Name = Name;
                        Submenu.Values.Add(call.WebSite.DefaultCulture, Name);
                        Submenu.RootId = RootId;
                        Submenu.Url = Url;
                        parentmenu.children.Insert(0, Submenu);
                        call.WebSite.SiteDb().Menus.AddOrUpdate(root, call.Context.User.Id);
                        return Submenu;
                    }

                }

            }
            return null;
        }

        [Permission(Feature.MENU, Action = Data.Permission.Action.VIEW)]
        protected Menu GetSubMenu(Menu Menu, Guid SubId)
        {
            if (Menu.Id == SubId)
            {
                return Menu;
            }

            foreach (var item in Menu.children)
            {
                var result = GetSubMenu(item, SubId);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        [Permission(Feature.MENU, Action = Data.Permission.Action.VIEW)]
        public virtual List<SiteMenuItemViewModel> GetFlat(ApiCall call)
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
                List<SiteMenuItemViewModel> FlatList = new List<SiteMenuItemViewModel>();
                MakeFlat(menu, FlatList, menu.Id, call.WebSite);
                return FlatList;
            }
            return null;
        }

        private SiteMenuItemViewModel MakeFlat(Menu topmenu, List<SiteMenuItemViewModel> result, Guid RootId, WebSite site)
        {
            var menuview = ToViewModel(topmenu, site);
            menuview.RootId = RootId;
            result.Add(menuview);
            foreach (var item in topmenu.children)
            {
                var subitem = MakeFlat(item, result, RootId, site);
                menuview.Children.Add(subitem);
            }

            return menuview;
        }

        [Kooboo.Attributes.RequireParameters("Id", "RootId")]
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public override bool Delete(ApiCall call)
        {

            string strrootid = call.GetValue("rootid");
            string strid = call.GetValue("id");

            Guid RootId;
            Guid Id;

            if (Guid.TryParse(strrootid, out RootId) && Guid.TryParse(strid, out Id))
            {
                if (Id == RootId)
                {
                    call.WebSite.SiteDb().Menus.Delete(Id, call.Context.User.Id);
                }
                else
                {
                    var rootmenu = call.WebSite.SiteDb().Menus.Get(RootId);
                    if (rootmenu != null)
                    {
                        RemoveSubItem(rootmenu, Id);
                    }
                    call.WebSite.SiteDb().Menus.AddOrUpdate(rootmenu, call.Context.User.Id);
                }
                return true;
            }
            return false;
        }

        private bool RemoveSubItem(Menu menu, Guid SubId)
        {
            Menu founditem = menu.children.Find(o => o.Id == SubId);
            if (founditem != null)
            {
                menu.children.Remove(founditem);
                return true;
            }
            else
            {
                foreach (var item in menu.children)
                {
                    if (RemoveSubItem(item, SubId))
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        private SiteMenuItemViewModel ToViewModel(Menu menu, WebSite site)
        {
            SiteMenuItemViewModel item = new SiteMenuItemViewModel();

            item.Id = menu.Id;
            item.ParentId = menu.ParentId;
            item.RootId = menu.RootId;
            item.SubItemContainer = menu.SubItemContainer;
            item.SubItemTemplate = menu.SubItemTemplate;
            item.Template = menu.Template;
            item.Name = Kooboo.Sites.Helper.MenuHelper.GetName(menu, site.DefaultCulture);
            item.Url = menu.Url;
            return item;
        }

        [Kooboo.Attributes.RequireParameters("rootid", "ida", "idb")]
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public void Swap(ApiCall call)
        {
            Guid rootid = call.GetGuidValue("rootid");
            Guid ida = call.GetGuidValue("ida");
            Guid idb = call.GetGuidValue("idb");

            call.WebSite.SiteDb().Menus.Swap(rootid, ida, idb, call.Context.User.Id);
        }

        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.MENU, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.MENU, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }
    }
}
