using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;

namespace Kooboo.Web.Api.V2
{
    [ApiVersion(ApiVersion.V2)]
    public class MenuApi : Implementation.MenuApi
    {
        public Menu GetMenu(ApiCall call)
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

            return menu;
        }

        [Kooboo.Attributes.RequireParameters("Id", "RootId", "Url", "Name", "Values")]
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public bool UpdateInfo(ApiCall call)
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
                    sub.Url = call.GetValue("Url");
                    sub.Name = call.GetValue("Name");
                    string values = call.GetValue("Values");
                    sub.Values = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, string>>(values);
                    call.WebSite.SiteDb().Menus.AddOrUpdate(root, call.Context.User.Id);
                    return true;
                }
            }
            return false;

        }


        [Kooboo.Attributes.RequireParameters("ParentId", "RootId", "Name", "Url", "Values")]
        public override Menu CreateSub(ApiCall call)
        {
            Guid ParentId = call.GetGuidValue("ParentId");
            Guid RootId = call.GetGuidValue("RootId");
            string values = call.GetValue("Values");

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
                        Submenu.Values = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, string>>(values);
                        parentmenu.children.Insert(0, Submenu);
                        call.WebSite.SiteDb().Menus.AddOrUpdate(root, call.Context.User.Id);
                        return Submenu;
                    }

                }

            }
            return null;
        }
    }
}

