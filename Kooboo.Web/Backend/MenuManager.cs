using Kooboo.Data.Context;
using Kooboo.Web.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Authorization.Model;
using Kooboo.Sites.Repository;

namespace Kooboo.Web.Menus
{
    public static class MenuManager
    {

        public static void VerifySortSideBar(List<CmsMenuViewModel> menus, RenderContext context)
        {
            var user = context.User;
            var sitedb = context.WebSite.SiteDb(); 

            if (user == null)
            {
                menus.RemoveAll(o => true);
            }

            else
            {
                if (!user.IsAdmin)
                {
                    var siteuser = sitedb.GetSiteRepository<SiteUserRepository>().Get(user.Id); 
                    if (siteuser == null)
                    {
                        menus.RemoveAll(o=>true); 
                    }
                    else
                    {
                        var repo = sitedb.GetSiteRepository<RolePermissionRepository>();
                        var role = repo.Get(siteuser.SiteRole);
                        AccessControl(menus, role); 
                    } 
                }
            }


            RemoveItems(menus); 

            if (menus.Any())
            {
                Sort(menus);  
            } 
        }


        public static void AccessControl(List<CmsMenuViewModel> menus, RolePermission role)
        {
            foreach (var item in menus)
            { 
                var permission = GetPermissionString(item);
                //var permission 
                var haspermission = Kooboo.Sites.Authorization.PermissionService.HasPermission(permission, role.Tree); 
                if (haspermission)
                {
                    continue; 
                }
                else
                {
                   if (item.HasSubItem)
                    {
                        bool HasNonHideItem = false; 
                        foreach (var submenu in item.Items)
                        {
                            var subPermission = GetPermissionString(submenu);
                            if (string.IsNullOrEmpty(subPermission))
                            {
                                continue; 
                            }
                            var subHasPermission = Kooboo.Sites.Authorization.PermissionService.HasPermission(subPermission, role.Tree); 
                            if (subHasPermission)
                            {
                                HasNonHideItem = true; 
                            }
                            else
                            {
                                submenu.Hide = true; 
                            } 
                        }
                         
                        if (!HasNonHideItem)
                        {
                            item.Hide = true;
                        }
                    }
                   else
                    {
                        item.Hide = true; 
                    }
                }
            }
        }

        public static string GetPermissionString(ICmsMenu menu)
        {
            if (menu == null)
            {
                return null;
            }
            if (menu is ISideBarMenu)
            {
                var sidebarmenu = menu as ISideBarMenu;
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


        public static string GetPermissionString(CmsMenuViewModel menu)
        {
            if (menu == null)
            {
                return null; 
            }

            if (menu.CmsMenu !=null)
            {
                return Kooboo.Sites.Authorization.MenuService.GetPermissionString(menu.CmsMenu);  
            }
            else
            {
                return menu.Name; 
            }
        }

        public static void RemoveItems(List<CmsMenuViewModel> list)
        {
            foreach (var item in list)
            {
                if (item.HasSubItem)
                {
                    RemoveItems(item.Items);
                    if (!item.HasSubItem)
                    {
                        item.Hide = true;
                    }
                }
            }
            list.RemoveAll(o => o.Hide);
        }


        public static void Sort(List<CmsMenuViewModel> list)
        {
            foreach (var item in list)
            {
                if (item.HasSubItem)
                {
                    Sort(item.Items);
                }
            }
            list.Sort(CompareMenu.instance);
        }


    }


    public class CompareMenu : IComparer<CmsMenuViewModel>
    {
        public int Compare(CmsMenuViewModel x, CmsMenuViewModel y)
        {
            return x.Order - y.Order;
        }

        private static CompareMenu _instance;
        public static CompareMenu instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CompareMenu();
                    return _instance;
                }
                return _instance;

            }
        }
    }


}
