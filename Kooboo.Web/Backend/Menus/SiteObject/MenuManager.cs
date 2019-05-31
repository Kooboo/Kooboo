using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Kooboo.Web.Menus.ObjectMenu
{
    public static class MenuManager
    { 
        // Menu for the table list.... 
        //public static List<Menu> GetSiteObjectMenu<T>() where T: ISiteObject
        //{ 
        //} 

        public static List<Kooboo.Api.ApiMethod> GetSiteMenuApiMethodByType(Type SiteObjectType)
        {
            List<Kooboo.Api.ApiMethod> result = new List<Kooboo.Api.ApiMethod>();

            var provider = GetApiProvider();
            var name = SiteObjectType.Name;

            if (provider.List.ContainsKey(name))
            {
                var allmethods = Lib.Reflection.TypeHelper.GetPublicMethods(SiteObjectType);

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
                if (item is Kooboo.Api.ApiMiddleware)
                {
                    var apimiddle = item as Kooboo.Api.ApiMiddleware; 
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
