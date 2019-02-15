using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Backend.Menu
{
    public class MenuViewModel
    {
        public MenuViewModel(IMenu menu, RenderContext context)
        {
            this.Name = menu.Name;
            this.Icon = menu.Icon;
            this.Url = menu.Url;
            this.DisplayName = menu.GetDisplayName(context); 

            if (menu.Items !=null)
            {
                foreach (var item in menu.Items)
                {
                    MenuViewModel itemmodel = new MenuViewModel(item, context);
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

        public string BadgeIcon { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

        private List<MenuViewModel> _items; 
        public List<MenuViewModel> Items
        {
            get; set;
        }
    }
}
