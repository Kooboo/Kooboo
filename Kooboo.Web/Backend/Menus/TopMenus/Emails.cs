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
  

    ///  header.Menu.Add(new GlobalMenuItem { Name = Hardcoded.GetValue("Emails", context), Url = AdminUrl("Emails/Inbox"), Icon = "fa fa-envelope", Count = 0, BadgeIcon = "badge-primary" });

    public class Emails : IHeaderMenu
    {
        public Emails()
        {
            this.Name = "Emails";
            this.Url = "Emails/Inbox";
            this.Icon = "fa fa-envelope";
            this.BadgeIcon = "badge-primary";
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

        public int Order => 5;

        public ICmsMenu ParentMenu { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Data.Language.Hardcoded.GetValue("Emails", Context);
        }
         
    }



}
