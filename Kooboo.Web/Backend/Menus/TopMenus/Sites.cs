//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System.Collections.Generic;


namespace Kooboo.Web.Menus
{ 

    public class Sites : IHeaderMenu
    {
        public Sites()
        {
            this.Name = "Sites";
            this.Url = "Sites";
            this.Icon = "fa fa-sitemap";
            this.BadgeIcon = "badge-success";
        }

        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }

        private List<ICmsMenu> _items;

        public List<ICmsMenu> SubItems
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<ICmsMenu>();
                }
                return _items;
            }
            set { }
        }

        public string BadgeIcon { get; set; }

        public int Order => 3;

        public string GetDisplayName(RenderContext Context)
        {
            return Data.Language.Hardcoded.GetValue("Sites", Context);
        }
         
    }



}
