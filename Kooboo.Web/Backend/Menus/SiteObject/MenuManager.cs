using System;
using System.Collections.Generic;
using System.Reflection;
using Kooboo.Api;

namespace Kooboo.Web.Menus.ObjectMenu
{
    public static class MenuManager
    {
        // Menu for the table list....
        //public static List<Menu> GetSiteObjectMenu<T>() where T: ISiteObject
        //{
        //}

        public static List<Kooboo.Api.ApiMethod> GetSiteMenuApiMethodByType(Type siteObjectType)
        {
            List<Kooboo.Api.ApiMethod> result = new List<Kooboo.Api.ApiMethod>();

            var provider = GetApiProvider();
            var name = siteObjectType.Name;

            if (provider.List.ContainsKey(name))
            {
                var allmethods = Lib.Reflection.TypeHelper.GetPublicMethods(siteObjectType);

                foreach (var item in allmethods)
                {
                    if (IsMenu(item))
                    {
                    }
                }
            }
            return null;
        }

        public static Kooboo.Api.IApiProvider GetApiProvider()
        {
            foreach (var item in Web.SystemStart.Middleware)
            {
                if (item is ApiMiddleware apimiddle)
                {
                    return apimiddle.ApiProvider;
                }
            }
            return null;
        }

        public static bool IsMenu(MethodInfo method)
        {
            return method.IsDefined(typeof(SiteObjectMenu));
        }
    }
}