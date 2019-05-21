//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Menus
{
    public static class MenuHelper
    {

        public static string AdminUrl(string relativeUrl)
        {
            return "/_Admin/" + relativeUrl;
        }

        public static string AdminUrl(string relativeUrl, SiteDb siteDb)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            if (siteDb != null)
            {
                para.Add("SiteId", siteDb.Id.ToString());
            }
            return Kooboo.Lib.Helper.UrlHelper.AppendQueryString("/_Admin/" + relativeUrl, para);
        }

        internal static MenuItem ConvertToOld(ICmsMenu menu, RenderContext Context)
        {
            Guid siteid = default(Guid);
            if (Context.WebSite != null)
            {
                siteid = Context.WebSite.Id;
            }

            if (menu != null && menu.CanShow(Context))
            {
                MenuItem result = new MenuItem();
                result.Icon = menu.Icon;
                result.Name = menu.Name;
                result.DisplayName = menu.GetDisplayName(Context);
                result.Icon = menu.Icon;
                if (siteid == default(Guid))
                {
                    result.Url = menu.Url;
                }
                else
                {
                    Dictionary<string, string> para = new Dictionary<string, string>();
                    para.Add("SiteId", siteid.ToString());
                    result.Url = Kooboo.Lib.Helper.UrlHelper.AppendQueryString(menu.Url, para); 
                } 

                if (menu.Items != null && menu.Items.Any())
                {
                    foreach (var item in menu.Items)
                    {
                        var menuitem = ConvertToOld(item, Context);
                        if (menuitem != null)
                        {
                            result.Items.Add(menuitem);
                        }
                    }
                }
                return result;
            }

            return null;

        }

    }
}
