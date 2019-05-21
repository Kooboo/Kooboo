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
  
        private List<ICmsMenu> _items; 

        public List<ICmsMenu> Items {
            get
            {
                if (_items == null)
                {
                    _items = new List<ICmsMenu>(); 
                } 
                return _items;  
            }
            set {  }
        }

        public string BadgeIcon { get; set; }
        public int Order { get => 1;  }

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
