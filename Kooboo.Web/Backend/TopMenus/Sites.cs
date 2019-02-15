using Kooboo.Data.Context;
using Kooboo.Web.Backend.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Menu
{ 

    public class Sites : ITopMenu
    {
        public Sites()
        {
            this.Name = "Sites";
            this.Url = MenuHelper.AdminUrl("Sites");
            this.Icon = "fa fa-sitemap";
            this.BadgeIcon = "badge-success";
        }

        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }

        private List<IMenu> _items;

        public List<IMenu> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<IMenu>();
                }
                return _items;
            }
            set { }
        }

        public string BadgeIcon { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Data.Language.Hardcoded.GetValue("Sites", Context);
        }

        public bool CanShow(RenderContext context)
        {
            return true; 
        }
    }



}
