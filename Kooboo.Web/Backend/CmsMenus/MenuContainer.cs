using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.CmsMenu
{
    public static class MenuContainer
    {
        private static object _locker = new object();
        private static List<IMenu> _items; 
        public static List<IMenu> Items
        {
            get
            {
                if (_items == null)
                {
                    lock(_locker)
                    {
                        if (_items == null)
                        {
                            _items = new List<IMenu>();
                            var alltypes = Kooboo.Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IMenu));
                            foreach (var item in alltypes)
                            {
                                var instance = Activator.CreateInstance(item) as IMenu;
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
          
        public static IMenu GetMenu(Type type)
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

        public static List<IMenu> SubMenus(Type type)
        {
            if (type.IsInterface)
            {
                return SubMenusByInterface(type); 
            }

            List<IMenu> result = new List<IMenu>(); 

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


        public static List<IMenu> SubMenusByInterface(Type type)
        {
            if (!type.IsInterface)
            {
                return SubMenus(type); 
            }
            List<IMenu> result = new List<IMenu>();

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
