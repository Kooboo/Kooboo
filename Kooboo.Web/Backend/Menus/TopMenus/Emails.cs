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

    public class Emails : ITopMenu
    {
        public Emails()
        {
            this.Name = "Emails";
            this.Url = MenuHelper.AdminUrl("Emails/Inbox");
            this.Icon = "fa fa-envelope";
            this.BadgeIcon = "badge-primary";
        }

        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }

        private List<ICmsMenu> _items;

        public List<ICmsMenu> Items
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

        public string GetDisplayName(RenderContext Context)
        {
            return Data.Language.Hardcoded.GetValue("Emails", Context);
        }

        public bool CanShow(RenderContext context)
        {
            return true;
        }
    }



}
