//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Menus
{
    public class CmsMenuViewModel
    {
        public CmsMenuViewModel(ICmsMenu menu, RenderContext context)
        {
            if (menu != null)
            {

                this.Name = menu.Name;
                this.Icon = menu.Icon;
                this.Url = menu.Url;
                this.DisplayName = menu.GetDisplayName(context);

                if (menu.Items != null)
                {
                    foreach (var item in menu.Items)
                    {
                        CmsMenuViewModel itemmodel = new CmsMenuViewModel(item, context);
                        this.Items.Add(itemmodel);
                    }
                }

                if (menu is ITopMenu)
                {
                    var topmenu = menu as ITopMenu;
                    this.BadgeIcon = topmenu.BadgeIcon;
                    this.Name = this.DisplayName;
                }
            }
        }

        public string BadgeIcon { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

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
    }
}
