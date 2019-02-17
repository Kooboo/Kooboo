using Kooboo.Data.Context;
using System.Collections.Generic;


namespace Kooboo.Web.CmsMenu
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

        public int Order => 3;

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
