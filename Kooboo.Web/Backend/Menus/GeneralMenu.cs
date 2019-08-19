using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;

namespace Kooboo.Web.Menus
{
    public class GeneralMenu : ICmsMenu
    {
        public string Name { get; set; }

        public string Icon  {get;set;}

        public string Url { get; set; }

        public int Order { get; set; }

        private List<ICmsMenu> _Subitems; 
        public List<ICmsMenu> SubItems {
            get {
                if (_Subitems == null)
                {
                    _Subitems = new List<ICmsMenu>(); 
                }
                return _Subitems; 
            }
            set
            {
                _Subitems = value; 
            } 
        }

        public string GetDisplayName(RenderContext Context)
        {
            return this.Name; 
        }
    }
}
