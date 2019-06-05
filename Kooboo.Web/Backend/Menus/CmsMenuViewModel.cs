//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Web.Backend.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
 

namespace Kooboo.Web.Menus
{
    public class CmsMenuViewModel
    {
        internal  ICmsMenu CmsMenu { get; set; }
       public bool Hide { get; set; }

        public CmsMenuViewModel(ICmsMenu menu, RenderContext context)
        {
            this.CmsMenu = menu; 

            if (menu != null)
            {
                this.Order = menu.Order; 
                this.Name = menu.Name;
                this.Icon = menu.Icon;
                this.Url = menu.Url;
                this.DisplayName = menu.GetDisplayName(context);

                Guid WebSiteId = default(Guid); 
                if (menu is IHeaderMenu)
                {
                    var topmenu = menu as IHeaderMenu;
                    this.BadgeIcon = topmenu.BadgeIcon;
                    this.Name = this.DisplayName; 
                } 
                else
                { 
                    if (context.WebSite !=null)
                    {
                        WebSiteId = context.WebSite.Id; 
                    }
                }

                this.Url = appendSiteId(this.Url, WebSiteId);

                List<ICmsMenu> subitems =null; 
                if (menu is IDynamicMenu)
                {
                    var dynamic = menu as IDynamicMenu;
                    if (!dynamic.Show(context))
                    {
                        this.Hide = true; 
                    }
                    else
                    {
                        subitems = dynamic.ShowSubItems(context);
                    } 
                }
                else
                {
                    subitems = menu.SubItems; 
                }

                if (subitems !=null && subitems.Any())
                {
                    foreach (var item in subitems)
                    {
                        var model = new CmsMenuViewModel(item, context);
                        this.Items.Add(model); 
                    } 
                }
            }
        }

        public  CmsMenuViewModel(string name, string displayname)
        {
            this.Name = name;
            this.DisplayName = displayname; 
        }

        private  string appendSiteId(string relativeUrl, Guid SiteId)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl))
            {
                return null; 
            }

            if (relativeUrl.StartsWith("/") || relativeUrl.StartsWith("\\"))
            {
                relativeUrl = relativeUrl.Substring(1); 
            }

            Dictionary<string, string> para = new Dictionary<string, string>();
            if (SiteId != default(Guid))
            {
                para.Add("SiteId", SiteId.ToString());
            }
            return Kooboo.Lib.Helper.UrlHelper.AppendQueryString("/_Admin/" + relativeUrl, para);
        }

        public string BadgeIcon { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

        public int Order { get; set; }

        private List<CmsMenuViewModel> _items; 
        public List<CmsMenuViewModel> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<CmsMenuViewModel>(); 
                }
                return _items; 
            }
            set
            {
                _items = value; 
            }
        }

        public bool HasSubItem
        {
            get
            {
                if (_items ==null)
                {
                    return false; 
                }

                return _items.Any(); 
            }
        }
    }
}
