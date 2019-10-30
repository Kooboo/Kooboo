using Kooboo.Web.Menus;

namespace Kooboo.Sites.Authorization
{
    public static class MenuService
    {
        public static string GetPermissionString(ICmsMenu menu)
        {
            if (menu == null)
            {
                return null;
            }
            if (menu is ISideBarMenu sidebarmenu)
            {
                if (sidebarmenu.Parent == SideBarSection.Root)
                {
                    return sidebarmenu.Name;
                }
                else
                {
                    return sidebarmenu.Parent.ToString() + "/" + sidebarmenu.Name;
                }
            }
            else if (menu is IFeatureMenu)
            {
                return "feature/" + menu.Name;
            }

            return null;
        }
    }
}