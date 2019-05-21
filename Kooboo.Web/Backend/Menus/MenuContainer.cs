//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
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
                    lock(_locker)
                    {
                        if (_items == null)
                        {
                            _items = new List<ICmsMenu>();
                            var alltypes = Kooboo.Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(ICmsMenu));
                            foreach (var item in alltypes)
                            {
                                var instance = Activator.CreateInstance(item) as ICmsMenu;
                                if (instance !=null)
                                {
                                    _items.Add(instance);
                                } 
                            } 
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

        public static List<ICmsMenu> SubMenus(Type type)
        {
            if (type.IsInterface)
            {
                return SubMenusByInterface(type); 
            }

            List<ICmsMenu> result = new List<ICmsMenu>(); 

            foreach (var item in Items)
            {
                if (item.GetType()== type)
                {
                    if (item.Items !=null)
                    {
                        result.InsertRange(0, item.Items); 
                    }
                }  

                if (item.GetType().BaseType == type)
                {
                    result.Add(item); 
                }
            }
            return result.OrderBy(o=>o.Order).ToList(); 
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
            return result.OrderBy(o=>o.Order).ToList();
        }
        
    }
}
