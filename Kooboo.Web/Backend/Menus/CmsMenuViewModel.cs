//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Menus
{
    public class CmsMenuViewModel
    {
        internal ICmsMenu CmsMenu { get; set; }
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

                Guid webSiteId = default(Guid);
                if (menu is IHeaderMenu topmenu)
                {
                    this.BadgeIcon = topmenu.BadgeIcon;
                    this.Name = this.DisplayName;
                }
                else
                {
                    if (context.WebSite != null)
                    {
                        webSiteId = context.WebSite.Id;
                    }
                }

                this.Url = appendSiteId(this.Url, webSiteId);

                List<ICmsMenu> subitems = null;
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

                if (subitems != null && subitems.Any())
                {
                    foreach (var item in subitems)
                    {
                        var model = new CmsMenuViewModel(item, context);
                        this.Items.Add(model);
                    }
                }
            }
        }

        public CmsMenuViewModel(string name, string displayname)
        {
            this.Name = name;
            this.DisplayName = displayname;
        }

        private string appendSiteId(string relativeUrl, Guid siteId)
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
            if (siteId != default(Guid))
            {
                para.Add("SiteId", siteId.ToString());
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
            get { return _items ?? (_items = new List<CmsMenuViewModel>()); }
            set
            {
                _items = value;
            }
        }

        public bool HasSubItem
        {
            get
            {
                return _items != null && _items.Any();
            }
        }
    }
}