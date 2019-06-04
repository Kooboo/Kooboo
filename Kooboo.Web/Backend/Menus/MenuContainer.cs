//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Web.Backend.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Menus
{
    public static class MenuContainer
    {
        private static object _locker = new object();
        private static List<ICmsMenu> _items;
        public static List<ICmsMenu> Items
        {
            get
            {
                if (_items == null)
                {
                    lock (_locker)
                    {
                        if (_items == null)
                        {
                            _items = Kooboo.Lib.IOC.Service.GetInstances<ICmsMenu>(); 
                        }
                    }
                }

                return _items;
            }
        }

        public static ICmsMenu GetMenu(Type type)
        {
            // TODO: also do the submenu. 
            foreach (var item in Items)
            {
                if (item.GetType() == type)
                {
                    return item;
                }
            }
            return null;
        }


        // Subitem, should use load by interface.... 
        public static List<ICmsMenu> SubMenus(Type type)
        {
            if (type.IsInterface)
            {
                return SubMenusByInterface(type);
            }

            List<ICmsMenu> result = new List<ICmsMenu>();

            foreach (var item in Items)
            {
                if (item.GetType() == type)
                {
                    if (item.SubItems != null)
                    {
                        result.InsertRange(0, item.SubItems);
                    }
                }

                if (item.GetType().BaseType == type)
                {
                    result.Add(item);
                }
            }
            return result.OrderBy(o => o.Order).ToList();
        }


        public static List<ICmsMenu> SubMenusByInterface(Type type)
        {
            if (!type.IsInterface)
            {
                return SubMenus(type);
            }
            List<ICmsMenu> result = new List<ICmsMenu>();

            foreach (var item in Items)
            {

                var itemtype = item.GetType();

                if (Lib.Reflection.TypeHelper.HasInterface(itemtype, type))
                {
                    if (itemtype.BaseType == typeof(object))
                    {
                        result.Add(item);
                    }
                }
            }
            return result.OrderBy(o => o.Order).ToList();
        }
         

        private static List<ICmsMenu> _featuremenu;
        public static List<ICmsMenu> FeatureMenus
        {
            get
            {
                if (_featuremenu == null)
                {
                    lock (_locker)
                    {
                        if (_featuremenu == null)
                        {
                            _featuremenu = MenuContainer.SubMenus(typeof(IFeatureMenu));
                        }
                    }
                }
                return _featuremenu;
            }
        }


        private static List<ISideBarMenu> _sidebarmenus;

        public static List<ISideBarMenu> SideBarMenus
        {
            get
            {
                if (_sidebarmenus == null)
                {
                    lock (_locker)
                    { 
                        if (_sidebarmenus == null)
                        {
                            _sidebarmenus = new List<ISideBarMenu>();
                            var sidebaritems = SubMenus(typeof(ISideBarMenu));
                            foreach (var item in sidebaritems)
                            {
                                var sidebaritem = item as ISideBarMenu;
                                if (sidebaritem != null)
                                {
                                    _sidebarmenus.Add(sidebaritem); 
                                }
                            }
                        }
                    }
                }
                return _sidebarmenus;
            }
        }
    }
}
