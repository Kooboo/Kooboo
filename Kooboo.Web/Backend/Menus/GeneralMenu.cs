using Kooboo.Data.Context;
using System.Collections.Generic;

namespace Kooboo.Web.Menus
{
    public class GeneralMenu : ICmsMenu
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

        public int Order { get; set; }

        private List<ICmsMenu> _subitems;

        public List<ICmsMenu> SubItems
        {
            get { return _subitems ?? (_subitems = new List<ICmsMenu>()); }
            set
            {
                _subitems = value;
            }
        }

        public string GetDisplayName(RenderContext context)
        {
            return this.Name;
        }
    }
}