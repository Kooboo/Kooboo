//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System.Collections.Generic;

namespace Kooboo.Web.Menus
{
    public class Domain : IHeaderMenu
    {
        public Domain()
        {
            this.Name = "Domains";
            this.Url = "Domains";
            this.Icon = "fa fa-at";
            this.BadgeIcon = "badge-info";
        }

        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }

        private List<ICmsMenu> _items;

        public List<ICmsMenu> SubItems
        {
            get { return _items ?? (_items = new List<ICmsMenu>()); }
            set { }
        }

        public string BadgeIcon { get; set; }
        public int Order { get => 1; }

        public ICmsMenu ParentMenu { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Domain", context);
        }
    }
}