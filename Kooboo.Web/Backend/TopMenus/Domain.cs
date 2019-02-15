using Kooboo.Data.Context;
using Kooboo.Web.Backend.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Menu
{
    public class Domain : ITopMenu 
    {
        public Domain()
        {
            this.Name = "Domains";
            this.Url = MenuHelper.AdminUrl("Domains");
            this.Icon = "fa fa-at";
            this.BadgeIcon = "badge-info"; 
        }
  
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
  
        private List<IMenu> _items; 

        public List<IMenu> Items {
            get
            {
                if (_items == null)
                {
                    _items = new List<IMenu>(); 
                } 
                return _items;  
            }
            set {  }
        }

        public string BadgeIcon { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Data.Language.Hardcoded.GetValue("Domain", Context); 
        }

        public bool CanShow(RenderContext context)
        {
            if (context.User !=null && context.User.IsAdmin)
            {
                return true; 
            }
            return false; 
        }
    }
}
